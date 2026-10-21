using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ABSTRACCIONES;
using BE;
using SERVICIOS;

namespace BLL
{
    internal interface IGeneradorRecomendaciones
    {
        bool ComprasDisponible { get; }

        bool InversionesDisponible { get; }

        Task<ResultadoGeneracion> GenerarAsync(SolicitudGeneracion solicitud, CancellationToken cancellationToken);
    }

    // Todo lo que el colaborador necesita saber de un pedido. Usuario lo captura el llamador
    // (el chat desde la sesion al enviar; la generacion automatica desde su propio listado):
    // el colaborador nunca consulta la sesion. Idioma es el del destinatario y se usa solo
    // para leer textos fijos, sin cambiar el idioma global de la aplicacion.
    internal sealed class SolicitudGeneracion
    {
        public CategoriaConsulta Categoria { get; set; }
        public Usuario Usuario { get; set; }
        // Null solo en los canales que conocen la intencion (generacion automatica): se usa
        // el texto fijo traducible de esa intencion, sin clasificador ni function calling.
        public string Pregunta { get; set; }
        public IReadOnlyList<IIntercambioChatbot> Historial { get; set; }
        public IIdioma Idioma { get; set; }
        public string NombreLista { get; set; }
        public PerfilInversor Perfil { get; set; }
        public decimal? Porcentaje { get; set; }
        // Solo la generacion automatica: si no hay ningun instrumento verificable no se llama a la IA
        // ni se redacta nada (el chat sigue explicando que no hay alternativas).
        public bool OmitirSiSinAlternativas { get; set; }
    }

    internal sealed class ResultadoGeneracion
    {
        private ResultadoGeneracion(bool aceptada, string texto, bool sinAlternativas = false)
        {
            Aceptada = aceptada;
            Texto = texto;
            SinAlternativas = sinAlternativas;
        }

        public bool Aceptada { get; }

        // Solo con SolicitudGeneracion.OmitirSiSinAlternativas: no hubo candidatos verificables.
        public bool SinAlternativas { get; }

        // Texto final, con los anexos fijos; null si la redaccion no fue confiable.
        public string Texto { get; }

        internal static ResultadoGeneracion Valida(string texto) => new ResultadoGeneracion(true, texto);

        internal static ResultadoGeneracion NoConfiable() => new ResultadoGeneracion(false, null);

        internal static ResultadoGeneracion SinCandidatos() => new ResultadoGeneracion(false, null, true);
    }

    // Colaborador compartido por el chat y por la generacion automatica: calcula el resumen
    // con los calculadores existentes, redacta con el servicio de IA sobre un contexto
    // cerrado, valida referencias y fechas de la salida y agrega los anexos fijos (datos
    // faltantes, zonas, limite de items, aviso legal). No usa sesion, clasificador, limite
    // de uso ni historial propio: no agrega intercambios ni consume el cupo del chat.
    internal sealed class GeneradorRecomendaciones : IGeneradorRecomendaciones
    {
        internal const string ClavePreguntaCompras = "Assistant.Automatic.ShoppingQuestion";
        internal const string ClavePreguntaInversiones = "Assistant.Automatic.InvestmentQuestion";

        private readonly IAsistenteFinancieroService _servicio;
        private readonly ValidadorRespuestaAsistente _validador;
        private readonly IRecomendacionComprasBLL _compras;
        private readonly IRecomendacionInversionesBLL _inversiones;

        internal GeneradorRecomendaciones(
            IAsistenteFinancieroService servicio,
            ValidadorRespuestaAsistente validador,
            IRecomendacionComprasBLL compras,
            IRecomendacionInversionesBLL inversiones)
        {
            _servicio = servicio;
            _validador = validador;
            _compras = compras;
            _inversiones = inversiones;
        }

        public bool ComprasDisponible => _compras != null;

        public bool InversionesDisponible => _inversiones != null;

