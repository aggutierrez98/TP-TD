---
name: test-suite-verification
description: Enforce end-of-task test suite verification for TpIDS. Use when closing any feature, bugfix, refactor, or test-harness change so the agent runs tests/tools/run-tests.ps1 -IncludeIntegration before handoff, reports explicit TRX-backed results, asks for user confirmation before running the desktop-locking FlaUI UI suite, and never uses -UiFilter without an explicit user request.
---

# Test Suite Verification

Aplicar esta skill al cierre de cualquier cambio sobre codigo productivo o de tests, antes de responder al usuario.

## Required workflow

1. Terminar la implementacion (con la skill `strict-tdd` si el cambio era de comportamiento en SERVICIOS/BLL/REPOSITORIO/DAL).
2. Correr el gate por defecto: `powershell -NoProfile -ExecutionPolicy Bypass -File tests\tools\run-tests.ps1 -IncludeIntegration` (unit + integracion; el script prepara solo la BD `TpCatedraIDS_Test`, nunca la productiva). Verde = exit code 0.
3. Compilar tambien `TpIDS.sln` segun `winforms-build-verification`. Los dos gates son obligatorios e independientes: el runner compila `tests/TpIDS.Tests.sln` (incluye los productivos pero usa NuGet), lo cual NO acredita que `TpIDS.sln` compile offline sin NuGet, que es la restriccion dura de portabilidad. Orden recomendado: primero el test gate (funcional), y el build de `TpIDS.sln` como ultimo paso antes de responder.
4. Si algo falla: corregir y volver a correr. Nunca cerrar la tarea con tests en rojo.

## UI suite protocol

Si el cambio toca flujos end-to-end o la UI, corresponde correr la suite FlaUI (`-IncludeUi`), pero SIEMPRE:

1. Avisar antes al usuario que la suite toma el control del escritorio unos 5 minutos y requiere sesion desbloqueada y SQL Server local.
2. Esperar confirmacion explicita.
3. Recien entonces ejecutarla.

Si el usuario no confirma o no esta disponible, cerrar informando explicitamente que la suite UI quedo pendiente y por que. Nunca lanzarla sin aviso.

## UiFilter protocol

`-UiFilter <prefijos>` (ej: `T04`, `T04,T05`, `T04_CU01`; sirve tambien para futuros casos de negocio `N0X`) solo se usa cuando el usuario lo requiere por velocidad:

- Nunca filtrar por decision propia del agente.
- Preguntar primero y esperar confirmacion explicita del usuario.
- Advertir el riesgo: una corrida filtrada no detecta regresiones cruzadas en los flujos no ejecutados.
- Si el filtro no matchea ningun test, el runner falla con exit 1: eso es correcto, no relajar el filtro en silencio para "arreglarlo".

## Environment limitations

El runner necesita MSBuild (VS 2022 o superior), VSTest y NuGet o su cache local para restaurar y compilar `tests/TpIDS.Tests.sln`. La integracion y la UI requieren ademas SQL Server local (default `.\SQLEXPRESS`, configurable con `-SqlServer`, autenticacion integrada) y `sqlcmd` en PATH.

Si SQL Server no esta disponible: correr al menos los unit tests (runner sin flags, no requieren BD) e informar explicitamente que integracion/UI no se pudieron ejecutar y por que. Nunca fingir exito ni reportar "deberia funcionar".

Comandos y requisitos completos en [references/test-gate-commands.md](references/test-gate-commands.md).

## Related

- [`../winforms-build-verification/SKILL.md`](../winforms-build-verification/SKILL.md): gate de portabilidad de `TpIDS.sln`; complementario, no reemplazado por esta skill.
- [`../strict-tdd/SKILL.md`](../strict-tdd/SKILL.md): ciclo durante el desarrollo; esta skill es el cierre.
- `tests/README.md`: mapa de la suite, datos seed y cobertura por requisito.

## Response rule

- Informar el comando exacto ejecutado, el exit code y los contadores reales de tests leidos del output o de `TestResults/*.trx`. No citar totales memorizados ni asumir verde: el runner ya valida que se ejecuto al menos 1 test y que no hubo fallos.
- Listar que suites quedaron sin correr y el motivo (UI pendiente de confirmacion, SQL Server no disponible, etc.).
- Ante un fallo de UI, mencionar que queda captura automatica en `TestResults/ui-screenshots/`.
