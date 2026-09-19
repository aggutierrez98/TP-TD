using System.Collections.Generic;
using BE;
using DAL;

namespace REPOSITORIO
{
    public class ComercioRepositorio : IComercioRepositorio
    {
        private readonly ComercioDAL _comercioDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public ComercioRepositorio()
            : this(null)
        {
        }

        internal ComercioRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _comercioDAL = new ComercioDAL();
            _accesoDatos = accesoDatos;
        }

        public Comercio ObtenerPorId(int id)
        {
            Comercio comercio = _comercioDAL.ObtenerPorId(id, _accesoDatos);
            Hidratar(comercio);
            return comercio;
        }

        public List<Comercio> ObtenerPorRubro(int rubroComercioId)
        {
            List<Comercio> comercios = _comercioDAL.ObtenerPorRubro(rubroComercioId, _accesoDatos);
            comercios.ForEach(Hidratar);
            return comercios;
        }

        public List<Comercio> ObtenerTodos()
        {
            List<Comercio> comercios = _comercioDAL.ObtenerTodos(_accesoDatos);
            comercios.ForEach(Hidratar);
            return comercios;
        }

        private void Hidratar(Comercio comercio)
        {
            if (comercio == null)
            {
                return;
            }

            comercio.RubroComercio = new RubroComercioRepositorio(_accesoDatos).ObtenerPorId(comercio.RubroComercio.Id);
        }

        public int InsertarYObtenerId(Comercio comercio)
        {
            return _comercioDAL.InsertarYObtenerId(comercio, _accesoDatos);
        }

        public bool Actualizar(Comercio comercio)
        {
            return _comercioDAL.Actualizar(comercio, _accesoDatos);
        }

        public bool Eliminar(int id)
        {
            return _comercioDAL.Eliminar(id, _accesoDatos);
        }
    }
}
