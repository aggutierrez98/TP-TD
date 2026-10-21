using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ABSTRACCIONES;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SERVICIOS.Tests
{
    // Tarea 3.1: cliente de Open Exchange Rates (latest.json, base USD). Todas las pruebas
    // usan un manejador HTTP simulado: nunca se llama a la API real.
    [TestClass]
    public sealed class OpenExchangeRatesProveedorTests
    {
        private const string Clave = "clave-de-prueba-123";

        private const string JsonValido =
            "{\"disclaimer\":\"https://openexchangerates.org/terms/\",\"license\":\"https://openexchangerates.org/license/\"," +
            "\"timestamp\":1449877801,\"base\":\"USD\",\"rates\":{\"AED\":3.672538,\"ARS\":1513.930812345678,\"USD\":1}}";

        private static HttpResponseMessage Json(string cuerpo, HttpStatusCode codigo = HttpStatusCode.OK)
        {
            return new HttpResponseMessage(codigo)
            {
                Content = new StringContent(cuerpo, Encoding.UTF8, "application/json")
            };
        }

        private static string JsonError(int estado, string mensaje)
        {
            return "{\"error\":true,\"status\":" + estado + ",\"message\":\"" + mensaje + "\",\"description\":\"detalle\"}";
        }

        private static OpenExchangeRatesProveedor Crear(ManejadorHttpFalso manejador, string clave = Clave, TimeSpan? tiempoMaximo = null)
        {
            return new OpenExchangeRatesProveedor(manejador, () => clave, tiempoMaximo ?? TimeSpan.FromSeconds(5));
        }

        [TestMethod]
        public async Task ObtenerTasasAsync_RespuestaValida_DevuelveBaseFechaYTasas()
        {
            ManejadorHttpFalso manejador = new ManejadorHttpFalso(() => Json(JsonValido));
            OpenExchangeRatesProveedor proveedor = Crear(manejador);

            ITasasCambioObtenidas resultado = await proveedor.ObtenerTasasAsync(CancellationToken.None);

            Assert.AreEqual("USD", resultado.MonedaBase);
            Assert.AreEqual(DateTimeOffset.FromUnixTimeSeconds(1449877801).UtcDateTime, resultado.FechaFuenteUtc);
            Assert.AreEqual(3, resultado.Tasas.Count);
            Assert.AreEqual(3.672538m, resultado.Tasas["AED"]);
            Assert.AreEqual(1m, resultado.Tasas["USD"]);
        }

        [TestMethod]
        public async Task ObtenerTasasAsync_ValorConMuchosDecimales_ConservaLaPrecisionDecimal()
        {
            ManejadorHttpFalso manejador = new ManejadorHttpFalso(() => Json(JsonValido));
            OpenExchangeRatesProveedor proveedor = Crear(manejador);

            ITasasCambioObtenidas resultado = await proveedor.ObtenerTasasAsync(CancellationToken.None);

            Assert.AreEqual(1513.930812345678m, resultado.Tasas["ARS"]);
        }

        [TestMethod]
        public async Task ObtenerTasasAsync_RespuestaValida_PideLatestJsonConLaClaveYUnaSolaPeticion()
        {
            ManejadorHttpFalso manejador = new ManejadorHttpFalso(() => Json(JsonValido));
            OpenExchangeRatesProveedor proveedor = Crear(manejador);

            await proveedor.ObtenerTasasAsync(CancellationToken.None);

            Assert.AreEqual(1, manejador.CantidadPeticiones);
            PeticionCapturada peticion = manejador.Peticiones[0];
            Assert.AreEqual(HttpMethod.Get, peticion.Metodo);
            Assert.AreEqual("https", peticion.Uri.Scheme);
            Assert.AreEqual("openexchangerates.org", peticion.Uri.Host);
            Assert.AreEqual("/api/latest.json", peticion.Uri.AbsolutePath);
            StringAssert.Contains(peticion.Uri.Query, "app_id=" + Clave);
        }

        [TestMethod]
        public async Task ObtenerTasasAsync_SinTimestamp_DevuelveFechaFuenteNula()
        {
            string json = "{\"base\":\"USD\",\"rates\":{\"ARS\":1500.5}}";
            ManejadorHttpFalso manejador = new ManejadorHttpFalso(() => Json(json));
            OpenExchangeRatesProveedor proveedor = Crear(manejador);

            ITasasCambioObtenidas resultado = await proveedor.ObtenerTasasAsync(CancellationToken.None);

            Assert.IsNull(resultado.FechaFuenteUtc);
            Assert.AreEqual(1500.5m, resultado.Tasas["ARS"]);
        }

        [TestMethod]
        [DataRow("{no es json")]
        [DataRow("")]
        [DataRow("[]")]
        [DataRow("{\"base\":\"USD\"}")]
        [DataRow("{\"base\":\"USD\",\"rates\":[1,2]}")]
        [DataRow("{\"base\":\"USD\",\"rates\":{}}")]
        [DataRow("{\"base\":\"USD\",\"rates\":{\"ARS\":\"abc\"}}")]
        [DataRow("{\"base\":\"USD\",\"rates\":{\"ARS\":null}}")]
        [DataRow("{\"base\":\"EUR\",\"rates\":{\"ARS\":1500}}")]
        [DataRow("{\"rates\":{\"ARS\":1500}}")]
        [DataRow("{\"error\":true,\"status\":401,\"message\":\"invalid_app_id\"}")]
        public async Task ObtenerTasasAsync_CuerpoInvalidoConStatus200_LanzaRespuestaInvalida(string cuerpo)
        {
            ManejadorHttpFalso manejador = new ManejadorHttpFalso(() => Json(cuerpo));
            OpenExchangeRatesProveedor proveedor = Crear(manejador);

            ProveedorTasasException ex = await Assert.ThrowsExactlyAsync<ProveedorTasasException>(
                () => proveedor.ObtenerTasasAsync(CancellationToken.None));

            Assert.AreEqual(EstadoProveedorTasas.RespuestaInvalida, ex.Estado);
            Assert.AreEqual(1, manejador.CantidadPeticiones);
        }

        [TestMethod]
        [DataRow(401, "invalid_app_id", EstadoProveedorTasas.ClaveInvalida)]
        [DataRow(401, "missing_app_id", EstadoProveedorTasas.ClaveInvalida)]
        [DataRow(403, "access_restricted", EstadoProveedorTasas.ClaveInvalida)]
        [DataRow(429, "not_allowed", EstadoProveedorTasas.LimiteExcedido)]
        [DataRow(400, "invalid_base", EstadoProveedorTasas.Rechazado)]
        [DataRow(404, "not_found", EstadoProveedorTasas.Rechazado)]
        [DataRow(500, "internal_error", EstadoProveedorTasas.Rechazado)]
        public async Task ObtenerTasasAsync_ErrorDelProveedor_SeTraduceAEstadoYNoReintenta(int codigo, string mensaje, EstadoProveedorTasas esperado)
        {
            ManejadorHttpFalso manejador = new ManejadorHttpFalso(() => Json(JsonError(codigo, mensaje), (HttpStatusCode)codigo));
            OpenExchangeRatesProveedor proveedor = Crear(manejador);

            ProveedorTasasException ex = await Assert.ThrowsExactlyAsync<ProveedorTasasException>(
                () => proveedor.ObtenerTasasAsync(CancellationToken.None));

            Assert.AreEqual(esperado, ex.Estado);
            Assert.AreEqual(codigo, ex.CodigoHttp);
            Assert.AreEqual(mensaje, ex.CodigoProveedor);
            Assert.AreEqual(1, manejador.CantidadPeticiones);
        }

        [TestMethod]
        public async Task ObtenerTasasAsync_ErrorHttpConCuerpoNoJson_LanzaRechazadoSinCodigoDeProveedor()
        {
            ManejadorHttpFalso manejador = new ManejadorHttpFalso(() => new HttpResponseMessage(HttpStatusCode.BadGateway)
            {
                Content = new StringContent("<html>Bad Gateway</html>", Encoding.UTF8, "text/html")
            });
            OpenExchangeRatesProveedor proveedor = Crear(manejador);

            ProveedorTasasException ex = await Assert.ThrowsExactlyAsync<ProveedorTasasException>(
                () => proveedor.ObtenerTasasAsync(CancellationToken.None));

            Assert.AreEqual(EstadoProveedorTasas.Rechazado, ex.Estado);
            Assert.AreEqual(502, ex.CodigoHttp);
            Assert.IsNull(ex.CodigoProveedor);
        }

        [TestMethod]
        public async Task ObtenerTasasAsync_ErrorDelProveedor_NoExponeLaClaveEnElMensaje()
        {
            ManejadorHttpFalso manejador = new ManejadorHttpFalso(() => Json(JsonError(401, "invalid_app_id"), HttpStatusCode.Unauthorized));
            OpenExchangeRatesProveedor proveedor = Crear(manejador);

            ProveedorTasasException ex = await Assert.ThrowsExactlyAsync<ProveedorTasasException>(
                () => proveedor.ObtenerTasasAsync(CancellationToken.None));

            Assert.IsFalse(ex.Message.Contains(Clave));
        }

        [TestMethod]
        public async Task ObtenerTasasAsync_FalloDeRed_LanzaConexionFallidaSinReintentar()
        {
            ManejadorHttpFalso manejador = new ManejadorHttpFalso((p, ct) => Task.FromException<HttpResponseMessage>(new HttpRequestException("sin red")));
            OpenExchangeRatesProveedor proveedor = Crear(manejador);

            ProveedorTasasException ex = await Assert.ThrowsExactlyAsync<ProveedorTasasException>(
                () => proveedor.ObtenerTasasAsync(CancellationToken.None));

            Assert.AreEqual(EstadoProveedorTasas.ConexionFallida, ex.Estado);
            Assert.IsNull(ex.CodigoHttp);
            Assert.AreEqual(1, manejador.CantidadPeticiones);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task ObtenerTasasAsync_SinRespuestaEnElTiempoMaximo_LanzaConexionFallida()
        {
            ManejadorHttpFalso manejador = new ManejadorHttpFalso((p, ct) =>
                Task.Delay(Timeout.Infinite, ct).ContinueWith<HttpResponseMessage>(t => throw new TaskCanceledException(), TaskScheduler.Default));
            OpenExchangeRatesProveedor proveedor = Crear(manejador, tiempoMaximo: TimeSpan.FromMilliseconds(100));

            ProveedorTasasException ex = await Assert.ThrowsExactlyAsync<ProveedorTasasException>(
                () => proveedor.ObtenerTasasAsync(CancellationToken.None));

            Assert.AreEqual(EstadoProveedorTasas.ConexionFallida, ex.Estado);
            Assert.AreEqual(1, manejador.CantidadPeticiones);
        }

        [TestMethod]
        public async Task ObtenerTasasAsync_CanceladaAntesDeEnviar_LanzaCancelacionSinPeticion()
        {
            ManejadorHttpFalso manejador = new ManejadorHttpFalso(() => Json(JsonValido));
            OpenExchangeRatesProveedor proveedor = Crear(manejador);
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                cts.Cancel();

                await Assert.ThrowsAsync<OperationCanceledException>(() => proveedor.ObtenerTasasAsync(cts.Token));
            }

            Assert.AreEqual(0, manejador.CantidadPeticiones);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task ObtenerTasasAsync_CanceladaDuranteLaPeticion_LanzaCancelacionYNoConexionFallida()
        {
            TaskCompletionSource<bool> enviada = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            ManejadorHttpFalso manejador = new ManejadorHttpFalso((p, ct) =>
            {
                enviada.TrySetResult(true);
                return Task.Delay(Timeout.Infinite, ct).ContinueWith<HttpResponseMessage>(t => throw new TaskCanceledException(), TaskScheduler.Default);
            });
            OpenExchangeRatesProveedor proveedor = Crear(manejador);

            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                Task<ITasasCambioObtenidas> tarea = proveedor.ObtenerTasasAsync(cts.Token);
                await enviada.Task;
                cts.Cancel();

                await Assert.ThrowsAsync<OperationCanceledException>(() => tarea);
            }
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task ObtenerTasasAsync_ClaveFaltante_LanzaClaveFaltanteSinConsultarLaApi(string clave)
        {
            ManejadorHttpFalso manejador = new ManejadorHttpFalso(() => Json(JsonValido));
            OpenExchangeRatesProveedor proveedor = Crear(manejador, clave);

            ProveedorTasasException ex = await Assert.ThrowsExactlyAsync<ProveedorTasasException>(
                () => proveedor.ObtenerTasasAsync(CancellationToken.None));

            Assert.AreEqual(EstadoProveedorTasas.ClaveFaltante, ex.Estado);
            Assert.IsNull(ex.CodigoHttp);
            Assert.AreEqual(0, manejador.CantidadPeticiones);
        }

        [TestMethod]
        public void Constructor_ForzaTls12SinQuitarLosProtocolosExistentes()
        {
            SecurityProtocolType original = ServicePointManager.SecurityProtocol;
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

                Crear(new ManejadorHttpFalso(() => Json(JsonValido)));

                Assert.IsTrue((ServicePointManager.SecurityProtocol & SecurityProtocolType.Tls12) == SecurityProtocolType.Tls12);
                Assert.IsTrue((ServicePointManager.SecurityProtocol & SecurityProtocolType.Tls) == SecurityProtocolType.Tls);
            }
            finally
            {
                ServicePointManager.SecurityProtocol = original;
            }
        }
    }
}
