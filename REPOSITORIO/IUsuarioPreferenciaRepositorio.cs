using SERVICIOS;

namespace REPOSITORIO
{
    public interface IUsuarioPreferenciaRepositorio
    {
        UsuarioPreferencia ObtenerPorUsuarioId(int usuarioId);

        bool Guardar(UsuarioPreferencia preferencia);

        bool Guardar(int usuarioId, int? idiomaId);
    }
}
