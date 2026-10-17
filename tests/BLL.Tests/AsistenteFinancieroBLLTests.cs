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
    public sealed class AsistenteFinancieroBLLTests
    {
        [TestMethod]
        public async Task EnviarMensajeAsync_PreguntaVacia_ThrowsExcepcionNegocioAndDoesNotCallService()
        {
            AsistenteFixture fixture = new AsistenteFixture(usuarioActualId: 1);
            AsistenteFinancieroBLL sut = fixture.CreateSut();

            await Assert.ThrowsExactlyAsync<ExcepcionNegocio>(() => sut.EnviarMensajeAsync("   "));
            Assert.IsNull(fixture.Servicio.LastPregunta);
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public async Task EnviarMensajeAsync_Valido_ConsultaServicioPersisteAuditaYGuarda()
        {
            AsistenteFixture fixture = new AsistenteFixture(usuarioActualId: 3);
            fixture.Servicio.NextRespuesta = "Para ahorrar mejor, revisa tus gastos fijos.";
            AsistenteFinancieroBLL sut = fixture.CreateSut();

            string respuesta = await sut.EnviarMensajeAsync("Como ahorro mejor?");

            Assert.AreEqual("Para ahorrar mejor, revisa tus gastos fijos.", respuesta);
            Assert.AreEqual("Como ahorro mejor?", fixture.Servicio.LastPregunta);

            ConsultaChatbot insertada = fixture.Unidad.ConsultaChatbotRepo.LastInserted;
            Assert.IsNotNull(insertada);
            Assert.AreEqual(3, insertada.UsuarioId);
            Assert.AreEqual("Como ahorro mejor?", insertada.Pregunta);
            Assert.AreEqual("Para ahorrar mejor, revisa tus gastos fijos.", insertada.Respuesta);
            Assert.IsTrue(insertada.EsPregunta);
            Assert.AreEqual(fixture.Chatbot.Chatbot.Id, insertada.ChatbotFinancieroId);

            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls.Single().Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public async Task EnviarMensajeAsync_ConHistorialPrevio_LoPasaAlServicioOrdenadoPorFecha()
        {
            AsistenteFixture fixture = new AsistenteFixture(usuarioActualId: 1);
            fixture.Unidad.ConsultaChatbotRepo.Consultas.Add(1, new ConsultaChatbot
            {
                Id = 1,
                ChatbotFinancieroId = 1,
                UsuarioId = 1,
                Pregunta = "Primera",
                Respuesta = "PrimeraRespuesta",
                Fecha = DateTime.Now.AddMinutes(-10),
                EsPregunta = true
            });
            fixture.Unidad.ConsultaChatbotRepo.Consultas.Add(2, new ConsultaChatbot
            {
                Id = 2,
                ChatbotFinancieroId = 1,
                UsuarioId = 1,
                Pregunta = "Segunda",
                Respuesta = "SegundaRespuesta",
                Fecha = DateTime.Now.AddMinutes(-5),
                EsPregunta = true
            });
            AsistenteFinancieroBLL sut = fixture.CreateSut();

            await sut.EnviarMensajeAsync("Tercera pregunta");

            Assert.AreEqual(2, fixture.Servicio.LastHistorial.Count);
            Assert.AreEqual("Primera", fixture.Servicio.LastHistorial[0].Pregunta);
            Assert.AreEqual("Segunda", fixture.Servicio.LastHistorial[1].Pregunta);
        }

        [TestMethod]
        public async Task EnviarMensajeAsync_ServicioFalla_ThrowsExcepcionTecnicaAndDoesNotCreateUow()
        {
            AsistenteFixture fixture = new AsistenteFixture(usuarioActualId: 1);
            fixture.Servicio.ThrowException = new InvalidOperationException("boom");
            AsistenteFinancieroBLL sut = fixture.CreateSut();

            await Assert.ThrowsExactlyAsync<ExcepcionTecnica>(() => sut.EnviarMensajeAsync("Hola"));
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void ObtenerHistorial_DevuelveConsultasDelUsuarioActualOrdenadasPorFecha()
        {
            AsistenteFixture fixture = new AsistenteFixture(usuarioActualId: 5);
            fixture.ConsultaRepo.Consultas.Add(1, new ConsultaChatbot { Id = 1, UsuarioId = 5, Pregunta = "P1", Respuesta = "R1", Fecha = DateTime.Now.AddMinutes(-1), EsPregunta = true });
            fixture.ConsultaRepo.Consultas.Add(2, new ConsultaChatbot { Id = 2, UsuarioId = 5, Pregunta = "P2", Respuesta = "R2", Fecha = DateTime.Now, EsPregunta = true });
            fixture.ConsultaRepo.Consultas.Add(3, new ConsultaChatbot { Id = 3, UsuarioId = 99, Pregunta = "Ajena", Respuesta = "Ajena", Fecha = DateTime.Now, EsPregunta = true });
            AsistenteFinancieroBLL sut = fixture.CreateSut();

            IReadOnlyList<ConsultaChatbot> historial = sut.ObtenerHistorial();

            Assert.AreEqual(2, historial.Count);
            Assert.AreEqual("P1", historial[0].Pregunta);
            Assert.AreEqual("P2", historial[1].Pregunta);
        }

        private sealed class AsistenteFixture
        {
            public readonly FakeConsultaChatbotRepositorio ConsultaRepo = new FakeConsultaChatbotRepositorio();
            public readonly FakeChatbotFinancieroRepositorio Chatbot = new FakeChatbotFinancieroRepositorio();
            public readonly FakeAsistenteFinancieroService Servicio = new FakeAsistenteFinancieroService();
            public readonly SpyBitacoraBLL Bitacora = new SpyBitacoraBLL();
            public readonly FakeSesion Sesion = new FakeSesion();
            public readonly FakeUnidadDeTrabajo Unidad;
            public readonly FakeUnidadDeTrabajoFactory UnidadFactory;

            public AsistenteFixture(int usuarioActualId)
            {
                Unidad = new FakeUnidadDeTrabajo
                {
                    ConsultaChatbotRepo = ConsultaRepo
                };
                UnidadFactory = new FakeUnidadDeTrabajoFactory(Unidad);
                Sesion.IniciarSesion(new Usuario { Id = usuarioActualId, Username = "test" });
            }

            public AsistenteFinancieroBLL CreateSut()
            {
                return new AsistenteFinancieroBLL(
                    ConsultaRepo,
                    Chatbot,
                    Servicio,
                    UnidadFactory,
                    Sesion,
                    Bitacora);
            }
        }
    }
}
