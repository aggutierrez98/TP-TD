using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ABSTRACCIONES;
using BE;
using REPOSITORIO;
using SERVICIOS;

namespace BLL
{
    public class AsistenteFinancieroBLL
    {
        private readonly IConsultaChatbotRepositorio _consultaChatbotRepositorio;
        private readonly IChatbotFinancieroRepositorio _chatbotFinancieroRepositorio;
        private readonly IAsistenteFinancieroService _asistenteFinancieroService;
        private readonly IUnidadDeTrabajoFactory _unidadDeTrabajoFactory;
        private readonly ISesion _sesion;
        private readonly IBitacoraBLL _bitacoraBLL;

        internal AsistenteFinancieroBLL(
            IConsultaChatbotRepositorio consultaChatbotRepositorio,
            IChatbotFinancieroRepositorio chatbotFinancieroRepositorio,
            IAsistenteFinancieroService asistenteFinancieroService,
            IUnidadDeTrabajoFactory unidadDeTrabajoFactory,
            ISesion sesion,
            IBitacoraBLL bitacoraBLL)
        {
            _consultaChatbotRepositorio = consultaChatbotRepositorio;
            _chatbotFinancieroRepositorio = chatbotFinancieroRepositorio;
            _asistenteFinancieroService = asistenteFinancieroService;
            _unidadDeTrabajoFactory = unidadDeTrabajoFactory;
            _sesion = sesion;
            _bitacoraBLL = bitacoraBLL;
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

        public async Task<string> EnviarMensajeAsync(string pregunta)
        {
            string preguntaNormalizada = ValidarPregunta(pregunta);
            int usuarioActualId = _sesion.GetUsuarioActualId().Value;

            List<IntercambioChatbot> historial = _consultaChatbotRepositorio
                .ObtenerPorUsuario(usuarioActualId)
                .OrderBy(item => item.Fecha)
                .Select(item => new IntercambioChatbot { Pregunta = item.Pregunta, Respuesta = item.Respuesta })
                .ToList();

            // La llamada al servicio de IA se resuelve antes de abrir la unidad de
            // trabajo a proposito: es una llamada HTTP externa potencialmente lenta y
            // no debe mantener una transaccion SQL abierta mientras se espera.
            string respuesta = await ObtenerRespuestaServicioAsync(preguntaNormalizada, historial).ConfigureAwait(false);

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
                        "AsistenteFinancieroBLL",
                        unidad);

                    unidad.Save();
                    return respuesta;
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

        private async Task<string> ObtenerRespuestaServicioAsync(string pregunta, List<IntercambioChatbot> historial)
        {
            try
            {
                return await _asistenteFinancieroService.ObtenerRespuestaAsync(pregunta, historial).ConfigureAwait(false);
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
                throw new ExcepcionTecnica("Error.Assistant.ResponseFailed", ex);
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
    }
}
