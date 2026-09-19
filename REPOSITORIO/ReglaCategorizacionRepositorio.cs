using System.Collections.Generic;
using BE;
using DAL;

namespace REPOSITORIO
{
    public class ReglaCategorizacionRepositorio : IReglaCategorizacionRepositorio
    {
        private readonly ReglaCategorizacionDAL _reglaCategorizacionDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public ReglaCategorizacionRepositorio()
            : this(null)
        {
        }

        internal ReglaCategorizacionRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _reglaCategorizacionDAL = new ReglaCategorizacionDAL();
            _accesoDatos = accesoDatos;
        }

        public ReglaCategorizacion ObtenerPorId(int id)
        {
            ReglaCategorizacion reglaCategorizacion = _reglaCategorizacionDAL.ObtenerPorId(id, _accesoDatos);
            Hidratar(reglaCategorizacion);
            return reglaCategorizacion;
        }

        public List<ReglaCategorizacion> ObtenerPorUsuario(int usuarioId)
        {
            List<ReglaCategorizacion> reglas = _reglaCategorizacionDAL.ObtenerPorUsuario(usuarioId, _accesoDatos);
            reglas.ForEach(Hidratar);
            return reglas;
        }

        private void Hidratar(ReglaCategorizacion reglaCategorizacion)
        {
            if (reglaCategorizacion == null)
            {
                return;
            }

            reglaCategorizacion.Usuario = new UsuarioRepositorio(_accesoDatos).ObtenerPorId(reglaCategorizacion.Usuario.Id);
            reglaCategorizacion.Categoria = new CategoriaRepositorio(_accesoDatos).ObtenerPorId(reglaCategorizacion.Categoria.Id);
        }

        public int InsertarYObtenerId(ReglaCategorizacion reglaCategorizacion)
        {
            return _reglaCategorizacionDAL.InsertarYObtenerId(reglaCategorizacion, _accesoDatos);
        }

        public bool Actualizar(ReglaCategorizacion reglaCategorizacion)
        {
            return _reglaCategorizacionDAL.Actualizar(reglaCategorizacion, _accesoDatos);
        }

        public bool Eliminar(int id)
        {
            return _reglaCategorizacionDAL.Eliminar(id, _accesoDatos);
        }
    }
}
