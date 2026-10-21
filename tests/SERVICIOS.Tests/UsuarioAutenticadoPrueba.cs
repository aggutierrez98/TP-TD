using ABSTRACCIONES;

namespace SERVICIOS.Tests
{
    // Doble de IUsuarioAutenticado para probar SERVICIOS sin depender de BE.Usuario.
    internal sealed class UsuarioAutenticadoPrueba : IUsuarioAutenticado
    {
        public int Id { get; set; }

        public string Username { get; set; }
    }
}
