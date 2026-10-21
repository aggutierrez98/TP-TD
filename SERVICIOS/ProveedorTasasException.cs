using System;
using ABSTRACCIONES;

namespace SERVICIOS
{
    // Error tecnico del proveedor de tasas de cambio, tipado por estado. El mensaje
    // nunca incluye la clave ni la URL consultada. CodigoHttp se informa cuando hubo
    // respuesta HTTP y CodigoProveedor, cuando el cuerpo trajo un codigo de error
    // propio del proveedor (por ejemplo invalid_app_id).
    public sealed class ProveedorTasasException : Exception
    {
        public ProveedorTasasException(EstadoProveedorTasas estado)
            : this(estado, null, null, null)
        {
        }

        public ProveedorTasasException(EstadoProveedorTasas estado, Exception innerException)
            : this(estado, null, null, innerException)
        {
        }

        public ProveedorTasasException(EstadoProveedorTasas estado, int? codigoHttp, string codigoProveedor, Exception innerException = null)
            : base("Error.TasasCambio.Provider." + estado, innerException)
        {
            Estado = estado;
            CodigoHttp = codigoHttp;
            CodigoProveedor = codigoProveedor;
        }

        public EstadoProveedorTasas Estado { get; }

        public int? CodigoHttp { get; }

        public string CodigoProveedor { get; }
    }
}
