using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ABSTRACCIONES;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SERVICIOS.Tests
{
    // Tarea 2.3: timeout, cancelacion, errores de conexion y estados HTTP.
    [TestClass]
    public sealed class NominatimBusquedaUbicacionesErroresTests
    {
        private static ConsultaUbicacion Consulta()
        {
            return new ConsultaUbicacion("Obelisco", "es", null);
        }

        private static Task<HttpResponseMessage> EsperarCancelacionAsync(CancellationToken token)
        {
            return Task.Delay(Timeout.Infinite, token).ContinueWith<HttpResponseMessage>(t => throw new TaskCanceledException(), TaskScheduler.Default);
        }

        [TestMethod]
        [DataRow(403, EstadoBusquedaUbicacion.LimiteExcedido)]
        [DataRow(429, EstadoBusquedaUbicacion.LimiteExcedido)]
        [DataRow(503, EstadoBusquedaUbicacion.LimiteExcedido)]
        [DataRow(500, EstadoBusquedaUbicacion.ConexionFallida)]
        [DataRow(404, EstadoBusquedaUbicacion.ConexionFallida)]
        [DataRow(502, EstadoBusquedaUbicacion.ConexionFallida)]
        public async Task BuscarAsync_EstadoHttpNoExitoso_SeTraduceAEstadoYNoReintenta(int codigo, EstadoBusquedaUbicacion esperado)
        {
            ManejadorHttpFalso manejador = new ManejadorHttpFalso(() => BusquedaUbicacionesPruebas.RespuestaXml(BusquedaUbicacionesPruebas.XmlVacio, (HttpStatusCode)codigo));
            NominatimBusquedaUbicacionesService servicio = BusquedaUbicacionesPruebas.Crear(manejador);

            BusquedaUbicacionException ex = await Assert.ThrowsExactlyAsync<BusquedaUbicacionException>(
                () => servicio.BuscarAsync(Consulta(), CancellationToken.None));

            Assert.AreEqual(esperado, ex.Estado);
            Assert.AreEqual(codigo, ex.CodigoHttp);
            Assert.AreEqual(1, manejador.CantidadPeticiones);
            Assert.IsFalse(ex.Message.Contains("Obelisco"));
        }

        [TestMethod]
        public async Task BuscarAsync_FalloDeRed_LanzaConexionFallidaSinReintentar()
        {
            ManejadorHttpFalso manejador = new ManejadorHttpFalso((p, ct) => Task.FromException<HttpResponseMessage>(new HttpRequestException("sin red")));
            NominatimBusquedaUbicacionesService servicio = BusquedaUbicacionesPruebas.Crear(manejador);

            BusquedaUbicacionException ex = await Assert.ThrowsExactlyAsync<BusquedaUbicacionException>(
                () => servicio.BuscarAsync(Consulta(), CancellationToken.None));

            Assert.AreEqual(EstadoBusquedaUbicacion.ConexionFallida, ex.Estado);
            Assert.IsNull(ex.CodigoHttp);
            Assert.AreEqual(1, manejador.CantidadPeticiones);
        }

        [TestMethod]
        public async Task BuscarAsync_FalloAlLeerElCuerpo_LanzaConexionFallida()
        {
            ManejadorHttpFalso manejador = new ManejadorHttpFalso(() => new HttpResponseMessage(HttpStatusCode.OK) { Content = new ContenidoQueFalla() });
            NominatimBusquedaUbicacionesService servicio = BusquedaUbicacionesPruebas.Crear(manejador);

            BusquedaUbicacionException ex = await Assert.ThrowsExactlyAsync<BusquedaUbicacionException>(
                () => servicio.BuscarAsync(Consulta(), CancellationToken.None));

            Assert.AreEqual(EstadoBusquedaUbicacion.ConexionFallida, ex.Estado);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task BuscarAsync_SinRespuestaEnQuinceSegundos_LanzaTiempoAgotado()
        {
            RelojFalso reloj = new RelojFalso();
            ManejadorHttpFalso manejador = new ManejadorHttpFalso((p, ct) => EsperarCancelacionAsync(ct), reloj);
            NominatimBusquedaUbicacionesService servicio = BusquedaUbicacionesPruebas.Crear(manejador, reloj);

            Task<IReadOnlyList<ResultadoBusquedaUbicacion>> busqueda = servicio.BuscarAsync(Consulta(), CancellationToken.None);
            await reloj.EsperaPendienteAsync(TimeSpan.FromSeconds(15));
            reloj.Avanzar(TimeSpan.FromSeconds(14));
            Assert.IsFalse(busqueda.IsCompleted);
            reloj.Avanzar(TimeSpan.FromSeconds(1));

            BusquedaUbicacionException ex = await Assert.ThrowsExactlyAsync<BusquedaUbicacionException>(() => busqueda);
            Assert.AreEqual(EstadoBusquedaUbicacion.TiempoAgotado, ex.Estado);
            Assert.AreEqual(1, manejador.CantidadPeticiones);
        }

        [TestMethod]
        public async Task BuscarAsync_CanceladaAntesDeEnviar_LanzaCancelacionSinPeticion()
        {
            ManejadorHttpFalso manejador = new ManejadorHttpFalso(() => BusquedaUbicacionesPruebas.RespuestaXml(BusquedaUbicacionesPruebas.XmlVacio));
            NominatimBusquedaUbicacionesService servicio = BusquedaUbicacionesPruebas.Crear(manejador);
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                cts.Cancel();

                await Assert.ThrowsAsync<OperationCanceledException>(() => servicio.BuscarAsync(Consulta(), cts.Token));
            }

            Assert.AreEqual(0, manejador.CantidadPeticiones);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task BuscarAsync_CanceladaDuranteLaPeticion_LanzaCancelacionYNoTiempoAgotado()
        {
            RelojFalso reloj = new RelojFalso();
            TaskCompletionSource<bool> enviada = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            ManejadorHttpFalso manejador = new ManejadorHttpFalso((p, ct) =>
            {
                enviada.TrySetResult(true);
                return EsperarCancelacionAsync(ct);
            }, reloj);
            NominatimBusquedaUbicacionesService servicio = BusquedaUbicacionesPruebas.Crear(manejador, reloj);

            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                Task<IReadOnlyList<ResultadoBusquedaUbicacion>> busqueda = servicio.BuscarAsync(Consulta(), cts.Token);
                await enviada.Task;
                cts.Cancel();

                await Assert.ThrowsAsync<OperationCanceledException>(() => busqueda);
            }

            Assert.AreEqual(0, reloj.CantidadEsperasPendientes(TimeSpan.FromSeconds(15)));
            Assert.AreEqual(1, manejador.CantidadPeticiones);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task BuscarAsync_TrasUnTimeout_NoQuedanTemporizadoresNiReintentos()
        {
            RelojFalso reloj = new RelojFalso();
            ManejadorHttpFalso manejador = new ManejadorHttpFalso((p, ct) => EsperarCancelacionAsync(ct), reloj);
            NominatimBusquedaUbicacionesService servicio = BusquedaUbicacionesPruebas.Crear(manejador, reloj);

            Task<IReadOnlyList<ResultadoBusquedaUbicacion>> busqueda = servicio.BuscarAsync(Consulta(), CancellationToken.None);
            await reloj.EsperaPendienteAsync(TimeSpan.FromSeconds(15));
            reloj.Avanzar(TimeSpan.FromSeconds(15));
            await Assert.ThrowsExactlyAsync<BusquedaUbicacionException>(() => busqueda);
            reloj.Avanzar(TimeSpan.FromMinutes(5));

            Assert.AreEqual(1, manejador.CantidadPeticiones);
        }

        private sealed class ContenidoQueFalla : HttpContent
        {
            protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
            {
                return Task.FromException(new IOException("conexion interrumpida"));
            }

            protected override bool TryComputeLength(out long length)
            {
                length = -1;
                return false;
            }
        }
    }
}
