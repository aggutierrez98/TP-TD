# Design

## Context

Ver `proposal.md` para motivacion. Hallazgos que condicionan el diseño:

- `ActualizadorTasas/Program.cs` es un host de consola que llama a BLL desde CompositionRoot. Su script registra una tarea horaria bajo el usuario de Windows actual. Es reutilizable como patron de despliegue, no como responsabilidad de actualizacion de tasas.
- `RecomendacionComprasBLL.CalcularAsync(usuarioId, nombreLista, ...)` ya evalua pendientes, zonas, precios, promociones, medios y conversiones. Limita el analisis a 20 items y comunica omitidos; conservar ese limite y hacerlo visible.
- `RecomendacionInversionesBLL` calcula el saldo sobre tres meses calendario completos, distingue Negativo/Ajustado/Comodo/Alto y requiere un porcentaje. `ResumenInversiones` usa `decimal` obligatorio para porcentaje y monto; debe admitir ausencia real.
- `perfil-inversor-usuario` esta en implementacion y define riesgo efectivo, restricciones por candidato y ausencia de perfil. Este cambio debe integrarse sobre esa politica, no sobre la version previa del recomendador.
- `NotificacionBLL` distingue lectura personal con sesion de operaciones de operador. No usar la sesion global ni simular login para crear avisos automaticos.
- `Notificacion` usa TPH con tres tipos, mensaje `VARCHAR(300)` y CHECK que exige ItemLista/FechaVencimiento a RecordatorioCompra. Es incompatible con una recomendacion agregada extensa sin adaptar esquema y mapeos.
- `ConsultaChatbot` guarda pregunta/respuesta/fecha, sin categoria ni indicacion fiable de recomendacion entregada. No inferir recomendaciones entregadas buscando palabras en texto libre; el chat queda fuera del intervalo de inversiones (decision 1).
- `script_datos_prueba.sql` es transaccional, idempotente y resuelve `user` por Username. El perfil Alto de ejemplo se incorpora desde el cambio de perfil en `script_bd.sql`.

## Goals / Non-Goals

**Goals:** calculos compartidos entre chat y automatizacion; entrega durable y explicable; no mantener transacciones abiertas durante consultas HTTP; reproducibilidad en laboratorio; fallas parciales observables.

**Non-Goals:** correo/push, ejecucion de inversiones, scraping, preferencias de horario por usuario, servicio Windows residente, reconstruccion semantica del chat historico o recuperacion de avisos por cada dia sin equipo disponible.

## Decisions

### 1. Acuerdos y decisiones propias

Acordado: patron del actualizador de tasas, bandeja interna, compras diarias aunque se repitan, movimientos a las 07:00 revisando ayer, siete dias entre notificaciones automaticas de inversion del usuario, primer envio elegible sin notificacion previa, criterios/perfil compartidos, porcentaje opcional en chat y ausente en automatizacion, ejemplos para `user`.

Decision del usuario (2026-10-20), que reemplaza el acuerdo anterior de intervalo compartido entre canales: "el chatbot no tiene por que validar ni registrar nada; eso es para el notificador automatico. Si una notificacion se borra, no hay forma de validar y se crea otra, punto. Tiene que ser simple". El intervalo de siete dias (7 * 24 h UTC, siete dias exactos permitidos) se calcula solo con la fecha de la ultima notificacion automatica de inversion del usuario; el chat ni lo valida ni lo registra y sus recomendaciones no bloquean las automaticas. Eliminar esa notificacion reinicia el intervalo sin error. No hay tabla de antecedentes.

Decisiones propias sujetas a revision del plan: horario comun 07:00; recuperacion de la jornada actual; aptitud proactiva limitada a Comodo/Alto (Ajustado conserva orientacion de reserva en chat); fecha de movimiento para detectar actividad; idioma del destinatario; historial tecnico sin pantalla nueva; recomendaciones consolidadas por usuario y limite actual de 20 items. No atribuir estas elecciones al usuario como acuerdos previos.

### 2. Host y programacion

