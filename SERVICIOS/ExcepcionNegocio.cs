using System;

namespace SERVICIOS
{
    public class ExcepcionNegocio : Exception
    {
        public ExcepcionNegocio(string messageKey)
            : base(messageKey)
        {
        }
    }
}
