using ABSTRACCIONES;
using BE;
using SERVICIOS;

namespace BLL
{
    // Punto de acceso unico de BLL al usuario en sesion. La sesion guarda el BE.Usuario
    // del login (UsuarioBLL); la conversion de IUsuarioAutenticado a Usuario vive solo aca.
    internal static class SesionUsuarioExtensiones
    {
        // Devuelve el Usuario en sesion; sin sesion (o con otro tipo guardado) lanza ExcepcionNegocio.
        internal static Usuario ObtenerUsuarioActual(this ISesion sesion)
        {
            Usuario usuario = sesion == null ? null : sesion.GetUsuarioActual() as Usuario;
            if (usuario == null)
            {
                throw new ExcepcionNegocio("FormUserSettings.Error.NoActiveSession");
            }

            return usuario;
        }
    }
}