Crear `GeneradorNotificaciones`, consola .NET Framework 4.7.2/C# 7.3 incluida en `TpIDS.sln`, con referencias permitidas a BLL/BE/ABSTRACCIONES/SERVICIOS segun necesidad. Todo ensamblado externo por `lib/` y referencias reproducibles; sin NuGet productivo. Constructor de dependencias exclusivamente CompositionRoot.

Tarea `FINAZAPP-GeneradorNotificaciones`: trigger diario 07:00 y recuperacion mediante StartWhenAvailable mas trigger de inicio de sesion. Principal interactivo del usuario actual, sin pedir ni guardar contraseñas. BLL omite antes de 07:00, usa la jornada local al iniciar y no recorre atrasos. Configurar no superponer instancias como defensa local; la base sigue siendo la autoridad. Una ejecucion manual sirve para diagnostico y respeta horarios/deduplicacion. La repeticion manual o recuperacion permite reintentar fallas de hoy, sin bucles inmediatos de IA. Scripts idempotentes de registro y eliminacion, con rutas absolutas y carpeta de trabajo explicita.

El host combina connectionStrings y secrets propios, copiables a salida como el actualizador. Incluir Gemini/embeddings solo cuando los servicios compartidos los requieran. Secretos sin versionar; example vacio. Validar base y conectividad, devolver 0 sin fallas (incluye omisiones), 1 con fallas parciales registradas, 2 ante error fatal que impide la corrida. Equipo apagado o sesion ausente impiden ejecutar; la recuperacion ocurre cuando Windows puede ejecutarla.

### 3. Reglas y contenido compartidos

Nuevo orquestador BLL interno de generacion con reloj inyectado; pagina usuarios elegibles existentes, sin exigir sesion de FINAZAPP. Cada usuario/tipo tiene resultado independiente. Resolver propietario desde repositorios de confianza y volver a verificar estado antes de entregar. Evitar construir BLL desde otros BLL; compartir colaboradores inyectados.

Decision propia tras revision: elegibles son cuentas Activo=true, sin filtrar por permisos administrativos ni por bloqueo temporal del login. Un bloqueo temporal impide autenticacion, no elimina sus datos ni suspende avisos; una cuenta inactiva se excluye y se revalida antes del commit. Las condiciones de cada tipo siguen exigiendo datos personales reales (pendientes/perfil/movimientos). No usar el rol como sustituto de esas condiciones.

Extraer coordinacion de resumen/redaccion/validacion a un colaborador BLL compartido: usa los calculadores y servicio de IA existentes, contexto cerrado, validacion de referencias/fechas y anexos fijos (datos faltantes, zonas, disclaimer). El batch conoce la intencion y no llama clasificador ni function calling para inventar una pregunta. No agrega intercambios artificiales al historial ni consume el limite interactivo del chat. Procesar usuarios secuencialmente y conservar cache/timeouts de proveedores; cancelacion detiene nuevas unidades de trabajo. Usar idioma del usuario sin cambiar estado global de idioma de la UI. Contenido generado es snapshot fechado, no se recalcula al leerlo.

Compras: una recomendacion consolidada de todas las listas con pendientes, sin filtro de nombre. Conservar limites y omisiones del calculador, sin inventar cobertura total. El texto comunica productos sin coincidencia y datos insuficientes. Un resultado valido de falta de coincidencias puede entregarse; una falla tecnica de IA/validacion no se sustituye por una recomendacion inventada.

Movimientos: consulta SQL por UsuarioId y Fecha >= ayer 00:00 AND Fecha < hoy 00:00; no usar la fecha de insercion ni la ultima actividad de login. Se considera cualquier ingreso/egreso registrado con esa fecha, aun cargado retroactivamente antes de evaluar. Plantilla traducible sin IA, con fecha del dia revisado. Una carga posterior no elimina avisos ya entregados.

