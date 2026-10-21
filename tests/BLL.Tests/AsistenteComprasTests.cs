using System.Collections.Generic;
using System.Threading.Tasks;
using ABSTRACCIONES;
using SERVICIOS;
using BLL.Tests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLL.Tests
{
    [TestClass]
    public sealed class AsistenteComprasTests
    {
        [TestMethod]
        [DataRow("Compra el 27/10/2026.", true)]
        [DataRow("Compra el 2026-10-27.", true)]
        [DataRow("Compra el 27/10.", true)]
        [DataRow("Compra el 19/10/2026.", false)]
        [DataRow("Compra el 2026-10-19.", false)]
        public async Task EnviarMensajeAsync_FechaExplicita_SoloAdmiteFechasDelResumen(string texto, bool rechazo)
        {
            ComprasFixture f = new ComprasFixture();
            f.Oferta(1, "Super", 100);
            f.Promocion();
            FakeUnidadDeTrabajo unidad = new FakeUnidadDeTrabajo();
            RespuestaAsistente r = await Crear(f, new FakeAsistenteFinancieroService { NextRespuesta = texto }, unidad).EnviarMensajeAsync("Donde y cuando compro?");
            Assert.AreEqual(rechazo, r.EsRechazo);
            Assert.AreEqual(!rechazo, unidad.SaveCalled);
        }

        [TestMethod]
        public async Task EnviarMensajeAsync_CambiaLaSesionDuranteLaRed_ConservaElUsuarioCapturadoAlEnviar()
        {
            ComprasFixture f = new ComprasFixture();
            f.Oferta(1, "Super propio", 100);
            FakeAsistenteFinancieroService servicio = new FakeAsistenteFinancieroService
            {
                NextRespuesta = "Compra en Super propio.",
                AlExtraerHerramienta = () => { f.Sesion.CerrarSesion(); f.Sesion.IniciarSesion(new BE.Usuario { Id = 8 }); }
            };
            FakeUnidadDeTrabajo unidad = new FakeUnidadDeTrabajo();
            RespuestaAsistente respuesta = await Crear(f, servicio, unidad).EnviarMensajeAsync("Donde compro lo de mi lista?");
            Assert.AreEqual(7, f.Repo.UsuarioItems);
            Assert.AreEqual(7, f.Repo.UsuarioCuentas);
            Assert.Contains("Super propio", servicio.LastSolicitud.Contexto.DatosJson);
            Assert.IsFalse(respuesta.EsRechazo);
            Assert.AreEqual(7, unidad.ConsultaChatbotRepo.LastInserted.Usuario.Id);
        }

        [TestMethod]
        public async Task EnviarMensajeAsync_SinZonasEItemNoEncontrado_AgregaAvisosTraducidosYLosPersiste()
        {
            ComprasFixture f = new ComprasFixture();
            FakeAsistenteFinancieroService servicio = new FakeAsistenteFinancieroService { NextRespuesta = "Respuesta calculada." };
            FakeUnidadDeTrabajo unidad = new FakeUnidadDeTrabajo();
            FakeObservado idioma = new FakeObservado();
            idioma.CambiarIdioma(new SERVICIOS.Idioma { Traducciones = new List<SERVICIOS.Traduccion>
            {
                new SERVICIOS.Traduccion { Palabra = new SERVICIOS.Palabra { Key = "Assistant.Shopping.NoZones" }, Valor = "Load your zones." },
                new SERVICIOS.Traduccion { Palabra = new SERVICIOS.Palabra { Key = "Assistant.Shopping.NotFound" }, Valor = "Not found: {0}" }
            } });
            RespuestaAsistente respuesta = await Crear(f, servicio, unidad, idioma).EnviarMensajeAsync("Donde compro lo de mi lista?");
            Assert.Contains("Load your zones.", respuesta.Texto);
            Assert.Contains("Not found: Cerveza", respuesta.Texto);
            Assert.AreEqual(respuesta.Texto, unidad.ConsultaChatbotRepo.LastInserted.Respuesta);
        }

        [TestMethod]
        [DataRow("{\"nombreLista\":\"Semanal\"}")]
        [DataRow("{}")]
        [DataRow("{\"nombreLista\":123}")]
        [DataRow("no es json")]
        public async Task EnviarMensajeAsync_Compras_ExtraeListaCalculaYRedactaConContexto(string argumentos)
        {
            ComprasFixture f = new ComprasFixture();
            f.Oferta(1, "Super real", 100);
            FakeAsistenteFinancieroService servicio = new FakeAsistenteFinancieroService
            {
                LlamadaHerramienta = new LlamadaHerramientaAsistente("recomendar_compras", argumentos),
                NextRespuesta = "Compra en Super real."
            };
            FakeUnidadDeTrabajo unidad = new FakeUnidadDeTrabajo();
            AsistenteFinancieroBLL sut = Crear(f, servicio, unidad);
            RespuestaAsistente r = await sut.EnviarMensajeAsync("Donde compro lo de mi lista Semanal?");
            Assert.IsFalse(r.EsRechazo);
            Assert.AreEqual("recomendar_compras", servicio.UltimaHerramienta?.Nombre);
            Assert.DoesNotContain("usuario", servicio.UltimaHerramienta.ParametrosJson);
            Assert.Contains("Super real", servicio.LastSolicitud.Contexto.DatosJson);
            Assert.IsTrue(servicio.LastSolicitud.Contexto.ReferenciasPermitidas.Count > 0);
            CollectionAssert.AreEqual(new[] { "herramienta", "respuesta" }, servicio.OrdenLlamadas);
            Assert.IsTrue(unidad.SaveCalled);
        }

        [TestMethod]
        public async Task EnviarMensajeAsync_RespuestaCitaComercioFueraDelResumen_LaDescartaSinGuardar()
        {
            ComprasFixture f = new ComprasFixture();
            for (int i = 1; i <= 4; i++) f.Oferta(i, "Comercio " + i, 100 * i);
            FakeAsistenteFinancieroService servicio = new FakeAsistenteFinancieroService { NextRespuesta = "Compra en Comercio 4." };
            FakeUnidadDeTrabajo unidad = new FakeUnidadDeTrabajo();
            RespuestaAsistente r = await Crear(f, servicio, unidad).EnviarMensajeAsync("Donde compro lo de mi lista?");
            Assert.IsTrue(r.EsRechazo);
            Assert.AreEqual(AsistenteFinancieroBLL.RechazoRespuestaNoConfiable, r.ClaveRechazo);
            Assert.IsFalse(unidad.SaveCalled);
        }

        private static AsistenteFinancieroBLL Crear(ComprasFixture f, FakeAsistenteFinancieroService servicio, FakeUnidadDeTrabajo unidad, IObservado idioma = null)
        {
            return new AsistenteFinancieroBLL(new FakeConsultaChatbotRepositorio(), new FakeChatbotFinancieroRepositorio(), servicio,
                new FakeClasificadorConsultas { NextCategoria = CategoriaConsulta.Compras }, new FakeLimiteUsoAsistente(1000),
                new FakeUnidadDeTrabajoFactory(unidad), f.Sesion, new SpyBitacoraBLL(), idioma ?? new FakeObservado(),
                new GeneradorRecomendaciones(servicio, new ValidadorRespuestaAsistente(), f.Crear(), null));
        }
    }
}
