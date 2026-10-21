using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ABSTRACCIONES;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SERVICIOS.Tests
{
    // Tarea 2.2: interpretacion acotada y segura de la respuesta XML.
    [TestClass]
    public sealed class NominatimBusquedaUbicacionesXmlTests
    {
        private static Task<IReadOnlyList<IResultadoBusquedaUbicacion>> BuscarConRespuestaAsync(HttpResponseMessage respuesta)
        {
            ManejadorHttpFalso manejador = new ManejadorHttpFalso(() => respuesta);
            NominatimBusquedaUbicacionesService servicio = BusquedaUbicacionesPruebas.Crear(manejador);
            return servicio.BuscarAsync(new ConsultaUbicacion("Obelisco", "es", null), CancellationToken.None);
        }

        private static Task<IReadOnlyList<IResultadoBusquedaUbicacion>> BuscarConXmlAsync(string xml)
        {
            return BuscarConRespuestaAsync(BusquedaUbicacionesPruebas.RespuestaXml(xml));
        }

        private static async Task AssertRespuestaInvalidaAsync(Func<Task<IReadOnlyList<IResultadoBusquedaUbicacion>>> accion)
        {
            BusquedaUbicacionException ex = await Assert.ThrowsExactlyAsync<BusquedaUbicacionException>(() => accion());
            Assert.AreEqual(EstadoBusquedaUbicacion.RespuestaInvalida, ex.Estado);
        }

        [TestMethod]
        public async Task BuscarAsync_UnaCoincidencia_MapeaDescripcionCoordenadasTipoYLimites()
        {
            string xml = BusquedaUbicacionesPruebas.Resultados(
                BusquedaUbicacionesPruebas.Place("Café Tortoni, Avenida de Mayo 829, Buenos Aires", "-34.6091", "-58.3797", "-34.6092,-34.6090,-58.3798,-58.3796", "cafe", "amenity"));

            IReadOnlyList<IResultadoBusquedaUbicacion> resultados = await BuscarConXmlAsync(xml);

            Assert.AreEqual(1, resultados.Count);
            IResultadoBusquedaUbicacion r = resultados[0];
            Assert.AreEqual("Café Tortoni, Avenida de Mayo 829, Buenos Aires", r.Descripcion);
            Assert.AreEqual(-34.6091, r.Latitud, 1e-9);
            Assert.AreEqual(-58.3797, r.Longitud, 1e-9);
            Assert.AreEqual("cafe", r.Tipo);
            Assert.IsNotNull(r.Limites);
            Assert.AreEqual(-34.6092, r.Limites.Sur, 1e-9);
            Assert.AreEqual(-34.6090, r.Limites.Norte, 1e-9);
            Assert.AreEqual(-58.3798, r.Limites.Oeste, 1e-9);
            Assert.AreEqual(-58.3796, r.Limites.Este, 1e-9);
        }

        [TestMethod]
        public async Task BuscarAsync_VariasCoincidencias_ConservaElOrdenDelProveedor()
        {
            string xml = BusquedaUbicacionesPruebas.Resultados(
                BusquedaUbicacionesPruebas.Place("Uno", "1", "1"),
                BusquedaUbicacionesPruebas.Place("Dos", "2", "2"),
                BusquedaUbicacionesPruebas.Place("Tres", "3", "3"));

            IReadOnlyList<IResultadoBusquedaUbicacion> resultados = await BuscarConXmlAsync(xml);

            CollectionAssert.AreEqual(new[] { "Uno", "Dos", "Tres" }, new List<string>(resultados.Select(x => x.Descripcion)).ToArray());
        }

        [TestMethod]
        public async Task BuscarAsync_MasDeCincoCoincidencias_DevuelveSoloCinco()
        {
            List<string> places = new List<string>();
            for (int i = 1; i <= 8; i++)
            {
                places.Add(BusquedaUbicacionesPruebas.Place("Lugar " + i, i.ToString(CultureInfo.InvariantCulture), "10"));
            }

            IReadOnlyList<IResultadoBusquedaUbicacion> resultados = await BuscarConXmlAsync(BusquedaUbicacionesPruebas.Resultados(places.ToArray()));

            Assert.AreEqual(5, resultados.Count);
        }

        [TestMethod]
        public async Task BuscarAsync_RaizValidaSinCoincidencias_DevuelveListaVacia()
        {
            IReadOnlyList<IResultadoBusquedaUbicacion> resultados = await BuscarConXmlAsync(BusquedaUbicacionesPruebas.XmlVacio);

            Assert.AreEqual(0, resultados.Count);
        }

        [TestMethod]
        public async Task BuscarAsync_RaizIncorrecta_LanzaRespuestaInvalida()
        {
            await AssertRespuestaInvalidaAsync(() => BuscarConXmlAsync("<html><body>Bad gateway</body></html>"));
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("no es xml")]
        [DataRow("<searchresults><place lat=\"1\"")]
        public async Task BuscarAsync_XmlMalformado_LanzaRespuestaInvalida(string xml)
        {
            await AssertRespuestaInvalidaAsync(() => BuscarConXmlAsync(xml));
        }

        [TestMethod]
        public async Task BuscarAsync_ConDtdYEntidadExterna_LanzaRespuestaInvalidaSinResolverla()
        {
            string xml = "<?xml version=\"1.0\"?><!DOCTYPE searchresults [<!ENTITY x SYSTEM \"file:///c:/windows/win.ini\">]><searchresults>&x;</searchresults>";

            await AssertRespuestaInvalidaAsync(() => BuscarConXmlAsync(xml));
        }

        [TestMethod]
        public async Task BuscarAsync_ConDtdVacia_LanzaRespuestaInvalida()
        {
            string xml = "<!DOCTYPE searchresults><searchresults></searchresults>";

            await AssertRespuestaInvalidaAsync(() => BuscarConXmlAsync(xml));
        }

        [TestMethod]
        public async Task BuscarAsync_CuerpoMayorAUnMiBConLongitudDeclarada_LanzaRespuestaInvalida()
        {
            byte[] cuerpo = Relleno(NominatimBusquedaUbicacionesService.MaximoBytesRespuesta + 1);
            HttpResponseMessage respuesta = new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = new ByteArrayContent(cuerpo) };

            await AssertRespuestaInvalidaAsync(() => BuscarConRespuestaAsync(respuesta));
        }

        [TestMethod]
        public async Task BuscarAsync_CuerpoMayorAUnMiBSinLongitudDeclarada_LanzaRespuestaInvalida()
        {
            byte[] cuerpo = Relleno(NominatimBusquedaUbicacionesService.MaximoBytesRespuesta + 1);
            HttpResponseMessage respuesta = new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = new ContenidoSinLongitud(cuerpo) };

            await AssertRespuestaInvalidaAsync(() => BuscarConRespuestaAsync(respuesta));
        }

        [TestMethod]
        public async Task BuscarAsync_CuerpoDeExactamenteUnMiB_SeInterpretaNormalmente()
        {
            byte[] cuerpo = Relleno(NominatimBusquedaUbicacionesService.MaximoBytesRespuesta);
            HttpResponseMessage respuesta = new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = new ContenidoSinLongitud(cuerpo) };

            IReadOnlyList<IResultadoBusquedaUbicacion> resultados = await BuscarConRespuestaAsync(respuesta);

            Assert.AreEqual(1, resultados.Count);
        }

        [TestMethod]
        [DataRow(null, "-58.3")]
        [DataRow("abc", "-58.3")]
        [DataRow("", "-58.3")]
        [DataRow("91", "-58.3")]
        [DataRow("-90.0001", "-58.3")]
        [DataRow("NaN", "-58.3")]
        [DataRow("Infinity", "-58.3")]
        [DataRow("-34.6", null)]
        [DataRow("-34.6", "181")]
        [DataRow("-34.6", "-180.5")]
        [DataRow("-34,6", "-58.3")]
        public async Task BuscarAsync_CoordenadaInvalida_DescartaElElementoYSiSonTodosInvalidosLanzaRespuestaInvalida(string lat, string lon)
        {
            string invalido = ConstruirPlaceCrudo("Invalido", lat, lon);
            string valido = BusquedaUbicacionesPruebas.Place("Valido", "-34.6", "-58.4");

            await AssertRespuestaInvalidaAsync(() => BuscarConXmlAsync(BusquedaUbicacionesPruebas.Resultados(invalido)));

            IReadOnlyList<IResultadoBusquedaUbicacion> resultados = await BuscarConXmlAsync(BusquedaUbicacionesPruebas.Resultados(invalido, valido));
            Assert.AreEqual(1, resultados.Count);
            Assert.AreEqual("Valido", resultados[0].Descripcion);
        }

        [TestMethod]
        public async Task BuscarAsync_SinDescripcion_DescartaElElemento()
        {
            string sinNombre = ConstruirPlaceCrudo("   ", "-34.6", "-58.3");
            string valido = BusquedaUbicacionesPruebas.Place("Valido", "-34.6", "-58.4");

            IReadOnlyList<IResultadoBusquedaUbicacion> resultados = await BuscarConXmlAsync(BusquedaUbicacionesPruebas.Resultados(sinNombre, valido));

            Assert.AreEqual(1, resultados.Count);
            Assert.AreEqual("Valido", resultados[0].Descripcion);
        }

        [TestMethod]
        [DataRow("-34.6,-34.6,-58.4,-58.4")]
        [DataRow("-34.5,-34.7,-58.4,-58.3")]
        [DataRow("-34.7,-34.5,-58.3,-58.4")]
        [DataRow("a,b,c,d")]
        [DataRow("-34.7,-34.5,-58.4")]
        [DataRow("-95,-34.5,-58.4,-58.3")]
        [DataRow("-34.7,-34.5,-58.4,190")]
        [DataRow("-34.7,NaN,-58.4,-58.3")]
        [DataRow("")]
        public async Task BuscarAsync_LimitesInvalidosODegenerados_OmitenLimitesSinPerderLaCoordenada(string boundingBox)
        {
            string xml = BusquedaUbicacionesPruebas.Resultados(BusquedaUbicacionesPruebas.Place("Lugar", "-34.6", "-58.35", boundingBox));

            IReadOnlyList<IResultadoBusquedaUbicacion> resultados = await BuscarConXmlAsync(xml);

            Assert.AreEqual(1, resultados.Count);
            Assert.IsNull(resultados[0].Limites);
            Assert.AreEqual(-34.6, resultados[0].Latitud, 1e-9);
            Assert.AreEqual(-58.35, resultados[0].Longitud, 1e-9);
        }

        [TestMethod]
        public async Task BuscarAsync_SinTipoPeroConClase_UsaLaClaseComoTipo()
        {
            string xml = "<searchresults><place lat=\"1\" lon=\"2\" display_name=\"X\" class=\"tourism\"/></searchresults>";

            IReadOnlyList<IResultadoBusquedaUbicacion> resultados = await BuscarConXmlAsync(xml);

            Assert.AreEqual("tourism", resultados[0].Tipo);
        }

        [TestMethod]
        public async Task BuscarAsync_SinTipoNiClase_DejaTipoNulo()
        {
            string xml = "<searchresults><place lat=\"1\" lon=\"2\" display_name=\"X\"/></searchresults>";

            IReadOnlyList<IResultadoBusquedaUbicacion> resultados = await BuscarConXmlAsync(xml);

            Assert.IsNull(resultados[0].Tipo);
        }

        [TestMethod]
        public async Task BuscarAsync_CulturaActualConComaDecimal_InterpretaNumerosConCulturaInvariante()
        {
            CultureInfo anterior = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("es-AR");
            try
            {
                string xml = BusquedaUbicacionesPruebas.Resultados(
                    BusquedaUbicacionesPruebas.Place("Lugar", "-34.6037", "-58.3816", "-34.7,-34.5,-58.5,-58.3"));

                IReadOnlyList<IResultadoBusquedaUbicacion> resultados = await BuscarConXmlAsync(xml);

                Assert.AreEqual(-34.6037, resultados[0].Latitud, 1e-9);
                Assert.AreEqual(-58.3816, resultados[0].Longitud, 1e-9);
                Assert.AreEqual(-58.3, resultados[0].Limites.Este, 1e-9);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = anterior;
            }
        }

        // Cuerpo XML valido de exactamente 'longitud' bytes: una coincidencia mas un
        // comentario de relleno al final.
        private static byte[] Relleno(int longitud)
        {
            string inicio = BusquedaUbicacionesPruebas.Resultados(BusquedaUbicacionesPruebas.Place("Lugar", "-34.6", "-58.4"));
            string cola = "<!--";
            string fin = "-->";
            int faltante = longitud - Encoding.UTF8.GetByteCount(inicio) - cola.Length - fin.Length;
            Assert.IsTrue(faltante >= 0);
            return Encoding.UTF8.GetBytes(inicio + cola + new string('x', faltante) + fin);
        }

        private static string ConstruirPlaceCrudo(string nombre, string lat, string lon)
        {
            string latAttr = lat == null ? string.Empty : " lat=\"" + lat + "\"";
            string lonAttr = lon == null ? string.Empty : " lon=\"" + lon + "\"";
            return "<place" + latAttr + lonAttr + " display_name=\"" + nombre + "\" class=\"amenity\" type=\"cafe\"/>";
        }
    }
}
