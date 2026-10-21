namespace ABSTRACCIONES
{
    // Rectangulo geografico en grados decimales (WGS84). Contrato tecnico de
    // intercambio: sirve tanto de preferencia de busqueda (zona visible del
    // mapa) como de extension devuelta por el proveedor. No contiene tipos de
    // GMap.NET para que el servicio no dependa del control de mapa.
    public interface ILimitesGeograficos
    {
        double Sur { get; }

        double Oeste { get; }

        double Norte { get; }

        double Este { get; }
    }
}