        public async Task<ResultadoGeneracion> GenerarAsync(SolicitudGeneracion solicitud, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            IContextoAsistente contexto = ContextoAsistente.Vacio;
            IReadOnlyList<IReferenciaAsistente> universo = new ReferenciaAsistente[0];
            IReadOnlyList<IIntercambioChatbot> historial = solicitud.Historial ?? new IntercambioChatbot[0];
            ResumenCompras compras = null;
            ResumenInversiones inversiones = null;

            if (solicitud.Categoria == CategoriaConsulta.Compras && _compras != null)
            {
                compras = await _compras.CalcularAsync(solicitud.Usuario, solicitud.NombreLista, cancellationToken).ConfigureAwait(false);
                contexto = compras.CrearContexto();
                universo = _compras.ObtenerUniversoReferencias(solicitud.Usuario);
            }
            else if (solicitud.Categoria == CategoriaConsulta.Inversiones && _inversiones != null)
            {
                inversiones = await _inversiones.CalcularAsync(solicitud.Usuario, solicitud.Porcentaje, cancellationToken, solicitud.Perfil).ConfigureAwait(false);
                contexto = inversiones.CrearContexto();
                universo = inversiones.UniversoReferencias;
                if (solicitud.OmitirSiSinAlternativas && inversiones.Instrumentos.Count == 0)
                {
                    return ResultadoGeneracion.SinCandidatos();
                }

                // El resumen actual es la unica autoridad: no se reenvian recomendaciones previas.
                historial = new IntercambioChatbot[0];
            }

            string pregunta = solicitud.Pregunta ?? PreguntaFija(solicitud);
            string respuesta = await ObtenerRespuestaAsync(new SolicitudAsistente(solicitud.Categoria, pregunta, historial, contexto)).ConfigureAwait(false);

            if ((compras != null && compras.TieneFechasNoPermitidas(respuesta))
                || _validador.Validar(respuesta, universo, contexto) != ResultadoValidacionRespuesta.Valida)
            {
                return ResultadoGeneracion.NoConfiable();
            }

            if (compras != null) respuesta = AgregarAnexosCompras(respuesta, compras, solicitud.Idioma);
            if (inversiones != null) respuesta = AgregarAnexosInversiones(respuesta, inversiones, solicitud.Idioma);
            return ResultadoGeneracion.Valida(respuesta);
        }

        private static string PreguntaFija(SolicitudGeneracion solicitud)
        {
            switch (solicitud.Categoria)
            {
                case CategoriaConsulta.Compras: return Traducir(solicitud.Idioma, ClavePreguntaCompras);
                case CategoriaConsulta.Inversiones: return Traducir(solicitud.Idioma, ClavePreguntaInversiones);
                default: throw new ArgumentException("La categoria no tiene una intencion fija.", nameof(solicitud));
            }
        }

        private async Task<string> ObtenerRespuestaAsync(ISolicitudAsistente solicitud)
        {
            try
            {
                return await _servicio.ObtenerRespuestaAsync(solicitud).ConfigureAwait(false);
            }
            catch (ExcepcionNegocio)
            {
                throw;
            }
            catch (ExcepcionTecnica)
            {
                throw;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ExcepcionTecnica("Error.Assistant.ResponseFailed", ex);
            }
        }

        private static string AgregarAnexosCompras(string respuesta, ResumenCompras resumen, IIdioma idioma)
        {
            if (resumen.SinZonas) respuesta += Environment.NewLine + Traducir(idioma, "Assistant.Shopping.NoZones");
            List<string> noEncontrados = resumen.Items.Where(item => item.NoEncontrado).Select(item => item.Descripcion).ToList();
            if (noEncontrados.Count > 0)
                respuesta += Environment.NewLine + string.Format(Traducir(idioma, "Assistant.Shopping.NotFound"), string.Join(", ", noEncontrados));
            if (resumen.ItemsOmitidos > 0)
                respuesta += Environment.NewLine + string.Format(Traducir(idioma, "Assistant.Shopping.Omitted"), ResumenCompras.MaximoItemsAnalizados, resumen.ItemsOmitidos);
            return respuesta;
        }

        private static string AgregarAnexosInversiones(string respuesta, ResumenInversiones resumen, IIdioma idioma)
        {
            if (resumen.CandidatosSinCompatibilidad.Count > 0)
                respuesta += Environment.NewLine + Traducir(idioma, "Assistant.Investment.UnverifiedCompatibility");
            if (resumen.FuentesFaltantes.Count > 0)
                respuesta += Environment.NewLine + string.Format(Traducir(idioma, "Assistant.Investment.MissingData"), string.Join(", ", resumen.FuentesFaltantes.Distinct()));
            return respuesta + Environment.NewLine + Traducir(idioma, "Assistant.Investment.Disclaimer");
        }

        private static string Traducir(IIdioma idioma, string clave) => idioma?.ObtenerTraduccion(clave) ?? clave;
    }
}
