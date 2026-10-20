namespace ABSTRACCIONES
{
    // Nombre visible de una entidad (comercio, promocion, cuenta o instrumento) que el
    // validador de respuestas busca en el texto generado por el modelo.
    public class ReferenciaAsistente
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
