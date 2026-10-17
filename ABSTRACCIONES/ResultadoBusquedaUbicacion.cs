namespace ABSTRACCIONES
{
    // Coincidencia de una busqueda de ubicaciones: descripcion legible,
    // coordenadas ya validadas, tipo descriptivo del proveedor (puede ser nulo)
    // y extension opcional usada para encuadrar el mapa (nula si no hay una
    // extension utilizable).
    public sealed class ResultadoBusquedaUbicacion
    {
        public ResultadoBusquedaUbicacion(string descripcion, double latitud, double longitud, string tipo, LimitesGeograficos limites)
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

        public LimitesGeograficos Limites { get; }
    }
}
