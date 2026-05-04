using ABSTRACCIONES;

namespace SERVICIOS
{
    public class UsuarioPreferencia : IUsuarioPreferencia
    {
        public int UsuarioId { get; set; }

        public int IdiomaId { get; set; }
    }
}
