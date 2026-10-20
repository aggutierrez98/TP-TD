using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using ABSTRACCIONES;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SERVICIOS.Tests
{
    // Tarea 2.4: serializacion, separacion minima de un segundo, cancelacion en
    // cola y cooldown por Retry-After. Todo con reloj controlado: sin sleeps.
    [TestClass]
    public sealed class NominatimBusquedaUbicacionesLimitadorTests
    {
        private static readonly TimeSpan UnSegundo = TimeSpan.FromSeconds(1);

        private static ConsultaUbicacion Consulta(string texto)
        {
            return new ConsultaUbicacion(texto, "es", null);
        }

        private static HttpResponseMessage RespuestaVacia()
        {
            return BusquedaUbicacionesPruebas.RespuestaXml(BusquedaUbicacionesPruebas.XmlVacio);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task BuscarAsync_DosConsultasConcurrentes_SeSerializanYSeSeparanUnSegundo()
        {
            RelojFalso reloj = new RelojFalso();
            TaskCompletionSource<bool> primeraEnVuelo = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            TaskCompletionSource<bool> liberarPrimera = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            ManejadorHttpFalso manejador = null;
            manejador = new ManejadorHttpFalso(async (p, ct) =>
            {
                if (manejador.CantidadPeticiones == 1)
                {
                    primeraEnVuelo.TrySetResult(true);
                    await liberarPrimera.Task;
                }

                return RespuestaVacia();
            }, reloj);
            NominatimBusquedaUbicacionesService servicio = BusquedaUbicacionesPruebas.Crear(manejador, reloj);

            Task primera = servicio.BuscarAsync(Consulta("uno"), CancellationToken.None);
            await primeraEnVuelo.Task;
            Task segunda = servicio.BuscarAsync(Consulta("dos"), CancellationToken.None);
            await Task.Yield();
            Assert.AreEqual(1, manejador.CantidadPeticiones, "La segunda no debe iniciar mientras la primera esta en curso.");

            liberarPrimera.SetResult(true);
            await primera;
            await reloj.EsperaPendienteAsync(UnSegundo);
            Assert.AreEqual(1, manejador.CantidadPeticiones, "La segunda debe esperar la separacion minima.");

            reloj.Avanzar(UnSegundo);
            await segunda;

            Assert.AreEqual(2, manejador.CantidadPeticiones);
            Assert.IsTrue(manejador.Peticiones[1].Instante - manejador.Peticiones[0].Instante >= UnSegundo);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task BuscarAsync_TresConsultasEnRafaga_NuncaHayDosEnVueloYSeparanSusInicios()
        {
            RelojFalso reloj = new RelojFalso();
            int enVuelo = 0;
            int maximoEnVuelo = 0;
            int siguientePeticion = -1;
            var iniciadas = Enumerable.Range(0, 3).Select(_ =>
                new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously)).ToArray();
            var liberar = Enumerable.Range(0, 3).Select(_ =>
                new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously)).ToArray();
            ManejadorHttpFalso manejador = new ManejadorHttpFalso(async (p, ct) =>
            {
                int indice = Interlocked.Increment(ref siguientePeticion);
                int actual = Interlocked.Increment(ref enVuelo);
                int visto;
                while ((visto = Volatile.Read(ref maximoEnVuelo)) < actual && Interlocked.CompareExchange(ref maximoEnVuelo, actual, visto) != visto)
                {
                }

                iniciadas[indice].SetResult(true);
                await liberar[indice].Task;
                Interlocked.Decrement(ref enVuelo);
                return RespuestaVacia();
            }, reloj);
            NominatimBusquedaUbicacionesService servicio = BusquedaUbicacionesPruebas.Crear(manejador, reloj);

            Task todas = Task.WhenAll(
                servicio.BuscarAsync(Consulta("a"), CancellationToken.None),
                servicio.BuscarAsync(Consulta("b"), CancellationToken.None),
                servicio.BuscarAsync(Consulta("c"), CancellationToken.None));
            // Avanzar solo cuando el servicio esta esperando la separacion minima.
            // El bucle anterior adelantaba el reloj tambien entre registrar el inicio
            // en el servicio y capturarlo en el handler, alterando la medicion.
            for (int i = 0; i < 3; i++)
            {
                await iniciadas[i].Task;
                Assert.AreEqual(i + 1, manejador.CantidadPeticiones);
                Assert.AreEqual(1, Volatile.Read(ref enVuelo));
                liberar[i].SetResult(true);
                if (i < 2)
                {
                    await reloj.EsperaPendienteAsync(UnSegundo);
                    Assert.AreEqual(i + 1, manejador.CantidadPeticiones);
                    reloj.Avanzar(UnSegundo);
                }
            }

            await todas;
            Assert.AreEqual(3, manejador.CantidadPeticiones);
            Assert.AreEqual(1, maximoEnVuelo);
            IReadOnlyList<PeticionCapturada> peticiones = manejador.Peticiones;
            Assert.IsTrue(peticiones[1].Instante - peticiones[0].Instante >= UnSegundo);
            Assert.IsTrue(peticiones[2].Instante - peticiones[1].Instante >= UnSegundo);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task BuscarAsync_SegundaConsultaTrasMedioSegundo_EsperaSoloLoQueFalta()
        {
            RelojFalso reloj = new RelojFalso();
            ManejadorHttpFalso manejador = new ManejadorHttpFalso(() => RespuestaVacia(), reloj);
            NominatimBusquedaUbicacionesService servicio = BusquedaUbicacionesPruebas.Crear(manejador, reloj);
            await servicio.BuscarAsync(Consulta("uno"), CancellationToken.None);
            reloj.Avanzar(TimeSpan.FromMilliseconds(400));

            Task segunda = servicio.BuscarAsync(Consulta("dos"), CancellationToken.None);
            await reloj.EsperaPendienteAsync(TimeSpan.FromMilliseconds(600));
            reloj.Avanzar(TimeSpan.FromMilliseconds(600));
            await segunda;

            Assert.AreEqual(2, manejador.CantidadPeticiones);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task BuscarAsync_ConUnSegundoYaTranscurrido_NoEspera()
        {
            RelojFalso reloj = new RelojFalso();
            ManejadorHttpFalso manejador = new ManejadorHttpFalso(() => RespuestaVacia(), reloj);
            NominatimBusquedaUbicacionesService servicio = BusquedaUbicacionesPruebas.Crear(manejador, reloj);
            await servicio.BuscarAsync(Consulta("uno"), CancellationToken.None);
            reloj.Avanzar(UnSegundo);

            await servicio.BuscarAsync(Consulta("dos"), CancellationToken.None);

            Assert.AreEqual(2, manejador.CantidadPeticiones);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task BuscarAsync_CanceladaMientrasEsperaEnCola_NoEnviaPeticionNiBloqueaLasSiguientes()
        {
            RelojFalso reloj = new RelojFalso();
            TaskCompletionSource<bool> primeraEnVuelo = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            TaskCompletionSource<bool> liberarPrimera = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            ManejadorHttpFalso manejador = null;
            manejador = new ManejadorHttpFalso(async (p, ct) =>
            {
                if (manejador.CantidadPeticiones == 1)
                {
                    primeraEnVuelo.TrySetResult(true);
                    await liberarPrimera.Task;
                }

                return RespuestaVacia();
            }, reloj);
            NominatimBusquedaUbicacionesService servicio = BusquedaUbicacionesPruebas.Crear(manejador, reloj);
            Task primera = servicio.BuscarAsync(Consulta("uno"), CancellationToken.None);
            await primeraEnVuelo.Task;

            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                Task enCola = servicio.BuscarAsync(Consulta("dos"), cts.Token);
                await Task.Yield();
                cts.Cancel();
                await Assert.ThrowsAsync<OperationCanceledException>(() => enCola);
            }

            liberarPrimera.SetResult(true);
            await primera;
            reloj.Avanzar(UnSegundo);
            await servicio.BuscarAsync(Consulta("tres"), CancellationToken.None);

            Assert.AreEqual(2, manejador.CantidadPeticiones);
            Assert.IsFalse(manejador.Peticiones.Any(x => x.Uri.Query.Contains("q=dos")));
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task BuscarAsync_CanceladaDuranteLaSeparacionMinima_NoEnviaPeticion()
        {
            RelojFalso reloj = new RelojFalso();
            ManejadorHttpFalso manejador = new ManejadorHttpFalso(() => RespuestaVacia(), reloj);
            NominatimBusquedaUbicacionesService servicio = BusquedaUbicacionesPruebas.Crear(manejador, reloj);
            await servicio.BuscarAsync(Consulta("uno"), CancellationToken.None);

            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                Task segunda = servicio.BuscarAsync(Consulta("dos"), cts.Token);
                await reloj.EsperaPendienteAsync(UnSegundo);
                cts.Cancel();

                await Assert.ThrowsAsync<OperationCanceledException>(() => segunda);
            }

            Assert.AreEqual(1, manejador.CantidadPeticiones);
            Assert.AreEqual(0, reloj.CantidadEsperasPendientes(UnSegundo));
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task BuscarAsync_Http429ConRetryAfterEnSegundos_RespetaElCooldownSinReintentar()
        {
            RelojFalso reloj = new RelojFalso();
            int llamada = 0;
            ManejadorHttpFalso manejador = new ManejadorHttpFalso(() =>
            {
                if (Interlocked.Increment(ref llamada) == 1)
                {
                    HttpResponseMessage limite = BusquedaUbicacionesPruebas.RespuestaXml(string.Empty, (HttpStatusCode)429);
                    limite.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromSeconds(120));
                    return limite;
                }

                return RespuestaVacia();
            }, reloj);
            NominatimBusquedaUbicacionesService servicio = BusquedaUbicacionesPruebas.Crear(manejador, reloj);

            BusquedaUbicacionException primero = await Assert.ThrowsExactlyAsync<BusquedaUbicacionException>(
                () => servicio.BuscarAsync(Consulta("uno"), CancellationToken.None));
            Assert.AreEqual(EstadoBusquedaUbicacion.LimiteExcedido, primero.Estado);
            Assert.AreEqual(TimeSpan.FromSeconds(120), primero.EsperaSugerida);

            reloj.Avanzar(TimeSpan.FromSeconds(20));
            BusquedaUbicacionException durante = await Assert.ThrowsExactlyAsync<BusquedaUbicacionException>(
                () => servicio.BuscarAsync(Consulta("dos"), CancellationToken.None));
            Assert.AreEqual(EstadoBusquedaUbicacion.LimiteExcedido, durante.Estado);
            Assert.AreEqual(TimeSpan.FromSeconds(100), durante.EsperaSugerida);
            Assert.AreEqual(1, manejador.CantidadPeticiones, "Durante el cooldown no se envia ninguna peticion externa.");

            reloj.Avanzar(TimeSpan.FromSeconds(100));
            await servicio.BuscarAsync(Consulta("tres"), CancellationToken.None);
            Assert.AreEqual(2, manejador.CantidadPeticiones);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task BuscarAsync_Http503ConRetryAfterEnFecha_ConvierteLaFechaEnEspera()
        {
            RelojFalso reloj = new RelojFalso();
            ManejadorHttpFalso manejador = new ManejadorHttpFalso(() =>
            {
                HttpResponseMessage limite = BusquedaUbicacionesPruebas.RespuestaXml(string.Empty, HttpStatusCode.ServiceUnavailable);
                limite.Headers.RetryAfter = new RetryConditionHeaderValue(reloj.AhoraUtc.AddSeconds(90));
                return limite;
            }, reloj);
            NominatimBusquedaUbicacionesService servicio = BusquedaUbicacionesPruebas.Crear(manejador, reloj);

            BusquedaUbicacionException ex = await Assert.ThrowsExactlyAsync<BusquedaUbicacionException>(
                () => servicio.BuscarAsync(Consulta("uno"), CancellationToken.None));

            Assert.AreEqual(TimeSpan.FromSeconds(90), ex.EsperaSugerida);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task BuscarAsync_Http429SinRetryAfter_UsaSesentaSegundos()
        {
            RelojFalso reloj = new RelojFalso();
            ManejadorHttpFalso manejador = new ManejadorHttpFalso(() => BusquedaUbicacionesPruebas.RespuestaXml(string.Empty, (HttpStatusCode)429), reloj);
            NominatimBusquedaUbicacionesService servicio = BusquedaUbicacionesPruebas.Crear(manejador, reloj);

            BusquedaUbicacionException ex = await Assert.ThrowsExactlyAsync<BusquedaUbicacionException>(
                () => servicio.BuscarAsync(Consulta("uno"), CancellationToken.None));

            Assert.AreEqual(TimeSpan.FromSeconds(60), ex.EsperaSugerida);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task BuscarAsync_RetryAfterExcesivo_SeAcotaAUnaHora()
        {
            RelojFalso reloj = new RelojFalso();
            ManejadorHttpFalso manejador = new ManejadorHttpFalso(() =>
            {
                HttpResponseMessage limite = BusquedaUbicacionesPruebas.RespuestaXml(string.Empty, (HttpStatusCode)429);
                limite.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromSeconds(999999));
                return limite;
            }, reloj);
            NominatimBusquedaUbicacionesService servicio = BusquedaUbicacionesPruebas.Crear(manejador, reloj);

            BusquedaUbicacionException ex = await Assert.ThrowsExactlyAsync<BusquedaUbicacionException>(
                () => servicio.BuscarAsync(Consulta("uno"), CancellationToken.None));

            Assert.AreEqual(TimeSpan.FromHours(1), ex.EsperaSugerida);
        }

        [TestMethod]
        [Timeout(10000)]
        [DataRow(503)]
        [DataRow(403)]
        public async Task BuscarAsync_RechazoSinRetryAfterNiRegla_NoImponeCooldown(int codigo)
        {
            RelojFalso reloj = new RelojFalso();
            ManejadorHttpFalso manejador = new ManejadorHttpFalso(() => BusquedaUbicacionesPruebas.RespuestaXml(string.Empty, (HttpStatusCode)codigo), reloj);
            NominatimBusquedaUbicacionesService servicio = BusquedaUbicacionesPruebas.Crear(manejador, reloj);

            BusquedaUbicacionException primero = await Assert.ThrowsExactlyAsync<BusquedaUbicacionException>(
                () => servicio.BuscarAsync(Consulta("uno"), CancellationToken.None));
            Assert.IsNull(primero.EsperaSugerida);

            reloj.Avanzar(UnSegundo);
            await Assert.ThrowsExactlyAsync<BusquedaUbicacionException>(() => servicio.BuscarAsync(Consulta("dos"), CancellationToken.None));
            Assert.AreEqual(2, manejador.CantidadPeticiones);
        }
    }
}
