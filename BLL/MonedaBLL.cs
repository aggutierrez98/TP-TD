using System;
using System.Collections.Generic;
using BE;
using REPOSITORIO;
using SERVICIOS;

namespace BLL
{
    public class MonedaBLL
    {
        private readonly IMonedaRepositorio _monedaRepositorio;
        private readonly IUnidadDeTrabajoFactory _unidadDeTrabajoFactory;
        private readonly IBitacoraBLL _bitacoraBLL;

        internal MonedaBLL(
            IMonedaRepositorio monedaRepositorio,
            IUnidadDeTrabajoFactory unidadDeTrabajoFactory,
            IBitacoraBLL bitacoraBLL)
        {
            _monedaRepositorio = monedaRepositorio;
            _unidadDeTrabajoFactory = unidadDeTrabajoFactory;
            _bitacoraBLL = bitacoraBLL;
        }

        public List<Moneda> ObtenerTodas()
        {
            return _monedaRepositorio.ObtenerTodas();
        }

        public Moneda Crear(string codigo, string nombre)
        {
            string codigoNormalizado = NormalizarCodigo(codigo);
            string nombreNormalizado = NormalizarNombre(nombre);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Moneda nuevaMoneda = new Moneda
                    {
                        Codigo = codigoNormalizado,
                        Nombre = nombreNormalizado
                    };

                    nuevaMoneda.Id = unidad.Monedas.InsertarYObtenerId(nuevaMoneda);
                    _bitacoraBLL.RegistrarOperacionInterno("FormCurrencyManagement.Value.Activity.MonedaCreation", "FormCurrencyManagement.Value.Info.MonedaCreation", "MonedaBLL", unidad);
                    unidad.Save();
                    return nuevaMoneda;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.CurrencyManagement.MonedaCreateFailed", ex);
                }
            }
        }

        public bool Actualizar(int id, string codigo, string nombre)
        {
            string codigoNormalizado = NormalizarCodigo(codigo);
            string nombreNormalizado = NormalizarNombre(nombre);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Moneda moneda = unidad.Monedas.ObtenerPorId(id);
                    if (moneda == null)
                    {
                        throw new ExcepcionNegocio("Error.CurrencyManagement.MonedaNotFound");
                    }

                    moneda.Codigo = codigoNormalizado;
                    moneda.Nombre = nombreNormalizado;

                    if (!unidad.Monedas.Actualizar(moneda))
                    {
                        throw new ExcepcionTecnica("Error.CurrencyManagement.MonedaUpdateFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormCurrencyManagement.Value.Activity.MonedaUpdate", "FormCurrencyManagement.Value.Info.MonedaUpdate", "MonedaBLL", unidad);
                    unidad.Save();
                    return true;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (ExcepcionTecnica)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.CurrencyManagement.MonedaUpdateFailed", ex);
                }
            }
        }

        public bool Eliminar(int id)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Moneda moneda = unidad.Monedas.ObtenerPorId(id);
                    if (moneda == null)
                    {
                        throw new ExcepcionNegocio("Error.CurrencyManagement.MonedaNotFound");
                    }

                    if (!unidad.Monedas.Eliminar(id))
                    {
                        throw new ExcepcionTecnica("Error.CurrencyManagement.MonedaDeleteFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormCurrencyManagement.Value.Activity.MonedaDeletion", "FormCurrencyManagement.Value.Info.MonedaDeletion", "MonedaBLL", unidad);
                    unidad.Save();
                    return true;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (ExcepcionTecnica)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.CurrencyManagement.MonedaDeleteFailed", ex);
                }
            }
        }

        private static string NormalizarCodigo(string codigo)
        {
            string normalizado = (codigo ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(normalizado))
            {
                throw new ExcepcionNegocio("Error.CurrencyManagement.MonedaCodeRequired");
            }

            if (normalizado.Length > 10)
            {
                throw new ExcepcionNegocio("Error.CurrencyManagement.MonedaCodeTooLong");
            }

            return normalizado;
        }

        private static string NormalizarNombre(string nombre)
        {
            string normalizado = (nombre ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(normalizado))
            {
                throw new ExcepcionNegocio("Error.CurrencyManagement.MonedaNameRequired");
            }

            if (normalizado.Length > 100)
            {
                throw new ExcepcionNegocio("Error.CurrencyManagement.MonedaNameTooLong");
            }

            return normalizado;
        }
    }
}
