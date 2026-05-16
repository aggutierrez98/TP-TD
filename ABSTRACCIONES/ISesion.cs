using System.Collections.Generic;

namespace ABSTRACCIONES
{
    public interface ISesion
    {
        void IniciarSesion(IUsuarioAutenticado usuario);

        void CerrarSesion();

        IUsuarioAutenticado GetUsuarioActual();

        bool HaySesionActiva();

        bool TienePermiso(string nombrePermiso);

        IList<IPermiso> ObtenerPermisosEfectivos();
    }
}