Inversiones: consultar primero perfil completo, despues calculo economico local; Comodo/Alto habilitan proactividad y Negativo/Ajustado se omiten. Esta puerta usa los niveles ya existentes, sin duplicar formulas. Seleccion final usa exactamente riesgo efectivo/liquidez del flujo con perfil. Si no hay candidatos verificables, omitir como SinAlternativas, no consumir el intervalo. Fuentes parciales permiten recomendar solo los candidatos comprobados con advertencias; ninguna alternativa amplifica riesgo por falta de datos.

### 4. Porcentaje realmente opcional

Cambiar contratos internos/resumen a `decimal?` para porcentaje/monto o representacion equivalente de ausencia; nunca usar cero/100 como centinela. Separar calculo de aptitud del calculo de monto. Ausencia conserva seleccion de alternativas; valor valido calcula monto como hoy; invalido explicito pide correccion. La extraccion debe distinguir ausente, invalido y error tecnico, no colapsarlos a null. Actualizar definicion de herramienta, prompts, redactor, serializacion y pruebas: sin porcentaje, no producir montos sugeridos ni atribuir porcentajes al usuario, aunque puedan citarse precios/tasas verificables. No recuperar porcentaje del historial. Mantener precedencia del perfil requerida por el cambio de perfil.

### 5. Persistencia, tipos y mensajes completos

Ampliar `Notificacion.Mensaje` a `NVARCHAR(MAX)` y alinear parametros SqlDbType, lectura y validaciones de longitud: mantener el limite de 300 del alta manual si es su contrato, sin truncar automaticamente. Revisar lista/preview y detalle WinForms para mensajes extensos y saltos de linea; el detalle ya sirve como destino y no requiere un centro de notificaciones nuevo.

Agregar metadata nullable `TipoAutomatico` (Compras/Inversiones/Movimientos), `FechaJornada` (DATE), `ClaveEjemplo` (nullable, estable) y restriccion de coherencia tipo/fecha. Indice UNIQUE filtrado `(UsuarioId, TipoAutomatico, FechaJornada)` cuando son automaticas, y otro `(UsuarioId, ClaveEjemplo)` para semillas. Compras usa RecordatorioCompra, inversiones RecordatorioInversion y movimientos Notificacion. Para compras agregadas, ItemLista/Promocion/FechaVencimiento pueden ser null cuando TipoAutomatico=Compras o ClaveEjemplo=Demo.Compras; mantener los requisitos anteriores en recordatorios individuales. Hacer nullable FechaVencimiento en BE y mapear DBNull en todos los lectores, sin asignar una fecha de vencimiento ficticia a un resumen diario.

El intervalo de inversiones no tiene tabla propia: se deriva de `Notificacion` (TipoAutomatico = Inversiones) con una consulta del repositorio que devuelve la fecha de la ultima del usuario en UTC. `Notificacion.Fecha` es DATETIME en hora local del ejecutor (como el resto de la bandeja); el repositorio la convierte a UTC para medir 7 * 24 h, sin columna nueva. Cuenta cualquier notificacion automatica de inversion, leida o no; no cuentan ejemplos (ClaveEjemplo), recordatorios manuales ni otros tipos. Borrarla no tiene FK que lo impida y reinicia el intervalo (se toma la anterior o ninguna). Las recomendaciones del chat no se registran en ningun lado ni intervienen.

### 6. Concurrencia y transacciones cortas

No copiar la transaccion larga de tasas alrededor de HTTP. Antes de calcular, leer estado para evitar trabajo innecesario; despues del calculo abrir transaccion corta, bloquear la fila de usuario (`INotificacionRepositorio.BloquearUsuarioParaEntrega`) como punto estable de serializacion entre lotes, revalidar destinatario, condicion relevante, deduplicacion y fecha de la ultima notificacion de inversion, insertar notificacion/resultado y confirmar. El chat no toma este bloqueo ni participa de la serializacion. Dos batch pueden consultar APIs, pero el indice y revalidacion garantizan una entrega. No se promete exactamente una llamada externa.

Revalidar perfil vigente antes de entregar: si cambio desde el snapshot, omitir esa recomendacion y registrar motivo; no entregar instrumentos basados en un perfil anterior. Pendientes y movimientos tambien se revalidan al guardar. Para inversiones medir 7 * 24 horas en UTC; para jornada y fechas de movimiento usar horario local del unico ejecutor y un reloj inyectable consistente. No hay zonas horarias por usuario en este alcance.

