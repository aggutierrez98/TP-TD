using System.Collections.Generic;
using BE;
using DAL;

namespace REPOSITORIO
{
    public class CategoriaRepositorio : ICategoriaRepositorio
    {
        private readonly CategoriaDAL _categoriaDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public CategoriaRepositorio()
            : this(null)
        {
        }

        internal CategoriaRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _categoriaDAL = new CategoriaDAL();
            _accesoDatos = accesoDatos;
        }

        public Categoria ObtenerPorId(int id)
        {
            return _categoriaDAL.ObtenerPorId(id, _accesoDatos);
        }

        public List<Categoria> ObtenerVisiblesParaUsuario(int usuarioId)
        {
            return _categoriaDAL.ObtenerVisiblesParaUsuario(usuarioId, _accesoDatos);
        }

        public int InsertarYObtenerId(Categoria categoria)
        {
            return _categoriaDAL.InsertarYObtenerId(categoria, _accesoDatos);
        }

        public bool Actualizar(Categoria categoria)
        {
            return _categoriaDAL.Actualizar(categoria, _accesoDatos);
        }

        public bool Eliminar(int id)
        {
            return _categoriaDAL.Eliminar(id, _accesoDatos);
        }

        public bool ExisteNombreEnScope(string nombre, int? usuarioId, int? idExcluido)
        {
            return _categoriaDAL.ExisteNombreEnScope(nombre, usuarioId, idExcluido, _accesoDatos);
        }
    }
}
