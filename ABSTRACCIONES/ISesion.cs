namespace ABSTRACCIONES
{
    public interface ISesion
    {
        void IniciarSesion(IUsuarioAutenticado usuario);

        void CerrarSesion();

        IUsuarioAutenticado GetUsuarioActual();

        int? GetUsuarioActualId();

        string GetUsuarioActualNombre();

        void ActualizarUsuarioActualNombre(string username);

        bool HaySesionActiva();
    }
}
