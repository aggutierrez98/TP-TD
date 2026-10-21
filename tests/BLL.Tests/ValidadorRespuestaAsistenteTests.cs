using ABSTRACCIONES;
using SERVICIOS;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLL.Tests
{
    [TestClass]
    public sealed class ValidadorRespuestaAsistenteTests
    {
        private static readonly IReferenciaAsistente[] Universo =
        {
            new ReferenciaAsistente(TipoReferenciaAsistente.Comercio, "Carrefour"),
            new ReferenciaAsistente(TipoReferenciaAsistente.Comercio, "Coto"),
            new ReferenciaAsistente(TipoReferenciaAsistente.Promocion, "Miércoles Coto"),
            new ReferenciaAsistente(TipoReferenciaAsistente.Cuenta, "Caja de ahorro Galicia"),
            new ReferenciaAsistente(TipoReferenciaAsistente.Instrumento, "FCI Money Market Alfa")
        };

        private readonly ValidadorRespuestaAsistente _sut = new ValidadorRespuestaAsistente();

        [TestMethod]
        [DataRow("Fondo de clase B, de renta variable o acciones.", false)]
        [DataRow("Compra DE.", true)]
        [DataRow("Compra $de.", true)]
        [DataRow("Compra el ticker B.", true)]
        [DataRow("Compra $B.", true)]
        [DataRow("Compra COME.", true)]
        public void Validar_TickerAmbiguo_DistingueProsaYClaseDeFondoDeSimboloBursatil(string respuesta, bool rechazo)
        {
            var universo = new[]
            {
                new ReferenciaAsistente(TipoReferenciaAsistente.Instrumento, "DE", true),
                new ReferenciaAsistente(TipoReferenciaAsistente.Instrumento, "O", true),
                new ReferenciaAsistente(TipoReferenciaAsistente.Instrumento, "B", true),
                new ReferenciaAsistente(TipoReferenciaAsistente.Instrumento, "COME", true)
            };
            Assert.AreEqual(rechazo ? ResultadoValidacionRespuesta.ReferenciaNoPermitida : ResultadoValidacionRespuesta.Valida,
                _sut.Validar(respuesta, universo, ContextoAsistente.Vacio));
        }

        [TestMethod]
        public void Validar_RespuestaConTripleBacktick_DevuelveBloqueDeCodigo()
        {
            string respuesta = "Podes calcularlo asi:\n```\nint total = 3;\n```";

            Assert.AreEqual(ResultadoValidacionRespuesta.BloqueDeCodigo, _sut.Validar(respuesta, null, ContextoAsistente.Vacio));
        }

        [TestMethod]
        public void Validar_RespuestaConBloqueIndentado_DevuelveBloqueDeCodigo()
        {
            string respuesta = "Usa esta formula:\n\n    total = ingresos - gastos;\n    print(total)\n\nListo.";

            Assert.AreEqual(ResultadoValidacionRespuesta.BloqueDeCodigo, _sut.Validar(respuesta, null, ContextoAsistente.Vacio));
        }

        [TestMethod]
        public void Validar_ListaAnidadaIndentada_EsValida()
        {
            string respuesta = "Opciones:\n- Ahorro\n\n    - Separar el 10% al cobrar\n    1. Revisar suscripciones";

            Assert.AreEqual(ResultadoValidacionRespuesta.Valida, _sut.Validar(respuesta, null, ContextoAsistente.Vacio));
        }

        [TestMethod]
        public void Validar_ComercioAusenteDelContexto_DevuelveReferenciaNoPermitida()
        {
            IContextoAsistente contexto = Contexto(new ReferenciaAsistente(TipoReferenciaAsistente.Comercio, "Coto"));

            ResultadoValidacionRespuesta resultado = _sut.Validar("Te conviene comprar en Carrefour.", Universo, contexto);

            Assert.AreEqual(ResultadoValidacionRespuesta.ReferenciaNoPermitida, resultado);
        }

        [TestMethod]
        public void Validar_SoloReferenciasPermitidas_EsValida()
        {
            IContextoAsistente contexto = Contexto(
                new ReferenciaAsistente(TipoReferenciaAsistente.Comercio, "Coto"),
                new ReferenciaAsistente(TipoReferenciaAsistente.Cuenta, "Caja de ahorro Galicia"));

            ResultadoValidacionRespuesta resultado = _sut.Validar(
                "Compra en COTO y pagá desde tu caja de ahorro galicia.", Universo, contexto);

            Assert.AreEqual(ResultadoValidacionRespuesta.Valida, resultado);
        }

        [TestMethod]
        public void Validar_ContextoVacioYMencionDelUniverso_DevuelveReferenciaNoPermitida()
        {
            ResultadoValidacionRespuesta resultado = _sut.Validar(
                "Podrias invertir en el FCI Money Market Alfa.", Universo, ContextoAsistente.Vacio);

            Assert.AreEqual(ResultadoValidacionRespuesta.ReferenciaNoPermitida, resultado);
        }

        [TestMethod]
        public void Validar_ReferenciaPermitidaDeOtroTipo_NoHabilitaLaMencion()
        {
            IContextoAsistente contexto = Contexto(new ReferenciaAsistente(TipoReferenciaAsistente.Comercio, "Coto"));

            ResultadoValidacionRespuesta resultado = _sut.Validar(
                "Aprovecha la promo Miercoles Coto.", Universo, contexto);

            Assert.AreEqual(ResultadoValidacionRespuesta.ReferenciaNoPermitida, resultado);
        }

        [TestMethod]
        public void Validar_NombreDentroDeOtraPalabra_NoCuentaComoMencion()
        {
            ResultadoValidacionRespuesta resultado = _sut.Validar(
                "Una cotorra no ahorra; arma un presupuesto.", Universo, ContextoAsistente.Vacio);

            Assert.AreEqual(ResultadoValidacionRespuesta.Valida, resultado);
        }

        [TestMethod]
        public void Validar_RespuestaGeneralSinEntidades_EsValida()
        {
            ResultadoValidacionRespuesta resultado = _sut.Validar(
                "Un presupuesto mensual te ayuda a separar gastos fijos y variables.", Universo, ContextoAsistente.Vacio);

            Assert.AreEqual(ResultadoValidacionRespuesta.Valida, resultado);
        }

        [TestMethod]
        public void Validar_RespuestaVacia_DevuelveRespuestaVacia()
        {
            Assert.AreEqual(ResultadoValidacionRespuesta.RespuestaVacia, _sut.Validar("  ", Universo, ContextoAsistente.Vacio));
        }

        private static IContextoAsistente Contexto(params IReferenciaAsistente[] permitidas)
        {
            return new ContextoAsistente("{}", permitidas);
        }
    }
}
