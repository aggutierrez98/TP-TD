using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ABSTRACCIONES;

namespace SERVICIOS
{
    public sealed class ProveedorCriptoYa : IProveedorCriptoYa
    {
        private readonly ClienteMercadoPublico _cliente;
        public ProveedorCriptoYa() : this(new HttpClientHandler(), TimeSpan.FromSeconds(8), () => DateTime.UtcNow) { }
        internal ProveedorCriptoYa(HttpMessageHandler handler, TimeSpan timeout, Func<DateTime> ahora) { _cliente = new ClienteMercadoPublico(handler, timeout, ahora, TimeSpan.FromMinutes(15)); }
        public Task<IReadOnlyList<IDatoMercado>> ObtenerPreciosAsync(string moneda, CancellationToken cancellationToken)
        {
            if (moneda != "BTC" && moneda != "ETH" && moneda != "USDT") throw new ArgumentException("Moneda no admitida.", nameof(moneda));
            return _cliente.ObtenerAsync("https://criptoya.com/api/" + moneda.ToLowerInvariant() + "/ars/1", raiz =>
            {
                var datos = new List<DatoMercado>();
                foreach (var exchange in raiz.EnumerateObject())
                {
                    decimal? precio = ClienteMercadoPublico.Numero(exchange.Value, "totalAsk");
                    decimal? timestamp = ClienteMercadoPublico.Numero(exchange.Value, "time");
                    if (precio > 0 && timestamp > 0 && timestamp < 253402300799m)
                        datos.Add(new DatoMercado { Nombre = moneda + " / " + exchange.Name, Tipo = "Cripto " + moneda, Fuente = "criptoya", Fecha = DateTimeOffset.FromUnixTimeSeconds((long)timestamp.Value).UtcDateTime, Precio = precio, Moneda = "ARS" });
                }
                return datos;
            }, cancellationToken);
        }
    }
}
