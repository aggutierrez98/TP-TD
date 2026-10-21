using System;
using System.Collections.Generic;
using System.Linq;
using BE;
using REPOSITORIO;
using SERVICIOS;

namespace BLL
{
    // Catalogo global de operador (back-office): EntidadFinanciera y su catalogo de
    // MediosPagoEntidad no tienen Usuario asociado, sin validacion de "dueno".
    public class EntidadFinancieraBLL
    {
        private readonly IEntidadFinancieraRepositorio _entidadFinancieraRepositorio;
        private readonly IUnidadDeTrabajoFactory _unidadDeTrabajoFactory;
        private readonly IBitacoraBLL _bitacoraBLL;

        internal EntidadFinancieraBLL(
            IEntidadFinancieraRepositorio entidadFinancieraRepositorio,
            IUnidadDeTrabajoFactory unidadDeTrabajoFactory,
            IBitacoraBLL bitacoraBLL)
        {
            _entidadFinancieraRepositorio = entidadFinancieraRepositorio;
            _unidadDeTrabajoFactory = unidadDeTrabajoFactory;
            _bitacoraBLL = bitacoraBLL;
        }

        // Liviana: sin el catalogo MediosPagoEntidad de cada entidad (grillas y combos).
        public List<EntidadFinanciera> ObtenerTodas()
        {
            return _entidadFinancieraRepositorio.ObtenerTodas();
        }

        // Catalogo MediosPagoEntidad de la entidad persistida (cada uno con su MedioPago).
        // Vacio si la entidad ya no existe.
        public List<MedioPagoEntidad> ObtenerMediosPagoEntidad(EntidadFinanciera entidad)
        {
            EntidadFinanciera persistida = _entidadFinancieraRepositorio.ObtenerPorId(entidad.Id);
            return persistida?.MediosPagoEntidad ?? new List<MedioPagoEntidad>();
        }

