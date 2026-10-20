using BE;

namespace REPOSITORIO
{
    public interface IPerfilInversorRepositorio
    {
        PerfilInversor ObtenerPorUsuario(int usuarioId);
        void Guardar(PerfilInversor perfil);
    }
}
