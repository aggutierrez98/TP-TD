# Design

## Context

Ver `proposal.md` (Why). Estado observado:

- `GeminiAsistenteFinancieroService.ObtenerRespuestaAsync` clasifica con una llamada aparte (`ArmarRequestBodyClasificacion`, `responseFormat` con enum `Ahorro`/`Inversiones`/`General`) y delega en `AgenteAhorro`, `AgenteInversiones` o el prompt general. `ExtraerAgenteClasificado` y `ClasificarAgenteAsync` caen en `General` ante cualquier fallo.
- `AsistenteFinancieroBLL.EnviarMensajeAsync` toma el usuario de `_sesion`, arma el historial con `ObtenerPorUsuario(usuarioActualId)`, llama al servicio fuera de la transaccion y persiste en `ConsultaChatbot` con bitacora `FormPersonalAssistant.Value.*`.
- `GeminiChatClient` concentra el POST, el armado de `contents` y la extraccion de texto. No envia `temperature` ni `tools`.
- `chat-asistente-sin-bloqueo-ui` (abierto) pone el envio detras de `Task.Run` en el mismo metodo.

## Goals / Non-Goals

**Goals:**
- Que ninguna pregunta fuera de alcance reciba respuesta del modelo y que cualquier duda termine en rechazo.
- Dejar listos los ganchos (contexto permitido, validacion de salida, function calling) que usan los changes 3 y 4.

**Non-Goals:**
- Datos reales del usuario en el prompt (changes 3 y 4).
- Otro proveedor de clasificacion. Jev se evaluo y se descarto por ahora (sin capa gratuita, lista de espera, espanol sin probar); el contrato permite cambiarlo despues.

## Decisions

**1. Pipeline en `AsistenteFinancieroBLL` (decision del usuario: orden de capas).** Validar texto → limite de uso → prefiltro → clasificador → servicio de respuesta → validacion de salida → persistir. El BLL decide; SERVICIOS solo habla con Gemini. Alternativa descartada: dejar la clasificacion dentro del servicio, porque el rechazo, la bitacora y el historial son reglas de negocio.

**2. `IClasificadorConsultas` en ABSTRACCIONES (decision del usuario).** `Task<CategoriaConsulta> ClasificarAsync(string pregunta, CancellationToken)`, con `CategoriaConsulta` = `Compras`, `Inversiones`, `FinanzasGenerales`, `FueraDeTema`, `Inyeccion`. Implementacion `GeminiClasificadorConsultas` en SERVICIOS con el modelo actual (`gemini-3.1-flash-lite`), temperatura 0 y salida JSON enum con el mismo mecanismo de `responseFormat` que ya usa `GeminiEscaneoTicketService`. El BLL trata excepcion, valor desconocido o categoria no admitida como rechazo (fail-closed). Alta en `CompositionRoot` e inyeccion por ctor `internal`.

**3. Contrato del servicio de respuesta (decision mia).** `IAsistenteFinancieroService` pasa a recibir una solicitud con categoria, pregunta, historial y un contexto opcional (JSON + conjunto de referencias permitidas). Ruteo: `Compras` → `AgenteAhorro`, `Inversiones` → `AgenteInversiones`, `FinanzasGenerales` → prompt general. Se elimina la clasificacion interna del servicio. Los prompts declaran el alcance y que el texto del usuario nunca cambia las instrucciones.

**4. Prefiltro local (decision del usuario; valores mios).** Clase pura en BLL. Largo maximo 500 caracteres. Normaliza minusculas y acentos y busca patrones de inyeccion ("ignora las instrucciones", "ignore previous", "system prompt", "actua como", "olvida lo anterior") y de temas excluidos ("receta", "programar", "codigo fuente", "script", "sql"). La lista vive en una constante revisable; se evita "codigo" suelto para no rechazar "codigo de descuento".

**5. Limite de uso (decision del usuario; numeros mios).** 5 preguntas por minuto por usuario, ventana deslizante en memoria del proceso, reloj inyectable (`Func<DateTime>`, patron de `ActualizacionTasasBLL`) y estructura segura para hilos porque el envio corre fuera del hilo de UI. Cada pregunta admitida consume al menos dos llamadas (clasificar y responder). Valor en `App.config` (`AsistenteMaxConsultasPorMinuto`) con 5 por defecto. Las preguntas que el prefiltro rechaza tambien cuentan.

