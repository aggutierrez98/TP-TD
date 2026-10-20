using System;
using System.Collections.Generic;
using BE;
using DAL;

namespace REPOSITORIO
{
    public class NotificacionRepositorio : INotificacionRepositorio
    {
        public Notificacion ObtenerPersonal(int id, int usuarioId) => _notificacionDAL.ObtenerPersonal(id, usuarioId, _accesoDatos);
        public bool MarcarPersonalLeida(int id, int usuarioId) => _notificacionDAL.MarcarPersonalLeida(id, usuarioId, _accesoDatos);
        public PaginaNotificaciones ObtenerPaginaPersonal(int usuarioId, FiltroNotificacion filtro, int pagina)
        {
            return _notificacionDAL.ObtenerPaginaPersonal(usuarioId, filtro, pagina, _accesoDatos);
        }

        public int ContarNoLeidas(int usuarioId)
        {
            return _notificacionDAL.ContarNoLeidas(usuarioId, _accesoDatos);
        }
        public DateTime? ObtenerFechaUltimaAutomaticaInversionUtc(int usuarioId) => _notificacionDAL.ObtenerFechaUltimaAutomaticaInversionUtc(usuarioId, _accesoDatos);
        public bool BloquearUsuarioParaEntrega(int usuarioId) => _notificacionDAL.BloquearUsuarioParaEntrega(usuarioId, _accesoDatos);
        public bool ExisteAutomatica(int usuarioId, TipoNotificacionAutomatica tipo, DateTime jornada) => _notificacionDAL.ExisteAutomatica(usuarioId, tipo, jornada, _accesoDatos);

        private readonly NotificacionDAL _notificacionDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public NotificacionRepositorio()
            : this(null)
        {
        }

        internal NotificacionRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _notificacionDAL = new NotificacionDAL();
            _accesoDatos = accesoDatos;
        }

        public Notificacion ObtenerPorId(int id)
        {
            Notificacion notificacion = _notificacionDAL.ObtenerPorId(id, _accesoDatos);
            Hidratar(notificacion);
            return notificacion;
        }

        public List<Notificacion> ObtenerPorUsuario(int usuarioId)
        {
            List<Notificacion> notificaciones = _notificacionDAL.ObtenerPorUsuario(usuarioId, _accesoDatos);
            notificaciones.ForEach(Hidratar);
            return notificaciones;
        }

        public List<Notificacion> ObtenerTodas()
        {
            List<Notificacion> notificaciones = _notificacionDAL.ObtenerTodas(_accesoDatos);
            notificaciones.ForEach(Hidratar);
            return notificaciones;
        }

        private void Hidratar(Notificacion notificacion)
        {
            if (notificacion == null)
            {
                return;
            }

            notificacion.Usuario = new UsuarioRepositorio(_accesoDatos).ObtenerPorId(notificacion.Usuario.Id);
        }

        public int InsertarYObtenerId(Notificacion notificacion)
        {
            return _notificacionDAL.InsertarYObtenerId(notificacion, _accesoDatos);
        }

        public bool Actualizar(Notificacion notificacion)
        {
            return _notificacionDAL.Actualizar(notificacion, _accesoDatos);
        }

        public bool Eliminar(int id)
        {
            return _notificacionDAL.Eliminar(id, _accesoDatos);
        }

        public int ContarPorPromocion(int promocionId)
        {
            return _notificacionDAL.ContarPorPromocion(promocionId, _accesoDatos);
        }
    }
}
