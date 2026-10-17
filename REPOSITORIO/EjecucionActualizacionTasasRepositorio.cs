using System;
using System.Collections.Generic;
using BE;
using DAL;

namespace REPOSITORIO
{
    public class EjecucionActualizacionTasasRepositorio : IEjecucionActualizacionTasasRepositorio
    {
        private readonly EjecucionActualizacionTasasDAL _ejecucionDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public EjecucionActualizacionTasasRepositorio()
            : this(null)
        {
        }

        internal EjecucionActualizacionTasasRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _ejecucionDAL = new EjecucionActualizacionTasasDAL();
            _accesoDatos = accesoDatos;
        }

        public int InsertarYObtenerId(EjecucionActualizacionTasas ejecucion)
        {
            return _ejecucionDAL.InsertarYObtenerId(ejecucion, _accesoDatos);
        }

        public int InsertarDetalleYObtenerId(ActualizacionTasaDetalle detalle)
        {
            return _ejecucionDAL.InsertarDetalleYObtenerId(detalle, _accesoDatos);
        }

        public List<EjecucionActualizacionTasas> ObtenerRecientes(int cantidad)
        {
            return _ejecucionDAL.ObtenerRecientes(cantidad, _accesoDatos);
        }

        public List<ActualizacionTasaDetalle> ObtenerDetallePorEjecucion(int ejecucionId)
        {
            List<ActualizacionTasaDetalle> detalles = _ejecucionDAL.ObtenerDetallePorEjecucion(ejecucionId, _accesoDatos);
            MonedaRepositorio monedaRepositorio = new MonedaRepositorio(_accesoDatos);
            foreach (ActualizacionTasaDetalle detalle in detalles)
            {
                detalle.MonedaBase = monedaRepositorio.ObtenerPorId(detalle.MonedaBase.Id);
                detalle.MonedaConversion = monedaRepositorio.ObtenerPorId(detalle.MonedaConversion.Id);
            }

            return detalles;
        }

        public EjecucionActualizacionTasas ObtenerUltimaExitosa()
        {
            return _ejecucionDAL.ObtenerUltimaExitosa(false, _accesoDatos);
        }

        public EjecucionActualizacionTasas ObtenerUltimaExitosaConBloqueo()
        {
            return _ejecucionDAL.ObtenerUltimaExitosa(true, _accesoDatos);
        }

        public int ContarRequestsDelMes(DateTime referencia)
        {
            return _ejecucionDAL.ContarRequestsDelMes(referencia, _accesoDatos);
        }
    }
}
