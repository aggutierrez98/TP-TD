using System;
using System.Collections.Generic;
using ABSTRACCIONES;
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
        private readonly ISesion _sesion;

        internal TasaConversionBLL(
            ITasaConversionRepositorio tasaConversionRepositorio,
            IUnidadDeTrabajoFactory unidadDeTrabajoFactory,
            IBitacoraBLL bitacoraBLL,
            ISesion sesion)
        {
            _tasaConversionRepositorio = tasaConversionRepositorio;
            _unidadDeTrabajoFactory = unidadDeTrabajoFactory;
            _bitacoraBLL = bitacoraBLL;
            _sesion = sesion;
        }

        // Devuelve la tasa a la actualizacion automatica. Si ya no era manual no hay nada
        // que escribir ni auditar: la operacion es idempotente.
        public bool Liberar(TasaConversion recibida)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    TasaConversion tasa = recibida == null ? null : unidad.TasasConversion.ObtenerPorId(recibida.Id);
                    if (tasa == null)
                    {
                        throw new ExcepcionNegocio("Error.CurrencyManagement.TasaNotFound");
                    }

                    if (!tasa.EsManual)
                    {
                        return true;
                    }

                    if (!unidad.TasasConversion.Liberar(tasa))
                    {
                        throw new ExcepcionTecnica("Error.CurrencyManagement.TasaReleaseFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormCurrencyManagement.Value.Activity.TasaReleased", "FormCurrencyManagement.Value.Info.TasaReleased", "TasaConversionBLL", unidad);
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
                    throw new ExcepcionTecnica("Error.CurrencyManagement.TasaReleaseFailed", ex);
                }
            }
        }

        public List<TasaConversion> ObtenerTodas()
        {
            return _tasaConversionRepositorio.ObtenerTodas();
        }

        public TasaConversion Crear(TasaConversion tasa)
        {
            ValidarMonedasDistintas(tasa?.MonedaBase, tasa?.MonedaConversion);
            ValidarCalculo(tasa?.CalculoConversion ?? 0m);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Moneda monedaBase = ObtenerMonedaBaseOLanzar(unidad, tasa.MonedaBase);
                    Moneda monedaConversion = ObtenerMonedaConversionOLanzar(unidad, tasa.MonedaConversion);

                    ValidarParNoDuplicado(unidad, monedaBase, monedaConversion, null);

                    tasa.MonedaBase = monedaBase;
                    tasa.MonedaConversion = monedaConversion;
                    tasa.FechaActualizacion = DateTime.Now;
                    tasa.EsManual = false;
                    tasa.ProtegidaPorUsuario = null;
                    tasa.FechaProteccion = null;
                    tasa.Id = unidad.TasasConversion.InsertarYObtenerId(tasa);

                    _bitacoraBLL.RegistrarOperacionInterno("FormCurrencyManagement.Value.Activity.TasaCreation", "FormCurrencyManagement.Value.Info.TasaCreation", "TasaConversionBLL", unidad);
                    unidad.Save();
                    return tasa;
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

        public bool Actualizar(TasaConversion recibida)
        {
            ValidarMonedasDistintas(recibida?.MonedaBase, recibida?.MonedaConversion);
            ValidarCalculo(recibida?.CalculoConversion ?? 0m);

            Usuario usuarioActual = _sesion.ObtenerUsuarioActual();

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    TasaConversion tasa = unidad.TasasConversion.ObtenerPorId(recibida.Id);
                    if (tasa == null)
                    {
                        throw new ExcepcionNegocio("Error.CurrencyManagement.TasaNotFound");
                    }

                    Moneda monedaBase = ObtenerMonedaBaseOLanzar(unidad, recibida.MonedaBase);
                    Moneda monedaConversion = ObtenerMonedaConversionOLanzar(unidad, recibida.MonedaConversion);

                    ValidarParNoDuplicado(unidad, monedaBase, monedaConversion, tasa);

                    // Solo un cambio real fija la tasa a mano: guardar sin tocar nada no debe
                    // sacarla de la actualizacion automatica.
                    bool huboCambio = tasa.CalculoConversion != recibida.CalculoConversion
                        || tasa.MonedaBase == null || tasa.MonedaBase.Id != monedaBase.Id
                        || tasa.MonedaConversion == null || tasa.MonedaConversion.Id != monedaConversion.Id;

                    tasa.MonedaBase = monedaBase;
                    tasa.MonedaConversion = monedaConversion;
                    tasa.CalculoConversion = recibida.CalculoConversion;
                    tasa.FechaActualizacion = DateTime.Now;

                    bool actualizado = unidad.TasasConversion.Actualizar(tasa);
                    if (!actualizado)
                    {
                        throw new ExcepcionTecnica("Error.CurrencyManagement.TasaUpdateFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormCurrencyManagement.Value.Activity.TasaUpdate", "FormCurrencyManagement.Value.Info.TasaUpdate", "TasaConversionBLL", unidad);

                    if (huboCambio)
                    {
                        if (!unidad.TasasConversion.MarcarManual(tasa, usuarioActual))
                        {
                            throw new ExcepcionTecnica("Error.CurrencyManagement.TasaUpdateFailed");
                        }

                        _bitacoraBLL.RegistrarOperacionInterno("FormCurrencyManagement.Value.Activity.TasaMarkedManual", "FormCurrencyManagement.Value.Info.TasaMarkedManual", "TasaConversionBLL", unidad);
                    }

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

        public bool Eliminar(TasaConversion recibida)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    TasaConversion tasa = recibida == null ? null : unidad.TasasConversion.ObtenerPorId(recibida.Id);
                    if (tasa == null)
                    {
                        throw new ExcepcionNegocio("Error.CurrencyManagement.TasaNotFound");
                    }

                    bool eliminado = unidad.TasasConversion.Eliminar(tasa);
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

        private static Moneda ObtenerMonedaBaseOLanzar(IUnidadDeTrabajo unidad, Moneda recibida)
        {
            Moneda monedaBase = recibida == null ? null : unidad.Monedas.ObtenerPorId(recibida.Id);
            if (monedaBase == null)
            {
                throw new ExcepcionNegocio("Error.CurrencyManagement.MonedaBaseNotFound");
            }

            return monedaBase;
        }

        private static Moneda ObtenerMonedaConversionOLanzar(IUnidadDeTrabajo unidad, Moneda recibida)
        {
            Moneda monedaConversion = recibida == null ? null : unidad.Monedas.ObtenerPorId(recibida.Id);
            if (monedaConversion == null)
            {
                throw new ExcepcionNegocio("Error.CurrencyManagement.MonedaConversionNotFound");
            }

            return monedaConversion;
        }

        // Al editar, la propia fila no es un par duplicado.
        private static void ValidarParNoDuplicado(IUnidadDeTrabajo unidad, Moneda monedaBase, Moneda monedaConversion, TasaConversion excluida)
        {
            TasaConversion existente = unidad.TasasConversion.ObtenerPorParDeMonedas(monedaBase, monedaConversion);
            if (existente != null && existente.Id != excluida?.Id)
            {
                throw new ExcepcionNegocio("Error.CurrencyManagement.ParDuplicado");
            }
        }

        private static void ValidarMonedasDistintas(Moneda monedaBase, Moneda monedaConversion)
        {
            if (monedaBase != null && monedaConversion != null && monedaBase.Id == monedaConversion.Id)
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
