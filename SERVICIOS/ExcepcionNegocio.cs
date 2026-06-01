using System;

namespace SERVICIOS
{
    public class ExcepcionNegocio : Exception
    {
        public ExcepcionNegocio(string message)
            : base(message)
        {
        }
    }
}
