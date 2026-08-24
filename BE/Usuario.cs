using ABSTRACCIONES;

namespace BE
{
    public class Usuario : IUsuarioAutenticado, IIntegrable
    {
        [DigitoVerificador(1)]
        public int Id { get; set; }

        [DigitoVerificador(2)]
        public string Username { get; set; }

        [DigitoVerificador(3)]
        public string PasswordHash { get; set; }

        [DigitoVerificador(4)]
        public bool Activo { get; set; }

        // Digito verificador horizontal (DVH) de la fila. Lo calcula IntegridadService.
        public string DVH { get; set; }

        public Usuario()
        {
            Activo = true;
        }

        public Usuario(string username)
            : this()
        {
            Username = username;
            Activo = true;
        }
    }
}