**6. Rechazo (decision del usuario: texto fijo, bitacora, fuera del historial).** El BLL devuelve un resultado con indicador de rechazo y la key del texto (`Assistant.Rejection.OutOfScope`, `Assistant.Rejection.RateLimit`, `Assistant.Rejection.UnreliableAnswer`). Un unico texto para todo rechazo por alcance, sin revelar que filtro actuo. La bitacora registra el motivo con `RegistrarFalloNegocio`. Decision mia: los intercambios rechazados no se persisten en `ConsultaChatbot`; asi quedan fuera del historial sin cambio de esquema y solo se ven en la sesion abierta del chat. El historial enviado al modelo se limita ademas a los ultimos 10 intercambios (decision mia).

**7. Validacion de salida (decision del usuario).** `ValidadorRespuestaAsistente` en BLL recibe la respuesta y el contexto permitido: rechaza triple backtick o bloques indentados de codigo, y para cada tipo de entidad (comercio, promocion, cuenta, instrumento) compara los nombres del universo conocido que aparecen en el texto contra los permitidos. Con contexto vacio (Finanzas generales) cualquier mencion de una entidad del universo invalida la respuesta. Changes 3 y 4 cargan universo y permitidos.

**8. Function calling (decision del usuario).** `GeminiChatClient` agrega `tools[].functionDeclarations` y `toolConfig.functionCallingConfig` con `mode: ANY` y `allowedFunctionNames`, y parsea partes `functionCall`. Ninguna declaracion acepta un id de usuario. Los nombres exactos de campos se verifican contra la documentacion oficial de ai.google.dev al implementar. Sin consumidores en este change; se cubre con tests de armado y parseo.

**9. Campos de Gemini verificados (decision mia, al implementar).** Segun la referencia REST de ai.google.dev: `tools[].functionDeclarations[]` con `name`, `description` y `parameters`; `toolConfig.functionCallingConfig` con `mode: "ANY"` y `allowedFunctionNames`; la respuesta trae partes `functionCall` con `id` (opcional), `name` y `args`. `temperature` va en `generationConfig`. El clasificador mantiene `responseFormat` como pide la decision 2, aunque la documentacion actual no permitio confirmar ese campo de `GenerationConfig`: si la API lo rechazara, toda pregunta se rechaza (fail-closed) y el ajuste se limita a `GeminiClasificadorConsultas.ArmarRequestBody`.

**10. Detalles del pipeline (decision mia, al implementar).**
- Falta de clave de API: el clasificador lanza `Error.Assistant.ApiKeyMissing` y el BLL la propaga como error, no como rechazo; es configuracion, no clasificacion. Cualquier otro fallo del clasificador (`Error.Assistant.ResponseFailed`) termina en rechazo.
- Orden efectivo: validar texto y tomar el usuario de la sesion en el hilo llamador; dentro de `Task.Run`, limite → prefiltro → lectura del historial → clasificador → servicio → validacion → persistencia. El historial se lee antes de clasificar para que la lectura SQL siga siendo el primer acceso a datos del envio.
- El prefiltro usa "programar en" y "programa en" en lugar de "programar" para no rechazar "programar una transferencia". Patrones con limite de palabra inicial, sobre texto sin acentos.
- El clasificador manda sus reglas en `systemInstruction` y la pregunta solo como contenido `user`.
- El ruteo usa tres especialistas (`AgenteAhorro`, `AgenteInversiones`, `AgenteFinanzasGenerales`) que comparten `AgenteIA.ReglasDeAlcance`; los datos del contexto van delimitados en las instrucciones.
- `DeclaracionFuncionGemini` rechaza parametros cuyo nombre contenga "usuario", "userid" o "user_id".
- `RespuestaAsistente` (BLL) trae `Texto` o `ClaveRechazo`; la UI traduce la clave. Motivos en Bitacora: `FormPersonalAssistant.Value.Info.Rejected.{Prefilter,Category,ClassificationFailed,RateLimit,OutputValidation}`.
- El validador compara por tipo, sin acentos y por palabra completa. En este change el universo y el contexto quedan vacios en `AsistenteFinancieroBLL.ProcesarMensajeAsync`; los changes 3 y 4 los cargan ahi.
- `BLL` referencia `System.Configuration` (ensamblado del framework, sin NuGet) para leer `AsistenteMaxConsultasPorMinuto` en `CompositionRoot`.

## Risks / Trade-offs

- [Falsos positivos del prefiltro o del clasificador] → El usuario reformula; los patrones son pocos y estan testeados con preguntas validas del dominio.
- [Validacion por nombres no detecta parafrasis] → Complementa, no reemplaza, al contexto cerrado; las changes 3 y 4 piden ademas referencias estructuradas.
- [Limite en memoria se reinicia al cerrar la app] → Aceptado: protege rafagas, no es facturacion.
- [Conflicto con `chat-asistente-sin-bloqueo-ui` en el mismo metodo] → Aplicar ese change primero.

## Open Questions

- Limite y largo maximo definitivos: se ajustan tras medir la cuota gratuita real sin cambiar el diseno.
