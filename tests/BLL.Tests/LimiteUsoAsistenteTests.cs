using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLL.Tests
{
    [TestClass]
    public sealed class LimiteUsoAsistenteTests
    {
        private static readonly DateTime Inicio = new DateTime(2026, 10, 18, 10, 0, 0);

        [TestMethod]
        public void IntentarConsumir_DentroDelLimite_AdmiteCadaPregunta()
        {
            DateTime ahora = Inicio;
            LimiteUsoAsistente sut = new LimiteUsoAsistente(5, () => ahora);

            for (int i = 0; i < 5; i++)
            {
                Assert.IsTrue(sut.IntentarConsumir(1), "Pregunta " + i);
                ahora = ahora.AddSeconds(1);
            }
        }

        [TestMethod]
        public void IntentarConsumir_ExcedeElLimiteEnElMinuto_RechazaLaPregunta()
        {
            DateTime ahora = Inicio;
            LimiteUsoAsistente sut = new LimiteUsoAsistente(5, () => ahora);
            for (int i = 0; i < 5; i++)
            {
                sut.IntentarConsumir(1);
            }

            ahora = Inicio.AddSeconds(59);

            Assert.IsFalse(sut.IntentarConsumir(1));
        }

        [TestMethod]
        public void IntentarConsumir_PasadoUnMinutoDeLaPrimera_LiberaLaVentana()
        {
            DateTime ahora = Inicio;
            LimiteUsoAsistente sut = new LimiteUsoAsistente(5, () => ahora);
            sut.IntentarConsumir(1);
            ahora = Inicio.AddSeconds(30);
            for (int i = 0; i < 4; i++)
            {
                sut.IntentarConsumir(1);
            }

            ahora = Inicio.AddSeconds(60);
            Assert.IsTrue(sut.IntentarConsumir(1), "La primera pregunta ya salio de la ventana.");
            Assert.IsFalse(sut.IntentarConsumir(1), "Las cuatro del segundo 30 siguen en la ventana.");
        }

        [TestMethod]
        public void IntentarConsumir_RechazosNoOcupanLugarEnLaVentana()
        {
            DateTime ahora = Inicio;
            LimiteUsoAsistente sut = new LimiteUsoAsistente(1, () => ahora);
            sut.IntentarConsumir(1);
            ahora = Inicio.AddSeconds(50);
            Assert.IsFalse(sut.IntentarConsumir(1));

            ahora = Inicio.AddSeconds(60);

            Assert.IsTrue(sut.IntentarConsumir(1));
        }

        [TestMethod]
        public void IntentarConsumir_UsuariosDistintos_TienenLimitesIndependientes()
        {
            LimiteUsoAsistente sut = new LimiteUsoAsistente(2, () => Inicio);
            sut.IntentarConsumir(1);
            sut.IntentarConsumir(1);

            Assert.IsFalse(sut.IntentarConsumir(1));
            Assert.IsTrue(sut.IntentarConsumir(2));
            Assert.IsTrue(sut.IntentarConsumir(2));
            Assert.IsFalse(sut.IntentarConsumir(2));
        }

        [TestMethod]
        public void IntentarConsumir_AccesoConcurrente_AdmiteExactamenteElLimite()
        {
            LimiteUsoAsistente sut = new LimiteUsoAsistente(5, () => Inicio);
            bool[] resultados = new bool[200];

            Parallel.For(0, resultados.Length, new ParallelOptions { MaxDegreeOfParallelism = 8 }, i =>
            {
                resultados[i] = sut.IntentarConsumir(7);
            });

            Assert.AreEqual(5, resultados.Count(admitida => admitida));
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("cinco")]
        [DataRow("0")]
        [DataRow("-3")]
        public void InterpretarMaximoConfigurado_ValorAusenteOInvalido_UsaCinco(string valor)
        {
            Assert.AreEqual(5, LimiteUsoAsistente.InterpretarMaximoConfigurado(valor));
        }

        [TestMethod]
        public void InterpretarMaximoConfigurado_ValorValido_LoUsa()
        {
            Assert.AreEqual(8, LimiteUsoAsistente.InterpretarMaximoConfigurado(" 8 "));
        }
    }
}