Registro tecnico propio de corrida/detalles: inicio/fin UTC, usuario/tipo/jornada, resultado Entregada/Omitida/Fallida, motivo estable y referencia a notificacion. Una caida de SQL que impide registrar falla se informa por stderr/exit 2; no reportar exito. No guardar prompts, secretos o respuestas privadas completas en logs. No se agrega pantalla de historial; consultar por consola y registros persistentes. La creacion sin sesion usa un registro tecnico de sistema explicito, sin forzar una identidad humana en Bitacora.

### 7. Ejemplos de laboratorio

En `script_datos_prueba.sql`, dentro de la transaccion existente y para @uid resuelto por `user`, insertar un ejemplo completo de cada tipo con claves estables Demo.Compras/Demo.Inversiones/Demo.Movimientos. Marcar en el mensaje que son ejemplos, sin APIs; referencias de compras compatibles con catalogos sembrados, inversion compatible con perfil Alto del seed sin monto/rendimientos inventados, y recordatorio con fecha explicita de ejemplo.

Los ejemplos tienen ClaveEjemplo pero NO TipoAutomatico/FechaJornada ni cuentan para el intervalo: no ocupan el cupo diario ni reinician los siete dias. Las fechas de primera carga se conservan, al igual que la lectura, al repetir el script incluso otro dia. No modificar Usuarios ni su integridad. Verificar instalacion con esquema mas datos, y apertura sin red; las pruebas ejecutan solo en base aislada redirigida por el harness.

### 8. Dependencias y orden documental

Coordinar implementacion con `perfil-inversor-usuario`: completar/estabilizar la integracion del perfil antes del bloque de inversiones de este cambio. Antes de sincronizar/archivar deltas, archivar `recomendacion-inversiones-asistente`, luego perfil y finalmente este cambio, comprobando que el requisito de porcentaje no vuelva a hacerse obligatorio por orden de archivo. No editar los cambios del usuario en esta fase.

Registrar en tasks.md la actualizacion de G07/G08 y casos de uso/manuales pertinentes, coordinada con `actualizar-diagramas-tp`; diagramas solo reflejan implementacion verificada. No crear notas de estado ni otro diseño paralelo.

## Risks / Trade-offs

- [Muchos usuarios y costos de IA] → Secuencial, filtros baratos previos y cache existente; no prometer plazo de entrega independiente del volumen.
- [Avisos repetidos y bandeja creciente] → Repeticion diaria solicitada, historial paginado y una entrega por jornada; no supresion por similitud.
- [Equipo no disponible] → Recuperar solo jornada actual; ninguna garantia de ejecucion con equipo apagado.
- [Doble calculo concurrente] → Aceptado para evitar locks durante HTTP; entrega unica en base.
- [Datos de prueba antiguos] → Etiqueta de ejemplo y sin afirmar vigencia comercial; nunca presentarlos como analisis en vivo.
- [Notificaciones eliminadas] → Eliminar la ultima notificacion de inversion reinicia el intervalo y permite otra; es el comportamiento buscado, no se protege con FK ni registros paralelos.

## Migration Plan

1. Incorporar esquema al script de recreacion existente, no introducir una via incremental de instalacion. Probar exclusivamente contra base aislada antes de usar laboratorio.
2. Compilar ambos hosts y UI con dependencias de lib reproducibles; configurar conexion y secretos por ejecutable.
3. Ejecutar esquema y script de datos para demo; verificar bandeja de `user` sin red y sin tareas registradas.
4. Registrar tarea nueva y comprobar una corrida real con sesion Windows iniciada y FINAZAPP cerrada; validar historial, codigo de salida y entrega unica.
5. Reversion operativa: eliminar/deshabilitar tarea nueva; restaurar respaldo para recuperar esquema/datos anteriores si se requiere, dado que script_bd recrea la base. No alterar la tarea de tasas.
