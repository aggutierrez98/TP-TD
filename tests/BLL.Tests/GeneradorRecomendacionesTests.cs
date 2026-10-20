using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ABSTRACCIONES;
using BE;
using BLL.Tests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SERVICIOS;

namespace BLL.Tests
{
    // Colaborador compartido por el chat y por la generacion automatica: calcula, redacta,
    // valida y agrega los anexos fijos sin depender de sesion, clasificador ni function
    // calling, y con el idioma del destinatario recibido por llamada.
    [TestClass]
    public sealed class GeneradorRecomendacionesTests
    {
        private static IIdioma Idioma(string prefijo)
        {
            return new Idioma { Traducciones = new List<SERVICIOS.Traduccion>
            {
                Traduccion("Assistant.Shopping.NoZones", prefijo + " zones"),
                Traduccion("Assistant.Shopping.NotFound", prefijo + " not found: {0}"),
                Traduccion("Assistant.Shopping.Omitted", prefijo + " analyzed {0}, skipped {1}"),
                Traduccion("Assistant.Investment.MissingData", prefijo + " missing: {0}"),
                Traduccion("Assistant.Investment.UnverifiedCompatibility", prefijo + " unverified"),
                Traduccion("Assistant.Investment.Disclaimer", prefijo + " disclaimer"),
                Traduccion("Assistant.Automatic.ShoppingQuestion", prefijo + " where to buy"),
                Traduccion("Assistant.Automatic.InvestmentQuestion", prefijo + " what to invest")
            } };
        }

        private static SERVICIOS.Traduccion Traduccion(string key, string valor) => new SERVICIOS.Traduccion { Palabra = new SERVICIOS.Palabra { Key = key }, Valor = valor };

        private static GeneradorRecomendaciones Crear(FakeAsistenteFinancieroService servicio, ComprasFixture compras, InversionFixture inversion)
        {
            return new GeneradorRecomendaciones(servicio, new ValidadorRespuestaAsistente(), compras?.Crear(), inversion?.Crear());
        }

        private static SolicitudGeneracion Compras(IIdioma idioma, string pregunta = "Donde compro lo de mi lista?", int usuarioId = 7)
        {
            return new SolicitudGeneracion { Categoria = CategoriaConsulta.Compras, UsuarioId = usuarioId, Pregunta = pregunta, Idioma = idioma };
        }

        private static SolicitudGeneracion Inversion(IIdioma idioma, decimal? porcentaje = null, string pregunta = "En que invierto lo que me sobra?", int usuarioId = 7)
        {
            return new SolicitudGeneracion { Categoria = CategoriaConsulta.Inversiones, UsuarioId = usuarioId, Pregunta = pregunta, Idioma = idioma, Perfil = PerfilAsistenteTests.Perfil(), Porcentaje = porcentaje };
        }

        [TestMethod]
        public async Task GenerarAsync_Compras_NoNecesitaSesionNiHerramientaYUsaElUsuarioRecibido()
        {
            ComprasFixture f = new ComprasFixture();
            f.Oferta(1, "Super propio", 100);
            f.Sesion.CerrarSesion();
            FakeAsistenteFinancieroService servicio = new FakeAsistenteFinancieroService { NextRespuesta = "Compra en Super propio." };
            ResultadoGeneracion r = await Crear(servicio, f, null).GenerarAsync(Compras(Idioma("A")), CancellationToken.None);
            Assert.IsTrue(r.Aceptada);
            Assert.AreEqual(7, f.Repo.UsuarioItems);
            Assert.AreEqual(7, f.Repo.UsuarioCuentas);
            CollectionAssert.AreEqual(new[] { "respuesta" }, servicio.OrdenLlamadas);
            Assert.Contains("Super propio", servicio.LastSolicitud.Contexto.DatosJson);
            Assert.AreEqual(CategoriaConsulta.Compras, servicio.LastSolicitud.Categoria);
        }

        [TestMethod]
        public async Task GenerarAsync_SinPregunta_UsaLaIntencionFijaTraducidaYNoAgregaHistorial()
        {
            ComprasFixture f = new ComprasFixture();
            f.Oferta(1, "Super propio", 100);
            FakeAsistenteFinancieroService servicio = new FakeAsistenteFinancieroService { NextRespuesta = "Compra en Super propio." };
            SolicitudGeneracion s = Compras(Idioma("A"), null);
            await Crear(servicio, f, null).GenerarAsync(s, CancellationToken.None);
            Assert.AreEqual("A where to buy", servicio.LastSolicitud.Pregunta);
            Assert.IsEmpty(servicio.LastSolicitud.Historial);
        }

