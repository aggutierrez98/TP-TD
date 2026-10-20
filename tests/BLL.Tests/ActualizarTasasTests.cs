using BE;
using CRONS.Tareas;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLL.Tests
{
    [TestClass]
    public sealed class ActualizarTasasTests
    {
        [TestMethod]
        public void CodigoDeSalida_ResultadoNulo_DevuelveErrorInesperado()
        {
            Assert.AreEqual(ActualizarTasas.SalidaErrorInesperado, ActualizarTasas.CodigoDeSalida(null));
        }

        [TestMethod]
        public void CodigoDeSalida_SinEjecucion_DevuelveErrorInesperado()
        {
            Assert.AreEqual(ActualizarTasas.SalidaErrorInesperado, ActualizarTasas.CodigoDeSalida(new ResultadoEjecucionActualizacionTasas()));
        }

        [TestMethod]
        public void CodigoDeSalida_EjecucionFallida_DevuelveFallidaRegistrada()
        {
            Assert.AreEqual(ActualizarTasas.SalidaFallidaRegistrada, ActualizarTasas.CodigoDeSalida(Con(ResultadoActualizacionTasas.Fallida)));
        }

        [TestMethod]
        public void CodigoDeSalida_EjecucionExitosaOOmitida_DevuelveCorrecta()
        {
            Assert.AreEqual(ActualizarTasas.SalidaCorrecta, ActualizarTasas.CodigoDeSalida(Con(ResultadoActualizacionTasas.Exitosa)));
            Assert.AreEqual(ActualizarTasas.SalidaCorrecta, ActualizarTasas.CodigoDeSalida(Con(ResultadoActualizacionTasas.Omitida)));
        }

        private static ResultadoEjecucionActualizacionTasas Con(ResultadoActualizacionTasas resultado)
        {
            return new ResultadoEjecucionActualizacionTasas
            {
                Ejecucion = new EjecucionActualizacionTasas { Resultado = resultado }
            };
        }
    }
}
