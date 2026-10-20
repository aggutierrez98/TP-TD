using System;
using System.Collections.Generic;
using System.Linq;
using BE;
using REPOSITORIO;
using SERVICIOS;
using ABSTRACCIONES;

namespace BLL
{
    public class NotificacionBLL
    {
        // Contrato del alta manual del operador. La columna Mensaje admite texto completo
        // (recomendaciones automaticas), asi que el limite se valida aca y nunca se trunca.
        public const int MaximoMensajeManual = 300;

        private readonly ISesion _sesion;

        private int ObtenerDestinatarioActual()
        {
            int? id = _sesion.GetUsuarioActualId();
            if (!id.HasValue) throw new ExcepcionNegocio("Error.Notification.SessionRequired");
            return id.Value;
        }

        public PaginaNotificaciones BuscarPersonales(FiltroNotificacion filtro, int pagina)
        {
            int usuarioId = ObtenerDestinatarioActual();
            filtro = filtro ?? new FiltroNotificacion();
            if (pagina < 1) throw new ExcepcionNegocio("Error.Notification.InvalidPage");
            if (filtro.FechaDesde.HasValue && filtro.FechaHasta.HasValue && filtro.FechaDesde.Value.Date > filtro.FechaHasta.Value.Date)
                throw new ExcepcionNegocio("Error.Notification.InvalidDateRange");
            return _notificacionRepositorio.ObtenerPaginaPersonal(usuarioId, filtro, pagina);
        }

        public int ContarPersonalesNoLeidas() => _notificacionRepositorio.ContarNoLeidas(ObtenerDestinatarioActual());

        public Notificacion LeerPersonal(int id)
        {
            int usuarioId = ObtenerDestinatarioActual();
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Notificacion n = unidad.Notificaciones.ObtenerPersonal(id, usuarioId);
                    if (n == null) throw new ExcepcionNegocio("Error.Notification.NotFound");
                    if (!n.Leida)
                    {
                        bool cambio = unidad.Notificaciones.MarcarPersonalLeida(id, usuarioId);
                        if (!cambio)
                        {
                            n = unidad.Notificaciones.ObtenerPersonal(id, usuarioId);
                            if (n == null || !n.Leida) throw new ExcepcionTecnica("Error.Notification.UpdateFailed");
                        }
                        else
                        {
                            _bitacoraBLL.RegistrarOperacionInterno("FormMyNotifications.Value.Activity.Read", "FormMyNotifications.Value.Info.Read", "NotificacionBLL", unidad);
                            n.Leida = true;
                        }
                    }
                    unidad.Save();
                    return n;
                }
                catch (ExcepcionNegocio) { throw; }
                catch (ExcepcionTecnica) { throw; }
                catch (Exception ex) { throw new ExcepcionTecnica("Error.Notification.UpdateFailed", ex); }
            }
        }

        public void MarcarPersonalLeida(int id) { LeerPersonal(id); }
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
            IBitacoraBLL bitacoraBLL,
            ISesion sesion)
        {
            _notificacionRepositorio = notificacionRepositorio;
            _recordatorioCompraRepositorio = recordatorioCompraRepositorio;
            _recordatorioInversionRepositorio = recordatorioInversionRepositorio;
            _unidadDeTrabajoFactory = unidadDeTrabajoFactory;
            _bitacoraBLL = bitacoraBLL;
            _sesion = sesion;
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

            if (mensajeNormalizado.Length > MaximoMensajeManual)
            {
                throw new ExcepcionNegocio("Error.Notification.MessageTooLong");
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
