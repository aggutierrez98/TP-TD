# Spec Delta

## Purpose

Define el ejecutable unico de tareas programadas de FINAZAPP: como se elige la tarea que corre, que codigos de salida lee el Programador de tareas y como se configuran y registran las tareas, para que agregar una tarea nueva no requiera un ejecutable nuevo.

## ADDED Requirements

### Requirement: Seleccion de la tarea por argumento
El ejecutable de tareas programadas SHALL ejecutar exactamente una tarea por invocacion, elegida por su primer argumento: `tasas` ejecuta la actualizacion automatica de tasas y `notificaciones` ejecuta la generacion diaria de notificaciones. No MUST ejecutar ninguna otra tarea en esa invocacion.

#### Scenario: Ejecutar la actualizacion de tasas
- **WHEN** se invoca el ejecutable con el argumento `tasas`
- **THEN** corre solo la actualizacion automatica de tasas, con origen programado, y no se genera ninguna notificacion

#### Scenario: Ejecutar la generacion de notificaciones
- **WHEN** se invoca el ejecutable con el argumento `notificaciones`
- **THEN** corre solo la generacion diaria de notificaciones y no se consulta la API de tasas

#### Scenario: Nombre de tarea en mayusculas
- **WHEN** se invoca el ejecutable con el argumento `TASAS`
- **THEN** corre la actualizacion de tasas igual que con `tasas`

### Requirement: Uso invalido
Si no se indica tarea o la tarea es desconocida, el ejecutable SHALL informar por la salida de error las tareas validas y terminar con el codigo de error inesperado, sin acceder a la base ni a servicios externos.

#### Scenario: Sin argumento
- **WHEN** se invoca el ejecutable sin argumentos
- **THEN** informa las tareas validas, termina con el codigo de error inesperado y no se abre ninguna conexion a la base

#### Scenario: Tarea desconocida
- **WHEN** se invoca el ejecutable con el argumento `reportes`
- **THEN** informa que la tarea no existe, lista `tasas` y `notificaciones` y termina con el codigo de error inesperado

### Requirement: Codigos de salida por tarea
El ejecutable SHALL conservar para cada tarea los codigos de salida que lee el Programador de tareas: 0 sin fallas, 1 con fallas parciales registradas en la corrida, 2 ante error fatal o corrida cancelada. Una corrida cancelada MUST NOT informarse como exito.

#### Scenario: Actualizacion de tasas fallida pero registrada
- **WHEN** la tarea `tasas` termina con resultado fallido registrado en el historial
- **THEN** el codigo de salida es 1

#### Scenario: Notificaciones con fallas parciales
- **WHEN** la tarea `notificaciones` termina con al menos una notificacion fallida
- **THEN** el codigo de salida es 1 y el resumen informa entregadas, omitidas y fallidas

#### Scenario: Cancelacion
- **WHEN** el operador cancela la corrida de cualquiera de las dos tareas con Ctrl+C
- **THEN** la corrida se cancela de forma ordenada y el codigo de salida es 2

### Requirement: Configuracion y secretos compartidos
El ejecutable SHALL leer la cadena de conexion `TpIDS` de su propia configuracion y los secretos de un `secrets.config` junto al ejecutable, excluido del control de versiones, con una plantilla `.example` versionada que contenga las claves de todas las tareas. Cada tarea MUST fallar solo por la falta de su propia clave.

#### Scenario: Falta la clave de tasas
- **WHEN** falta `OpenExchangeRatesApiKey` y se ejecuta `tasas`
- **THEN** la corrida se registra como fallida por la clave faltante

#### Scenario: Falta la clave de Gemini
- **WHEN** falta `GeminiApiKey` y se ejecuta `notificaciones`
- **THEN** las recomendaciones de compras e inversiones se registran como fallidas en la corrida y el recordatorio de movimientos faltantes igual se genera

#### Scenario: Falta una clave que la tarea no usa
- **WHEN** falta `GeminiApiKey` y se ejecuta `tasas`
- **THEN** la actualizacion de tasas corre normalmente

### Requirement: Registro y baja idempotentes de las tareas
El repositorio SHALL incluir scripts que registren y den de baja cada tarea en el Programador de tareas del usuario actual sin pedir contrasena. Cada tarea MUST conservar su nombre y sus disparadores (`FINAZAPP-ActualizadorTasas` cada hora; `FINAZAPP-GeneradorNotificaciones` a las 07:00 y al iniciar sesion). Registrar dos veces deja una sola tarea y dar de baja una tarea inexistente no falla.

#### Scenario: Registrar la tarea de tasas
- **WHEN** se ejecuta el script de registro para `tasas`
- **THEN** existe la tarea `FINAZAPP-ActualizadorTasas` que lanza `CRONS.exe tasas` cada hora

#### Scenario: Registrar la tarea de notificaciones
- **WHEN** se ejecuta el script de registro para `notificaciones`
- **THEN** existe la tarea `FINAZAPP-GeneradorNotificaciones` que lanza `CRONS.exe notificaciones` a las 07:00 y al iniciar sesion, sin superponer instancias

#### Scenario: Registrar y dar de baja repetidamente
- **WHEN** se registra una tarea dos veces y luego se da de baja dos veces
- **THEN** nunca hay mas de una tarea con ese nombre y ninguna de las operaciones falla
