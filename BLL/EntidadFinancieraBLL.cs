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

        // Hidratada con su catalogo MediosPagoEntidad (cada uno con su MedioPago).
        public EntidadFinanciera ObtenerPorId(int id)
        {
            return _entidadFinancieraRepositorio.ObtenerPorId(id);
        }

        public EntidadFinanciera Crear(string nombre)
        {
            string nombreNormalizado = NormalizarNombre(nombre);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    ValidarNombreUnico(unidad, 0, nombreNormalizado);

                    EntidadFinanciera nuevaEntidad = new EntidadFinanciera { Nombre = nombreNormalizado };
                    nuevaEntidad.Id = unidad.EntidadesFinancieras.InsertarYObtenerId(nuevaEntidad);

                    _bitacoraBLL.RegistrarOperacionInterno("FormFinancialEntityManagement.Value.Activity.Creation", "FormFinancialEntityManagement.Value.Info.Creation", "EntidadFinancieraBLL", unidad);
                    unidad.Save();
                    return nuevaEntidad;
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

        public bool Actualizar(int id, string nombre)
        {
            string nombreNormalizado = NormalizarNombre(nombre);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    EntidadFinanciera entidad = unidad.EntidadesFinancieras.ObtenerPorId(id);
                    if (entidad == null)
                    {
                        throw new ExcepcionNegocio("Error.FinancialEntityManagement.NotFound");
                    }

                    ValidarNombreUnico(unidad, id, nombreNormalizado);

                    entidad.Nombre = nombreNormalizado;
                    if (!unidad.EntidadesFinancieras.Actualizar(entidad))
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

        public bool Eliminar(int id)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    EntidadFinanciera entidad = unidad.EntidadesFinancieras.ObtenerPorId(id);
                    if (entidad == null)
                    {
                        throw new ExcepcionNegocio("Error.FinancialEntityManagement.NotFound");
                    }

                    // Validacion proactiva: no se borra una entidad con catalogo propio, con
                    // cuentas de usuarios asociadas o referenciada por una promocion (hay que
                    // quitarlos primero).
                    if (unidad.MediosPagoEntidad.ObtenerPorEntidadFinanciera(id).Count > 0
                        || unidad.CuentasFinancieras.ObtenerPorEntidadFinanciera(id).Count > 0
                        || unidad.PromocionEntidadesFinancieras.ContarPorEntidadFinanciera(id) > 0)
                    {
                        throw new ExcepcionNegocio("Error.FinancialEntityManagement.InUse");
                    }

                    if (!unidad.EntidadesFinancieras.Eliminar(id))
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

        public MedioPagoEntidad AgregarMedioPagoEntidad(int entidadFinancieraId, int medioPagoId, string segmento)
        {
            string segmentoNormalizado = NormalizarSegmento(segmento);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    if (unidad.EntidadesFinancieras.ObtenerPorId(entidadFinancieraId) == null)
                    {
                        throw new ExcepcionNegocio("Error.FinancialEntityManagement.NotFound");
                    }

                    if (unidad.MediosPago.ObtenerPorId(medioPagoId) == null)
                    {
                        throw new ExcepcionNegocio("Error.FinancialEntityManagement.PaymentMethodNotFound");
                    }

                    // UQ_MedioPagoEntidad_Entidad_Medio_Segmento: se valida antes para un error legible.
                    bool yaAgregado = unidad.MediosPagoEntidad.ObtenerPorEntidadFinanciera(entidadFinancieraId).Any(item =>
                        item.MedioPago != null
                        && item.MedioPago.Id == medioPagoId
                        && string.Equals(item.Segmento, segmentoNormalizado, StringComparison.OrdinalIgnoreCase));
                    if (yaAgregado)
                    {
                        throw new ExcepcionNegocio("Error.FinancialEntityManagement.PaymentMethodAlreadyAdded");
                    }

                    MedioPagoEntidad nuevoMedioPagoEntidad = new MedioPagoEntidad
                    {
                        EntidadFinanciera = new EntidadFinanciera { Id = entidadFinancieraId },
                        MedioPago = new MedioPago { Id = medioPagoId },
                        Segmento = segmentoNormalizado
                    };
                    nuevoMedioPagoEntidad.Id = unidad.MediosPagoEntidad.InsertarYObtenerId(nuevoMedioPagoEntidad);

                    _bitacoraBLL.RegistrarOperacionInterno("FormFinancialEntityManagement.Value.Activity.PaymentMethodAddition", "FormFinancialEntityManagement.Value.Info.PaymentMethodAddition", "EntidadFinancieraBLL", unidad);
                    unidad.Save();
                    return nuevoMedioPagoEntidad;
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

        public bool QuitarMedioPagoEntidad(int medioPagoEntidadId)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    if (unidad.MediosPagoEntidad.ObtenerPorId(medioPagoEntidadId) == null)
                    {
                        throw new ExcepcionNegocio("Error.FinancialEntityManagement.EntityPaymentMethodNotFound");
                    }

                    // No se quita un medio de pago de la entidad si hay cuentas de usuarios que
                    // lo tienen contratado.
                    if (unidad.CuentasFinancieras.ContarPorMedioPagoEntidad(medioPagoEntidadId) > 0)
                    {
                        throw new ExcepcionNegocio("Error.FinancialEntityManagement.EntityPaymentMethodInUse");
                    }

                    if (!unidad.MediosPagoEntidad.Eliminar(medioPagoEntidadId))
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
        private static void ValidarNombreUnico(IUnidadDeTrabajo unidad, int idActual, string nombre)
        {
            bool duplicado = unidad.EntidadesFinancieras.ObtenerTodas().Any(item =>
                item.Id != idActual && string.Equals(item.Nombre, nombre, StringComparison.OrdinalIgnoreCase));

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
