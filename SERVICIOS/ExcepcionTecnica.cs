using System;

namespace SERVICIOS
{
    public class ExcepcionTecnica : Exception
    {
        public ExcepcionTecnica(string messageKey)
            : base(messageKey)
        {
        }

        public ExcepcionTecnica(string messageKey, Exception innerException)
            : base(messageKey, innerException)
        {
        }
    }
}
