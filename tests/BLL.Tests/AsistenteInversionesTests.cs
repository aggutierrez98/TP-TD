using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ABSTRACCIONES;
using BE;
using BLL.Tests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SERVICIOS;

namespace BLL.Tests
{
    [TestClass]
    public sealed class AsistenteInversionesTests
    {
        [TestMethod]
        [DataRow(1000)]
        [DataRow(950)]
        [DataRow(850)]
        [DataRow(500)]
        public async Task EnviarMensajeAsync_InflacionDelResumen_AdmiteLaReferenciaYAgregaAvisoEnTodosLosNiveles(int gasto)
        {
            InversionFixture f = new InversionFixture(gasto);
            var servicio = new FakeAsistenteFinancieroService { LlamadaHerramienta = new LlamadaHerramientaAsistente("recomendar_inversion", "{\"porcentaje\":20}"), NextRespuesta = "Inflacion conocido: 20%." };
            var unidad = new FakeUnidadDeTrabajo();
            var r = await Crear(f, servicio, unidad).EnviarMensajeAsync("Invertir el 20%");
            Assert.IsFalse(r.EsRechazo);
            Assert.EndsWith("Educational information.", r.Texto);
            Assert.IsTrue(unidad.SaveCalled);
        }

        [TestMethod]
        [DataRow("{}")]
        [DataRow("{\"porcentaje\":null}")]
        [DataRow(null)]
        public async Task EnviarMensajeAsync_PorcentajeAusente_RecomiendaSinMontoNiPorcentajeNiPedirOtroPaso(string argumentos)
        {
            InversionFixture f = new InversionFixture(500);
            var servicio = new FakeAsistenteFinancieroService
            {
                LlamadaHerramienta = argumentos == null ? null : new LlamadaHerramientaAsistente("recomendar_inversion", argumentos),
                NextRespuesta = "Considera CEDEAR conocido."
            };
            var unidad = new FakeUnidadDeTrabajo();
            RespuestaAsistente r = await Crear(f, servicio, unidad).EnviarMensajeAsync("En que invierto lo que me sobra?");
            Assert.IsFalse(r.EsRechazo);
            Assert.DoesNotContain("What percentage?", r.Texto);
            Assert.DoesNotContain("valid percentage", r.Texto);
            Assert.EndsWith("Educational information.", r.Texto);
            CollectionAssert.AreEqual(new[] { "herramienta", "respuesta" }, servicio.OrdenLlamadas);
            Assert.IsNotNull(f.Repo.LastObtenerTotalesPorMesArgs);
            Assert.Contains("aunque sea invalido", servicio.UltimaHerramienta.ParametrosJson);
            string datos = servicio.LastSolicitud.Contexto.DatosJson;
            Assert.DoesNotContain("Porcentaje", datos);
            Assert.DoesNotContain("Monto", datos);
            Assert.Contains("\"RiesgoEfectivo\"", datos);
            Assert.Contains("\"Instrumentos\"", datos);
            Assert.AreEqual(r.Texto, unidad.ConsultaChatbotRepo.LastInserted.Respuesta);
            Assert.IsTrue(unidad.SaveCalled);
        }

        [TestMethod]
        [DataRow("{\"porcentaje\":0}")]
        [DataRow("{\"porcentaje\":101}")]
        [DataRow("{\"porcentaje\":150}")]
        [DataRow("{\"porcentaje\":-5}")]
        [DataRow("{\"porcentaje\":\"20\"}")]
        public async Task EnviarMensajeAsync_PorcentajeExplicitoInvalido_PideCorreccionSinFuentesNiRedaccionNiMonto(string argumentos)
        {
            InversionFixture f = new InversionFixture(500);
            var servicio = new FakeAsistenteFinancieroService { LlamadaHerramienta = new LlamadaHerramientaAsistente("recomendar_inversion", argumentos) };
            var unidad = new FakeUnidadDeTrabajo();
            RespuestaAsistente r = await Crear(f, servicio, unidad).EnviarMensajeAsync("Quiero invertir un porcentaje de lo que sobra");
            Assert.IsFalse(r.EsRechazo);
            Assert.Contains("Enter a valid percentage.", r.Texto);
            Assert.EndsWith("Educational information.", r.Texto);
            Assert.IsNull(f.Repo.LastObtenerTotalesPorMesArgs);
            Assert.IsEmpty(f.Mercado.Llamadas);
            CollectionAssert.AreEqual(new[] { "herramienta" }, servicio.OrdenLlamadas);
        }

