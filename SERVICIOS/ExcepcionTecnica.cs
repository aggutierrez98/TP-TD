using System;

namespace SERVICIOS
{
    public class ExcepcionTecnica : Exception
    {
        public ExcepcionTecnica(string message)
            : base(message)
        {
        }

        public ExcepcionTecnica(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
