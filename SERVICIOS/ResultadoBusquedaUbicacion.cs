using ABSTRACCIONES;

namespace SERVICIOS
{
    // Implementacion de IResultadoBusquedaUbicacion.
    public sealed class ResultadoBusquedaUbicacion : IResultadoBusquedaUbicacion
    {
        public ResultadoBusquedaUbicacion(string descripcion, double latitud, double longitud, string tipo, ILimitesGeograficos limites)
        {
            Descripcion = descripcion;
            Latitud = latitud;
            Longitud = longitud;
            Tipo = tipo;
            Limites = limites;
        }

        public string Descripcion { get; }

        public double Latitud { get; }

        public double Longitud { get; }

        public string Tipo { get; }

        public ILimitesGeograficos Limites { get; }
    }
}