        [TestMethod]
        [DataRow("no-json")]
        [DataRow("[]")]
        public async Task EnviarMensajeAsync_ArgumentosDeHerramientaIlegibles_FallaTecnicaSinCalcularNiGuardar(string argumentos)
        {
            InversionFixture f = new InversionFixture(500);
            var servicio = new FakeAsistenteFinancieroService { LlamadaHerramienta = new LlamadaHerramientaAsistente("recomendar_inversion", argumentos) };
            var unidad = new FakeUnidadDeTrabajo();
            await Assert.ThrowsExactlyAsync<ExcepcionTecnica>(() => Crear(f, servicio, unidad).EnviarMensajeAsync("Quiero invertir el 20%"));
            Assert.IsNull(f.Repo.LastObtenerTotalesPorMesArgs);
            Assert.IsFalse(unidad.SaveCalled);
        }

        [TestMethod]
        public async Task EnviarMensajeAsync_FallaLaExtraccionDelPorcentaje_PropagaElErrorTecnicoSinRecomendarNiGuardar()
        {
            InversionFixture f = new InversionFixture(500);
            var servicio = new FakeAsistenteFinancieroService { ThrowOnHerramienta = new ExcepcionTecnica("Error.Assistant.ResponseFailed") };
            var unidad = new FakeUnidadDeTrabajo();
            await Assert.ThrowsExactlyAsync<ExcepcionTecnica>(() => Crear(f, servicio, unidad).EnviarMensajeAsync("Quiero invertir el 20%"));
            Assert.IsNull(f.Repo.LastObtenerTotalesPorMesArgs);
            Assert.IsFalse(unidad.SaveCalled);
        }

        [TestMethod]
        public async Task EnviarMensajeAsync_SinPerfilYSinPorcentaje_ExigePerfilAntesDeExtraerOCalcular()
        {
            InversionFixture f = new InversionFixture(500);
            var servicio = new FakeAsistenteFinancieroService { LlamadaHerramienta = new LlamadaHerramientaAsistente("recomendar_inversion", "{}") };
            var unidad = new FakeUnidadDeTrabajo();
            RespuestaAsistente r = await Crear(f, servicio, unidad, sinPerfil: true).EnviarMensajeAsync("En que invierto lo que me sobra?");
            Assert.Contains("Complete your profile.", r.Texto);
            Assert.IsEmpty(servicio.OrdenLlamadas);
            Assert.IsNull(f.Repo.LastObtenerTotalesPorMesArgs);
        }

        [TestMethod]
        public async Task EnviarMensajeAsync_PorcentajeValido_ExtraeCalculaYRedactaSoloConResumenDelUsuarioCapturado()
        {
            InversionFixture f = new InversionFixture(500);
            FakeSesion sesion = new FakeSesion();
            sesion.IniciarSesion(new Usuario { Id = 7 });
            var servicio = new FakeAsistenteFinancieroService
            {
                LlamadaHerramienta = new LlamadaHerramientaAsistente("recomendar_inversion", "{\"porcentaje\":20,\"usuarioId\":999}"),
                NextRespuesta = "Considera CEDEAR conocido.",
                AlExtraerHerramienta = () => { sesion.CerrarSesion(); sesion.IniciarSesion(new Usuario { Id = 8 }); }
            };
            var unidad = new FakeUnidadDeTrabajo();
            RespuestaAsistente r = await Crear(f, servicio, unidad, sesion).EnviarMensajeAsync("Quiero invertir el 20% de lo que sobra");
            Assert.IsFalse(r.EsRechazo);
            CollectionAssert.AreEqual(new[] { "herramienta", "respuesta" }, servicio.OrdenLlamadas);
            Assert.AreEqual("recomendar_inversion", servicio.UltimaHerramienta.Nombre);
            Assert.DoesNotContain("usuario", servicio.UltimaHerramienta.ParametrosJson);
            Assert.Contains("\"Monto\":100", servicio.LastSolicitud.Contexto.DatosJson);
            Assert.AreEqual(7, f.Repo.LastObtenerTotalesPorMesArgs.Item1.Id);
            Assert.AreEqual(7, unidad.ConsultaChatbotRepo.LastInserted.Usuario.Id);
            Assert.AreEqual(r.Texto, unidad.ConsultaChatbotRepo.LastInserted.Respuesta);
            Assert.EndsWith("Educational information.", r.Texto);
        }

