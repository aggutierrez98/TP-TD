# Proposal

## Why

`ActualizadorTasas` y `GeneradorNotificaciones` son dos ejecutables de consola casi identicos: mismas referencias (ABSTRACCIONES, BE, BLL, SERVICIOS), misma cadena `TpIDS`, mismo mecanismo `secrets.config`, misma convencion de codigos de salida (0/1/2) y los mismos scripts de alta y baja en el Programador de tareas. Cada tarea programada nueva repetiria csproj, config, scripts, guard de arquitectura y entradas de documentacion. Unificarlos en un solo host de tareas programadas elimina esa duplicacion y deja un lugar unico para las proximas.

## What Changes

- Nuevo proyecto de consola `CRONS` (`CRONS.exe`, capa de presentacion como `TpIDS`) que reemplaza a `ActualizadorTasas` y `GeneradorNotificaciones`.
- `CRONS.exe <tarea>` ejecuta una sola tarea por invocacion: `tasas` (actualizacion automatica de tasas) o `notificaciones` (generacion diaria de notificaciones). Sin argumento o con una tarea desconocida informa el uso y termina con codigo de salida de error.
- Una sola `App.config` y un solo `secrets.config.example` con las claves de ambas tareas (`OpenExchangeRatesApiKey`, `GeminiApiKey`); `GeminiModel` pasa a la `App.config` de `CRONS`.
- Scripts `registrar-tarea` y `eliminar-tarea` en `CRONS/scripts/`, uno por tarea programada (parametro de tarea). Las tareas del Programador mantienen nombre y disparadores: `FINAZAPP-ActualizadorTasas` (cada hora) y `FINAZAPP-GeneradorNotificaciones` (07:00 y al iniciar sesion); solo cambia el ejecutable y su argumento.
- Se eliminan los proyectos `ActualizadorTasas` y `GeneradorNotificaciones` de `TpIDS.sln`, del disco y de las listas de proyectos que los nombran (guard de arquitectura, `Directory.Build.targets`, `.gitignore`, README, `tests/BLL.Tests`).
- **BREAKING**: las tareas ya registradas apuntan a los `.exe` viejos y dejan de funcionar al borrarlos; hay que eliminarlas con los scripts viejos y registrarlas de nuevo con los de `CRONS`.

## Capabilities

### New Capabilities
- `ejecucion-tareas-programadas`: contrato del ejecutable unico de tareas programadas: seleccion de la tarea por argumento, uso invalido, codigos de salida para el Programador de tareas, configuracion y secretos compartidos, y registro/baja idempotente de cada tarea.

### Modified Capabilities
<!-- Ninguna: los requisitos de actualizacion-automatica-tasas ("el ejecutable", "secrets.config junto al ejecutable", cadena `TpIDS` del config del propio ejecutable) siguen siendo ciertos con CRONS.exe. El comportamiento de cada tarea no cambia. -->

## Impact

- Codigo: proyecto nuevo `CRONS/`; borrado de `ActualizadorTasas/` y `GeneradorNotificaciones/`; `TpIDS.sln`; `Directory.Build.targets` (condicion de `SystemTextJson.references.props`); `.gitignore`; `tests/BLL.Tests` (link de `EjecucionGenerador.cs` y su namespace).
- Arquitectura: `.agents/scripts/verify-architecture-dependencies.ps1` y `dependency-matrix.md` pasan a listar `CRONS` con las mismas dependencias permitidas (ABSTRACCIONES, BE, BLL, SERVICIOS).
- Documentacion: `README.md` (secciones de ambos programas) y referencias en `actualizar-diagramas-tp` (tareas 2.6, 2.12, 3.5, 3.12).
- Operacion: re-registro de las tareas programadas en cada maquina donde esten registradas. Sin cambios de base de datos, NuGet ni DLL externas.
- Coordinacion: `notificaciones-automaticas-diarias` (22/27) sigue activo y nombra a `GeneradorNotificaciones`; este cambio se implementa despues de archivarlo (ver design.md).
