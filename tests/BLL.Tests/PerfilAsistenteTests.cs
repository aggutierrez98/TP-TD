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
    public sealed class PerfilAsistenteTests
    {
        [TestMethod]
        [DataRow("En que me conviene invertir?")]
        [DataRow("Invertir 20%, acepto riesgo alto")]
        [DataRow("Que es un CEDEAR y si me conviene comprarlo con mis ahorros?")]
        public async Task EnviarMensajeAsync_SinPerfil_SoloInvitacionPersistidaSinHerramientasNiMercado(string pregunta)
        {
            var f = new Fixture();
            var r = await f.Crear().EnviarMensajeAsync(pregunta);
            Assert.AreEqual("Complete your investor profile.", r.Texto);
            Assert.IsFalse(r.EsRechazo);
            Assert.AreEqual(7, f.Perfiles.PropietarioConsultado);
            Assert.IsEmpty(f.Servicio.OrdenLlamadas);
            Assert.IsNull(f.Inversion.Repo.LastObtenerTotalesPorMesArgs);
            Assert.IsEmpty(f.Inversion.Mercado.Llamadas);
            Assert.AreEqual(r.Texto, f.Unidad.ConsultaChatbotRepo.LastInserted.Respuesta);
        }

        [TestMethod]
        public async Task EnviarMensajeAsync_ErrorPerfil_NoLoTrataComoAusenciaNiContinua()
        {
            var f = new Fixture();
            f.Perfiles.Fallar = true;
            var ex = await Assert.ThrowsExactlyAsync<ExcepcionTecnica>(() => f.Crear().EnviarMensajeAsync("Invertir 20%"));
            Assert.AreEqual("Error.InvestorProfile.ReadFailed", ex.Message);
            Assert.IsEmpty(f.Servicio.OrdenLlamadas);
            Assert.IsFalse(f.Unidad.SaveCalled);
        }

        [TestMethod]
        public async Task EnviarMensajeAsync_EducacionSinPerfil_NoConsultaPerfilNiMovimientos()
        {
            var f = new Fixture();
            f.Clasificador.NextCategoria = CategoriaConsulta.FinanzasGenerales;
            f.Perfiles.Fallar = true;
            var r = await f.Crear().EnviarMensajeAsync("Que es un plazo fijo?");
            Assert.AreEqual("Orientacion.", r.Texto);
            Assert.IsNull(f.Perfiles.PropietarioConsultado);
            Assert.IsNull(f.Inversion.Repo.LastObtenerTotalesPorMesArgs);
            CollectionAssert.AreEqual(new[] { "respuesta" }, f.Servicio.OrdenLlamadas);
        }

        [TestMethod]
        public async Task EnviarMensajeAsync_PerfilPresenteSinPorcentaje_RecomiendaConElPerfilDelUsuarioYSinMonto()
        {
            var f = new Fixture();
            f.Perfiles.Ultimo = Perfil();
            f.Servicio.LlamadaHerramienta = new LlamadaHerramientaAsistente("recomendar_inversion", "{}");
            RespuestaAsistente r = await f.Crear().EnviarMensajeAsync("Quiero invertir");
            Assert.IsFalse(r.EsRechazo);
            Assert.AreEqual(7, f.Perfiles.PropietarioConsultado);
            Assert.IsNotEmpty(f.Inversion.Mercado.Llamadas);
            Assert.Contains("\"Perfil\":\"Alto\"", f.Servicio.LastSolicitud.Contexto.DatosJson);
            Assert.DoesNotContain("Monto", f.Servicio.LastSolicitud.Contexto.DatosJson);
        }

        [TestMethod]
        public async Task EnviarMensajeAsync_PerfilPresentePorcentajeInvalido_PideCorreccionYNoConsultaMercado()
        {
            var f = new Fixture();
            f.Perfiles.Ultimo = Perfil();
            f.Servicio.LlamadaHerramienta = new LlamadaHerramientaAsistente("recomendar_inversion", "{\"porcentaje\":150}");
            Assert.Contains("Enter a valid percentage.", (await f.Crear().EnviarMensajeAsync("Quiero invertir el 150%")).Texto);
            Assert.AreEqual(7, f.Perfiles.PropietarioConsultado);
            Assert.IsEmpty(f.Inversion.Mercado.Llamadas);
        }

        [TestMethod]
        public async Task EnviarMensajeAsync_CambioDePerfil_ReleePropietarioYNoReenviaHistorialAnterior()
        {
            var f = new Fixture();
            f.Perfiles.Ultimo = Perfil();
            f.Historial.Consultas[1] = new ConsultaChatbot { Id = 1, Usuario = new Usuario { Id = 7 }, Pregunta = "Invertir", Respuesta = "Compra BTC", Fecha = DateTime.Now };
            f.Servicio.AlExtraerHerramienta = () =>
            {
                f.Sesion.CerrarSesion();
                f.Sesion.IniciarSesion(new Usuario { Id = 8 });
                f.Perfiles.Ultimo = Perfil(NivelRiesgoInversor.Bajo);
            };
            var sut = f.Crear();
            await sut.EnviarMensajeAsync("Invertir 20%");
            Assert.AreEqual(7, f.Perfiles.PropietarioConsultado);
            Assert.IsEmpty(f.Servicio.LastHistorial);
            Assert.Contains("\"Perfil\":\"Alto\"", f.Servicio.LastSolicitud.Contexto.DatosJson);
            Assert.Contains("\"Porcentaje\":20", f.Servicio.LastSolicitud.Contexto.DatosJson);
            Assert.Contains("\"Monto\":100", f.Servicio.LastSolicitud.Contexto.DatosJson);
            Assert.AreEqual(7, f.Unidad.ConsultaChatbotRepo.LastInserted.Usuario.Id);
            f.Sesion.CerrarSesion();
            f.Sesion.IniciarSesion(new Usuario { Id = 7 });
            f.Servicio.AlExtraerHerramienta = null;
            await sut.EnviarMensajeAsync("Invertir 20%");
            Assert.Contains("\"Perfil\":\"Bajo\"", f.Servicio.LastSolicitud.Contexto.DatosJson);
        }

        // El intervalo de siete dias es solo del notificador automatico (notificaciones-automaticas-diarias):
        // el chat persiste su intercambio como siempre y no valida ni registra nada sobre ese intervalo.
        [TestMethod]
        public async Task EnviarMensajeAsync_RecomendacionDeInversion_SoloPersisteElIntercambioSinTocarNotificaciones()
        {
            var f = new Fixture();
            f.Perfiles.Ultimo = Perfil();

            RespuestaAsistente r = await f.Crear().EnviarMensajeAsync("En que invierto lo que me sobra?");

            Assert.IsFalse(r.EsRechazo);
            Assert.AreEqual(1, f.Unidad.SaveCallCount);
            Assert.AreEqual(7, f.Unidad.ConsultaChatbotRepo.LastInserted.Usuario.Id);
            Assert.IsEmpty(f.Unidad.NotificacionRepo.Llamadas);
            Assert.IsEmpty(f.Unidad.NotificacionRepo.Notificaciones);
        }

        internal static PerfilInversor Perfil(NivelRiesgoInversor nivel = NivelRiesgoInversor.Alto)
        {
            var perfil = CalculadorPerfilInversor.Evaluar(1, CalculadorPerfilInversorTests.Respuestas("CDCDCCDDDC"));
            perfil.Usuario = new Usuario { Id = 7 };
            perfil.NivelRiesgo = nivel;
            perfil.FechaEvaluacionUtc = new DateTime(2026, 10, 20, 0, 0, 0, DateTimeKind.Utc);
            return perfil;
        }

        private sealed class Fixture
        {
            internal readonly InversionFixture Inversion = new InversionFixture(500);
            internal readonly PerfilInversorBLLTests.RepoPerfil Perfiles = new PerfilInversorBLLTests.RepoPerfil();
            internal readonly FakeUnidadDeTrabajo Unidad = new FakeUnidadDeTrabajo();
            internal readonly FakeSesion Sesion = new FakeSesion();
            internal readonly FakeConsultaChatbotRepositorio Historial = new FakeConsultaChatbotRepositorio();
            internal readonly FakeClasificadorConsultas Clasificador = new FakeClasificadorConsultas { NextCategoria = CategoriaConsulta.Inversiones };
            internal readonly FakeAsistenteFinancieroService Servicio = new FakeAsistenteFinancieroService
            {
                LlamadaHerramienta = new LlamadaHerramientaAsistente("recomendar_inversion", "{\"porcentaje\":20}"), NextRespuesta = "Orientacion."
            };
            internal Fixture() { Sesion.IniciarSesion(new Usuario { Id = 7 }); }
            internal AsistenteFinancieroBLL Crear()
            {
                var idioma = new FakeObservado();
                idioma.CambiarIdioma(new Idioma { Traducciones = new List<Traduccion>
                {
                    new Traduccion { Palabra = new Palabra { Key = "Assistant.Investment.ProfileRequired" }, Valor = "Complete your investor profile." },
                    new Traduccion { Palabra = new Palabra { Key = "Assistant.Investment.InvalidPercentage" }, Valor = "Enter a valid percentage." }
                } });
                return new AsistenteFinancieroBLL(Historial, new FakeChatbotFinancieroRepositorio(), Servicio, Clasificador,
                    new FakeLimiteUsoAsistente(100),
                    new FakeUnidadDeTrabajoFactory(Unidad), Sesion, new SpyBitacoraBLL(), idioma,
                    new GeneradorRecomendaciones(Servicio, new ValidadorRespuestaAsistente(), null, Inversion.Crear()), Perfiles);
            }
        }
    }
}
