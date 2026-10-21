using BE;

namespace REPOSITORIO
{
    public interface IPerfilInversorRepositorio
    {
        PerfilInversor ObtenerPorUsuario(Usuario usuario);
        void Guardar(PerfilInversor perfil);
    }
}
