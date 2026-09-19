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
            Categoria categoria = _categoriaDAL.ObtenerPorId(id, _accesoDatos);
            Hidratar(categoria);
            return categoria;
        }

        public List<Categoria> ObtenerVisiblesParaUsuario(int usuarioId)
        {
            List<Categoria> categorias = _categoriaDAL.ObtenerVisiblesParaUsuario(usuarioId, _accesoDatos);
            categorias.ForEach(Hidratar);
            return categorias;
        }

        private void Hidratar(Categoria categoria)
        {
            if (categoria == null || categoria.Usuario == null)
            {
                return;
            }

            categoria.Usuario = new UsuarioRepositorio(_accesoDatos).ObtenerPorId(categoria.Usuario.Id);
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