        [TestMethod]
        public async Task GenerarAsync_IdiomaPorDestinatario_CadaLlamadaUsaSuPropioIdiomaSinEstadoCompartido()
        {
            ComprasFixture f = new ComprasFixture();
            FakeAsistenteFinancieroService servicio = new FakeAsistenteFinancieroService { NextRespuesta = "Respuesta calculada." };
            GeneradorRecomendaciones sut = Crear(servicio, f, null);
            IIdioma ingles = Idioma("EN");
            IIdioma portugues = Idioma("PT");
            ResultadoGeneracion en = await sut.GenerarAsync(Compras(ingles, null), CancellationToken.None);
            string preguntaEn = servicio.LastSolicitud.Pregunta;
            ResultadoGeneracion pt = await sut.GenerarAsync(Compras(portugues, null), CancellationToken.None);
            string preguntaPt = servicio.LastSolicitud.Pregunta;
            ResultadoGeneracion sin = await sut.GenerarAsync(Compras(null, "Donde compro?"), CancellationToken.None);
            Assert.Contains("EN zones", en.Texto);
            Assert.Contains("EN not found: Cerveza", en.Texto);
            Assert.DoesNotContain("PT", en.Texto);
            Assert.Contains("PT zones", pt.Texto);
            Assert.DoesNotContain("EN", pt.Texto);
            Assert.AreEqual("EN where to buy", preguntaEn);
            Assert.AreEqual("PT where to buy", preguntaPt);
            Assert.Contains("Assistant.Shopping.NoZones", sin.Texto);
        }

        [TestMethod]
        [DataRow("Compra el 27/10/2026.", true)]
        [DataRow("Compra el 2026-10-27.", true)]
        [DataRow("Compra el 27/10.", true)]
        [DataRow("Compra el 19/10/2026.", false)]
        [DataRow("Compra el 2026-10-19.", false)]
        public async Task GenerarAsync_Compras_FechaExplicitaFueraDelResumen_NoSeAcepta(string texto, bool rechazada)
        {
            ComprasFixture f = new ComprasFixture();
            f.Oferta(1, "Super", 100);
            f.Promocion();
            ResultadoGeneracion r = await Crear(new FakeAsistenteFinancieroService { NextRespuesta = texto }, f, null).GenerarAsync(Compras(Idioma("A")), CancellationToken.None);
            Assert.AreEqual(!rechazada, r.Aceptada);
            if (rechazada) Assert.IsNull(r.Texto);
        }

        [TestMethod]
        public async Task GenerarAsync_Compras_ReferenciaFueraDelContextoCerrado_NoSeAcepta()
        {
            ComprasFixture f = new ComprasFixture();
            for (int i = 1; i <= 4; i++) f.Oferta(i, "Comercio " + i, 100 * i);
            ResultadoGeneracion r = await Crear(new FakeAsistenteFinancieroService { NextRespuesta = "Compra en Comercio 4." }, f, null).GenerarAsync(Compras(Idioma("A")), CancellationToken.None);
            Assert.IsFalse(r.Aceptada);
        }

        [TestMethod]
        public async Task GenerarAsync_Compras_FuentesParciales_AgregaAvisosFijosYNoLosDeInversion()
        {
            ComprasFixture f = new ComprasFixture();
            ResultadoGeneracion r = await Crear(new FakeAsistenteFinancieroService { NextRespuesta = "Respuesta calculada." }, f, null).GenerarAsync(Compras(Idioma("A")), CancellationToken.None);
            Assert.IsTrue(r.Aceptada);
            Assert.AreEqual("Respuesta calculada." + Environment.NewLine + "A zones" + Environment.NewLine + "A not found: Cerveza", r.Texto);
        }

        [TestMethod]
        public async Task GenerarAsync_Compras_MasDeVeinteItems_InformaElLimiteAnalizadoYLosOmitidos()
        {
            ComprasFixture f = new ComprasFixture();
            f.Oferta(1, "Super", 100);
            f.Repo.Items.Clear();
            for (int i = 1; i <= 25; i++)
                f.Repo.Items.Add(new ItemLista { Id = i, Descripcion = "Cerveza", Cantidad = 1, ListaDeCompras = new ListaDeCompras { Nombre = "Semanal", Usuario = new Usuario { Id = 7 } } });
            ResultadoGeneracion r = await Crear(new FakeAsistenteFinancieroService { NextRespuesta = "Respuesta calculada." }, f, null).GenerarAsync(Compras(Idioma("A")), CancellationToken.None);
            Assert.IsTrue(r.Aceptada);
            Assert.EndsWith("A analyzed 20, skipped 5", r.Texto);
        }

