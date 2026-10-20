# Proposal

## Why

FINAZAPP ya calcula recomendaciones a pedido, pero el usuario debe recordar solicitar ayuda. Se busca entregar recomendaciones proactivas y recordatorios dentro de Mis notificaciones, aprovechando el mecanismo de tareas programadas que ya actualiza las divisas y dejando ejemplos visibles para la demostracion en el laboratorio.

## What Changes

- Proceso independiente de FINAZAPP, programado en Windows con sesion de Windows iniciada, que evalua los usuarios sin exigir sesiones de la aplicacion.
- Recomendacion diaria de compras mientras existan items pendientes, aunque repita la del dia anterior; una entrega por usuario y dia.
- Revision a las 07:00 del registro de movimientos del dia anterior, con recordatorio si no hubo ninguno.
- Revision diaria de inversiones: perfil completo, aptitud determinada por las mismas reglas del chat y al menos siete dias desde la ultima notificacion automatica de inversion del propio usuario. El chat no participa del intervalo: no lo valida ni lo registra. Quien nunca recibio una (o la elimino) puede recibirla en su primera revision elegible.
- Recomendaciones automaticas de inversion sin porcentaje ni monto. El chat tambien permite omitir el porcentaje; si se indica, lo valida y calcula el monto como hoy.
- Reutilizacion de calculos y validaciones del asistente, historial tecnico, aislamiento de fallas por usuario y controles persistentes contra duplicados.
- Ejemplos completos, sin leer e idempotentes para `user` en `script_datos_prueba.sql`, consultables sin llamadas externas.

## Capabilities

### New Capabilities

- `notificaciones-automaticas`: evaluacion programada, reglas por destinatario, entrega persistente, deduplicacion e historial operativo.

### Modified Capabilities

- `recomendacion-inversiones`: porcentaje opcional y calculo condicional de monto. Capacidad introducida por el cambio predecesor `recomendacion-inversiones-asistente`, aun no archivado; se usan sus nombres exactos y debe archivarse primero.
- `datos-de-prueba`: ejemplos de las tres notificaciones para el usuario de demostracion.

## Impact

Nuevo ejecutable .NET Framework 4.7.2 y scripts de registro/eliminacion de tarea, `TpIDS.sln`, CompositionRoot, BLL de recomendaciones/asistente/notificaciones y sus contratos, entidades, repositorios, DAL, esquema SQL, datos de prueba y traducciones. Mis notificaciones debe soportar el contenido completo y los tipos automaticos manteniendo aislamiento, paginacion y lectura existentes.

Dependencia de planificacion: coordinar con `perfil-inversor-usuario`; no duplicar ni contradecir su politica de riesgo/liquidez. No agregar paquetes NuGet productivos ni servicios residentes. Los detalles propuestos de horario comun, recuperacion y elegibilidad economica se identifican como decisiones de diseño sujetas a revision.
