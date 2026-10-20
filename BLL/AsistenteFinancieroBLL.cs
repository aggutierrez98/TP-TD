using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using ABSTRACCIONES;
using BE;
using REPOSITORIO;
using SERVICIOS;

namespace BLL
{
    // Caso de uso del asistente personal. Aplica los guardrails en este orden: validar el
    // texto, limite de uso, prefiltro local, clasificador (fail-closed), servicio de
    // respuesta, validacion de la respuesta y recien entonces persistencia. El usuario sale
    // solo de la sesion. Un rechazo devuelve la key de un texto fijo, se registra en
    // Bitacora con su motivo y no se persiste en ConsultaChatbot, asi nunca vuelve al
    // modelo como historial.
    public class AsistenteFinancieroBLL
    {
        internal const string RechazoFueraDeAlcance = "Assistant.Rejection.OutOfScope";
        internal const string RechazoLimiteDeUso = "Assistant.Rejection.RateLimit";
        internal const string RechazoRespuestaNoConfiable = "Assistant.Rejection.UnreliableAnswer";

        private const string ActividadRechazo = "FormPersonalAssistant.Value.Activity.QuestionRejected";
        private const string MotivoPrefiltro = "FormPersonalAssistant.Value.Info.Rejected.Prefilter";
        private const string MotivoCategoria = "FormPersonalAssistant.Value.Info.Rejected.Category";
        private const string MotivoFalloClasificacion = "FormPersonalAssistant.Value.Info.Rejected.ClassificationFailed";
        private const string MotivoLimiteDeUso = "FormPersonalAssistant.Value.Info.Rejected.RateLimit";
        private const string MotivoValidacionSalida = "FormPersonalAssistant.Value.Info.Rejected.OutputValidation";
        private const string Origen = "AsistenteFinancieroBLL";

        // Intercambios previos que se reenvian al modelo como contexto conversacional.
        internal const int MaximoIntercambiosHistorial = 10;

        private readonly IConsultaChatbotRepositorio _consultaChatbotRepositorio;
        private readonly IChatbotFinancieroRepositorio _chatbotFinancieroRepositorio;
        private readonly IAsistenteFinancieroService _asistenteFinancieroService;
        private readonly IClasificadorConsultas _clasificadorConsultas;
        private readonly LimiteUsoAsistente _limiteUso;
        private readonly IUnidadDeTrabajoFactory _unidadDeTrabajoFactory;
        private readonly ISesion _sesion;
        private readonly IBitacoraBLL _bitacoraBLL;
        private readonly IObservado _idioma;
        private readonly IGeneradorRecomendaciones _generador;
        private readonly IPerfilInversorRepositorio _perfilesInversor;

        private static readonly HerramientaAsistente RecomendarCompras = new HerramientaAsistente(
            "recomendar_compras", "Recomienda donde y cuando comprar los items pendientes; nombreLista es opcional, solo si la pregunta nombra una lista.",
            "{\"type\":\"OBJECT\",\"properties\":{\"nombreLista\":{\"type\":\"STRING\",\"description\":\"Nombre de la lista indicada en la pregunta\"}}}");

        private static readonly HerramientaAsistente RecomendarInversion = new HerramientaAsistente(
            "recomendar_inversion", "Extrae solo el porcentaje a invertir indicado en la pregunta actual. Es opcional: si no esta indicado, omitirlo; nunca asumir ni usar uno del historial.",
            "{\"type\":\"OBJECT\",\"properties\":{\"porcentaje\":{\"type\":\"NUMBER\",\"description\":\"Porcentaje indicado explicitamente en la pregunta, tal cual esta escrito en la pregunta aunque sea invalido (cero, negativo o mayor a 100): el sistema lo valida. Omitir si la pregunta no indica ninguno\"}}}");

        internal AsistenteFinancieroBLL(
            IConsultaChatbotRepositorio consultaChatbotRepositorio,
            IChatbotFinancieroRepositorio chatbotFinancieroRepositorio,
            IAsistenteFinancieroService asistenteFinancieroService,
            IClasificadorConsultas clasificadorConsultas,
            LimiteUsoAsistente limiteUso,
            IUnidadDeTrabajoFactory unidadDeTrabajoFactory,
            ISesion sesion,
            IBitacoraBLL bitacoraBLL,
            IObservado idioma,
            IGeneradorRecomendaciones generador,
            IPerfilInversorRepositorio perfilesInversor = null)
        {
            _consultaChatbotRepositorio = consultaChatbotRepositorio;
            _chatbotFinancieroRepositorio = chatbotFinancieroRepositorio;
            _asistenteFinancieroService = asistenteFinancieroService;
            _clasificadorConsultas = clasificadorConsultas;
            _limiteUso = limiteUso;
            _unidadDeTrabajoFactory = unidadDeTrabajoFactory;
            _sesion = sesion;
            _bitacoraBLL = bitacoraBLL;
            _idioma = idioma;
            _generador = generador;
            _perfilesInversor = perfilesInversor;
        }