        [TestMethod]
        public async Task GenerarAsync_Compras_ConVeinteItemsONenos_NoAgregaAvisoDeLimite()
        {
            ComprasFixture f = new ComprasFixture();
            ResultadoGeneracion r = await Crear(new FakeAsistenteFinancieroService { NextRespuesta = "Respuesta calculada." }, f, null).GenerarAsync(Compras(Idioma("A")), CancellationToken.None);
            Assert.DoesNotContain("analyzed", r.Texto);
        }

        [TestMethod]
        public async Task GenerarAsync_Inversiones_ConPorcentaje_ContextoConMontoYAnexosFijosEnOrden()
        {
            InversionFixture f = new InversionFixture(500);
            f.Mercado.Fallas.Add("BTC");
            FakeAsistenteFinancieroService servicio = new FakeAsistenteFinancieroService { NextRespuesta = "Orientacion calculada." };
            ResultadoGeneracion r = await Crear(servicio, null, f).GenerarAsync(Inversion(Idioma("A"), 20m), CancellationToken.None);
            Assert.IsTrue(r.Aceptada);
            Assert.Contains("\"Monto\":100", servicio.LastSolicitud.Contexto.DatosJson);
            Assert.IsEmpty(servicio.LastSolicitud.Historial);
            Assert.AreEqual("Orientacion calculada." + Environment.NewLine + "A missing: criptoya BTC" + Environment.NewLine + "A disclaimer", r.Texto);
            CollectionAssert.AreEqual(new[] { "respuesta" }, servicio.OrdenLlamadas);
        }

        [TestMethod]
        public async Task GenerarAsync_Inversiones_SinPorcentaje_NoPresentaMontoNiPorcentajeYConservaPerfil()
        {
            InversionFixture f = new InversionFixture(500);
            FakeAsistenteFinancieroService servicio = new FakeAsistenteFinancieroService { NextRespuesta = "Orientacion calculada." };
            ResultadoGeneracion r = await Crear(servicio, null, f).GenerarAsync(Inversion(Idioma("A")), CancellationToken.None);
            Assert.IsTrue(r.Aceptada);
            Assert.DoesNotContain("Monto", servicio.LastSolicitud.Contexto.DatosJson);
            Assert.DoesNotContain("Porcentaje", servicio.LastSolicitud.Contexto.DatosJson);
            Assert.Contains("\"Perfil\":\"Alto\"", servicio.LastSolicitud.Contexto.DatosJson);
            Assert.EndsWith("A disclaimer", r.Texto);
        }

        [TestMethod]
        public async Task GenerarAsync_Inversiones_InstrumentoConocidoNoSeleccionado_NoSeAcepta()
        {
            InversionFixture f = new InversionFixture(850);
            f.Mercado.Fondos = Enumerable.Range(1, 4).Select(i => new DatoMercado { Nombre = "Fondo " + i, Tipo = "FCI rentaFija", Vcp = 100, Fecha = f.Hoy, MesesInmovilizacion = 0, DiasHabilesDisponibilidad = 0, RequierePreaviso = false, RequierePermanencia = false, FuenteCondiciones = "Fixture v1" }).ToList();
            ResultadoGeneracion r = await Crear(new FakeAsistenteFinancieroService { NextRespuesta = "Compra Fondo 4." }, null, f).GenerarAsync(Inversion(Idioma("A")), CancellationToken.None);
            Assert.IsFalse(r.Aceptada);
        }

        [TestMethod]
        public async Task GenerarAsync_Inversiones_SinPerfil_ExigePerfilSinConsultarFuentes()
        {
            InversionFixture f = new InversionFixture(500);
            SolicitudGeneracion s = Inversion(Idioma("A"));
            s.Perfil = null;
            ExcepcionNegocio ex = await Assert.ThrowsExactlyAsync<ExcepcionNegocio>(() => Crear(new FakeAsistenteFinancieroService(), null, f).GenerarAsync(s, CancellationToken.None));
            Assert.AreEqual("Assistant.Investment.ProfileRequired", ex.Message);
            Assert.IsEmpty(f.Mercado.Llamadas);
        }

