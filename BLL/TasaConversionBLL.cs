using System;
using System.Collections.Generic;
using BE;
using REPOSITORIO;
using SERVICIOS;

namespace BLL
{
    // Pantalla de operador (back-office), seccion "Tasas de Conversion" de Gestion de
    // Monedas. Las reglas de negocio (monedas existentes, distintas, calculo positivo,
    // par unico) se validan aca antes de llegar a las constraints de la base, para dar
    // un error de negocio claro en vez de una excepcion tecnica de SQL.
    public class TasaConversionBLL
    {
        private readonly ITasaConversionRepositorio _tasaConversionRepositorio;
        private readonly IUnidadDeTrabajoFactory _unidadDeTrabajoFactory;
        private readonly IBitacoraBLL _bitacoraBLL;

        internal TasaConversionBLL(
            ITasaConversionRepositorio tasaConversionRepositorio,
            IUnidadDeTrabajoFactory unidadDeTrabajoFactory,
            IBitacoraBLL bitacoraBLL)
        {
            _tasaConversionRepositorio = tasaConversionRepositorio;
            _unidadDeTrabajoFactory = unidadDeTrabajoFactory;
            _bitacoraBLL = bitacoraBLL;
        }

        public List<TasaConversion> ObtenerTodas()
        {
            return _tasaConversionRepositorio.ObtenerTodas();
        }

        public TasaConversion Crear(int monedaBaseId, int monedaConversionId, decimal calculoConversion)
        {
            ValidarMonedasDistintas(monedaBaseId, monedaConversionId);
            ValidarCalculo(calculoConversion);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Moneda monedaBase = ObtenerMonedaBaseOLanzar(unidad, monedaBaseId);
                    Moneda monedaConversion = ObtenerMonedaConversionOLanzar(unidad, monedaConversionId);

                    ValidarParNoDuplicado(unidad, monedaBaseId, monedaConversionId, null);

                    TasaConversion nuevaTasa = new TasaConversion
                    {
                        MonedaBase = monedaBase,
                        MonedaConversion = monedaConversion,
                        CalculoConversion = calculoConversion,
                        FechaActualizacion = DateTime.Now
                    };

                    int id = unidad.TasasConversion.InsertarYObtenerId(nuevaTasa);
                    nuevaTasa.Id = id;

                    _bitacoraBLL.RegistrarOperacionInterno("FormCurrencyManagement.Value.Activity.TasaCreation", "FormCurrencyManagement.Value.Info.TasaCreation", "TasaConversionBLL", unidad);
                    unidad.Save();
                    return nuevaTasa;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.CurrencyManagement.TasaCreateFailed", ex);
                }
            }
        }

        public bool Actualizar(int id, int monedaBaseId, int monedaConversionId, decimal calculoConversion)
        {
            ValidarMonedasDistintas(monedaBaseId, monedaConversionId);
            ValidarCalculo(calculoConversion);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    TasaConversion tasa = unidad.TasasConversion.ObtenerPorId(id);
                    if (tasa == null)
                    {
                        throw new ExcepcionNegocio("Error.CurrencyManagement.TasaNotFound");
                    }

                    Moneda monedaBase = ObtenerMonedaBaseOLanzar(unidad, monedaBaseId);
                    Moneda monedaConversion = ObtenerMonedaConversionOLanzar(unidad, monedaConversionId);

                    ValidarParNoDuplicado(unidad, monedaBaseId, monedaConversionId, id);

                    tasa.MonedaBase = monedaBase;
                    tasa.MonedaConversion = monedaConversion;
                    tasa.CalculoConversion = calculoConversion;
                    tasa.FechaActualizacion = DateTime.Now;

                    bool actualizado = unidad.TasasConversion.Actualizar(tasa);
                    if (!actualizado)
                    {
                        throw new ExcepcionTecnica("Error.CurrencyManagement.TasaUpdateFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormCurrencyManagement.Value.Activity.TasaUpdate", "FormCurrencyManagement.Value.Info.TasaUpdate", "TasaConversionBLL", unidad);
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
                    throw new ExcepcionTecnica("Error.CurrencyManagement.TasaUpdateFailed", ex);
                }
            }
        }

        public bool Eliminar(int id)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    TasaConversion tasa = unidad.TasasConversion.ObtenerPorId(id);
                    if (tasa == null)
                    {
                        throw new ExcepcionNegocio("Error.CurrencyManagement.TasaNotFound");
                    }

                    bool eliminado = unidad.TasasConversion.Eliminar(id);
                    if (!eliminado)
                    {
                        throw new ExcepcionTecnica("Error.CurrencyManagement.TasaDeleteFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormCurrencyManagement.Value.Activity.TasaDeletion", "FormCurrencyManagement.Value.Info.TasaDeletion", "TasaConversionBLL", unidad);
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
                    throw new ExcepcionTecnica("Error.CurrencyManagement.TasaDeleteFailed", ex);
                }
            }
        }

        private static Moneda ObtenerMonedaBaseOLanzar(IUnidadDeTrabajo unidad, int monedaBaseId)
        {
            Moneda monedaBase = unidad.Monedas.ObtenerPorId(monedaBaseId);
            if (monedaBase == null)
            {
                throw new ExcepcionNegocio("Error.CurrencyManagement.MonedaBaseNotFound");
            }

            return monedaBase;
        }

        private static Moneda ObtenerMonedaConversionOLanzar(IUnidadDeTrabajo unidad, int monedaConversionId)
        {
            Moneda monedaConversion = unidad.Monedas.ObtenerPorId(monedaConversionId);
            if (monedaConversion == null)
            {
                throw new ExcepcionNegocio("Error.CurrencyManagement.MonedaConversionNotFound");
            }

            return monedaConversion;
        }

        // excluirId: al actualizar, si el par no cambio, ObtenerPorParDeMonedas devuelve la
        // propia fila (Id == excluirId) y no debe considerarse un duplicado.
        private static void ValidarParNoDuplicado(IUnidadDeTrabajo unidad, int monedaBaseId, int monedaConversionId, int? excluirId)
        {
            TasaConversion existente = unidad.TasasConversion.ObtenerPorParDeMonedas(monedaBaseId, monedaConversionId);
            if (existente != null && existente.Id != excluirId)
            {
                throw new ExcepcionNegocio("Error.CurrencyManagement.ParDuplicado");
            }
        }

        private static void ValidarMonedasDistintas(int monedaBaseId, int monedaConversionId)
        {
            if (monedaBaseId == monedaConversionId)
            {
                throw new ExcepcionNegocio("Error.CurrencyManagement.MonedasIguales");
            }
        }

        private static void ValidarCalculo(decimal calculoConversion)
        {
            if (calculoConversion <= 0)
            {
                throw new ExcepcionNegocio("Error.CurrencyManagement.CalculoInvalido");
            }
        }
    }
}
