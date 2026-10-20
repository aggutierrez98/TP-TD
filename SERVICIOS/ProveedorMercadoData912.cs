using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ABSTRACCIONES;

namespace SERVICIOS
{
    public sealed class ProveedorMercadoData912 : IProveedorMercadoData912
    {
        private readonly ClienteMercadoPublico _cliente;
        private readonly Func<DateTime> _ahora;
        public ProveedorMercadoData912() : this(new HttpClientHandler(), TimeSpan.FromSeconds(8), () => DateTime.UtcNow) { }
        internal ProveedorMercadoData912(HttpMessageHandler handler, TimeSpan timeout, Func<DateTime> ahora)
        {
            _cliente = new ClienteMercadoPublico(handler, timeout, ahora, TimeSpan.FromMinutes(15));
            _ahora = ahora;
        }
        public Task<IReadOnlyList<DatoMercado>> ObtenerCotizacionesAsync(bool cedears, CancellationToken cancellationToken)
        {
            return _cliente.ObtenerAsync("https://data912.com/live/" + (cedears ? "arg_cedears" : "arg_stocks"), raiz =>
            {
                var datos = new List<DatoMercado>();
                foreach (var fila in raiz.EnumerateArray())
                {
                    string nombre = ClienteMercadoPublico.Texto(fila, "symbol");
                    decimal? precio = ClienteMercadoPublico.Numero(fila, "c");
                    decimal? volumen = ClienteMercadoPublico.Numero(fila, "v");
                    if (!string.IsNullOrWhiteSpace(nombre) && precio > 0 && volumen >= 0)
                        datos.Add(new DatoMercado { Nombre = nombre, Tipo = cedears ? "CEDEAR" : "Accion", Fuente = "data912", Fecha = _ahora(), Precio = precio, Volumen = volumen });
                }
                return datos;
            }, cancellationToken);
        }
    }
}
