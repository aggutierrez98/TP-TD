using ABSTRACCIONES;

namespace SERVICIOS
{
    // Implementacion de ILimitesGeograficos.
    public sealed class LimitesGeograficos : ILimitesGeograficos
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
