using BE;
using DAL;

namespace REPOSITORIO
{
    public sealed class PerfilInversorRepositorio : IPerfilInversorRepositorio
    {
        public PerfilInversorRepositorio() : this(null) { }
        private readonly AccesoDatosSqlServer _acceso;
        private readonly PerfilInversorDAL _dal = new PerfilInversorDAL();
        internal PerfilInversorRepositorio(AccesoDatosSqlServer acceso) { _acceso = acceso; }
        public PerfilInversor ObtenerPorUsuario(Usuario usuario) => _dal.ObtenerPorUsuario(usuario, _acceso);
        public void Guardar(PerfilInversor perfil) => _dal.Guardar(perfil, _acceso);
    }
}
