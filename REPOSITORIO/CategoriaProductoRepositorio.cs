using System.Collections.Generic;
using BE;
using DAL;

namespace REPOSITORIO
{
    public class CategoriaProductoRepositorio : ICategoriaProductoRepositorio
    {
        private readonly CategoriaProductoDAL _categoriaProductoDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public CategoriaProductoRepositorio()
            : this(null)
        {
        }

        internal CategoriaProductoRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _categoriaProductoDAL = new CategoriaProductoDAL();
            _accesoDatos = accesoDatos;
        }

        public CategoriaProducto ObtenerPorId(int id)
        {
            return _categoriaProductoDAL.ObtenerPorId(id, _accesoDatos);
        }

        public List<CategoriaProducto> ObtenerTodas()
        {
            return _categoriaProductoDAL.ObtenerTodas(_accesoDatos);
        }

        public int InsertarYObtenerId(CategoriaProducto categoria)
        {
            return _categoriaProductoDAL.InsertarYObtenerId(categoria, _accesoDatos);
        }

        public bool Actualizar(CategoriaProducto categoria)
        {
            return _categoriaProductoDAL.Actualizar(categoria, _accesoDatos);
        }

        public bool Eliminar(int id)
        {
            return _categoriaProductoDAL.Eliminar(id, _accesoDatos);
        }
    }
}
