using ABSTRACCIONES;

namespace SERVICIOS
{
    public class UsuarioPreferencia : IUsuarioPreferencia
    {
        public IUsuarioAutenticado Usuario { get; set; }

        public IIdioma Idioma { get; set; }
    }
}
