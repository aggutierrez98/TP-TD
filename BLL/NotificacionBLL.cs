using System;
using System.Collections.Generic;
using System.Linq;
using BE;
using REPOSITORIO;
using SERVICIOS;

namespace BLL
{
    public class NotificacionBLL
    {
        private readonly INotificacionRepositorio _notificacionRepositorio;
        private readonly IRecordatorioCompraRepositorio _recordatorioCompraRepositorio;
        private readonly IRecordatorioInversionRepositorio _recordatorioInversionRepositorio;
        private readonly IUnidadDeTrabajoFactory _unidadDeTrabajoFactory;
        private readonly IBitacoraBLL _bitacoraBLL;

        internal NotificacionBLL(
            INotificacionRepositorio notificacionRepositorio,
            IRecordatorioCompraRepositorio recordatorioCompraRepositorio,
            IRecordatorioInversionRepositorio recordatorioInversionRepositorio,
            IUnidadDeTrabajoFactory unidadDeTrabajoFactory,
            IBitacoraBLL bitacoraBLL)
        {
            _notificacionRepositorio = notificacionRepositorio;
            _recordatorioCompraRepositorio = recordatorioCompraRepositorio;
            _recordatorioInversionRepositorio = recordatorioInversionRepositorio;
            _unidadDeTrabajoFactory = unidadDeTrabajoFactory;
            _bitacoraBLL = bitacoraBLL;
        }

        // Pantalla de operador: notificaciones de todos los usuarios, de las 3 subclases mezcladas.
        public List<Notificacion> ObtenerTodas()
        {
            List<Notificacion> notificaciones = new List<Notificacion>();
            notificaciones.AddRange(_notificacionRepositorio.ObtenerTodas());
            notificaciones.AddRange(_recordatorioCompraRepositorio.ObtenerTodas());
            notificaciones.AddRange(_recordatorioInversionRepositorio.ObtenerTodas());

            return notificaciones.OrderByDescending(item => item.Fecha).ToList();
        }

        // Alta manual: solo el tipo base. Los subtipos los genera un proceso futuro, fuera de alcance.
        public Notificacion CrearManual(int usuarioId, string mensaje)
        {
            string mensajeNormalizado = (mensaje ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(mensajeNormalizado))
            {
                throw new ExcepcionNegocio("Error.Notification.MessageRequired");
            }

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Usuario usuario = unidad.Usuarios.ObtenerPorId(usuarioId);
                    if (usuario == null)
                    {
                        throw new ExcepcionNegocio("Error.Notification.UserNotFound");
                    }

                    Notificacion nuevaNotificacion = new Notificacion
                    {
                        Usuario = new Usuario { Id = usuarioId },
                        Mensaje = mensajeNormalizado,
                        Fecha = DateTime.Now,
                        Leida = false
                    };

                    int id = unidad.Notificaciones.InsertarYObtenerId(nuevaNotificacion);
                    nuevaNotificacion.Id = id;

                    _bitacoraBLL.RegistrarOperacionInterno("FormNotificationManagement.Value.Activity.NotificationCreation", "FormNotificationManagement.Value.Info.NotificationCreation", "NotificacionBLL", unidad);
                    unidad.Save();
                    return nuevaNotificacion;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.Notification.CreateFailed", ex);
                }
            }
        }

        // El subtipo real no se conoce de antemano: se prueba en orden hasta encontrar la entidad.
        public bool MarcarLeida(int notificacionId, bool leida)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Notificacion notificacion = unidad.Notificaciones.ObtenerPorId(notificacionId);
                    if (notificacion != null)
                    {
                        notificacion.Leida = leida;
                        if (!unidad.Notificaciones.Actualizar(notificacion))
                        {
                            throw new ExcepcionTecnica("Error.Notification.UpdateFailed");
                        }
                    }
                    else
                    {
                        RecordatorioCompra recordatorioCompra = unidad.RecordatoriosCompra.ObtenerPorId(notificacionId);
                        if (recordatorioCompra != null)
                        {
                            recordatorioCompra.Leida = leida;
                            if (!unidad.RecordatoriosCompra.Actualizar(recordatorioCompra))
                            {
                                throw new ExcepcionTecnica("Error.Notification.UpdateFailed");
                            }
                        }
                        else
                        {
                            RecordatorioInversion recordatorioInversion = unidad.RecordatoriosInversion.ObtenerPorId(notificacionId);
                            if (recordatorioInversion != null)
                            {
                                recordatorioInversion.Leida = leida;
                                if (!unidad.RecordatoriosInversion.Actualizar(recordatorioInversion))
                                {
                                    throw new ExcepcionTecnica("Error.Notification.UpdateFailed");
                                }
                            }
                            else
                            {
                                throw new ExcepcionNegocio("Error.Notification.NotFound");
                            }
                        }
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormNotificationManagement.Value.Activity.NotificationUpdate", "FormNotificationManagement.Value.Info.NotificationUpdate", "NotificacionBLL", unidad);
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
                    throw new ExcepcionTecnica("Error.Notification.UpdateFailed", ex);
                }
            }
        }

        public bool Eliminar(int notificacionId)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    bool eliminado;
                    if (unidad.Notificaciones.ObtenerPorId(notificacionId) != null)
                    {
                        eliminado = unidad.Notificaciones.Eliminar(notificacionId);
                    }
                    else if (unidad.RecordatoriosCompra.ObtenerPorId(notificacionId) != null)
                    {
                        eliminado = unidad.RecordatoriosCompra.Eliminar(notificacionId);
                    }
                    else if (unidad.RecordatoriosInversion.ObtenerPorId(notificacionId) != null)
                    {
                        eliminado = unidad.RecordatoriosInversion.Eliminar(notificacionId);
                    }
                    else
                    {
                        throw new ExcepcionNegocio("Error.Notification.NotFound");
                    }

                    if (!eliminado)
                    {
                        throw new ExcepcionTecnica("Error.Notification.DeleteFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormNotificationManagement.Value.Activity.NotificationDeletion", "FormNotificationManagement.Value.Info.NotificationDeletion", "NotificacionBLL", unidad);
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
                    throw new ExcepcionTecnica("Error.Notification.DeleteFailed", ex);
                }
            }
        }
    }
}
