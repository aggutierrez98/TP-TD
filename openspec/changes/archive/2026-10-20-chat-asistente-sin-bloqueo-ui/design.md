# Design

## Context

Ver `proposal.md` para la motivacion. La inspeccion confirma:

- `UI/FormAsistentePersonal.cs:42-67` ya tiene un handler async que espera `EnviarMensajeAsync` y recupera los controles en finally.
- `BLL/AsistenteFinancieroBLL.cs:53-57` lee y materializa el historial mediante SQL sincrono antes del primer await. La ejecucion de un metodo async hasta ese await ocurre en el hilo llamador.
- Gemini usa `HttpClient.PostAsync` y `ConfigureAwait(false)`; no se encontraron Wait, Result ni GetAwaiter().GetResult() en los servicios inspeccionados.
- El guardado y la bitacora son sincronos. Normalmente continuan fuera de UI tras el await, pero un servicio que devuelve una Task ya completada no garantiza ese cambio de hilo.
- `MovimientoBLL.EscanearBorradorAsync` llega al servicio asincrono sin consultar SQL previamente y no persiste el borrador.

Esto demuestra un camino bloqueante, pero no confirma que la congelacion reportada dure toda la llamada HTTP. Se verificara con dependencias controladas y con el ejecutable recompilado antes de atribuirle todo el sintoma. La carga inicial del historial al abrir el formulario es otro flujo sincrono, fuera del alcance de este envio.

## Goals / Non-Goals

**Goals:**
- Garantizar que ninguna fase costosa del envio se ejecute en el hilo de UI, incluso con servicio completado sin suspension.
- Conservar el orden historial → IA → transaccion de consulta/bitacora → resultado.
- Mantener todas las actualizaciones visuales en el contexto WinForms y soportar el cierre durante la espera.

**Non-Goals:**
- Cambiar los prompts, agentes, API HTTP, modelo de datos o tiempos de respuesta de Gemini.
- Migrar contratos de persistencia de otras capacidades ni añadir infraestructura de UI.

## Decisions

### 1. Frontera asincrona en la BLL para la persistencia sincrona existente

Validar y normalizar la pregunta y capturar el ID del usuario antes de programar el trabajo. Extraer el flujo restante a un metodo privado asincrono y ejecutarlo con `Task.Run(Func<Task<string>>)`, esperado con `ConfigureAwait(false)`. El delegado asincrono sigue esperando HTTP sin bloquear un worker durante la red; la frontera garantiza que el historial, la preparacion y el guardado sincronos no vuelvan al hilo llamador, aun con dobles de servicio completados.

Alternativas: convertir toda la cadena SQL a APIs asincronas seria valido pero ampliaria innecesariamente contratos y DAL; envolver el envio solamente desde UI resolveria ese formulario, dejando la garantia de `EnviarMensajeAsync` dependiente del consumidor. Agregar solamente ConfigureAwait(false) no mueve el trabajo previo al primer await.

No compartir una conexion o unidad de trabajo creada en UI: cada operacion conserva sus instancias locales y la unidad se crea despues de obtener la respuesta. Mantener el constructor y CompositionRoot existentes.

### 2. Continuacion visual en el handler existente

Conservar await sin ConfigureAwait(false) en UI, la captura de la pregunta antes de esperar y la inhibicion de envios. Agregar una guarda de envio en curso en el handler, ademas del estado de los controles, y comprobar si el formulario esta cerrado/disposed antes de actualizar historial, mostrar errores, rehabilitar controles o enfocar el campo. Restaurar el estado en finally para un formulario que sigue abierto.

Cerrar el formulario no cancela HTTP ni revierte un intercambio que pudo persistirse. La operacion es observada por el handler, pero su continuacion visual se omite tras el cierre. Se conserva el estado visual existente sin requerir cambios de Designer o nuevas traducciones.

### 3. Verificacion determinista del camino bloqueante

Agregar en `AsistenteFinancieroBLLTests` dobles locales con señales y esperas acotadas. Comprobar que el metodo devuelve una Task pendiente mientras el repositorio retiene la lectura y que el hilo llamador puede avanzar; liberar las señales en finally. Cubrir tambien persistencia lenta con servicio ya completado, orden del historial, propagacion de errores y unidad de trabajo creada solo tras la respuesta. Evitar usar demoras arbitrarias como criterio de correccion.

