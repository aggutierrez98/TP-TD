# Proposal

## Why

El asistente personal responde cualquier pregunta: el clasificador actual (`GeminiAsistenteFinancieroService`, enum Ahorro/Inversiones/General con `responseFormat`) solo enruta, nunca rechaza, y ante cualquier fallo cae en "General". Antes de darle al asistente datos reales del usuario (`recomendacion-compras-asistente`, `recomendacion-inversiones-asistente`) tiene que quedar limitado a temas financieros y no poder exponer datos de otros usuarios.

## What Changes

- Defensa en capas, en este orden: prefiltro local determinista (largo maximo y patrones obvios de inyeccion o fuera de tema), clasificador con salida JSON enum y criterio fail-closed, aislamiento de datos por la sesion y validacion de la respuesta en codigo.
- Categorias admitidas: `COMPRAS`, `INVERSIONES` y `FINANZAS_GENERALES`. Se rechazan `FUERA_DE_TEMA`, `INYECCION`, los pedidos mixtos y cualquier salida invalida o fallo del clasificador.
- **BREAKING (interno)**: la clasificacion sale de `GeminiAsistenteFinancieroService` y pasa a un contrato nuevo `IClasificadorConsultas` (ABSTRACCIONES) con implementacion Gemini en SERVICIOS, alta en `CompositionRoot` y orquestacion en `AsistenteFinancieroBLL`.
- El rechazo muestra un texto fijo traducible (no generado por el modelo), se registra en Bitacora y no se reenvia como historial en turnos posteriores.
- Limite de preguntas por minuto por usuario para cuidar la cuota gratuita de Gemini.
- Validacion de salida reutilizable: sin bloques de codigo y sin citar comercios, promociones, cuentas o instrumentos ausentes del contexto entregado al modelo. Los changes 3 y 4 la usan.
- Base de function calling de Gemini (modo `ANY` con `allowedFunctionNames`) en el cliente compartido; las funciones concretas llegan en los changes siguientes.

## Capabilities

### New Capabilities
- `guardrails-asistente`: limites de alcance, rechazo, aislamiento de datos, validacion de respuestas y limite de uso del asistente personal.

### Modified Capabilities
Ninguna. `chat-asistente-personal` todavia no esta archivada (vive en `chat-asistente-sin-bloqueo-ui`); este change no altera sus requisitos.

## Impact

- **ABSTRACCIONES**: `IClasificadorConsultas`, enum de categoria y contrato de solicitud al servicio de asistente.
- **SERVICIOS**: clasificador Gemini nuevo; `GeminiAsistenteFinancieroService`, `AgenteIA`, `AgenteAhorro`, `AgenteInversiones` y `GeminiChatClient` (temperatura, `tools`/`toolConfig`, prompts endurecidos).
- **BLL**: `AsistenteFinancieroBLL` (pipeline, limite de uso, validacion de salida, bitacora), `CompositionRoot`.
- **UI**: `FormAsistentePersonal` muestra el rechazo sin tratarlo como error.
- **Traducciones**: `translation-seeds/base.es.json`, `base.in.json` y `Palabra`/`Traduccion` de ambas bases.
- **Orden**: aplicar despues de `chat-asistente-sin-bloqueo-ui`, que mueve el envio fuera del hilo de UI sobre el mismo metodo.
- **Fuera de alcance**: datos reales del usuario en el prompt, otro proveedor de clasificacion (Jev evaluado y descartado por ahora), moderacion de imagenes y cambios de esquema SQL.
