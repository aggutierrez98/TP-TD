using System;
using System.Collections.Generic;
using System.Linq;
using BE;
using REPOSITORIO;
using SERVICIOS;

namespace BLL
{
    // Catalogo global de operador (back-office): sin validacion de "dueno", MedioPago no tiene
    // Usuario asociado. El producto concreto de cada entidad se gestiona en EntidadFinancieraBLL.
    public class MedioPagoBLL
    {
        private readonly IMedioPagoRepositorio _medioPagoRepositorio;
        private readonly IUnidadDeTrabajoFactory _unidadDeTrabajoFactory;
        private readonly IBitacoraBLL _bitacoraBLL;

        internal MedioPagoBLL(
            IMedioPagoRepositorio medioPagoRepositorio,
            IUnidadDeTrabajoFactory unidadDeTrabajoFactory,
            IBitacoraBLL bitacoraBLL)
        {
            _medioPagoRepositorio = medioPagoRepositorio;
            _unidadDeTrabajoFactory = unidadDeTrabajoFactory;
            _bitacoraBLL = bitacoraBLL;
        }

        public List<MedioPago> ObtenerTodos()
        {
            return _medioPagoRepositorio.ObtenerTodos();
        }

        public MedioPago Crear(TipoMedioPago tipo, string marca)
        {
            ValidarTipo(tipo);
            string marcaNormalizada = NormalizarMarca(marca);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    ValidarNoDuplicado(unidad, 0, tipo, marcaNormalizada);

                    MedioPago nuevoMedioPago = new MedioPago { Tipo = tipo, Marca = marcaNormalizada };
                    nuevoMedioPago.Id = unidad.MediosPago.InsertarYObtenerId(nuevoMedioPago);

                    _bitacoraBLL.RegistrarOperacionInterno("FormPaymentMethodManagement.Value.Activity.Creation", "FormPaymentMethodManagement.Value.Info.Creation", "MedioPagoBLL", unidad);
                    unidad.Save();
                    return nuevoMedioPago;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.PaymentMethodManagement.CreateFailed", ex);
                }
            }
        }

        public bool Actualizar(int id, TipoMedioPago tipo, string marca)
        {
            ValidarTipo(tipo);
            string marcaNormalizada = NormalizarMarca(marca);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    MedioPago medioPago = unidad.MediosPago.ObtenerPorId(id);
                    if (medioPago == null)
                    {
                        throw new ExcepcionNegocio("Error.PaymentMethodManagement.NotFound");
                    }

                    ValidarNoDuplicado(unidad, id, tipo, marcaNormalizada);

                    medioPago.Tipo = tipo;
                    medioPago.Marca = marcaNormalizada;
                    if (!unidad.MediosPago.Actualizar(medioPago))
                    {
                        throw new ExcepcionTecnica("Error.PaymentMethodManagement.UpdateFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormPaymentMethodManagement.Value.Activity.Update", "FormPaymentMethodManagement.Value.Info.Update", "MedioPagoBLL", unidad);
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
                    throw new ExcepcionTecnica("Error.PaymentMethodManagement.UpdateFailed", ex);
                }
            }
        }

        public bool Eliminar(int id)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    MedioPago medioPago = unidad.MediosPago.ObtenerPorId(id);
                    if (medioPago == null)
                    {
                        throw new ExcepcionNegocio("Error.PaymentMethodManagement.NotFound");
                    }

                    // Validacion proactiva: el medio no se puede borrar si alguna entidad lo ofrece.
                    if (unidad.MediosPagoEntidad.ObtenerPorMedioPago(id).Count > 0)
                    {
                        throw new ExcepcionNegocio("Error.PaymentMethodManagement.InUse");
                    }

                    if (!unidad.MediosPago.Eliminar(id))
                    {
                        throw new ExcepcionTecnica("Error.PaymentMethodManagement.DeleteFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormPaymentMethodManagement.Value.Activity.Deletion", "FormPaymentMethodManagement.Value.Info.Deletion", "MedioPagoBLL", unidad);
                    unidad.Save();
                    return true;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    // Red de seguridad ante una condicion de carrera entre la validacion proactiva
                    // y el delete real (mismo criterio que RubroComercioBLL.Eliminar).
                    if (ViolacionIntegridadReferencialHelper.EsViolacionRestriccionForanea(ex))
                    {
                        throw new ExcepcionNegocio("Error.PaymentMethodManagement.InUse");
                    }

                    if (ex is ExcepcionTecnica)
                    {
                        throw;
                    }

                    throw new ExcepcionTecnica("Error.PaymentMethodManagement.DeleteFailed", ex);
                }
            }
        }

        // UQ_MedioPago_Tipo_Marca: se valida antes para devolver un error de negocio legible.
        private static void ValidarNoDuplicado(IUnidadDeTrabajo unidad, int idActual, TipoMedioPago tipo, string marca)
        {
            bool duplicado = unidad.MediosPago.ObtenerTodos().Any(item =>
                item.Id != idActual
                && item.Tipo == tipo
                && string.Equals(item.Marca, marca, StringComparison.OrdinalIgnoreCase));

            if (duplicado)
            {
                throw new ExcepcionNegocio("Error.PaymentMethodManagement.Duplicate");
            }
        }

        private static void ValidarTipo(TipoMedioPago tipo)
        {
            if (!Enum.IsDefined(typeof(TipoMedioPago), tipo))
            {
                throw new ExcepcionNegocio("Error.PaymentMethodManagement.InvalidType");
            }
        }

        // Marca es opcional (ej. "Cuenta DNI" no tiene marca): en blanco se persiste NULL.
        private static string NormalizarMarca(string marca)
        {
            string marcaNormalizada = (marca ?? string.Empty).Trim();
            return marcaNormalizada.Length == 0 ? null : marcaNormalizada;
        }
    }
}
