using SERVICIOS;
using ABSTRACCIONES;

namespace REPOSITORIO
{
    public interface IUsuarioPreferenciaRepositorio
    {
        UsuarioPreferencia ObtenerPorUsuario(IUsuarioAutenticado usuario);

        bool Guardar(UsuarioPreferencia preferencia);

        bool Guardar(IUsuarioAutenticado usuario, IIdioma idioma);
    }
}
