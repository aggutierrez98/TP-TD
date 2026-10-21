namespace ABSTRACCIONES
{
    public interface ISesion
    {
        void IniciarSesion(IUsuarioAutenticado usuario);

        void CerrarSesion();

        IUsuarioAutenticado GetUsuarioActual();

        string GetUsuarioActualNombre();

        void ActualizarUsuarioActualNombre(string username);

        bool HaySesionActiva();
    }
}
