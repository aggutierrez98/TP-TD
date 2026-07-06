# Comandos del gate de tests

## Comandos

Gate por defecto al cierre de un desarrollo (unit, no requiere BD):

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tests\tools\run-tests.ps1
```

Suite UI completa (solo tras avisar al usuario y recibir confirmacion; toma el escritorio unos 5 minutos):

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tests\tools\run-tests.ps1 -IncludeUi
```

Corrida UI filtrada (solo a pedido explicito del usuario, advirtiendo el riesgo; implica -IncludeUi):

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tests\tools\run-tests.ps1 -UiFilter T04
powershell -NoProfile -ExecutionPolicy Bypass -File tests\tools\run-tests.ps1 -UiFilter T04,T05
powershell -NoProfile -ExecutionPolicy Bypass -File tests\tools\run-tests.ps1 -UiFilter T04_CU01
```

## Requisitos de entorno

- MSBuild de VS 2022 o superior (el runner lo resuelve con fallback de rutas conocidas).
- VSTest (`vstest.console.exe`, resuelto por el runner).
- NuGet o cache local de paquetes para restaurar `tests/TpIDS.Tests.sln`.
- Para la suite UI: SQL Server local (default `.\SQLEXPRESS`, configurable con `-SqlServer`, autenticacion integrada) y `sqlcmd` en PATH.
- Para UI: sesion interactiva desbloqueada y escritorio libre durante toda la corrida.

## Exit codes y guards

- Exit 0 = todo verde. Cualquier otro valor = fallo de build, de tests o de validacion.
- Guard de "0 tests ejecutados": si una suite ejecuta 0 tests (por ejemplo un `-UiFilter` con typo), el runner falla en vez de reportar verde vacio.
- Validacion TRX: el runner lee los TRX y exige al menos 1 test ejecutado y 0 fallos por suite.

## Guard de BD de test

`tests/tools/prepare-test-database.ps1` exige que el nombre de BD empiece con `TpCatedraIDS_Test` y rechaza la BD productiva `TpCatedraIDS`. Nunca apuntar el runner a la BD productiva con `-DatabaseName`.

## Resultados

- TRX de cada corrida: `TestResults/*.trx`.
- Capturas automaticas de fallos UI: `TestResults/ui-screenshots/`.
