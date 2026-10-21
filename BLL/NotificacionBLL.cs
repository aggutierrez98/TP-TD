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

        private Usuario ObtenerDestinatarioActual()
        {
            if (!_sesion.HaySesionActiva()) throw new ExcepcionNegocio("Error.Notification.SessionRequired");
            return _sesion.ObtenerUsuarioActual();
        }

        public PaginaNotificaciones BuscarPersonales(FiltroNotificacion filtro, int pagina)
        {
            Usuario usuario = ObtenerDestinatarioActual();
            filtro = filtro ?? new FiltroNotificacion();
            if (pagina < 1) throw new ExcepcionNegocio("Error.Notification.InvalidPage");
            if (filtro.FechaDesde.HasValue && filtro.FechaHasta.HasValue && filtro.FechaDesde.Value.Date > filtro.FechaHasta.Value.Date)
                throw new ExcepcionNegocio("Error.Notification.InvalidDateRange");
            return _notificacionRepositorio.ObtenerPaginaPersonal(usuario, filtro, pagina);
        }

        public int ContarPersonalesNoLeidas() => _notificacionRepositorio.ContarNoLeidas(ObtenerDestinatarioActual());

        public Notificacion LeerPersonal(Notificacion recibida)
        {
            Usuario usuario = ObtenerDestinatarioActual();
            if (recibida == null) throw new ExcepcionNegocio("Error.Notification.NotFound");
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Notificacion n = unidad.Notificaciones.ObtenerPersonal(recibida, usuario);
                    if (n == null) throw new ExcepcionNegocio("Error.Notification.NotFound");
                    if (!n.Leida)
                    {
                        bool cambio = unidad.Notificaciones.MarcarPersonalLeida(n, usuario);
                        if (!cambio)
                        {
                            n = unidad.Notificaciones.ObtenerPersonal(n, usuario);
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

        public void MarcarPersonalLeida(Notificacion notificacion) { LeerPersonal(notificacion); }
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
        public Notificacion CrearManual(Notificacion recibida)
        {
            string mensajeNormalizado = (recibida?.Mensaje ?? string.Empty).Trim();
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
                    Usuario usuario = recibida.Usuario == null ? null : unidad.Usuarios.ObtenerPorId(recibida.Usuario.Id);
                    if (usuario == null)
                    {
                        throw new ExcepcionNegocio("Error.Notification.UserNotFound");
                    }

                    Notificacion nuevaNotificacion = recibida;
                    nuevaNotificacion.Usuario = usuario;
                    nuevaNotificacion.Mensaje = mensajeNormalizado;
                    nuevaNotificacion.Fecha = DateTime.Now;
                    nuevaNotificacion.Leida = false;
                    nuevaNotificacion.TipoAutomatico = null;
                    nuevaNotificacion.FechaJornada = null;
                    nuevaNotificacion.ClaveEjemplo = null;

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
        public bool MarcarLeida(Notificacion recibida, bool leida)
        {
            if (recibida == null) throw new ExcepcionNegocio("Error.Notification.NotFound");
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Notificacion notificacion = unidad.Notificaciones.ObtenerPorId(recibida.Id);
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
                        RecordatorioCompra recordatorioCompra = unidad.RecordatoriosCompra.ObtenerPorId(recibida.Id);
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
                            RecordatorioInversion recordatorioInversion = unidad.RecordatoriosInversion.ObtenerPorId(recibida.Id);
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

        public bool Eliminar(Notificacion recibida)
        {
            if (recibida == null) throw new ExcepcionNegocio("Error.Notification.NotFound");
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    bool eliminado;
                    RecordatorioCompra compra;
                    RecordatorioInversion inversion;
                    Notificacion notificacion = unidad.Notificaciones.ObtenerPorId(recibida.Id);
                    if (notificacion != null)
                    {
                        eliminado = unidad.Notificaciones.Eliminar(notificacion);
                    }
                    else if ((compra = unidad.RecordatoriosCompra.ObtenerPorId(recibida.Id)) != null)
                    {
                        eliminado = unidad.RecordatoriosCompra.Eliminar(compra);
                    }
                    else if ((inversion = unidad.RecordatoriosInversion.ObtenerPorId(recibida.Id)) != null)
                    {
                        eliminado = unidad.RecordatoriosInversion.Eliminar(inversion);
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
