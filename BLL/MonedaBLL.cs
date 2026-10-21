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

        // Devuelve la misma instancia recibida, normalizada y con Id cargado.
        public Moneda Crear(Moneda moneda)
        {
            string codigoNormalizado = NormalizarCodigo(moneda?.Codigo);
            string nombreNormalizado = NormalizarNombre(moneda?.Nombre);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    moneda.Codigo = codigoNormalizado;
                    moneda.Nombre = nombreNormalizado;

                    moneda.Id = unidad.Monedas.InsertarYObtenerId(moneda);
                    _bitacoraBLL.RegistrarOperacionInterno("FormCurrencyManagement.Value.Activity.MonedaCreation", "FormCurrencyManagement.Value.Info.MonedaCreation", "MonedaBLL", unidad);
                    unidad.Save();
                    return moneda;
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

        // Recibe la moneda con los valores nuevos y los copia sobre la persistida.
        public bool Actualizar(Moneda moneda)
        {
            string codigoNormalizado = NormalizarCodigo(moneda?.Codigo);
            string nombreNormalizado = NormalizarNombre(moneda?.Nombre);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Moneda persistida = unidad.Monedas.ObtenerPorId(moneda.Id);
                    if (persistida == null)
                    {
                        throw new ExcepcionNegocio("Error.CurrencyManagement.MonedaNotFound");
                    }

                    persistida.Codigo = codigoNormalizado;
                    persistida.Nombre = nombreNormalizado;

                    if (!unidad.Monedas.Actualizar(persistida))
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

        public bool Eliminar(Moneda moneda)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Moneda persistida = moneda == null ? null : unidad.Monedas.ObtenerPorId(moneda.Id);
                    if (persistida == null)
                    {
                        throw new ExcepcionNegocio("Error.CurrencyManagement.MonedaNotFound");
                    }

                    if (!unidad.Monedas.Eliminar(persistida))
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
