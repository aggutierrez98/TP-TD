using System;

namespace ABSTRACCIONES
{
    // Rectangulo geografico en grados decimales (WGS84). Contrato tecnico de
    // intercambio: sirve tanto de preferencia de busqueda (zona visible del
    // mapa) como de extension devuelta por el proveedor. No contiene tipos de
    // GMap.NET para que el servicio no dependa del control de mapa.
    public sealed class LimitesGeograficos
    {
        public LimitesGeograficos(double sur, double oeste, double norte, double este)
        {
            Sur = sur;
            Oeste = oeste;
            Norte = norte;
            Este = este;
        }

        public double Sur { get; }

        public double Oeste { get; }

        public double Norte { get; }

        public double Este { get; }
    }
}