        // Devuelve la misma instancia recibida, con el nombre normalizado y el Id cargado.
        public EntidadFinanciera Crear(EntidadFinanciera entidad)
        {
            string nombreNormalizado = NormalizarNombre(entidad?.Nombre);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    ValidarNombreUnico(unidad, null, nombreNormalizado);

                    entidad.Nombre = nombreNormalizado;
                    entidad.Id = unidad.EntidadesFinancieras.InsertarYObtenerId(entidad);

                    _bitacoraBLL.RegistrarOperacionInterno("FormFinancialEntityManagement.Value.Activity.Creation", "FormFinancialEntityManagement.Value.Info.Creation", "EntidadFinancieraBLL", unidad);
                    unidad.Save();
                    return entidad;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.FinancialEntityManagement.CreateFailed", ex);
                }
            }
        }

        // Recibe la entidad con el nombre nuevo y lo copia sobre la persistida.
        public bool Actualizar(EntidadFinanciera entidad)
        {
            string nombreNormalizado = NormalizarNombre(entidad?.Nombre);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    EntidadFinanciera persistida = unidad.EntidadesFinancieras.ObtenerPorId(entidad.Id);
                    if (persistida == null)
                    {
                        throw new ExcepcionNegocio("Error.FinancialEntityManagement.NotFound");
                    }

                    ValidarNombreUnico(unidad, persistida, nombreNormalizado);

                    persistida.Nombre = nombreNormalizado;
                    if (!unidad.EntidadesFinancieras.Actualizar(persistida))
                    {
                        throw new ExcepcionTecnica("Error.FinancialEntityManagement.UpdateFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormFinancialEntityManagement.Value.Activity.Update", "FormFinancialEntityManagement.Value.Info.Update", "EntidadFinancieraBLL", unidad);
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
                    throw new ExcepcionTecnica("Error.FinancialEntityManagement.UpdateFailed", ex);
                }
            }
        }

        public bool Eliminar(EntidadFinanciera entidad)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    EntidadFinanciera persistida = entidad == null ? null : unidad.EntidadesFinancieras.ObtenerPorId(entidad.Id);
                    if (persistida == null)
                    {
                        throw new ExcepcionNegocio("Error.FinancialEntityManagement.NotFound");
                    }

                    // Validacion proactiva: no se borra una entidad con catalogo propio, con
                    // cuentas de usuarios asociadas o referenciada por una promocion (hay que
                    // quitarlos primero).
                    if (unidad.MediosPagoEntidad.ObtenerPorEntidadFinanciera(persistida).Count > 0
                        || unidad.CuentasFinancieras.ObtenerPorEntidadFinanciera(persistida).Count > 0
                        || unidad.PromocionEntidadesFinancieras.ContarPorEntidadFinanciera(persistida) > 0)
                    {
                        throw new ExcepcionNegocio("Error.FinancialEntityManagement.InUse");
                    }

                    if (!unidad.EntidadesFinancieras.Eliminar(persistida))
                    {
                        throw new ExcepcionTecnica("Error.FinancialEntityManagement.DeleteFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormFinancialEntityManagement.Value.Activity.Deletion", "FormFinancialEntityManagement.Value.Info.Deletion", "EntidadFinancieraBLL", unidad);
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
                    throw new ExcepcionTecnica("Error.FinancialEntityManagement.DeleteFailed", ex);
                }
            }
        }

        // Recibe el producto armado por la UI (EntidadFinanciera, MedioPago y Segmento). Relee la
        // entidad y el medio de pago y devuelve la misma instancia con los persistidos y su Id.
        public MedioPagoEntidad AgregarMedioPagoEntidad(MedioPagoEntidad medioPagoEntidad)
        {
            string segmentoNormalizado = NormalizarSegmento(medioPagoEntidad?.Segmento);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    EntidadFinanciera entidad = medioPagoEntidad?.EntidadFinanciera == null ? null : unidad.EntidadesFinancieras.ObtenerPorId(medioPagoEntidad.EntidadFinanciera.Id);
                    if (entidad == null)
                    {
                        throw new ExcepcionNegocio("Error.FinancialEntityManagement.NotFound");
                    }

                    MedioPago medioPago = medioPagoEntidad.MedioPago == null ? null : unidad.MediosPago.ObtenerPorId(medioPagoEntidad.MedioPago.Id);
                    if (medioPago == null)
                    {
                        throw new ExcepcionNegocio("Error.FinancialEntityManagement.PaymentMethodNotFound");
                    }

                    // UQ_MedioPagoEntidad_Entidad_Medio_Segmento: se valida antes para un error legible.
                    bool yaAgregado = unidad.MediosPagoEntidad.ObtenerPorEntidadFinanciera(entidad).Any(item =>
                        item.MedioPago != null
                        && item.MedioPago.Id == medioPago.Id
                        && string.Equals(item.Segmento, segmentoNormalizado, StringComparison.OrdinalIgnoreCase));
                    if (yaAgregado)
                    {
                        throw new ExcepcionNegocio("Error.FinancialEntityManagement.PaymentMethodAlreadyAdded");
                    }

                    medioPagoEntidad.EntidadFinanciera = entidad;
                    medioPagoEntidad.MedioPago = medioPago;
                    medioPagoEntidad.Segmento = segmentoNormalizado;
                    medioPagoEntidad.Id = unidad.MediosPagoEntidad.InsertarYObtenerId(medioPagoEntidad);

                    _bitacoraBLL.RegistrarOperacionInterno("FormFinancialEntityManagement.Value.Activity.PaymentMethodAddition", "FormFinancialEntityManagement.Value.Info.PaymentMethodAddition", "EntidadFinancieraBLL", unidad);
                    unidad.Save();
                    return medioPagoEntidad;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.FinancialEntityManagement.PaymentMethodAddFailed", ex);
                }
            }
        }

        public bool QuitarMedioPagoEntidad(MedioPagoEntidad medioPagoEntidad)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    MedioPagoEntidad persistido = medioPagoEntidad == null ? null : unidad.MediosPagoEntidad.ObtenerPorId(medioPagoEntidad.Id);
                    if (persistido == null)
                    {
                        throw new ExcepcionNegocio("Error.FinancialEntityManagement.EntityPaymentMethodNotFound");
                    }

                    // No se quita un medio de pago de la entidad si hay cuentas de usuarios que
                    // lo tienen contratado.
                    if (unidad.CuentasFinancieras.ContarPorMedioPagoEntidad(persistido) > 0)
                    {
                        throw new ExcepcionNegocio("Error.FinancialEntityManagement.EntityPaymentMethodInUse");
                    }

                    if (!unidad.MediosPagoEntidad.Eliminar(persistido))
                    {
                        throw new ExcepcionTecnica("Error.FinancialEntityManagement.PaymentMethodRemoveFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormFinancialEntityManagement.Value.Activity.PaymentMethodRemoval", "FormFinancialEntityManagement.Value.Info.PaymentMethodRemoval", "EntidadFinancieraBLL", unidad);
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
                    throw new ExcepcionTecnica("Error.FinancialEntityManagement.PaymentMethodRemoveFailed", ex);
                }
            }
        }

        // UQ_EntidadFinanciera_Nombre: se valida antes para devolver un error de negocio legible.
        // excluida null = alta (sin exclusion).
        private static void ValidarNombreUnico(IUnidadDeTrabajo unidad, EntidadFinanciera excluida, string nombre)
        {
            int idExcluido = excluida?.Id ?? 0;
            bool duplicado = unidad.EntidadesFinancieras.ObtenerTodas().Any(item =>
                item.Id != idExcluido && string.Equals(item.Nombre, nombre, StringComparison.OrdinalIgnoreCase));

            if (duplicado)
            {
                throw new ExcepcionNegocio("Error.FinancialEntityManagement.DuplicateName");
            }
        }

        private static string NormalizarNombre(string nombre)
        {
            string nombreNormalizado = (nombre ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(nombreNormalizado))
            {
                throw new ExcepcionNegocio("Error.FinancialEntityManagement.NameRequired");
            }

            return nombreNormalizado;
        }

        // Segmento es opcional (ej. "Eminent", "Black"): en blanco se persiste NULL.
        private static string NormalizarSegmento(string segmento)
        {
            string segmentoNormalizado = (segmento ?? string.Empty).Trim();
            return segmentoNormalizado.Length == 0 ? null : segmentoNormalizado;
        }
    }
}