        [TestMethod]
        public async Task GenerarAsync_Inversiones_OmitirSiSinAlternativas_NoRedactaSinInstrumentosVerificables()
        {
            InversionFixture f = new InversionFixture(500);
            f.Mercado.FallaTodo = true;
            FakeAsistenteFinancieroService servicio = new FakeAsistenteFinancieroService { NextRespuesta = "Orientacion calculada." };
            SolicitudGeneracion s = Inversion(Idioma("A"));
            s.OmitirSiSinAlternativas = true;

            ResultadoGeneracion r = await Crear(servicio, null, f).GenerarAsync(s, CancellationToken.None);

            Assert.IsFalse(r.Aceptada);
            Assert.IsTrue(r.SinAlternativas);
            Assert.IsNull(r.Texto);
            Assert.AreEqual(0, servicio.CallCount);
        }

        [TestMethod]
        public async Task GenerarAsync_Inversiones_SinAlternativasEnElChat_SigueRedactandoLaExplicacion()
        {
            InversionFixture f = new InversionFixture(500);
            f.Mercado.FallaTodo = true;
            FakeAsistenteFinancieroService servicio = new FakeAsistenteFinancieroService { NextRespuesta = "Orientacion calculada." };

            ResultadoGeneracion r = await Crear(servicio, null, f).GenerarAsync(Inversion(Idioma("A")), CancellationToken.None);

            Assert.IsFalse(r.SinAlternativas);
            Assert.AreEqual(1, servicio.CallCount);
        }

        [TestMethod]
        public async Task GenerarAsync_Inversiones_OmitirSiSinAlternativasConFuentesParciales_RecomiendaSoloLoVerificadoYAvisa()
        {
            InversionFixture f = new InversionFixture(500);
            f.Mercado.Fallas.Add("BTC");
            FakeAsistenteFinancieroService servicio = new FakeAsistenteFinancieroService { NextRespuesta = "Orientacion calculada." };
            SolicitudGeneracion s = Inversion(Idioma("A"));
            s.OmitirSiSinAlternativas = true;

            ResultadoGeneracion r = await Crear(servicio, null, f).GenerarAsync(s, CancellationToken.None);

            Assert.IsTrue(r.Aceptada);
            Assert.IsFalse(r.SinAlternativas);
            Assert.Contains("A missing: criptoya BTC", r.Texto);
            Assert.EndsWith("A disclaimer", r.Texto);
        }

        [TestMethod]
        public async Task GenerarAsync_FalloDelServicioDeRedaccion_SeInformaComoErrorTecnico()
        {
            ComprasFixture f = new ComprasFixture();
            FakeAsistenteFinancieroService servicio = new FakeAsistenteFinancieroService { ThrowException = new InvalidOperationException("red") };
            ExcepcionTecnica ex = await Assert.ThrowsExactlyAsync<ExcepcionTecnica>(() => Crear(servicio, f, null).GenerarAsync(Compras(Idioma("A")), CancellationToken.None));
            Assert.AreEqual("Error.Assistant.ResponseFailed", ex.Message);
        }

        [TestMethod]
        public async Task GenerarAsync_Cancelado_NoCalculaNiRedacta()
        {
            ComprasFixture f = new ComprasFixture();
            FakeAsistenteFinancieroService servicio = new FakeAsistenteFinancieroService();
            await Assert.ThrowsAsync<OperationCanceledException>(() => Crear(servicio, f, null).GenerarAsync(Compras(Idioma("A")), new CancellationToken(true)));
            Assert.AreEqual(0, servicio.CallCount);
        }

        [TestMethod]
        public async Task GenerarAsync_CategoriaGeneral_RedactaConContextoVacioYValidaSinReferencias()
        {
            FakeAsistenteFinancieroService servicio = new FakeAsistenteFinancieroService { NextRespuesta = "Un plazo fijo es un deposito." };
            var historial = new List<IntercambioChatbot> { new IntercambioChatbot { Pregunta = "Hola", Respuesta = "Chau" } };
            SolicitudGeneracion s = new SolicitudGeneracion { Categoria = CategoriaConsulta.FinanzasGenerales, UsuarioId = 7, Pregunta = "Que es un plazo fijo?", Historial = historial, Idioma = Idioma("A") };
            ResultadoGeneracion r = await Crear(servicio, null, null).GenerarAsync(s, CancellationToken.None);
            Assert.IsTrue(r.Aceptada);
            Assert.AreEqual("Un plazo fijo es un deposito.", r.Texto);
            Assert.AreEqual(1, servicio.LastSolicitud.Historial.Count);
            Assert.IsNull(servicio.LastSolicitud.Contexto.DatosJson);
        }

