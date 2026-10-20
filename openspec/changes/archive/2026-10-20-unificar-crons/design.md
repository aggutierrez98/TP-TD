# Design

## Context

- `ActualizadorTasas/Program.cs` y `GeneradorNotificaciones/Program.cs` son hosts de consola que llaman a `CompositionRoot.ActualizacionTasas` y `CompositionRoot.GeneracionNotificaciones`. Los csproj son iguales salvo el nombre, el GUID y los `Compile`; las `App.config` solo difieren en `GeminiModel` (la de notificaciones) y los `secrets.config.example` en la clave.
- `CompositionRoot.Inicializar()` construye todo el grafo de BLL en el primer acceso a cualquier propiedad, asi que ambos hosts ya inicializan lo mismo; unificar no agrega costo de arranque.
- Codigos de salida: `ActualizadorTasas` usa 0/1/2 con `CodigoDeSalida`; `GeneradorNotificaciones` usa 0/1/2 con `EjecucionGenerador` (testeado en `tests/BLL.Tests/EjecucionGeneradorTests.cs` por link del `.cs`). Cancelacion: el generador usa `Console.CancelKeyPress`; el actualizador usa `CancellationToken.None`.
- Los nombres de ambos proyectos aparecen en `TpIDS.sln`, `Directory.Build.targets`, `.gitignore`, `verify-architecture-dependencies.ps1`, `dependency-matrix.md`, `README.md` y `tests/BLL.Tests.csproj`. Las capas, la restriccion de laboratorio (sin NuGet) y el CompositionRoot como unico lugar de construccion de BLL se mantienen (AGENTS.md).
- Ver proposal.md - Why.

## Goals / Non-Goals

**Goals:**
- Un unico proyecto `CRONS` con un `Program` que despacha por argumento y una clase por tarea.
- Cero cambios de comportamiento de cada tarea: mismos BLL, mismos resumenes, mismos codigos de salida.
- Agregar una tarea futura = una clase + una entrada en la tabla de despacho + un par de scripts.

**Non-Goals:**
- Un scheduler propio o un servicio de Windows residente: el Programador de tareas sigue siendo el disparador.
- Ejecutar varias tareas en una sola invocacion (`CRONS.exe tasas notificaciones`).
- Cambiar horarios, nombres de tarea, esquema de base de datos, BLL o specs de comportamiento de las tareas.
- Mover logica de negocio al host: `CRONS` solo despacha, informa y traduce a codigos de salida.

## Decisions

**D1. Nombre y forma del proyecto.** `CRONS` (pedido del usuario), exe .NET Framework 4.7.2 / C# 7.3 igual que los hosts actuales, `AssemblyName` y `RootNamespace` `CRONS`. Mismas 4 referencias de proyecto, mismos `Reference` de sistema y mismo manejo de `secrets.config` condicional. El GUID se reutiliza del proyecto con mas historia (`ActualizadorTasas`) solo si no complica la sln; si no, se genera uno nuevo.

**D2. Despacho por subcomando posicional, tabla en vez de cadena de `if`.** `Program.Main` resuelve `args[0]` (sin distinguir mayusculas) contra un diccionario `nombre -> Func<CancellationToken, Task<int>>` y ejecuta una sola tarea. Alternativas descartadas: (a) un exe que corre todas las tareas — acopla horarios distintos (cada hora vs. 07:00) y una falla de una afectaria a la otra; (b) un switch por flags (`--tasas`) — mas ruido sin ventaja, no hay combinacion de tareas; (c) una interfaz `ITareaProgramada` con descubrimiento por reflexion — sobreingenieria para dos tareas.

**D3. Una clase por tarea, con el codigo existente movido tal cual.** `Tareas/ActualizarTasas.cs` conserva `Informar` y `CodigoDeSalida` de `ActualizadorTasas/Program.cs`; `Tareas/GenerarNotificaciones.cs` envuelve la llamada con `EjecucionGenerador` (que se mueve sin cambios de logica a `CRONS/`). Esto mantiene los tests actuales validos: solo cambia el `using` y la ruta del `Compile Include` link de `tests/BLL.Tests`. El despachador se aisla en una clase con dependencias inyectables (tabla de tareas, `TextWriter`) para testearlo en `BLL.Tests` por link, igual que `EjecucionGenerador`.

