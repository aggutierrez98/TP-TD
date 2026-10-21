using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ABSTRACCIONES;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SERVICIOS.Tests
{
    // Tarea 2.5: cache en memoria de exitos y listas vacias (TTL 24 h, maximo 100
    // entradas), clave normalizada y revalidacion dentro de la exclusion.
    [TestClass]
    public sealed class NominatimBusquedaUbicacionesCacheTests
    {
        private static readonly TimeSpan UnSegundo = TimeSpan.FromSeconds(1);

        private sealed class Escenario
        {
            public RelojFalso Reloj = new RelojFalso();
            public ManejadorHttpFalso Manejador;
            public NominatimBusquedaUbicacionesService Servicio;
            public string Endpoint = BusquedaUbicacionesPruebas.EndpointPorDefecto;

            public Escenario(Func<PeticionCapturada, CancellationToken, Task<HttpResponseMessage>> responder = null)
            {
                Manejador = new ManejadorHttpFalso(responder ?? ResponderConEcoDelTexto, Reloj);
                Servicio = new NominatimBusquedaUbicacionesService(Manejador, () => Endpoint, Reloj);
            }

            // Avanza un segundo antes para no depender de la espera de separacion.
            public async Task<IReadOnlyList<IResultadoBusquedaUbicacion>> BuscarAsync(string texto, string idioma = "es", ILimitesGeograficos zona = null)
            {
                Reloj.Avanzar(UnSegundo);
                return await Servicio.BuscarAsync(new ConsultaUbicacion(texto, idioma, zona), CancellationToken.None);
            }
        }

        private static Task<HttpResponseMessage> ResponderConEcoDelTexto(PeticionCapturada peticion, CancellationToken ct)
        {
            string q = BusquedaUbicacionesPruebas.ParsearConsulta(peticion.Uri)["q"];
            string xml = BusquedaUbicacionesPruebas.Resultados(BusquedaUbicacionesPruebas.Place(q, "1", "2"));
            return Task.FromResult(BusquedaUbicacionesPruebas.RespuestaXml(xml));
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task BuscarAsync_ConsultaRepetida_ReutilizaLaRespuestaSinPeticionExterna()
        {
            Escenario e = new Escenario();

            IReadOnlyList<IResultadoBusquedaUbicacion> primera = await e.BuscarAsync("Obelisco");
            IReadOnlyList<IResultadoBusquedaUbicacion> segunda = await e.BuscarAsync("Obelisco");

            Assert.AreEqual(1, e.Manejador.CantidadPeticiones);
            Assert.AreEqual(1, segunda.Count);
            Assert.AreEqual(primera[0].Descripcion, segunda[0].Descripcion);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task BuscarAsync_ListaVaciaTambienSeCachea()
        {
            Escenario e = new Escenario((p, ct) => Task.FromResult(BusquedaUbicacionesPruebas.RespuestaXml(BusquedaUbicacionesPruebas.XmlVacio)));

            await e.BuscarAsync("Lugar inexistente");
            IReadOnlyList<IResultadoBusquedaUbicacion> repetida = await e.BuscarAsync("Lugar inexistente");

            Assert.AreEqual(1, e.Manejador.CantidadPeticiones);
            Assert.AreEqual(0, repetida.Count);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task BuscarAsync_TextoConEspaciosSobrantes_ComparteEntradaDeCache()
        {
            Escenario e = new Escenario();

            await e.BuscarAsync("Obelisco Buenos Aires");
            await e.BuscarAsync("  Obelisco   Buenos \t Aires  ");

            Assert.AreEqual(1, e.Manejador.CantidadPeticiones);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task BuscarAsync_AcentosYSignosDistintos_NoComparteEntrada()
        {
            Escenario e = new Escenario();

            await e.BuscarAsync("cafe");
            await e.BuscarAsync("café");
            await e.BuscarAsync("cafe #1");

            Assert.AreEqual(3, e.Manejador.CantidadPeticiones);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task BuscarAsync_IdiomaDistinto_NoComparteEntrada()
        {
            Escenario e = new Escenario();

            await e.BuscarAsync("Obelisco", "es");
            await e.BuscarAsync("Obelisco", "en");
            await e.BuscarAsync("Obelisco", "en");

            Assert.AreEqual(2, e.Manejador.CantidadPeticiones);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task BuscarAsync_ViewboxDistintoONormalizado_SeDiferenciaSoloPorLaZonaEfectiva()
        {
            Escenario e = new Escenario();
            ILimitesGeograficos zona = new LimitesGeograficos(-34.7, -58.5, -34.5, -58.3);
            ILimitesGeograficos zonaCasiIgual = new LimitesGeograficos(-34.70001, -58.50001, -34.50001, -58.30001);
            ILimitesGeograficos otraZona = new LimitesGeograficos(-31.5, -64.3, -31.3, -64.1);
            ILimitesGeograficos zonaNoRepresentable = new LimitesGeograficos(-10, 170, 10, -170);

            await e.BuscarAsync("Obelisco", "es", zona);
            await e.BuscarAsync("Obelisco", "es", zonaCasiIgual);
            Assert.AreEqual(1, e.Manejador.CantidadPeticiones, "Una zona equivalente tras normalizar reutiliza la entrada.");

            await e.BuscarAsync("Obelisco", "es", otraZona);
            await e.BuscarAsync("Obelisco", "es", null);
            Assert.AreEqual(3, e.Manejador.CantidadPeticiones, "Otra zona y la ausencia de zona son entradas distintas.");

            await e.BuscarAsync("Obelisco", "es", zonaNoRepresentable);
            Assert.AreEqual(3, e.Manejador.CantidadPeticiones, "Una zona no representable equivale a no enviar viewbox.");
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task BuscarAsync_EndpointDistinto_NoComparteEntrada()
        {
            Escenario e = new Escenario();

            await e.BuscarAsync("Obelisco");
            e.Endpoint = "https://otro.example.org/search";
            await e.BuscarAsync("Obelisco");

            Assert.AreEqual(2, e.Manejador.CantidadPeticiones);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task BuscarAsync_EntradaVencidaA24Horas_VuelveAConsultar()
        {
            Escenario e = new Escenario();
            await e.BuscarAsync("Obelisco");

            e.Reloj.Avanzar(TimeSpan.FromHours(24) - TimeSpan.FromSeconds(2));
            await e.BuscarAsync("Obelisco");
            Assert.AreEqual(1, e.Manejador.CantidadPeticiones, "Antes de 24 horas sigue vigente.");

            e.Reloj.Avanzar(TimeSpan.FromSeconds(1));
            await e.BuscarAsync("Obelisco");
            Assert.AreEqual(2, e.Manejador.CantidadPeticiones, "A las 24 horas vence.");
        }

        [TestMethod]
        [Timeout(20000)]
        public async Task BuscarAsync_MasDeCienEntradas_DesalojaLasMasAntiguas()
        {
            Escenario e = new Escenario();
            int maximo = NominatimBusquedaUbicacionesService.MaximoEntradasCache;
            for (int i = 1; i <= maximo + 1; i++)
            {
                await e.BuscarAsync("lugar " + i);
            }

            Assert.AreEqual(maximo + 1, e.Manejador.CantidadPeticiones);

            await e.BuscarAsync("lugar " + (maximo + 1));
            await e.BuscarAsync("lugar 2");
            Assert.AreEqual(maximo + 1, e.Manejador.CantidadPeticiones, "Las entradas recientes siguen en cache.");

            await e.BuscarAsync("lugar 1");
            Assert.AreEqual(maximo + 2, e.Manejador.CantidadPeticiones, "La entrada mas antigua fue desalojada.");
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task BuscarAsync_Errores_NoSeCachean()
        {
            int llamada = 0;
            Escenario e = new Escenario((p, ct) =>
            {
                int n = Interlocked.Increment(ref llamada);
                if (n == 1)
                {
                    return Task.FromException<HttpResponseMessage>(new HttpRequestException("sin red"));
                }

                if (n == 2)
                {
                    return Task.FromResult(BusquedaUbicacionesPruebas.RespuestaXml("<html/>"));
                }

                return ResponderConEcoDelTexto(p, ct);
            });

            await Assert.ThrowsExactlyAsync<BusquedaUbicacionException>(() => e.BuscarAsync("Obelisco"));
            await Assert.ThrowsExactlyAsync<BusquedaUbicacionException>(() => e.BuscarAsync("Obelisco"));
            IReadOnlyList<IResultadoBusquedaUbicacion> resultados = await e.BuscarAsync("Obelisco");

            Assert.AreEqual(1, resultados.Count);
            Assert.AreEqual(3, e.Manejador.CantidadPeticiones);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task BuscarAsync_Cancelacion_NoSeCachea()
        {
            int llamada = 0;
            TaskCompletionSource<bool> enviada = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            Escenario e = new Escenario((p, ct) =>
            {
                if (Interlocked.Increment(ref llamada) == 1)
                {
                    enviada.TrySetResult(true);
                    return Task.Delay(Timeout.Infinite, ct).ContinueWith<HttpResponseMessage>(t => throw new TaskCanceledException(), TaskScheduler.Default);
                }

                return ResponderConEcoDelTexto(p, ct);
            });

            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                Task<IReadOnlyList<IResultadoBusquedaUbicacion>> cancelada = e.Servicio.BuscarAsync(new ConsultaUbicacion("Obelisco", "es", null), cts.Token);
                await enviada.Task;
                cts.Cancel();
                await Assert.ThrowsAsync<OperationCanceledException>(() => cancelada);
            }

            IReadOnlyList<IResultadoBusquedaUbicacion> resultados = await e.BuscarAsync("Obelisco");

            Assert.AreEqual(1, resultados.Count);
            Assert.AreEqual(2, e.Manejador.CantidadPeticiones);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task BuscarAsync_ConsultasIgualesConcurrentes_EnvianUnaSolaPeticion()
        {
            TaskCompletionSource<bool> primeraEnVuelo = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            TaskCompletionSource<bool> liberar = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            Escenario e = new Escenario(async (p, ct) =>
            {
                primeraEnVuelo.TrySetResult(true);
                await liberar.Task;
                return await ResponderConEcoDelTexto(p, ct);
            });
            IConsultaUbicacion consulta = new ConsultaUbicacion("Obelisco", "es", null);

            Task<IReadOnlyList<IResultadoBusquedaUbicacion>> a = e.Servicio.BuscarAsync(consulta, CancellationToken.None);
            await primeraEnVuelo.Task;
            Task<IReadOnlyList<IResultadoBusquedaUbicacion>> b = e.Servicio.BuscarAsync(consulta, CancellationToken.None);
            Task<IReadOnlyList<IResultadoBusquedaUbicacion>> c = e.Servicio.BuscarAsync(consulta, CancellationToken.None);
            await Task.Yield();
            liberar.SetResult(true);
            IReadOnlyList<IResultadoBusquedaUbicacion>[] todas = await Task.WhenAll(a, b, c);

            Assert.AreEqual(1, e.Manejador.CantidadPeticiones);
            Assert.IsTrue(todas.All(r => r.Count == 1 && r[0].Descripcion == "Obelisco"));
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task BuscarAsync_DuranteCooldown_LosAciertosDeCacheSiguenDisponibles()
        {
            int llamada = 0;
            Escenario e = new Escenario((p, ct) =>
            {
                if (Interlocked.Increment(ref llamada) == 2)
                {
                    return Task.FromResult(BusquedaUbicacionesPruebas.RespuestaXml(string.Empty, (HttpStatusCode)429));
                }

                return ResponderConEcoDelTexto(p, ct);
            });
            await e.BuscarAsync("guardada");
            await Assert.ThrowsExactlyAsync<BusquedaUbicacionException>(() => e.BuscarAsync("rechazada"));

            IReadOnlyList<IResultadoBusquedaUbicacion> desdeCache = await e.BuscarAsync("guardada");
            BusquedaUbicacionException sinCache = await Assert.ThrowsExactlyAsync<BusquedaUbicacionException>(() => e.BuscarAsync("nueva"));

            Assert.AreEqual(1, desdeCache.Count);
            Assert.AreEqual(EstadoBusquedaUbicacion.LimiteExcedido, sinCache.Estado);
            Assert.AreEqual(2, e.Manejador.CantidadPeticiones);
        }
    }
}
