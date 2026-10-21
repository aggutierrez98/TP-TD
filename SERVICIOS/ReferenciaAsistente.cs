using ABSTRACCIONES;

namespace SERVICIOS
{
    // Implementacion de IReferenciaAsistente.
    public class ReferenciaAsistente : IReferenciaAsistente
    {
        public ReferenciaAsistente(TipoReferenciaAsistente tipo, string nombre, bool esTicker = false)
        {
            Tipo = tipo;
            Nombre = nombre;
            EsTicker = esTicker;
        }

        public TipoReferenciaAsistente Tipo { get; private set; }

        public string Nombre { get; private set; }
        public bool EsTicker { get; private set; }
    }
}
