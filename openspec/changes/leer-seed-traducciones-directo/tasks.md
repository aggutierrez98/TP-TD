# Tasks

## 1. Generador y script

- [x] 1.1 Cambiar `sync-i18n-base-seed.ps1` para emitir el bloque con `BULK INSERT ... CODEPAGE='65001'` hacia una tabla temporal `NVARCHAR(MAX)` (es e ingles) desde `$(RepoRoot)\translation-seeds\`, sin linea `!!` ni constantes de `C:\Users\Public`, manteniendo los errores 50000-50005. Verificacion: el generador corre sin errores y `rg "SINGLE_NCLOB|Users.Public|\!\!" script_bd.sql` no devuelve nada.
- [x] 1.2 Regenerar el bloque en `script_bd.sql` y revisar el diff. Verificacion: el diff solo toca el bloque `AUTO-GENERATED I18N BASE SEED`.

## 2. Retiro del helper

- [x] 2.1 Quitar la invocacion del helper y su validacion de `setup-database.ps1` y de `tests/tools/prepare-test-database.ps1`; borrar `emit-seed-json-sqlcmd-path.ps1`. Verificacion: `rg --hidden -i "emit-seed-json|SqlSeed"` fuera de `openspec/changes/archive/` no devuelve nada.

## 3. Documentacion

- [x] 3.1 Actualizar `.agents/skills/translation-base-seed-sync/SKILL.md` y `README.md`: lectura directa, requisito de permiso de lectura del servicio SQL sobre el repo, fuera la advertencia de `sqlcmd` ODBC 15.x. Verificacion: ningun texto menciona el helper ni la linea `!!`.

## 4. Verificacion

- [x] 4.1 Ejecutar `tests/tools/prepare-test-database.ps1` (base de test) y comprobar `Palabra`/`Traduccion` con ambos idiomas, y que una traduccion con `→` o `−` quedo sin `?`. Verificacion: conteos y valor leidos de la base.
- [x] 4.2 Compilar `TpIDS.sln` (skill `winforms-build-verification`) y correr `tests/tools/run-tests.ps1 -IncludeIntegration` (skill `test-suite-verification`). Verificacion: exit code 0 y contadores reales del TRX.
- [ ] 4.3 El usuario ejecuta `script_bd.sql` en el laboratorio sobre una copia del repo sin `.agents/` y confirma que carga y que `@@VERSION` es 2016 o superior. Verificacion: `SELECT COUNT(*) FROM dbo.Traduccion` en el laboratorio.