**D4. Cancelacion uniforme.** El `Program` crea un `CancellationTokenSource` ligado a `Console.CancelKeyPress` (patron actual del generador) y lo pasa a ambas tareas. Cambio menor para `tasas`: hoy ignora Ctrl+C; pasa a cancelar de forma ordenada y terminar con 2, que es lo que su `catch (OperationCanceledException)` ya contempla.

**D5. Codigo de salida de uso invalido = 2.** Se reutiliza el codigo de error inesperado en vez de introducir uno nuevo: el Programador de tareas solo distingue "0 / distinto de 0" y el contrato 0/1/2 queda cerrado. Se informa por `Console.Error` y no se toca `CompositionRoot`, asi un typo en la tarea no abre la base.

**D6. Configuracion unica.** `CRONS/App.config` con la cadena `TpIDS` y `GeminiModel` (valor de la de notificaciones); `CRONS/secrets.config.example` con `OpenExchangeRatesApiKey` y `GeminiApiKey` (ambas vacias, con comentario de que cada tarea falla solo por su clave). `.gitignore` ignora `CRONS/secrets.config`. Una maquina con los `secrets.config` viejos debe fusionarlos manualmente en uno (paso de migracion).

**D7. Scripts parametrizados por tarea.** `CRONS/scripts/registrar-tarea.ps1 -Tarea tasas|notificaciones` y `eliminar-tarea.ps1 -Tarea ...`, con `ValidateSet`, que conservan exactamente las reglas actuales de cada tarea (schtasks horario para tasas; `Register-ScheduledTask` con disparadores 07:00 + inicio de sesion, `StartWhenAvailable`, `IgnoreNew` y limite de 4 h para notificaciones) y los nombres `FINAZAPP-ActualizadorTasas` / `FINAZAPP-GeneradorNotificaciones`. Se mantienen esos nombres para que `eliminar-tarea` nuevo limpie tambien tareas registradas con los scripts viejos. Alternativa descartada: un solo script que registre ambas — dificulta registrar una sin la otra (por ejemplo, sin `GeminiApiKey` en una maquina de prueba).

**D8. Guard de arquitectura.** `verify-architecture-dependencies.ps1` y `dependency-matrix.md` reemplazan las dos entradas por `CRONS -> ABSTRACCIONES, BE, BLL, SERVICIOS`; `Directory.Build.targets` reemplaza ambas condiciones por `CRONS`. La matriz no cambia en sustancia.

**D9. Orden respecto de `notificaciones-automaticas-diarias`.** Ese cambio (22/27) tiene tareas abiertas que citan `GeneradorNotificaciones.exe` (por ejemplo la corrida disparada por el Programador). Este cambio se aplica despues de archivarlo; al aplicarlo se actualizan las referencias vivas (`actualizar-diagramas-tp` 2.6, 2.12, 3.5, 3.12). Los artefactos archivados no se tocan: son historia.

## Risks / Trade-offs

- [Tareas programadas existentes quedan apuntando a un `.exe` borrado y fallan en silencio] → Paso de migracion explicito en tasks.md: `eliminar-tarea` con los scripts viejos *antes* de borrar los proyectos, y `registrar-tarea` nuevo despues; verificar con `Get-ScheduledTaskInfo`.
- [Un error de arranque en `CRONS` afecta a ambas tareas] → El costo ya existia (ambas inicializan el mismo `CompositionRoot`); el despacho es trivial y esta cubierto por tests unitarios.
- [`secrets.config` viejos no se migran solos] → Se documenta en el README y en el paso de migracion; `secrets.config.example` lista ambas claves.
- [Cancelar `tasas` con Ctrl+C ahora devuelve 2] → Es el comportamiento que ya declaraba su `catch`; sin impacto para el Programador de tareas, que no envia Ctrl+C.
- [Menos separacion para publicar/desplegar una tarea sola] → Aceptado: el despliegue es local (maquina del usuario) y los scripts siguen registrando cada tarea de forma independiente.

## Migration Plan

1. En cada maquina con tareas registradas: ejecutar los `eliminar-tarea.ps1` actuales de ambos proyectos.
2. Aplicar el cambio (crear `CRONS`, mover codigo, ajustar sln/guards/tests/docs, borrar los proyectos viejos).
3. Fusionar los `secrets.config` viejos en `CRONS/secrets.config`.
4. Compilar `TpIDS.sln` y registrar las tareas con `CRONS/scripts/registrar-tarea.ps1 -Tarea tasas` y `-Tarea notificaciones`.
5. Rollback: revertir el commit del cambio, volver a registrar con los scripts viejos. No hay cambios de datos.