        [TestMethod]
        public async Task GenerarAsync_ParidadConElChat_MismoTextoParaCompras()
        {
            ComprasFixture f = new ComprasFixture();
            f.Oferta(1, "Super real", 100);
            IIdioma idioma = Idioma("A");
            FakeAsistenteFinancieroService servicioChat = new FakeAsistenteFinancieroService { NextRespuesta = "Compra en Super real.", LlamadaHerramienta = new LlamadaHerramientaAsistente("recomendar_compras", "{}") };
            FakeUnidadDeTrabajo unidad = new FakeUnidadDeTrabajo();
            FakeObservado observado = new FakeObservado();
            observado.CambiarIdioma(idioma);
            AsistenteFinancieroBLL chat = new AsistenteFinancieroBLL(new FakeConsultaChatbotRepositorio(), new FakeChatbotFinancieroRepositorio(), servicioChat,
                new FakeClasificadorConsultas { NextCategoria = CategoriaConsulta.Compras }, new LimiteUsoAsistente(1000, () => ComprasFixture.Hoy),
                new FakeUnidadDeTrabajoFactory(unidad), f.Sesion, new SpyBitacoraBLL(), observado, Crear(servicioChat, f, null));
            RespuestaAsistente delChat = await chat.EnviarMensajeAsync("Donde compro lo de mi lista?");
            FakeAsistenteFinancieroService servicioBatch = new FakeAsistenteFinancieroService { NextRespuesta = "Compra en Super real." };
            ResultadoGeneracion delBatch = await Crear(servicioBatch, f, null).GenerarAsync(Compras(idioma), CancellationToken.None);
            Assert.AreEqual(delChat.Texto, delBatch.Texto);
            Assert.AreEqual(servicioChat.LastSolicitud.Contexto.DatosJson, servicioBatch.LastSolicitud.Contexto.DatosJson);
        }

        [TestMethod]
        public async Task GenerarAsync_ParidadConElChat_MismoTextoParaInversionesConPorcentaje()
        {
            InversionFixture f = new InversionFixture(500);
            IIdioma idioma = Idioma("A");
            FakeAsistenteFinancieroService servicioChat = new FakeAsistenteFinancieroService { NextRespuesta = "Orientacion calculada.", LlamadaHerramienta = new LlamadaHerramientaAsistente("recomendar_inversion", "{\"porcentaje\":20}") };
            FakeSesion sesion = new FakeSesion();
            sesion.IniciarSesion(new Usuario { Id = 7 });
            FakeObservado observado = new FakeObservado();
            observado.CambiarIdioma(idioma);
            AsistenteFinancieroBLL chat = new AsistenteFinancieroBLL(new FakeConsultaChatbotRepositorio(), new FakeChatbotFinancieroRepositorio(), servicioChat,
                new FakeClasificadorConsultas { NextCategoria = CategoriaConsulta.Inversiones }, new LimiteUsoAsistente(1000, () => f.Hoy),
                new FakeUnidadDeTrabajoFactory(new FakeUnidadDeTrabajo()), sesion, new SpyBitacoraBLL(), observado, Crear(servicioChat, null, f),
                new PerfilInversorBLLTests.RepoPerfil { Ultimo = PerfilAsistenteTests.Perfil() });
            RespuestaAsistente delChat = await chat.EnviarMensajeAsync("Quiero invertir el 20%");
            FakeAsistenteFinancieroService servicioBatch = new FakeAsistenteFinancieroService { NextRespuesta = "Orientacion calculada." };
            ResultadoGeneracion delBatch = await Crear(servicioBatch, null, new InversionFixture(500)).GenerarAsync(Inversion(idioma, 20m), CancellationToken.None);
            Assert.AreEqual(delChat.Texto, delBatch.Texto);
            Assert.AreEqual(servicioChat.LastSolicitud.Contexto.DatosJson, servicioBatch.LastSolicitud.Contexto.DatosJson);
        }
    }
}
