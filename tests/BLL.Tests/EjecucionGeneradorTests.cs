using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CRONS;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLL.Tests
{
    [TestClass]
    public sealed class EjecucionGeneradorTests
    {
        private static async Task<(int codigo, string salida, string error)> Ejecutar(Func<CancellationToken, Task<ResultadoCorridaNotificaciones>> corrida, CancellationToken ct = default(CancellationToken))
        {
            StringWriter salida = new StringWriter(), error = new StringWriter();
            int codigo = await EjecucionGenerador.EjecutarAsync(corrida, salida, error, ct);
            return (codigo, salida.ToString(), error.ToString());
        }

        [TestMethod]
        public async Task CorridaExitosa_SalidaCeroConResumen()
        {
            var r = await Ejecutar(ct => Task.FromResult(new ResultadoCorridaNotificaciones { CorridaId = 7, Jornada = new DateTime(2026, 10, 20), Entregadas = 3, Omitidas = 2 }));
            Assert.AreEqual(0, r.codigo);
            StringAssert.Contains(r.salida, "3 delivered");
            StringAssert.Contains(r.salida, "2 skipped");
            Assert.AreEqual(string.Empty, r.error);
        }

        [TestMethod]
        public async Task CorridaSoloConOmisiones_SalidaCero()
        {
            var r = await Ejecutar(ct => Task.FromResult(new ResultadoCorridaNotificaciones { CorridaId = 1, Omitidas = 9 }));
            Assert.AreEqual(0, r.codigo);
        }

        [TestMethod]
        public async Task AntesDelHorario_SalidaCeroIndicandoQueNoHizoNada()
        {
            var r = await Ejecutar(ct => Task.FromResult(new ResultadoCorridaNotificaciones { AntesDelHorario = true }));
            Assert.AreEqual(0, r.codigo);
            StringAssert.Contains(r.salida, "before 07:00");
        }

        [TestMethod]
        public async Task CorridaParcialmenteFallida_SalidaUno()
        {
            var r = await Ejecutar(ct => Task.FromResult(new ResultadoCorridaNotificaciones { CorridaId = 4, Entregadas = 5, Fallidas = 1 }));
            Assert.AreEqual(1, r.codigo);
            StringAssert.Contains(r.salida, "1 failed");
        }

        [TestMethod]
        public async Task ErrorFatal_SalidaDosPorStderrSinStackTrace()
        {
            var r = await Ejecutar(ct => { throw new ExcepcionTecnicaDePrueba("SQL down"); });
            Assert.AreEqual(2, r.codigo);
            StringAssert.Contains(r.error, "SQL down");
            Assert.IsFalse(r.error.Contains(" at "));
            Assert.AreEqual(string.Empty, r.salida);
        }

        [TestMethod]
        public async Task ErrorFatalAsincrono_SalidaDos()
        {
            var r = await Ejecutar(async ct => { await Task.Yield(); throw new InvalidOperationException("boom"); });
            Assert.AreEqual(2, r.codigo);
            StringAssert.Contains(r.error, "boom");
        }

        [TestMethod]
        public async Task CorridaCancelada_SalidaDosNuncaExitoFalso()
        {
            var r = await Ejecutar(ct => Task.FromResult(new ResultadoCorridaNotificaciones { CorridaId = 2, Entregadas = 1, Cancelada = true }));
            Assert.AreEqual(2, r.codigo);
            StringAssert.Contains(r.error, "cancelled");
        }

        [TestMethod]
        public async Task OperacionCanceladaPorExcepcion_SalidaDos()
        {
            var r = await Ejecutar(ct => { throw new OperationCanceledException(); });
            Assert.AreEqual(2, r.codigo);
            StringAssert.Contains(r.error, "cancelled");
        }

        [TestMethod]
        public async Task ResultadoNulo_SalidaDos()
        {
            var r = await Ejecutar(ct => Task.FromResult<ResultadoCorridaNotificaciones>(null));
            Assert.AreEqual(2, r.codigo);
        }

        [TestMethod]
        public async Task PasaElTokenDeCancelacionALaCorrida()
        {
            using (var cts = new CancellationTokenSource())
            {
                CancellationToken recibido = default(CancellationToken);
                await Ejecutar(ct => { recibido = ct; return Task.FromResult(new ResultadoCorridaNotificaciones()); }, cts.Token);
                Assert.AreEqual(cts.Token, recibido);
            }
        }

        private sealed class ExcepcionTecnicaDePrueba : Exception
        {
            public ExcepcionTecnicaDePrueba(string mensaje) : base(mensaje) { }
        }
    }
}
