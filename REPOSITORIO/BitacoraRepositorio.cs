using System.Collections.Generic;
using DAL;
using SERVICIOS;

namespace REPOSITORIO
{
    public class BitacoraRepositorio : IBitacoraRepositorio
    {
        private readonly BitacoraDAL _bitacoraDAL;

        public BitacoraRepositorio()
        {
            _bitacoraDAL = new BitacoraDAL();
        }

        public int Guardar(EventoBitacora evento, IUnidadDeTrabajo unidad = null)
        {
            return _bitacoraDAL.Guardar(evento, ObtenerAccesoDatos(unidad));
        }

        public List<EventoBitacora> Buscar(FiltroBitacora filtro)
        {
            return _bitacoraDAL.Buscar(filtro);
        }

        private static AccesoDatosSqlServer ObtenerAccesoDatos(IUnidadDeTrabajo unidad)
        {
            return unidad == null ? null : ((UnidadDeTrabajoSqlServer)unidad).AccesoDatosSqlServer;
        }
    }
}
