using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ABSTRACCIONES;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SERVICIOS.Tests
{
    // Tarea 2.1: construccion de la peticion HTTPS (query codificada, parametros
    // fijos, contexto geografico, User-Agent y validacion del endpoint).
    [TestClass]
    public sealed class NominatimBusquedaUbicacionesServiceTests
    {
        private static ManejadorHttpFalso ManejadorVacio()
        {
            return new ManejadorHttpFalso(() => BusquedaUbicacionesPruebas.RespuestaXml(BusquedaUbicacionesPruebas.XmlVacio));
        }

        private static async Task<PeticionCapturada> BuscarYCapturarAsync(ConsultaUbicacion consulta, string endpoint = BusquedaUbicacionesPruebas.EndpointPorDefecto)
        {
            ManejadorHttpFalso manejador = ManejadorVacio();
            NominatimBusquedaUbicacionesService servicio = BusquedaUbicacionesPruebas.Crear(manejador, null, endpoint);

            await servicio.BuscarAsync(consulta, CancellationToken.None);

            Assert.AreEqual(1, manejador.CantidadPeticiones);
            return manejador.Peticiones[0];
        }

        [TestMethod]
        public async Task BuscarAsync_TextoConAcentosAmpersandNumeralYMas_CodificaCadaValor()
        {
            PeticionCapturada peticion = await BuscarYCapturarAsync(new ConsultaUbicacion("Café & Bar #5 + más", "es", null));

            StringAssert.Contains(peticion.Uri.Query, "q=Caf%C3%A9%20%26%20Bar%20%235%20%2B%20m%C3%A1s");
            Assert.AreEqual("Café & Bar #5 + más", BusquedaUbicacionesPruebas.ParsearConsulta(peticion.Uri)["q"]);
            Assert.AreEqual(string.Empty, peticion.Uri.Fragment);
        }

        [TestMethod]
        public async Task BuscarAsync_ConsultaSimple_EnviaGetHttpsConParametrosFijos()
        {
            PeticionCapturada peticion = await BuscarYCapturarAsync(new ConsultaUbicacion("Obelisco", "es", null));
            Dictionary<string, string> parametros = BusquedaUbicacionesPruebas.ParsearConsulta(peticion.Uri);

            Assert.AreEqual(System.Net.Http.HttpMethod.Get, peticion.Metodo);
            Assert.AreEqual("https", peticion.Uri.Scheme);
            Assert.AreEqual("nominatim.example.org", peticion.Uri.Host);
            Assert.AreEqual("/search", peticion.Uri.AbsolutePath);
            Assert.AreEqual("Obelisco", parametros["q"]);
            Assert.AreEqual("xml", parametros["format"]);
            Assert.AreEqual("5", parametros["limit"]);
            Assert.AreEqual("address,poi", parametros["layer"]);
            Assert.AreEqual("es", parametros["accept-language"]);
            Assert.IsFalse(parametros.ContainsKey("countrycodes"));
            Assert.IsFalse(parametros.ContainsKey("viewbox"));
            Assert.IsFalse(parametros.ContainsKey("bounded"));
        }

        [TestMethod]
        [DataRow("en", "en")]
        [DataRow("es", "es")]
        [DataRow("EN", "en")]
        [DataRow(null, "es")]
        [DataRow("fr", "es")]
        public async Task BuscarAsync_Idioma_SeNormalizaAEsOEn(string idioma, string esperado)
        {
            PeticionCapturada peticion = await BuscarYCapturarAsync(new ConsultaUbicacion("Obelisco", idioma, null));

            Assert.AreEqual(esperado, BusquedaUbicacionesPruebas.ParsearConsulta(peticion.Uri)["accept-language"]);
        }

        [TestMethod]
        public async Task BuscarAsync_ZonaPreferidaValida_EnviaViewboxNoAcotadoConCulturaInvariante()
        {
            CultureInfo anterior = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("es-AR");
            try
            {
                LimitesGeograficos zona = new LimitesGeograficos(-34.7, -58.5, -34.5, -58.3);

                PeticionCapturada peticion = await BuscarYCapturarAsync(new ConsultaUbicacion("Obelisco", "es", zona));
                Dictionary<string, string> parametros = BusquedaUbicacionesPruebas.ParsearConsulta(peticion.Uri);

                Assert.AreEqual("-58.5,-34.5,-58.3,-34.7", parametros["viewbox"]);
                Assert.AreEqual("0", parametros["bounded"]);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = anterior;
            }
        }

        [TestMethod]
        public async Task BuscarAsync_ZonaQueCruzaElAntimeridiano_OmiteElViewbox()
        {
            LimitesGeograficos zona = new LimitesGeograficos(-10, 170, 10, -170);

            PeticionCapturada peticion = await BuscarYCapturarAsync(new ConsultaUbicacion("Suva", "es", zona));
            Dictionary<string, string> parametros = BusquedaUbicacionesPruebas.ParsearConsulta(peticion.Uri);

            Assert.IsFalse(parametros.ContainsKey("viewbox"));
            Assert.IsFalse(parametros.ContainsKey("bounded"));
        }

        [TestMethod]
        [DataRow(double.NaN, -58.5, -34.5, -58.3)]
        [DataRow(-34.7, -58.5, double.PositiveInfinity, -58.3)]
        [DataRow(-34.5, -58.5, -34.7, -58.3)]
        [DataRow(-34.7, -58.3, -34.5, -58.3)]
        [DataRow(-95, -58.5, -34.5, -58.3)]
        [DataRow(-34.7, -58.5, 91, -58.3)]
        [DataRow(-34.7, -181, -34.5, -58.3)]
        [DataRow(-34.7, -58.5, -34.5, 181)]
        public async Task BuscarAsync_ZonaNoRepresentable_OmiteElViewbox(double sur, double oeste, double norte, double este)
        {
            PeticionCapturada peticion = await BuscarYCapturarAsync(new ConsultaUbicacion("Obelisco", "es", new LimitesGeograficos(sur, oeste, norte, este)));

            Assert.IsFalse(BusquedaUbicacionesPruebas.ParsearConsulta(peticion.Uri).ContainsKey("viewbox"));
        }

        [TestMethod]
        public async Task BuscarAsync_Peticion_IdentificaLaAplicacionConUserAgent()
        {
            PeticionCapturada peticion = await BuscarYCapturarAsync(new ConsultaUbicacion("Obelisco", "es", null));

            Assert.AreEqual("FinazAppTP-UAI/1.0", peticion.UserAgent);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow("http://nominatim.example.org/search")]
        [DataRow("ftp://nominatim.example.org/search")]
        [DataRow("no es una uri")]
        [DataRow("/search")]
        [DataRow("https://usuario:clave@nominatim.example.org/search")]
        [DataRow("https://nominatim.example.org/search?format=json")]
        public async Task BuscarAsync_EndpointInvalido_LanzaConfiguracionInvalidaSinEnviarPeticion(string endpoint)
        {
            ManejadorHttpFalso manejador = ManejadorVacio();
            NominatimBusquedaUbicacionesService servicio = BusquedaUbicacionesPruebas.Crear(manejador, null, endpoint);

            BusquedaUbicacionException ex = await Assert.ThrowsExactlyAsync<BusquedaUbicacionException>(
                () => servicio.BuscarAsync(new ConsultaUbicacion("Obelisco", "es", null), CancellationToken.None));

            Assert.AreEqual(EstadoBusquedaUbicacion.ConfiguracionInvalida, ex.Estado);
            Assert.AreEqual(0, manejador.CantidadPeticiones);
        }

        [TestMethod]
        public async Task BuscarAsync_EndpointInvalidoNoImpideConstruirElServicio()
        {
            // La validacion ocurre al buscar: construir el servicio con un endpoint
            // roto no debe lanzar (la seleccion manual de ubicacion sigue operativa).
            NominatimBusquedaUbicacionesService servicio = BusquedaUbicacionesPruebas.Crear(ManejadorVacio(), null, "http://inseguro");

            await Assert.ThrowsExactlyAsync<BusquedaUbicacionException>(
                () => servicio.BuscarAsync(new ConsultaUbicacion("x", "es", null), CancellationToken.None));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task BuscarAsync_TextoVacio_LanzaArgumentExceptionSinEnviarPeticion(string texto)
        {
            ManejadorHttpFalso manejador = ManejadorVacio();
            NominatimBusquedaUbicacionesService servicio = BusquedaUbicacionesPruebas.Crear(manejador);

            await Assert.ThrowsAsync<ArgumentException>(
                () => servicio.BuscarAsync(new ConsultaUbicacion(texto, "es", null), CancellationToken.None));

            Assert.AreEqual(0, manejador.CantidadPeticiones);
        }

        [TestMethod]
        public async Task BuscarAsync_ConsultaNula_LanzaArgumentNullException()
        {
            NominatimBusquedaUbicacionesService servicio = BusquedaUbicacionesPruebas.Crear(ManejadorVacio());

            await Assert.ThrowsAsync<ArgumentNullException>(() => servicio.BuscarAsync(null, CancellationToken.None));
        }
    }
}