        // Historial del usuario actual, ordenado cronologicamente (mas viejo primero)
        // para poder mostrarlo tal cual en la UI y reusarlo como contexto conversacional.
        public IReadOnlyList<ConsultaChatbot> ObtenerHistorial()
        {
            int usuarioActualId = _sesion.GetUsuarioActualId().Value;
            return _consultaChatbotRepositorio
                .ObtenerPorUsuario(usuarioActualId)
                .OrderBy(item => item.Fecha)
                .ToList();
        }

        public async Task<RespuestaAsistente> EnviarMensajeAsync(string pregunta)
        {
            string preguntaNormalizada = ValidarPregunta(pregunta);
            int usuarioActualId = _sesion.GetUsuarioActualId().Value;

            // Los guardrails, la lectura del historial y el guardado incluyen trabajo
            // sincrono (SQL, Bitacora, clasificacion). Un metodo async ejecuta todo lo previo
            // al primer await en el hilo llamador (la UI). Task.Run fija la frontera: ese
            // trabajo corre en un worker y las llamadas HTTP siguen esperandose con await,
            // sin retener el worker durante la red. El usuario se toma antes, de la sesion.
            return await Task.Run(() => ProcesarMensajeAsync(preguntaNormalizada, usuarioActualId)).ConfigureAwait(false);
        }

        private async Task<RespuestaAsistente> ProcesarMensajeAsync(string preguntaNormalizada, int usuarioActualId)
        {
            // Cuenta toda pregunta que llega hasta aca, aunque despues la rechace el prefiltro.
            if (!_limiteUso.IntentarConsumir(usuarioActualId))
            {
                return Rechazar(RechazoLimiteDeUso, MotivoLimiteDeUso);
            }

            if (PrefiltroConsultasAsistente.Evaluar(preguntaNormalizada) != ResultadoPrefiltro.Admitida)
            {
                return Rechazar(RechazoFueraDeAlcance, MotivoPrefiltro);
            }

            // Solo intercambios admitidos (los rechazos no se persisten), limitado a los ultimos.
            List<IntercambioChatbot> historial = _consultaChatbotRepositorio
                .ObtenerPorUsuario(usuarioActualId)
                .OrderBy(item => item.Fecha)
                .Select(item => new IntercambioChatbot { Pregunta = item.Pregunta, Respuesta = item.Respuesta })
                .ToList();
            if (historial.Count > MaximoIntercambiosHistorial)
            {
                historial = historial.Skip(historial.Count - MaximoIntercambiosHistorial).ToList();
            }

            CategoriaConsulta? categoria = await ClasificarAsync(preguntaNormalizada).ConfigureAwait(false);
            if (!categoria.HasValue)
            {
                return Rechazar(RechazoFueraDeAlcance, MotivoFalloClasificacion);
            }

            if (!EsCategoriaAdmitida(categoria.Value))
            {
                return Rechazar(RechazoFueraDeAlcance, MotivoCategoria);
            }

            // Datos propios del canal de chat: la herramienta extrae argumentos de la pregunta y
            // el perfil se lee antes que el porcentaje. Calculo, redaccion, validacion y anexos
            // fijos los resuelve el colaborador compartido con la generacion automatica.
            SolicitudGeneracion solicitud = new SolicitudGeneracion
            {
                Categoria = categoria.Value,
                UsuarioId = usuarioActualId,
                Pregunta = preguntaNormalizada,
                Historial = historial,
                Idioma = _idioma.IdiomaActual
            };
            if (categoria.Value == CategoriaConsulta.Compras && _generador.ComprasDisponible)
            {
                SolicitudAsistente extraccion = new SolicitudAsistente(categoria.Value, preguntaNormalizada, historial, ContextoAsistente.Vacio);
                LlamadaHerramientaAsistente llamada = await _asistenteFinancieroService.ObtenerLlamadaHerramientaAsync(extraccion, RecomendarCompras, CancellationToken.None).ConfigureAwait(false);
                solicitud.NombreLista = ExtraerNombreLista(llamada);
            }
            else if (categoria.Value == CategoriaConsulta.Inversiones && _generador.InversionesDisponible)
            {
                PerfilInversor perfil;
                try
                {
                    var persistido = _perfilesInversor?.ObtenerPorUsuario(usuarioActualId);
                    perfil = persistido == null ? null : new PerfilInversor
                    {
                        UsuarioId = persistido.UsuarioId, VersionCuestionario = persistido.VersionCuestionario,
                        FechaEvaluacionUtc = persistido.FechaEvaluacionUtc, NivelRiesgo = persistido.NivelRiesgo,
                        Respuestas = persistido.Respuestas.Select(r => new RespuestaPerfilInversor { PreguntaId = r.PreguntaId, OpcionId = r.OpcionId }).ToList()
                    };
                }
                catch (Exception ex) { throw new ExcepcionTecnica("Error.InvestorProfile.ReadFailed", ex); }
                if (perfil == null)
                    return Persistir(preguntaNormalizada, Traducir("Assistant.Investment.ProfileRequired"), usuarioActualId);
                SolicitudAsistente extraccion = new SolicitudAsistente(categoria.Value, preguntaNormalizada, null, ContextoAsistente.Vacio);
                LlamadaHerramientaAsistente llamada = await _asistenteFinancieroService.ObtenerLlamadaHerramientaAsync(extraccion, RecomendarInversion, CancellationToken.None).ConfigureAwait(false);
                // Ausente: se recomienda sin porcentaje ni monto. Invalido: se pide correccion sin
                // consultar fuentes. El error tecnico de lectura se propaga como excepcion.
                ResultadoPorcentaje porcentaje = ExtractorPorcentajeInversion.Extraer(llamada, RecomendarInversion.Nombre);
                if (porcentaje.Estado == EstadoPorcentaje.Invalido)
                    return Persistir(preguntaNormalizada, Traducir("Assistant.Investment.InvalidPercentage") + Environment.NewLine + Traducir("Assistant.Investment.Disclaimer"), usuarioActualId);
                solicitud.Perfil = perfil;
                solicitud.Porcentaje = porcentaje.Valor;
            }

            // La llamada al servicio de IA se resuelve antes de abrir la unidad de
            // trabajo a proposito: es una llamada HTTP externa potencialmente lenta y
            // no debe mantener una transaccion SQL abierta mientras se espera.
            ResultadoGeneracion generado = await _generador.GenerarAsync(solicitud, CancellationToken.None).ConfigureAwait(false);
            if (!generado.Aceptada)
            {
                return Rechazar(RechazoRespuestaNoConfiable, MotivoValidacionSalida);
            }

            return Persistir(preguntaNormalizada, generado.Texto, usuarioActualId);
        }

