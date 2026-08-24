using System.Collections.Generic;
using DAL;
using SERVICIOS;

namespace REPOSITORIO
{
    public class BitacoraRepositorio : IBitacoraRepositorio
    {
        private readonly BitacoraDAL _bitacoraDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public BitacoraRepositorio()
            : this(null)
        {
        }

        internal BitacoraRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _bitacoraDAL = new BitacoraDAL();
            _accesoDatos = accesoDatos;
        }

        public int Guardar(EventoBitacora evento)
        {
            return _bitacoraDAL.Guardar(evento, _accesoDatos);
        }

        public List<EventoBitacora> Buscar(FiltroBitacora filtro)
        {
            return _bitacoraDAL.Buscar(filtro);
        }

    }
}
