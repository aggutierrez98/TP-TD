using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ABSTRACCIONES;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SERVICIOS.Tests
{
    [TestClass]
    public sealed class ProveedoresMercadoTests
    {
        private static readonly DateTime Hoy = new DateTime(2026, 10, 19);

        [TestMethod]
        public async Task ObtenerAsync_CachePorEndpoint_RespetaVigenciaCortaYLarga()
        {
            DateTime reloj = Hoy;
            MercadoHandler h = new MercadoHandler((r, ct) => Task.FromResult(Json("[{\"symbol\":\"A\",\"c\":1,\"v\":2}]")));
            var cotizaciones = new ProveedorMercadoData912(h, TimeSpan.FromSeconds(2), () => reloj);
            await cotizaciones.ObtenerCotizacionesAsync(true, CancellationToken.None);
            reloj = reloj.AddMinutes(14);
            await cotizaciones.ObtenerCotizacionesAsync(true, CancellationToken.None);
            Assert.HasCount(1, h.Rutas);
            await cotizaciones.ObtenerCotizacionesAsync(false, CancellationToken.None);
            Assert.HasCount(2, h.Rutas);
            reloj = reloj.AddMinutes(1);
            await cotizaciones.ObtenerCotizacionesAsync(true, CancellationToken.None);
            Assert.HasCount(3, h.Rutas);

            MercadoHandler tasasHandler = new MercadoHandler((r, ct) => Task.FromResult(Json("[{\"entidad\":\"Banco\",\"tnaClientes\":0.2}]")));
            var tasas = new ProveedorArgentinaDatos(tasasHandler, TimeSpan.FromSeconds(2), () => reloj);
            await tasas.ObtenerPlazosFijosAsync(CancellationToken.None);
            reloj = reloj.AddHours(5.99);
            await tasas.ObtenerPlazosFijosAsync(CancellationToken.None);
            Assert.HasCount(1, tasasHandler.Rutas);
            reloj = reloj.AddMinutes(1);
            await tasas.ObtenerPlazosFijosAsync(CancellationToken.None);
            Assert.HasCount(2, tasasHandler.Rutas);
        }

        [TestMethod]
        public async Task ObtenerAsync_FalloNoCacheado_ReintentaYCancelacionSigueVigenteConCache()
        {
            int llamadas = 0;
            MercadoHandler h = new MercadoHandler((r, ct) => Task.FromResult(Json(++llamadas == 1 ? "{}" : "[{\"symbol\":\"A\",\"c\":1,\"v\":2}]")));
            var p = new ProveedorMercadoData912(h, TimeSpan.FromSeconds(2), () => Hoy);
            await Assert.ThrowsAsync<ExcepcionTecnica>(() => p.ObtenerCotizacionesAsync(true, CancellationToken.None));
            await p.ObtenerCotizacionesAsync(true, CancellationToken.None);
            await p.ObtenerCotizacionesAsync(true, CancellationToken.None);
            Assert.AreEqual(2, llamadas);
            await Assert.ThrowsAsync<OperationCanceledException>(() => p.ObtenerCotizacionesAsync(true, new CancellationToken(true)));
        }

        [TestMethod]
        public async Task ObtenerAsync_FixturesReales_ParseaCamposYUnidadesSinConfundirTnaYApy()
        {
            MercadoHandler h = new MercadoHandler((r, ct) => Task.FromResult(Json(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fixtures", "Mercado", Archivo(r.RequestUri.AbsolutePath) + ".json")))));
            ProveedorArgentinaDatos a = new ProveedorArgentinaDatos(h, TimeSpan.FromSeconds(2), () => Hoy);
            var f = await a.ObtenerFondosAsync("mercadoDinero", null, CancellationToken.None);
            Assert.AreEqual(242182.465m, f[0].Vcp);
            Assert.AreEqual(new DateTime(2026, 10, 16), f[0].Fecha);
            Assert.IsNull(f[0].Moneda, "La fuente de VCP no identifica moneda; no asumir ARS.");
            var previo = await a.ObtenerFondosAsync("mercadoDinero", new DateTime(2026, 9, 18), CancellationToken.None);
            Assert.AreEqual(84890.11m, previo[0].Vcp);
            Assert.IsTrue(h.Rutas.Contains("/v1/finanzas/fci/mercadoDinero/2026/09/18"));
            var pf = await a.ObtenerPlazosFijosAsync(CancellationToken.None);
            Assert.AreEqual(19.25m, pf[0].Tasa);
            Assert.AreEqual("TNA clientes (%)", pf[0].TipoTasa);
            var cuentas = await a.ObtenerCuentasRemuneradasAsync(CancellationToken.None);
            Assert.HasCount(2, cuentas);
            Assert.AreEqual("ARS", cuentas[0].Moneda);
            Assert.AreEqual("APY (%)", cuentas[0].TipoTasa);
            Assert.AreEqual(16.9m, cuentas[0].Tasa);
            var inflacion = await a.ObtenerInflacionAsync(CancellationToken.None);
            Assert.AreEqual(1.9m, inflacion.Last().Tasa);
            var cotizaciones = await new ProveedorMercadoData912(h, TimeSpan.FromSeconds(2), () => Hoy).ObtenerCotizacionesAsync(true, CancellationToken.None);
            Assert.AreEqual("AAL", cotizaciones[0].Nombre);
            Assert.AreEqual(9640m, cotizaciones[0].Precio);
            Assert.AreEqual(9913m, cotizaciones[0].Volumen);
            Assert.IsNull(cotizaciones[0].Moneda, "No inventar la moneda de la especie.");
            var cripto = await new ProveedorCriptoYa(h, TimeSpan.FromSeconds(2), () => Hoy).ObtenerPreciosAsync("BTC", CancellationToken.None);
            Assert.AreEqual(134348093.81m, cripto.Single(c => c.Nombre == "BTC / ripio").Precio);
            Assert.IsTrue(cripto.All(c => c.Precio > 0));
        }

        [TestMethod]
        [DataRow("data912")]
        [DataRow("argentina")]
        [DataRow("cripto")]
        public async Task ObtenerAsync_ErrorHttpJsonInesperadoYTimeout_FallaComoFuenteTecnica(string fuente)
        {
            foreach (string cuerpo in new[] { "{}", "null", "no-json", "[]", "[{\"error\":true}]" })
                await Assert.ThrowsAsync<ExcepcionTecnica>(() => Consultar(fuente, new MercadoHandler((r, ct) => Task.FromResult(Json(cuerpo)))));
            await Assert.ThrowsAsync<ExcepcionTecnica>(() => Consultar(fuente, new MercadoHandler((r, ct) => Task.FromResult(Json("[]", HttpStatusCode.ServiceUnavailable)))));
            await Assert.ThrowsAsync<ExcepcionTecnica>(() => Consultar(fuente, new MercadoHandler(async (r, ct) => { await Task.Delay(5000, ct); return Json("[]"); }), TimeSpan.FromMilliseconds(15)));
        }

        [TestMethod]
        [DataRow("data912")]
        [DataRow("argentina")]
        [DataRow("cripto")]
        public async Task ObtenerAsync_CancelacionDelConsumidor_NoLaConvierteEnFalloDeFuente(string fuente)
        {
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                cts.Cancel();
                await Assert.ThrowsAsync<OperationCanceledException>(() => Consultar(fuente, new MercadoHandler((r, ct) => Task.FromResult(Json("[]"))), null, cts.Token));
            }
        }

        private static Task<IReadOnlyList<DatoMercado>> Consultar(string fuente, MercadoHandler h, TimeSpan? timeout = null, CancellationToken ct = default(CancellationToken))
        {
            TimeSpan maximo = timeout ?? TimeSpan.FromSeconds(2);
            if (fuente == "argentina") return new ProveedorArgentinaDatos(h, maximo, () => Hoy).ObtenerPlazosFijosAsync(ct);
            if (fuente == "cripto") return new ProveedorCriptoYa(h, maximo, () => Hoy).ObtenerPreciosAsync("BTC", ct);
            return new ProveedorMercadoData912(h, maximo, () => Hoy).ObtenerCotizacionesAsync(false, ct);
        }

        private static string Archivo(string ruta)
        {
            if (ruta.Contains("ultimo")) return "fci-ultimo";
            if (ruta.Contains("/2026/")) return "fci-historico";
            if (ruta.Contains("plazoFijo")) return "plazo-fijo";
            if (ruta.Contains("rendimientos")) return "rendimientos";
            if (ruta.Contains("inflacion")) return "inflacion";
            if (ruta.Contains("arg_cedears")) return "cedears";
            return "cripto";
        }

        internal static HttpResponseMessage Json(string texto, HttpStatusCode estado = HttpStatusCode.OK) => new HttpResponseMessage(estado) { Content = new StringContent(texto, Encoding.UTF8, "application/json") };
    }

    internal sealed class MercadoHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _responder;
        internal List<string> Rutas { get; } = new List<string>();
        internal MercadoHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> responder) { _responder = responder; }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Rutas.Add(request.RequestUri.AbsolutePath);
            return _responder(request, cancellationToken);
        }
    }
}
