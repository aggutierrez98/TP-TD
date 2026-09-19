using System.Collections.Generic;
using BE;
using DAL;

namespace REPOSITORIO
{
    public class NotificacionRepositorio : INotificacionRepositorio
    {
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
    }
}
