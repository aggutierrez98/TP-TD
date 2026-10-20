using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CRONS;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLL.Tests
{
    [TestClass]
    public sealed class DespachadorTests
    {
        private sealed class Escenario
        {
            public readonly List<string> Invocadas = new List<string>();
            public readonly StringWriter Error = new StringWriter();
            public Despachador Despachador;

            public Escenario(int salidaTasas = 0, int salidaNotificaciones = 0)
            {
                Despachador = new Despachador(
                    new Dictionary<string, Func<CancellationToken, Task<int>>>
                    {
                        { "tasas", ct => { Invocadas.Add("tasas"); return Task.FromResult(salidaTasas); } },
                        { "notificaciones", ct => { Invocadas.Add("notificaciones"); return Task.FromResult(salidaNotificaciones); } }
                    },
                    Error);
            }
        }

        [TestMethod]
        public async Task EjecutarAsync_Tasas_EjecutaSoloTasasYDevuelveSuCodigo()
        {
            Escenario e = new Escenario(salidaTasas: 1);

            int codigo = await e.Despachador.EjecutarAsync(new[] { "tasas" }, CancellationToken.None);

            Assert.AreEqual(1, codigo);
            CollectionAssert.AreEqual(new[] { "tasas" }, e.Invocadas);
        }

        [TestMethod]
        public async Task EjecutarAsync_Notificaciones_EjecutaSoloNotificacionesYDevuelveSuCodigo()
        {
            Escenario e = new Escenario(salidaNotificaciones: 1);

            int codigo = await e.Despachador.EjecutarAsync(new[] { "notificaciones" }, CancellationToken.None);

            Assert.AreEqual(1, codigo);
            CollectionAssert.AreEqual(new[] { "notificaciones" }, e.Invocadas);
        }

        [TestMethod]
        public async Task EjecutarAsync_NombreEnMayusculas_EjecutaLaTarea()
        {
            Escenario e = new Escenario();

            int codigo = await e.Despachador.EjecutarAsync(new[] { "TASAS" }, CancellationToken.None);

            Assert.AreEqual(0, codigo);
            CollectionAssert.AreEqual(new[] { "tasas" }, e.Invocadas);
        }

        [TestMethod]
        public async Task EjecutarAsync_SinArgumentos_InformaLasTareasValidasYNoEjecutaNinguna()
        {
            Escenario e = new Escenario();

            int codigo = await e.Despachador.EjecutarAsync(new string[0], CancellationToken.None);

            Assert.AreEqual(Despachador.SalidaErrorInesperado, codigo);
            Assert.AreEqual(0, e.Invocadas.Count);
            StringAssert.Contains(e.Error.ToString(), "tasas");
            StringAssert.Contains(e.Error.ToString(), "notificaciones");
        }

        [TestMethod]
        public async Task EjecutarAsync_TareaDesconocida_InformaElNombreYLasValidasYNoEjecutaNinguna()
        {
            Escenario e = new Escenario();

            int codigo = await e.Despachador.EjecutarAsync(new[] { "reportes" }, CancellationToken.None);

            Assert.AreEqual(Despachador.SalidaErrorInesperado, codigo);
            Assert.AreEqual(0, e.Invocadas.Count);
            string error = e.Error.ToString();
            StringAssert.Contains(error, "reportes");
            StringAssert.Contains(error, "tasas");
            StringAssert.Contains(error, "notificaciones");
        }

        [TestMethod]
        public async Task EjecutarAsync_TareaCancelada_DevuelveErrorInesperado()
        {
            Despachador despachador = new Despachador(
                new Dictionary<string, Func<CancellationToken, Task<int>>>
                {
                    { "tasas", ct => { throw new OperationCanceledException(ct); } }
                },
                new StringWriter());

            int codigo = await despachador.EjecutarAsync(new[] { "tasas" }, new CancellationToken(true));

            Assert.AreEqual(Despachador.SalidaErrorInesperado, codigo);
        }
    }
}
