using System;
using System.Globalization;
using System.Linq;
using BE;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SERVICIOS;

namespace BLL.Tests
{
    [TestClass]
    public sealed class CalculadorPerfilInversorTests
    {
        internal static RespuestaPerfilInversor[] Respuestas(string opciones) => opciones.Select((opcion, i) =>
            new RespuestaPerfilInversor { Pregunta = new PreguntaPerfilInversor(i + 1), OpcionId = opcion.ToString() }).ToArray();

        [TestMethod]
        [DataRow("AAAAAAAAAA", 0, 0, 0, NivelRiesgoInversor.Bajo)]
        [DataRow("BCCCCCCCCB", 8680, 5850, 6416, NivelRiesgoInversor.Moderado)]
        [DataRow("CDCDCCDDDC", 10000, 10000, 10000, NivelRiesgoInversor.Alto)]
        public void Evaluar_VectoresDeAceptacion_CalculaPuntajesYNivel(string opciones, int capacidad, int tolerancia, int puntaje, NivelRiesgoInversor nivel)
        {
            PerfilInversor perfil = CalculadorPerfilInversor.Evaluar(1, Respuestas(opciones));
            Assert.AreEqual(capacidad / 100m, perfil.Capacidad);
            Assert.AreEqual(tolerancia / 100m, perfil.Tolerancia);
            Assert.AreEqual(puntaje / 100m, perfil.Puntaje);
            Assert.AreEqual(nivel, perfil.NivelRiesgo);
        }

        [TestMethod]
        [DataRow(2, "A", NivelRiesgoInversor.Bajo, "HorizonteMuyCorto")]
        [DataRow(3, "A", NivelRiesgoInversor.Bajo, "LiquidezInmediata")]
        [DataRow(4, "A", NivelRiesgoInversor.Bajo, "RespaldoInsuficiente")]
        [DataRow(5, "A", NivelRiesgoInversor.Bajo, "GastosNoCubiertos")]
        [DataRow(6, "A", NivelRiesgoInversor.Bajo, "GastosComprometidos")]
        [DataRow(9, "A", NivelRiesgoInversor.Bajo, "NoAceptaPerdidas")]
        [DataRow(2, "B", NivelRiesgoInversor.Moderado, "HorizonteLimitado")]
        [DataRow(2, "C", NivelRiesgoInversor.Moderado, "HorizonteLimitado")]
        [DataRow(3, "B", NivelRiesgoInversor.Moderado, "LiquidezProxima")]
        [DataRow(4, "B", NivelRiesgoInversor.Moderado, "RespaldoLimitado")]
        [DataRow(5, "B", NivelRiesgoInversor.Moderado, "CapacidadLimitada")]
        [DataRow(6, "B", NivelRiesgoInversor.Moderado, "CapacidadLimitada")]
        [DataRow(7, "A", NivelRiesgoInversor.Moderado, "ExperienciaLimitada")]
        [DataRow(7, "B", NivelRiesgoInversor.Moderado, "ExperienciaLimitada")]
        [DataRow(1, "A", NivelRiesgoInversor.Moderado, "RespuestasDispares")]
        public void Evaluar_RestriccionSobreVectorAlto_AplicaLimite(int pregunta, string opcion, NivelRiesgoInversor nivel, string motivo)
        {
            var respuestas = Respuestas("CDCDCCDDDC");
            respuestas[pregunta - 1].OpcionId = opcion;
            var perfil = CalculadorPerfilInversor.Evaluar(1, respuestas);
            Assert.AreEqual(nivel, perfil.NivelRiesgo);
            Assert.Contains(motivo, perfil.MotivosLimite);
        }

        [TestMethod]
        public void Evaluar_VariosLimites_ConservaTodosYElMasRestrictivo()
        {
            var perfil = CalculadorPerfilInversor.Evaluar(1, Respuestas("CACDCCADDC"));
            Assert.AreEqual(NivelRiesgoInversor.Bajo, perfil.NivelRiesgo);
            CollectionAssert.AreEqual(new[] { "HorizonteMuyCorto", "ExperienciaLimitada" }, perfil.MotivosLimite.ToArray());
        }

        [TestMethod]
        public void Evaluar_EntradasInvalidas_RechazaSinEvaluacionParcial()
        {
            Assert.ThrowsExactly<ExcepcionNegocio>(() => CalculadorPerfilInversor.Evaluar(2, Respuestas("CDCDCCDDDC")));
            Assert.ThrowsExactly<ExcepcionNegocio>(() => CalculadorPerfilInversor.Evaluar(1, null));
            Assert.ThrowsExactly<ExcepcionNegocio>(() => CalculadorPerfilInversor.Evaluar(1, Respuestas("CDCDCCDDD")));
            Assert.ThrowsExactly<ExcepcionNegocio>(() => CalculadorPerfilInversor.Evaluar(1, Respuestas("CDCDCCDDDCC")));
            Assert.ThrowsExactly<ExcepcionNegocio>(() => CalculadorPerfilInversor.Evaluar(1, Respuestas("DDCDCCDDDC")));
            var duplicadas = Respuestas("CDCDCCDDDC");
            duplicadas[9].Pregunta = new PreguntaPerfilInversor(1);
            Assert.ThrowsExactly<ExcepcionNegocio>(() => CalculadorPerfilInversor.Evaluar(1, duplicadas));
            duplicadas[9] = null;
            Assert.ThrowsExactly<ExcepcionNegocio>(() => CalculadorPerfilInversor.Evaluar(1, duplicadas));
        }

        [TestMethod]
        [DataRow(3999, NivelRiesgoInversor.Bajo)]
        [DataRow(4000, NivelRiesgoInversor.Moderado)]
        [DataRow(6999, NivelRiesgoInversor.Moderado)]
        [DataRow(7000, NivelRiesgoInversor.Alto)]
        public void ClasificarPuntaje_Bordes_NoRedondea(int centesimas, NivelRiesgoInversor esperado) =>
            Assert.AreEqual(esperado, CalculadorPerfilInversor.ClasificarPuntaje(centesimas / 100m));

        [TestMethod]
        public void Evaluar_IdiomaYOrdenDistintos_ResultadoIdenticoYSinAliasDelBorrador()
        {
            var anterior = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("es-AR");
                var respuestas = Respuestas("BCCCCCCCCB");
                var primero = CalculadorPerfilInversor.Evaluar(1, respuestas);
                CultureInfo.CurrentCulture = new CultureInfo("en-US");
                var segundo = CalculadorPerfilInversor.Evaluar(1, respuestas.Reverse().ToArray());
                respuestas[0].OpcionId = "A";
                Assert.AreEqual(primero.Puntaje, segundo.Puntaje);
                Assert.AreEqual(primero.NivelRiesgo, segundo.NivelRiesgo);
                Assert.AreEqual("B", primero.Respuestas[0].OpcionId);
            }
            finally { CultureInfo.CurrentCulture = anterior; }
        }
    }
}