        [TestMethod]
        public async Task EnviarMensajeAsync_FuenteFaltante_AvisoTraducidoPersistidoYDisclaimerFinal()
        {
            InversionFixture f = new InversionFixture(500);
            f.Mercado.Fallas.Add("BTC");
            var servicio = new FakeAsistenteFinancieroService { LlamadaHerramienta = new LlamadaHerramientaAsistente("recomendar_inversion", "{\"porcentaje\":20}"), NextRespuesta = "Orientacion calculada." };
            var unidad = new FakeUnidadDeTrabajo();
            var r = await Crear(f, servicio, unidad).EnviarMensajeAsync("Invertir el 20%");
            Assert.Contains("Missing: criptoya BTC", r.Texto);
            Assert.EndsWith("Educational information.", r.Texto);
            Assert.AreEqual(r.Texto, unidad.ConsultaChatbotRepo.LastInserted.Respuesta);
        }

        [TestMethod]
        public async Task EnviarMensajeAsync_InstrumentoConocidoNoSeleccionado_RechazaSinGuardar()
        {
            InversionFixture f = new InversionFixture(850);
            f.Mercado.Fondos = Enumerable.Range(1, 4).Select(i => new DatoMercado { Nombre = "Fondo " + i, Tipo = "FCI rentaFija", Vcp = 100, Fecha = f.Hoy }).ToList();
            var servicio = new FakeAsistenteFinancieroService { LlamadaHerramienta = new LlamadaHerramientaAsistente("recomendar_inversion", "{\"porcentaje\":20}"), NextRespuesta = "Compra Fondo 4." };
            var unidad = new FakeUnidadDeTrabajo();
            var r = await Crear(f, servicio, unidad).EnviarMensajeAsync("Invertir el 20%");
            Assert.IsTrue(r.EsRechazo);
            Assert.AreEqual(AsistenteFinancieroBLL.RechazoRespuestaNoConfiable, r.ClaveRechazo);
            Assert.IsFalse(unidad.SaveCalled);
        }

        private static AsistenteFinancieroBLL Crear(InversionFixture f, FakeAsistenteFinancieroService servicio, FakeUnidadDeTrabajo unidad, FakeSesion sesion = null, bool sinPerfil = false)
        {
            if (sesion == null) { sesion = new FakeSesion(); sesion.IniciarSesion(new Usuario { Id = 7 }); }
            var idioma = new FakeObservado();
            idioma.CambiarIdioma(new Idioma { Traducciones = new List<Traduccion>
            {
                new Traduccion { Palabra = new Palabra { Key = "Assistant.Investment.InvalidPercentage" }, Valor = "Enter a valid percentage." },
                new Traduccion { Palabra = new Palabra { Key = "Assistant.Investment.ProfileRequired" }, Valor = "Complete your profile." },
                new Traduccion { Palabra = new Palabra { Key = "Assistant.Investment.MissingData" }, Valor = "Missing: {0}" },
                new Traduccion { Palabra = new Palabra { Key = "Assistant.Investment.Disclaimer" }, Valor = "Educational information." }
            } });
            return new AsistenteFinancieroBLL(new FakeConsultaChatbotRepositorio(), new FakeChatbotFinancieroRepositorio(), servicio,
                new FakeClasificadorConsultas { NextCategoria = CategoriaConsulta.Inversiones }, new FakeLimiteUsoAsistente(1000),
                new FakeUnidadDeTrabajoFactory(unidad), sesion, new SpyBitacoraBLL(), idioma,
                new GeneradorRecomendaciones(servicio, new ValidadorRespuestaAsistente(), null, f.Crear()),
                new PerfilInversorBLLTests.RepoPerfil { Ultimo = sinPerfil ? null : PerfilAsistenteTests.Perfil() });
        }
    }
}
