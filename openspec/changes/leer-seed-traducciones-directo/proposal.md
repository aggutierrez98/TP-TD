# Proposal

## Why

`script_bd.sql` no carga las traducciones leyendo `translation-seeds/` sino una copia UTF-16 que un helper PowerShell (`emit-seed-json-sqlcmd-path.ps1`, dentro de `.agents/`) deja en `C:\Users\Public\Documents\TpIDS\SqlSeed\`. En el laboratorio, con solo el codigo del TP (sin `.agents/`), el helper no existe, la copia no se genera y la carga falla; al apuntar a mano el script al JSON original (UTF-8) con `SINGLE_NCLOB` aparece `JSON text is not properly formatted. Unexpected character 'ൻ' is found at position 0`. La copia intermedia es fragil y rompe la restriccion de poder correr el TP desde su codigo fuente en el laboratorio.

## What Changes

- `script_bd.sql` lee `translation-seeds\base.es.json` y `translation-seeds\base.in.json` directamente desde el repo (`$(RepoRoot)`), con `BULK INSERT ... CODEPAGE = '65001'` hacia una tabla temporal `NVARCHAR(MAX)`; sin copias, sin helper, sin `C:\Users\Public`.
- Se elimina la linea `!!` y la dependencia de PowerShell y de `.agents/` para crear la base.
- Se elimina `emit-seed-json-sqlcmd-path.ps1` y sus invocaciones en `setup-database.ps1` y `tests/tools/prepare-test-database.ps1`.
- El generador `sync-i18n-base-seed.ps1` emite el nuevo bloque (rutas bajo `$(RepoRoot)`, sin constantes de `Public`).
- Se actualizan la skill `translation-base-seed-sync` y el README (la advertencia de `sqlcmd` ODBC 15.x por la linea `!!` deja de aplicar).

## Capabilities

### New Capabilities
- `seed-traducciones-base-datos`: como `script_bd.sql` carga el catalogo de traducciones (idioma base y ingles) desde `translation-seeds/` conservando Unicode, sin artefactos intermedios.

### Modified Capabilities

## Impact

- `script_bd.sql` (bloque autogenerado), `.agents/skills/translation-base-seed-sync/` (generador, `SKILL.md`, helper eliminado), `setup-database.ps1`, `tests/tools/prepare-test-database.ps1`, `README.md`.
- Sin cambios en C#, en tests de la solucion ni en el esquema de la base.
- Requiere SQL Server 2016 o superior y el mismo permiso de lectura de archivos que ya exige el script (`sysadmin`, o `dbcreator` y `bulkadmin`); la cuenta del servicio SQL debe poder leer la carpeta del repo.
