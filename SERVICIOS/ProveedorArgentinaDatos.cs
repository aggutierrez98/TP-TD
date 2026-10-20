using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Globalization;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ABSTRACCIONES;

namespace SERVICIOS
{
    public sealed class ProveedorArgentinaDatos : IProveedorArgentinaDatos
    {
        private const string Base = "https://api.argentinadatos.com/v1/finanzas/";
        private readonly ClienteMercadoPublico _cliente;
        private readonly Func<DateTime> _ahora;
        public ProveedorArgentinaDatos() : this(new HttpClientHandler(), TimeSpan.FromSeconds(8), () => DateTime.UtcNow) { }
        internal ProveedorArgentinaDatos(HttpMessageHandler handler, TimeSpan timeout, Func<DateTime> ahora)
        {
            _cliente = new ClienteMercadoPublico(handler, timeout, ahora, TimeSpan.FromHours(6));
            _ahora = ahora;
        }

        public Task<IReadOnlyList<DatoMercado>> ObtenerFondosAsync(string tipo, DateTime? fecha, CancellationToken cancellationToken)
        {
            if (tipo != "mercadoDinero" && tipo != "rentaFija" && tipo != "rentaVariable") throw new ArgumentException("Tipo de fondo no admitido.", nameof(tipo));
            return _cliente.ObtenerAsync(Base + "fci/" + tipo + "/" + (fecha.HasValue ? fecha.Value.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture) : "ultimo"), raiz =>
            {
                var datos = new List<DatoMercado>();
                foreach (var fila in raiz.EnumerateArray())
                {
                    string nombre = ClienteMercadoPublico.Texto(fila, "fondo");
                    DateTime? dia = ClienteMercadoPublico.Fecha(fila, "fecha");
                    decimal? vcp = ClienteMercadoPublico.Numero(fila, "vcp");
                    if (!string.IsNullOrWhiteSpace(nombre) && dia.HasValue && vcp > 0)
                        datos.Add(new DatoMercado { Nombre = nombre, Tipo = "FCI " + tipo, Fuente = "ArgentinaDatos", Fecha = dia.Value, Vcp = vcp });
                }
                return datos;
            }, cancellationToken);
        }

        public Task<IReadOnlyList<DatoMercado>> ObtenerPlazosFijosAsync(CancellationToken cancellationToken)
        {
            return _cliente.ObtenerAsync(Base + "tasas/plazoFijo", raiz =>
            {
                var datos = new List<DatoMercado>();
                foreach (var fila in raiz.EnumerateArray())
                {
                    string nombre = ClienteMercadoPublico.Texto(fila, "entidad");
                    decimal? tasa = ClienteMercadoPublico.Numero(fila, "tnaClientes");
                    if (!string.IsNullOrWhiteSpace(nombre) && tasa > 0)
                        datos.Add(new DatoMercado { Nombre = nombre, Tipo = "Plazo fijo", Fuente = "ArgentinaDatos", Fecha = _ahora(), Tasa = tasa * 100m, TipoTasa = "TNA clientes (%)", Moneda = "ARS" });
                }
                return datos;
            }, cancellationToken);
        }

        public Task<IReadOnlyList<DatoMercado>> ObtenerCuentasRemuneradasAsync(CancellationToken cancellationToken)
        {
            return _cliente.ObtenerAsync(Base + "rendimientos", raiz =>
            {
                var datos = new List<DatoMercado>();
                foreach (var fila in raiz.EnumerateArray())
                {
                    string nombre = ClienteMercadoPublico.Texto(fila, "entidad");
                    if (string.IsNullOrWhiteSpace(nombre) || !fila.TryGetProperty("rendimientos", out JsonElement rendimientos) || rendimientos.ValueKind != JsonValueKind.Array) continue;
                    foreach (var rendimiento in rendimientos.EnumerateArray())
                    {
                        DateTime? dia = ClienteMercadoPublico.Fecha(rendimiento, "fecha");
                        decimal? tasa = ClienteMercadoPublico.Numero(rendimiento, "apy");
                        if (ClienteMercadoPublico.Texto(rendimiento, "moneda") == "ARS" && dia.HasValue && tasa > 0)
                            datos.Add(new DatoMercado { Nombre = nombre, Tipo = "Cuenta remunerada", Fuente = "ArgentinaDatos", Fecha = dia.Value, Tasa = tasa, TipoTasa = "APY (%)", Moneda = "ARS" });
                    }
                }
                return datos;
            }, cancellationToken);
        }

        public Task<IReadOnlyList<DatoMercado>> ObtenerInflacionAsync(CancellationToken cancellationToken)
        {
            return _cliente.ObtenerAsync(Base + "indices/inflacion", raiz =>
            {
                var datos = new List<DatoMercado>();
                foreach (var fila in raiz.EnumerateArray())
                {
                    DateTime? dia = ClienteMercadoPublico.Fecha(fila, "fecha");
                    decimal? tasa = ClienteMercadoPublico.Numero(fila, "valor");
                    if (dia.HasValue && tasa.HasValue)
                        datos.Add(new DatoMercado { Nombre = "Inflacion mensual", Tipo = "Inflacion", Fuente = "ArgentinaDatos", Fecha = dia.Value, Tasa = tasa, TipoTasa = "Mensual (%)" });
                }
                return datos;
            }, cancellationToken);
        }
    }
}
