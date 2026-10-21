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

        // Devuelve la misma instancia recibida, con la marca normalizada y el Id cargado.
        public MedioPago Crear(MedioPago medioPago)
        {
            ValidarTipo(medioPago);
            string marcaNormalizada = NormalizarMarca(medioPago.Marca);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    ValidarNoDuplicado(unidad, null, medioPago.Tipo, marcaNormalizada);

                    medioPago.Marca = marcaNormalizada;
                    medioPago.Id = unidad.MediosPago.InsertarYObtenerId(medioPago);

                    _bitacoraBLL.RegistrarOperacionInterno("FormPaymentMethodManagement.Value.Activity.Creation", "FormPaymentMethodManagement.Value.Info.Creation", "MedioPagoBLL", unidad);
                    unidad.Save();
                    return medioPago;
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

        // Recibe el medio de pago con Tipo y Marca nuevos y los copia sobre el persistido.
        public bool Actualizar(MedioPago medioPago)
        {
            ValidarTipo(medioPago);
            string marcaNormalizada = NormalizarMarca(medioPago.Marca);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    MedioPago persistido = unidad.MediosPago.ObtenerPorId(medioPago.Id);
                    if (persistido == null)
                    {
                        throw new ExcepcionNegocio("Error.PaymentMethodManagement.NotFound");
                    }

                    ValidarNoDuplicado(unidad, persistido, medioPago.Tipo, marcaNormalizada);

                    persistido.Tipo = medioPago.Tipo;
                    persistido.Marca = marcaNormalizada;
                    if (!unidad.MediosPago.Actualizar(persistido))
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

        public bool Eliminar(MedioPago medioPago)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    MedioPago persistido = medioPago == null ? null : unidad.MediosPago.ObtenerPorId(medioPago.Id);
                    if (persistido == null)
                    {
                        throw new ExcepcionNegocio("Error.PaymentMethodManagement.NotFound");
                    }

                    // Validacion proactiva: el medio no se puede borrar si alguna entidad lo ofrece.
                    if (unidad.MediosPagoEntidad.ObtenerPorMedioPago(persistido).Count > 0
                        || unidad.PromocionMediosPago.ContarPorMedioPago(persistido) > 0)
                    {
                        throw new ExcepcionNegocio("Error.PaymentMethodManagement.InUse");
                    }

                    if (!unidad.MediosPago.Eliminar(persistido))
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
                catch (ExcepcionTecnica)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.PaymentMethodManagement.DeleteFailed", ex);
                }
            }
        }

        // UQ_MedioPago_Tipo_Marca: se valida antes para devolver un error de negocio legible.
        // excluido null = alta (sin exclusion).
        private static void ValidarNoDuplicado(IUnidadDeTrabajo unidad, MedioPago excluido, TipoMedioPago tipo, string marca)
        {
            int idExcluido = excluido?.Id ?? 0;
            bool duplicado = unidad.MediosPago.ObtenerTodos().Any(item =>
                item.Id != idExcluido
                && item.Tipo == tipo
                && string.Equals(item.Marca, marca, StringComparison.OrdinalIgnoreCase));

            if (duplicado)
            {
                throw new ExcepcionNegocio("Error.PaymentMethodManagement.Duplicate");
            }
        }

        // Sin entidad no hay Tipo valido: mismo error que un Tipo fuera del enum.
        private static void ValidarTipo(MedioPago medioPago)
        {
            if (medioPago == null || !Enum.IsDefined(typeof(TipoMedioPago), medioPago.Tipo))
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
