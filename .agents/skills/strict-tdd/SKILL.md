---
name: strict-tdd
description: Enforce a strict red-green TDD cycle at the unit level for behavior changes in SERVICIOS. Use when adding or modifying business rules, services, validations, or any observable pure logic so the failing test is written and observed BEFORE the implementation. FlaUI UI tests are written after development, never as part of the red-green cycle.
---

# Strict TDD

Aplicar esta skill ante cualquier cambio de comportamiento observable en la logica pura de `SERVICIOS` (nuevas reglas, validaciones, correccion de bugs de logica). El ciclo es red-green: primero el test en rojo, despues la implementacion en verde.

## Scope

Aplica a:

- Cambios de comportamiento en `SERVICIOS` (logica pura: hash, DVH/DVV, permisos, idioma, sesion) via `tests/SERVICIOS.Tests` (unit).
- Bugfixes de logica en `SERVICIOS`: el test rojo debe reproducir el bug antes de arreglarlo.

No aplica a:

- Cambios en `BLL` o `DAL`: en esta rama las BLL construyen sus dependencias con `new` directo (arquitectura simple, sin inyeccion), por lo que no son unit-testeables; su cobertura llega end-to-end via los tests UI FlaUI. No introducir interfaces ni inyeccion de dependencias solo para testear: la simplicidad de la arquitectura prevalece.
- Cambios puramente visuales o de layout WinForms, `.Designer.cs`, `Resources`.
- Documentacion, diagramas, scripts.
- Refactors sin cambio de comportamiento: los tests existentes deben quedar verdes, pero no se exige test nuevo.

Los tests UI FlaUI (`tests/TpIDS.UiTests`) se escriben o actualizan despues del desarrollo, nunca como sustituto del ciclo unit/integracion.

## Required workflow

0. **Baseline verde**: correr la suite unit ANTES de escribir el test nuevo (`run-tests.ps1` sin flags). Si la baseline ya esta roja, resolver eso primero: no se puede afirmar que "falla solo el test nuevo" sobre una suite rota.
1. Identificar el comportamiento a cambiar y la clase de test correcta dentro de `tests/SERVICIOS.Tests`.
2. Escribir el test PRIMERO. Convencion unit: clase `NombreClaseTests`, metodo `Metodo_Escenario_Resultado` (sin prefijo `T0X`; ese prefijo es exclusivo de los tests UI).
3. **Rojo**: correr el runner (ver [references/test-commands.md](references/test-commands.md)) y verificar que falla SOLO el test nuevo, idealmente con falla de assertion. Un error de compilacion solo cuenta como rojo si el test expresa deliberadamente una API o seam que todavia no existe; en ese caso el runner corta en el build y no deja TRX: anotarlo como evidencia y implementar el seam minimo hasta obtener el rojo de assertion. Si el test nace verde, detenerse: un test que nunca fallo no prueba nada (revisar el test o el supuesto).
4. Implementar el cambio minimo para pasar el test.
5. **Verde**: volver a correr el runner y verificar exit code 0.
6. Refactorizar solo con la suite en verde.
7. Al cerrar el desarrollo, aplicar la skill `test-suite-verification` (gate completo) y, si el cambio afecta flujos end-to-end, actualizar o crear los tests UI (`T0X_CUYY_Escenario`, trazables a `diagramas/T0X-*/`).

## Test run commands

El comando canonico del ciclo es el runner completo: `tests/tools/run-tests.ps1` sin flags (~1s de ejecucion; el tiempo lo domina el build de `tests/TpIDS.Tests.sln`, que hay que hacer igual). El runner tiene el guard de "0 tests ejecutados" que una invocacion manual de vstest no tiene.

El `vstest.console.exe` directo con `/TestCaseFilter` queda documentado en [references/test-commands.md](references/test-commands.md) solo como opcional para iterar sobre un unico test durante un rojo largo. No reemplaza la corrida completa antes del verde final.

## Forbidden patterns

- Implementar antes de escribir el test.
- Declarar el rojo sin haber corrido el runner (fallo supuesto, no observado).
- Escribir el test despues de implementar y presentarlo como TDD.
- Ablandar o borrar un test para forzar el verde.
- Usar tests UI FlaUI como sustituto del ciclo unit/integracion.
- Saltear el rojo porque "el cambio es trivial".
- Escribir el test nuevo sobre una baseline roja.

## Related

- [`../csharp-mstest/SKILL.md`](../csharp-mstest/SKILL.md): framework de assertions usado por los proyectos de test.
- [`../test-suite-verification/SKILL.md`](../test-suite-verification/SKILL.md): gate de cierre de tarea; esta skill es el ciclo durante el desarrollo.
- `tests/README.md`: mapa de proyectos de test y convenciones de nomenclatura.

## Response rule

- Informar el ciclo explicitamente: que test se escribio, la evidencia del rojo (tipo de fallo observado: assertion o build por seam deliberado) y la evidencia del verde (corrida posterior con exit 0).
- Si la skill no aplico (cambio visual, doc, refactor puro), decirlo explicitamente en la respuesta.
