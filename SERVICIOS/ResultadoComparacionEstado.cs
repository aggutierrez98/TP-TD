using ABSTRACCIONES;

namespace SERVICIOS
{
    public class ResultadoComparacionEstado : IResultadoComparacionEstado
    {
        public string Campo { get; set; }

        public string ValorAnterior { get; set; }

        public string ValorNuevo { get; set; }
    }
}
