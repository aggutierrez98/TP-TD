namespace ABSTRACCIONES
{
    // Coincidencia de una busqueda de ubicaciones: descripcion legible,
    // coordenadas ya validadas, tipo descriptivo del proveedor (puede ser nulo)
    // y extension opcional usada para encuadrar el mapa (nula si no hay una
    // extension utilizable).
    public interface IResultadoBusquedaUbicacion
    {
        string Descripcion { get; }

        double Latitud { get; }

        double Longitud { get; }

        string Tipo { get; }

        ILimitesGeograficos Limites { get; }
    }
}
