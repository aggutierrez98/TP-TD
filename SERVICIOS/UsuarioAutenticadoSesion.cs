using ABSTRACCIONES;

namespace SERVICIOS
{
    public class UsuarioAutenticadoSesion : IUsuarioAutenticado
    {
        public int Id { get; set; }

        public string Username { get; set; }
    }
}
