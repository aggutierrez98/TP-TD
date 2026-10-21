using System;
using System.Collections.Generic;
using BE;
using DAL;

namespace REPOSITORIO
{
    public class GeneracionNotificacionesRepositorio : IGeneracionNotificacionesRepositorio
    {
        private readonly GeneracionNotificacionesDAL _dal;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public GeneracionNotificacionesRepositorio()
            : this(null)
        {
        }

        internal GeneracionNotificacionesRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _dal = new GeneracionNotificacionesDAL();
            _accesoDatos = accesoDatos;
        }

        public int InsertarCorridaYObtenerId(CorridaGeneracionNotificaciones corrida)
        {
            return _dal.InsertarCorridaYObtenerId(corrida, _accesoDatos);
        }

        public bool FinalizarCorrida(CorridaGeneracionNotificaciones corrida, DateTime finUtc, int entregadas, int omitidas, int fallidas)
        {
            return _dal.FinalizarCorrida(corrida, finUtc, entregadas, omitidas, fallidas, _accesoDatos);
        }

        public int InsertarDetalleYObtenerId(DetalleGeneracionNotificacion detalle)
        {
            return _dal.InsertarDetalleYObtenerId(detalle, _accesoDatos);
        }

        public CorridaGeneracionNotificaciones ObtenerPorId(int id)
        {
            return _dal.ObtenerPorId(id, _accesoDatos);
        }

        public List<CorridaGeneracionNotificaciones> ObtenerRecientes(int cantidad)
        {
            return _dal.ObtenerRecientes(cantidad, _accesoDatos);
        }

        public List<DetalleGeneracionNotificacion> ObtenerDetallePorCorrida(CorridaGeneracionNotificaciones corrida)
        {
            List<DetalleGeneracionNotificacion> detalles = _dal.ObtenerDetallePorCorrida(corrida, _accesoDatos);
            UsuarioRepositorio usuarios = new UsuarioRepositorio(_accesoDatos);
            NotificacionRepositorio notificaciones = new NotificacionRepositorio(_accesoDatos);
            RecordatorioCompraRepositorio compras = new RecordatorioCompraRepositorio(_accesoDatos);
            RecordatorioInversionRepositorio inversiones = new RecordatorioInversionRepositorio(_accesoDatos);
            foreach (DetalleGeneracionNotificacion detalle in detalles)
            {
                detalle.Usuario = usuarios.ObtenerPorId(detalle.Usuario.Id);
                if (detalle.Notificacion != null)
                {
                    int id = detalle.Notificacion.Id;
                    detalle.Notificacion = notificaciones.ObtenerPorId(id)
                        ?? (Notificacion)compras.ObtenerPorId(id)
                        ?? inversiones.ObtenerPorId(id);
                }
            }
            return detalles;
        }
    }
}