        private string Traducir(string clave) => _idioma.IdiomaActual?.ObtenerTraduccion(clave) ?? clave;

        // Fail-closed: cualquier fallo de clasificacion (red, cuota, salida invalida) devuelve
        // null y la pregunta se rechaza; nunca se responde como tema general. La falta de
        // clave de API es configuracion, no clasificacion: se propaga como error.
        private async Task<CategoriaConsulta?> ClasificarAsync(string pregunta)
        {
            try
            {
                return await _clasificadorConsultas.ClasificarAsync(pregunta, CancellationToken.None).ConfigureAwait(false);
            }
            catch (ExcepcionTecnica ex) when (ex.Message == "Error.Assistant.ApiKeyMissing")
            {
                throw;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static bool EsCategoriaAdmitida(CategoriaConsulta categoria)
        {
            return categoria == CategoriaConsulta.Compras
                || categoria == CategoriaConsulta.Inversiones
                || categoria == CategoriaConsulta.FinanzasGenerales;
        }

        private RespuestaAsistente Rechazar(string claveRechazo, string motivo)
        {
            _bitacoraBLL.RegistrarFalloNegocio(ActividadRechazo, motivo, Origen);
            return RespuestaAsistente.Rechazada(claveRechazo);
        }

        private RespuestaAsistente Persistir(string preguntaNormalizada, string respuesta, int usuarioActualId)
        {
            ChatbotFinanciero chatbot = _chatbotFinancieroRepositorio.ObtenerUnico();
            if (chatbot == null)
            {
                throw new ExcepcionTecnica("Error.Assistant.ChatbotNotConfigured");
            }

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    ConsultaChatbot nuevaConsulta = new ConsultaChatbot
                    {
                        ChatbotFinancieroId = chatbot.Id,
                        UsuarioId = usuarioActualId,
                        Pregunta = preguntaNormalizada,
                        Respuesta = respuesta,
                        Fecha = DateTime.Now,
                        EsPregunta = true
                    };

                    unidad.ConsultasChatbot.InsertarYObtenerId(nuevaConsulta);

                    _bitacoraBLL.RegistrarOperacionInterno(
                        "FormPersonalAssistant.Value.Activity.MessageSent",
                        "FormPersonalAssistant.Value.Info.MessageSent",
                        Origen,
                        unidad);

                    unidad.Save();
                    return RespuestaAsistente.Respondida(respuesta);
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (ExcepcionTecnica)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.Assistant.SaveFailed", ex);
                }
            }
        }

        private static string ValidarPregunta(string pregunta)
        {
            string normalizado = (pregunta ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(normalizado))
            {
                throw new ExcepcionNegocio("Error.Assistant.QuestionRequired");
            }

            return normalizado;
        }

        private static string ExtraerNombreLista(LlamadaHerramientaAsistente llamada)
        {
            if (llamada?.Nombre != RecomendarCompras.Nombre || string.IsNullOrWhiteSpace(llamada.ArgumentosJson)) return null;
            try
            {
                using (JsonDocument argumentos = JsonDocument.Parse(llamada.ArgumentosJson))
                {
                    JsonElement nombre;
                    return argumentos.RootElement.ValueKind == JsonValueKind.Object
                        && argumentos.RootElement.TryGetProperty("nombreLista", out nombre) && nombre.ValueKind == JsonValueKind.String
                        ? nombre.GetString() : null;
                }
            }
            catch (JsonException) { return null; }
        }
    }
}
