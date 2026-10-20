# Spec Delta

## Purpose

Entregar recomendaciones proactivas de compras e inversiones y recordatorios de movimientos en la bandeja personal, mediante una revision programada independiente de la aplicacion.

## ADDED Requirements

### Requirement: Destinatarios activos
El sistema SHALL evaluar solamente cuentas activas y revalidar ese estado antes de entregar. Un bloqueo temporal de acceso MUST NOT suspender los avisos de una cuenta activa; los permisos administrativos MUST NOT determinar la elegibilidad personal.

#### Scenario: Cuenta desactivada durante el calculo
- **WHEN** se desactiva la cuenta antes de confirmar una recomendacion calculada
- **THEN** no se entrega y se registra la omision

#### Scenario: Bloqueo temporal del login
- **WHEN** una cuenta activa tiene bloqueado temporalmente el acceso pero cumple las condiciones del aviso
- **THEN** puede recibir la notificacion para consultarla cuando vuelva a acceder

### Requirement: Ejecucion diaria independiente
El sistema SHALL evaluar los usuarios diariamente a las 07:00, hora local del equipo ejecutor, mediante una tarea de Windows que requiere sesion de Windows iniciada pero no FINAZAPP abierta. Una ejecucion demorada SHALL procesar solo la jornada actual a partir de las 07:00, sin recuperar notificaciones de dias anteriores.

#### Scenario: Aplicacion cerrada
- **WHEN** llega el horario y el equipo y la sesion de Windows estan disponibles con acceso a la base
- **THEN** se ejecuta la revision sin iniciar FINAZAPP ni autenticar a cada destinatario

#### Scenario: Horario perdido
- **WHEN** se recupera la tarea a las 11:00 tras perder la ejecucion de las 07:00
- **THEN** se evalua el dia actual y no se generan avisos retroactivos por cada dia perdido

#### Scenario: Arranque antes del horario
- **WHEN** el proceso se ejecuta antes de las 07:00
- **THEN** no genera las notificaciones diarias de esa jornada

### Requirement: Compras diarias con pendientes
El sistema SHALL generar una recomendacion diaria consolidada por usuario con items pendientes, reutilizando los criterios del asistente, aunque el contenido coincida con el dia anterior. Sin pendientes SHALL omitirla. Si faltan coincidencias o datos SHALL explicarlo sin inventar ofertas.

#### Scenario: Lista sin cambios
- **WHEN** un usuario conserva los mismos pendientes durante dos jornadas
- **THEN** recibe una notificacion de compras cada jornada, aunque ambas coincidan

#### Scenario: Todas las compras completadas
- **WHEN** no hay items pendientes en ninguna lista del usuario
- **THEN** no se genera una notificacion de compras

### Requirement: Recordatorio del registro del dia anterior
El sistema SHALL generar un recordatorio si no existe ningun movimiento del usuario con fecha de movimiento dentro del dia calendario anterior a la revision. La notificacion SHALL indicar la fecha revisada. No SHALL exigir movimientos con fecha del dia que comienza.

#### Scenario: Sin actividad del dia anterior
- **WHEN** el 21 de octubre se revisa al usuario y no tiene movimientos fechados el 20
- **THEN** recibe un recordatorio referido al 20 de octubre

#### Scenario: Movimiento anterior cargado hoy
- **WHEN** antes de la revision el usuario ya cargo un movimiento con fecha del dia anterior
- **THEN** se considera registrado y no se genera el recordatorio

### Requirement: Inversion segun aptitud y frecuencia
El sistema SHALL recomendar inversiones automaticamente solo con perfil completo y nivel economico Comodo o Alto segun el mismo calculo del chat, respetando riesgo y liquidez vigentes, sin porcentaje ni monto. SHALL exigir siete dias completos desde la ultima notificacion automatica de inversion del usuario, tomada de las propias notificaciones; sin una SHALL permitir la primera revision elegible. Las recomendaciones del chat MUST NOT validar, bloquear ni reiniciar el intervalo.

#### Scenario: Primera oportunidad
- **WHEN** un usuario con perfil completo y nivel Comodo nunca recibio recomendaciones de inversion
- **THEN** puede recibir una en la primera revision, sin monto supuesto

#### Scenario: Notificacion reciente
- **WHEN** hace menos de siete dias se entrego al usuario una notificacion automatica de inversion
- **THEN** se omite la automatica

#### Scenario: Consulta al chat
- **WHEN** el usuario recibio hace menos de siete dias una recomendacion de inversion por chat y no tiene notificacion automatica reciente
- **THEN** la revision automatica puede entregarle una recomendacion

#### Scenario: Limite de siete dias
- **WHEN** transcurrieron exactamente siete dias desde su ultima notificacion automatica de inversion y conserva aptitud
- **THEN** puede recibir una nueva recomendacion

#### Scenario: Sin perfil o sin excedente suficiente
- **WHEN** no tiene perfil completo o su nivel economico es Negativo o Ajustado
- **THEN** se omite la recomendacion automatica de inversiones

### Requirement: Intervalo derivado de las notificaciones
El intervalo de siete dias SHALL calcularse unicamente con la fecha de la ultima notificacion automatica de inversion del usuario, sin tabla de antecedentes ni registro del chat. Ejemplos de laboratorio, recordatorios manuales y notificaciones de otros tipos o usuarios MUST NOT contarse. Si el usuario o un operador elimina esa notificacion, el sistema SHALL tomar la anterior o, sin ninguna, permitir una nueva en la siguiente revision elegible, sin error ni bloqueo de la eliminacion.

#### Scenario: Fallo de entrega
- **WHEN** falla el guardado de una notificacion automatica de inversion
- **THEN** no queda una notificacion que impida la siguiente entrega

#### Scenario: Notificacion eliminada
- **WHEN** se elimina la ultima notificacion automatica de inversion del usuario
- **THEN** la eliminacion se completa y la siguiente revision elegible puede crear otra

#### Scenario: Notificacion leida
- **WHEN** el usuario ya leyo su ultima notificacion automatica de inversion
- **THEN** sigue marcando el inicio del intervalo

### Requirement: Entrega personal completa
Las notificaciones SHALL guardarse sin leer y mostrar su contenido completo en Mis notificaciones, con fecha del analisis y limitaciones de datos. Las recomendaciones SHALL respetar el contexto validado del asistente y el idioma del destinatario; inversiones SHALL incluir el aviso legal fijo existente. La falta de una fuente MUST NOT habilitar instrumentos excluidos por el perfil.

#### Scenario: Recomendacion extensa
- **WHEN** el contenido supera 300 caracteres
- **THEN** el usuario puede abrirlo completo sin truncamiento y marcarlo como leido mediante el flujo existente

### Requirement: Entrega unica y aislamiento de fallas
El sistema SHALL permitir como maximo una notificacion automatica por usuario, tipo y jornada, incluso ante concurrencia y reintentos. Un fallo por usuario o tipo SHALL registrarse sin impedir el procesamiento de los demas; los fallos no SHALL contarse como entregas. Cada corrida SHALL informar cantidades entregadas, omitidas y fallidas.

#### Scenario: Corridas superpuestas
- **WHEN** dos procesos intentan entregar compras al mismo usuario en la misma jornada
- **THEN** solo una notificacion queda confirmada

#### Scenario: Falla de IA para compras
- **WHEN** falla la recomendacion de compras de un usuario
- **THEN** se registra la falla y se continua con sus otras revisiones y con otros usuarios
