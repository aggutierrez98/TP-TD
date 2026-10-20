# Tasks

El progreso de este change vive solo en este archivo. Aplicar despues de `chat-asistente-sin-bloqueo-ui`. Los cambios de comportamiento en BLL y SERVICIOS siguen la skill `strict-tdd` (baseline en verde, test en rojo observado, implementacion en verde).

## 1. Contratos y clasificador

- [x] 1.1 Correr `tests/tools/run-tests.ps1 -IncludeIntegration` como baseline y anotar exit code y contadores del TRX. Baseline: exit 0, unit 729/729, integracion 192/192.
- [x] 1.2 Agregar `CategoriaConsulta`, `IClasificadorConsultas` y el contrato de solicitud con contexto permitido en ABSTRACCIONES; verificar que compila y que la skill `architecture-dependency-guard` no reporta violaciones.
- [x] 1.3 Escribir en `SERVICIOS.Tests` los tests de `GeminiClasificadorConsultas` (cuerpo con temperatura 0 y enum de cinco valores; parseo de cada valor; JSON invalido, valor desconocido y fallo HTTP lanzan o devuelven no admitido) y verlos fallar; implementar y verlos en verde.
- [x] 1.4 Extender `GeminiChatClient` con temperatura, `tools`/`toolConfig` (`ANY` + `allowedFunctionNames`) y parseo de `functionCall`, verificando los nombres de campos en la documentacion oficial; tests de armado y parseo en rojo y luego en verde.
- [x] 1.5 Quitar la clasificacion interna de `GeminiAsistenteFinancieroService`, rutear por categoria y endurecer los prompts de `AgenteIA`; adaptar los tests existentes y verificar `SERVICIOS.Tests` en verde.

## 2. Pipeline en BLL

- [x] 2.1 Tests del prefiltro (largo, patrones de inyeccion y de temas excluidos, preguntas validas como "codigo de descuento" que MUST pasar) en rojo; implementar y verlos en verde.
- [x] 2.2 Tests del limite de uso con reloj inyectable (dentro del limite, exceso, ventana que se libera, usuarios independientes, acceso concurrente) en rojo; implementar y verlos en verde.
- [x] 2.3 Tests de `ValidadorRespuestaAsistente` (bloque de codigo, entidad fuera del contexto, contexto vacio, respuesta valida) en rojo; implementar y verlos en verde.
- [x] 2.4 Tests de `AsistenteFinancieroBLL` (rechazo por cada capa sin llamar al servicio, fail-closed del clasificador, rechazo no persistido ni enviado como historial, historial limitado a 10, bitacora con motivo, usuario siempre de la sesion) en rojo; implementar el pipeline y verlos en verde.
- [x] 2.5 Dar de alta clasificador, limite y validador en `CompositionRoot` segun `composition-root-guard`; verificar con la compilacion que la guardia pasa.

## 3. UI y traducciones

- [x] 3.1 Mostrar en `FormAsistentePersonal` el texto de rechazo como mensaje del asistente, sin dialogo de error; verificar con una prueba manual de una pregunta fuera de tema. Verificado con el test FlaUI `FIN_ChatAsistente_PreguntaFueraDeTema_MuestraRechazoFijoSinDialogoNiPersistencia`.
- [x] 3.2 Agregar las keys `Assistant.Rejection.*` y las de bitacora a `base.es.json` y `base.in.json`, ejecutar `translation-base-seed-sync` e insertarlas en `Palabra`/`Traduccion` de `TpCatedraIDS` y `TpCatedraIDS_Test`; verificar por consulta en ambos idiomas.
- [x] 3.3 Agregar `AsistenteMaxConsultasPorMinuto` al `App.config` de UI con valor 5; verificar que el BLL lo lee y usa 5 si falta.

## 4. Cierre

- [x] 4.1 Compilar `TpIDS.sln` con MSBuild (skill `winforms-build-verification`) con guardias de capas y Composition Root; verificar exit code 0.
- [x] 4.2 Correr `tests/tools/run-tests.ps1 -IncludeIntegration` (skill `test-suite-verification`); verificar exit code 0 y reportar contadores reales del TRX. Resultado: exit 0; unit 814/814, integracion 192/192, UI 127/128 (1 omitido preexistente).
- [x] 4.3 Registrar en `openspec/changes/actualizar-diagramas-tp/tasks.md` el pendiente de G07 (`IClasificadorConsultas`, `GeminiClasificadorConsultas`, prefiltro, limite y validador) y la secuencia del envio con guardrails, indicando que G08 no cambia; verificar que la tarea quedo registrada.
