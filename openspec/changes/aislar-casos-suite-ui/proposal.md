# Proposal

## Why

La corrida UI del 2026-10-20 descubrio 140 casos, pero uno quedo inconcluso porque encontro categorias creadas por otros casos. Ese mismo caso paso al ejecutarlo solo con la base regenerada: la suite necesita aislamiento de estado por caso para ofrecer resultados independientes del orden.

## What Changes

- Establecer un estado de base conocido por caso UI y liberar sus recursos aunque falle; cubrir tambien configuracion, archivos y estado de aplicacion que afecten otros casos.
- Adaptar las precondiciones de los tests existentes, incluido el catalogo vacio, sin depender de ejecuciones previas ni instrucciones de correr clases solas.
- Endurecer la validacion TRX del runner para rechazar casos seleccionados no aprobados, incluidos inconclusos y no ejecutados, aunque VSTest devuelva cero.
- Verificar ejecucion individual, repetida y en orden alternativo, y medir el costo del aislamiento.

## Capabilities

### New Capabilities

- `aislamiento-tests-ui`: ciclo de vida aislado de los casos FlaUI y reporte verificable de su ejecucion completa.

### Modified Capabilities

Ninguna. `compatibilidad-tests-ui` regula automatizabilidad de controles; este cambio regula el harness y el estado de cada prueba.

## Impact

`tests/TpIDS.UiTests/Infrastructure`, fixtures UI, `tests/tools/run-tests.ps1`, preparacion de base de pruebas y `tests/README.md`. Solo tooling de desarrollo: conserva .NET Framework 4.7.2, separacion de la solucion productiva y ausencia de NuGet en produccion. No cambia reglas de negocio ni esquema productivo. Planificacion para implementacion futura.
