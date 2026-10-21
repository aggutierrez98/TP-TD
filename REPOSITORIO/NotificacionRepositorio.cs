using System;
using System.Collections.Generic;
using BE;
using DAL;

namespace REPOSITORIO
{
    public class NotificacionRepositorio : INotificacionRepositorio
    {
        public Notificacion ObtenerPersonal(Notificacion recibida, Usuario usuario)
        {
            Notificacion notificacion = _notificacionDAL.ObtenerPersonal(recibida, usuario, _accesoDatos);
            Hidratar(notificacion);
            return notificacion;
        }
        public bool MarcarPersonalLeida(Notificacion notificacion, Usuario usuario) => _notificacionDAL.MarcarPersonalLeida(notificacion, usuario, _accesoDatos);
        public PaginaNotificaciones ObtenerPaginaPersonal(Usuario usuario, FiltroNotificacion filtro, int pagina)
        {
            PaginaNotificaciones resultado = _notificacionDAL.ObtenerPaginaPersonal(usuario, filtro, pagina, _accesoDatos);
            resultado.Elementos.ForEach(Hidratar);
            return resultado;
        }

        public int ContarNoLeidas(Usuario usuario)
        {
            return _notificacionDAL.ContarNoLeidas(usuario, _accesoDatos);
        }
        public DateTime? ObtenerFechaUltimaAutomaticaInversionUtc(Usuario usuario) => _notificacionDAL.ObtenerFechaUltimaAutomaticaInversionUtc(usuario, _accesoDatos);
        public bool BloquearUsuarioParaEntrega(Usuario usuario) => _notificacionDAL.BloquearUsuarioParaEntrega(usuario, _accesoDatos);
        public bool ExisteAutomatica(Usuario usuario, TipoNotificacionAutomatica tipo, DateTime jornada) => _notificacionDAL.ExisteAutomatica(usuario, tipo, jornada, _accesoDatos);

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

        public List<Notificacion> ObtenerPorUsuario(Usuario usuario)
        {
            List<Notificacion> notificaciones = _notificacionDAL.ObtenerPorUsuario(usuario, _accesoDatos);
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
            if (notificacion is RecordatorioCompra compra)
            {
                if (compra.ItemLista != null) compra.ItemLista = new ItemListaRepositorio(_accesoDatos).ObtenerPorId(compra.ItemLista.Id);
                if (compra.Promocion != null) compra.Promocion = new PromocionRepositorio(_accesoDatos).ObtenerPorId(compra.Promocion.Id);
            }
        }

        public int InsertarYObtenerId(Notificacion notificacion)
        {
            return _notificacionDAL.InsertarYObtenerId(notificacion, _accesoDatos);
        }

        public bool Actualizar(Notificacion notificacion)
        {
            return _notificacionDAL.Actualizar(notificacion, _accesoDatos);
        }

        public bool Eliminar(Notificacion notificacion)
        {
            return _notificacionDAL.Eliminar(notificacion, _accesoDatos);
        }

        public int ContarPorPromocion(Promocion promocion)
        {
            return _notificacionDAL.ContarPorPromocion(promocion, _accesoDatos);
        }
    }
}
