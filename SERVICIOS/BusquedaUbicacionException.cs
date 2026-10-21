using System;

namespace ABSTRACCIONES
{
    // Error tecnico de una busqueda de ubicaciones, tipado por estado para que
    // la UI lo traduzca sin interpretar mensajes. El mensaje no incluye el
    // texto buscado. EsperaSugerida se informa cuando el proveedor pidio esperar
    // (estado LimiteExcedido); CodigoHttp, cuando hubo respuesta HTTP.
    public sealed class BusquedaUbicacionException : Exception
    {
        public BusquedaUbicacionException(EstadoBusquedaUbicacion estado)
            : this(estado, null, null, null)
        {
        }

        public BusquedaUbicacionException(EstadoBusquedaUbicacion estado, Exception innerException)
            : this(estado, null, null, innerException)
        {
        }

        public BusquedaUbicacionException(EstadoBusquedaUbicacion estado, int? codigoHttp, TimeSpan? esperaSugerida, Exception innerException = null)
            : base("Error.Ubicacion.Search." + estado, innerException)
        {
            Estado = estado;
            CodigoHttp = codigoHttp;
            EsperaSugerida = esperaSugerida;
        }

        public EstadoBusquedaUbicacion Estado { get; }

        public int? CodigoHttp { get; }

        public TimeSpan? EsperaSugerida { get; }
    }
}