La verificacion visual debera distinguir ventana congelada de controles de envio deliberadamente deshabilitados. Comprobar repintado, movimiento de ventana, navegacion del shell, restauracion tras error y cierre con respuesta pendiente. La suite UI requiere confirmacion previa segun las reglas del proyecto.

## Risks / Trade-offs

- [SQL sincrono ocupa un worker durante cada consulta] → Aceptable para un envio interactivo; no bloquear workers mediante Wait/Result durante HTTP.
- [Objetos compartidos o sesion cambian durante la espera] → Capturar el usuario al inicio y verificar que repositorios/factory no almacenen conexiones compartidas. La bitacora ya consulta la sesion al guardar en el flujo actual; conservar esa semantica y registrar cualquier conflicto real antes de ampliar el cambio.
- [La causa de toda la espera difiere del camino SQL identificado] → Reproducir el envio con el binario actual y localizar cualquier bloqueo adicional antes de dar el problema por resuelto; ajustar este plan si aparece otra causa.
- [Cierre durante continuacion] → Comprobar vida del formulario en los caminos de exito, error y finally; no acceder a controles desde el worker.
- [Cambios previos extensos en el working tree] → Limitar las ediciones a los archivos de este cambio y preservar el trabajo existente.

## Migration Plan

Sin migracion de base de datos ni configuracion. Compilar la solucion y ejecutar la suite requerida; reemplazar el ejecutable despues de verificar. El rollback consiste en revertir las modificaciones acotadas del envio y del formulario.

## Implementation Notes

- Diagnostico confirmado (1.1/1.2): baseline unitaria en verde (724). Con la BLL original, las regresiones de lectura lenta y de guardado lento con servicio completado fallaron por assertion: el hilo llamador quedo retenido hasta vencer la espera acotada de 5 s. Tras `Task.Run` pasan (729/729). No aparecio otra causa de bloqueo en el envio.
- Dependencias del envio (1.4): `ConsultaChatbotRepositorio` y `ChatbotFinancieroRepositorio` se construyen en CompositionRoot sin `AccesoDatosSqlServer`, por lo que el DAL abre y libera una conexion propia por operacion; la unidad de trabajo se crea por envio despues de la respuesta. No hay conexiones compartidas ni Wait/Result en el camino de Gemini. La bitacora sigue leyendo la sesion al guardar, como antes (ya ocurria fuera de UI tras el await).
- La validacion de la pregunta y la captura del usuario quedan antes de `Task.Run`; sus errores siguen llegando como Task fallida. Los errores de lectura se propagan sin envolver, igual que antes.
- Formulario (2.1): guarda `envioEnCurso` contra reentrada (boton y Enter) y `FormularioDisponible` (FormClosed/IsDisposed/Disposing) antes de tocar controles o mostrar dialogos en exito, error y finally.
- Verificacion UI (2.2): el exe real no admite un servicio falso; la dependencia lenta controlada es un bloqueo exclusivo sobre `dbo.ConsultaChatbot` tomado por la prueba (BD sin READ_COMMITTED_SNAPSHOT), confirmado por `sys.dm_exec_requests`. La copia stageada de la suite UI ya no incluye `secrets.config`, de modo que el envio termina sin red por `Error.Assistant.ApiKeyMissing`; el camino exitoso con Gemini real queda cubierto por los tests de BLL y no se automatiza contra el proveedor.
- Evidencia 2.1/2.2 (`FIN_ChatAsistenteSinBloqueoTests`, ejecutable recompilado, suite UI completa autorizada por el usuario): con la lectura retenida, el shell responde `WM_NULL` (SendMessageTimeout con SMTO_ABORTIFHUNG), la ventana del asistente se mueve, se navega a Bitacora, boton y campo quedan inhibidos; al liberar aparece un unico error, la pregunta se conserva, no se persiste nada y el envio se rehabilita. Cerrar con el envio pendiente no muestra dialogo tardio, el shell sigue operativo y un formulario nuevo arranca habilitado. Repintado verificado de forma indirecta (bombeo de mensajes y movimiento); exito con Gemini real no automatizado.
