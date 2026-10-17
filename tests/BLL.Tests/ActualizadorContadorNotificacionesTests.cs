using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TpIDS;

namespace BLL.Tests
{
    [TestClass]
    public sealed class ActualizadorContadorNotificacionesTests
    {
        [TestMethod]
        public async Task ActualizarAsync_ConsultaLentaYLectura_DescartaViejaYSinSuperposicion()
        {
            var primera = new TaskCompletionSource<int>();
            int llamadas = 0, publicado = -1;
            var actualizador = new ActualizadorContadorNotificaciones(() => 5, () => ++llamadas == 1 ? primera.Task : Task.FromResult(2), n => publicado = n, ex => Assert.Fail(ex.Message));
            Task pendiente = actualizador.ActualizarAsync();
            await actualizador.ActualizarAsync();
            Assert.AreEqual(1, llamadas);
            await actualizador.ActualizarAsync(true);
            primera.SetResult(3);
            await pendiente;
            Assert.AreEqual(2, llamadas);
            Assert.AreEqual(2, publicado);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task ActualizarAsync_CierreOCambioSesion_NoPublicaRespuestaTardia(bool detener)
        {
            var respuesta = new TaskCompletionSource<int>();
            int? usuario = 5;
            int publicado = -1;
            var actualizador = new ActualizadorContadorNotificaciones(() => usuario, () => respuesta.Task, n => publicado = n, ex => Assert.Fail());
            Task pendiente = actualizador.ActualizarAsync();
            if (detener) actualizador.Detener(); else usuario = 7;
            respuesta.SetResult(9);
            await pendiente;
            Assert.AreEqual(-1, publicado);
        }

        [TestMethod]
        public async Task ActualizarAsync_FalloTransitorio_ConservaValorYReintenta()
        {
            int llamada = 0, publicado = -1, fallos = 0;
            var actualizador = new ActualizadorContadorNotificaciones(() => 5,
                () => ++llamada == 2 ? Task.FromException<int>(new InvalidOperationException()) : Task.FromResult(llamada),
                n => publicado = n, ex => fallos++);
            await actualizador.ActualizarAsync();
            await actualizador.ActualizarAsync();
            Assert.AreEqual(1, publicado);
            Assert.AreEqual(1, fallos);
            await actualizador.ActualizarAsync();
            Assert.AreEqual(3, publicado);
        }
    }
}
