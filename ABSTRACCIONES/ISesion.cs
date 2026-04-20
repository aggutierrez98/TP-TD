namespace ABSTRACCIONES
{
    public interface ISesion
    {
        void IniciarSesion(object usuario);

        void CerrarSesion();

        object GetUsuarioActual();

        bool HaySesionActiva();
    }
}
