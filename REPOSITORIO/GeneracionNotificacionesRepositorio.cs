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

        public bool FinalizarCorrida(int corridaId, DateTime finUtc, int entregadas, int omitidas, int fallidas)
        {
            return _dal.FinalizarCorrida(corridaId, finUtc, entregadas, omitidas, fallidas, _accesoDatos);
        }

        public int InsertarDetalleYObtenerId(DetalleGeneracionNotificacion detalle)
        {
            return _dal.InsertarDetalleYObtenerId(detalle, _accesoDatos);
        }

        public CorridaGeneracionNotificaciones ObtenerCorrida(int id)
        {
            return _dal.ObtenerCorrida(id, _accesoDatos);
        }

        public List<CorridaGeneracionNotificaciones> ObtenerRecientes(int cantidad)
        {
            return _dal.ObtenerRecientes(cantidad, _accesoDatos);
        }

        public List<DetalleGeneracionNotificacion> ObtenerDetallePorCorrida(int corridaId)
        {
            return _dal.ObtenerDetallePorCorrida(corridaId, _accesoDatos);
        }
    }
}
