# Design

## Context

El bloque `-- BEGIN/END AUTO-GENERATED I18N BASE SEED` de `script_bd.sql` lo emite `sync-i18n-base-seed.ps1`. Hoy ejecuta un helper (`!!`) que convierte los JSON UTF-8 a UTF-16LE en `C:\Users\Public\Documents\TpIDS\SqlSeed\` y los lee con `OPENROWSET(BULK ..., SINGLE_NCLOB)`. Ver proposal.md para el motivo.

Verificado en SQL Server 2025 (Modern_Spanish_CI_AS) sobre `base.in.json`, que tiene BOM y los caracteres `→` y `−`:

- `OPENROWSET ... SINGLE_CLOB, CODEPAGE='65001'` devuelve `VARCHAR`: pierde `→` y `−`.
- `BULK INSERT` a una tabla `NVARCHAR(MAX)` con `CODEPAGE='65001'` conserva ambos, ignora el BOM y `OPENJSON` parsea las 1387 claves.

## Goals / Non-Goals

**Goals:**
- Leer los JSON originales de `translation-seeds/`, sin copias ni helpers.
- Cero dependencia de `.agents/`, PowerShell o `C:\Users\Public` para crear la base.

**Non-Goals:**
- No cambia el formato de los JSON ni el esquema `Palabra`/`Traduccion`.
- No cambia el comportamiento de `:setvar RepoRoot` (sigue siendo el unico parametro).
- No se embebe el JSON dentro del `.sql`.

## Decisions

1. **`BULK INSERT` con `CODEPAGE='65001'` hacia `#SeedJson (Contenido NVARCHAR(MAX))`**, un archivo completo como un unico campo y una unica fila: `FIELDTERMINATOR` y `ROWTERMINATOR` con una cadena que no aparece en los JSON (`'|~|~|'`). Se lee `base.es.json` y `base.in.json` con la misma estructura; `OPENJSON` recibe el contenido sin mas conversion.
   - Alternativas descartadas: `SINGLE_CLOB`+65001 (pierde caracteres, probado); copia UTF-16 por helper (fragil, es el bug); `SINGLE_BLOB` + decodificacion manual (complejo); embeber JSON como literal ASCII con `\uXXXX` (cambia el generador de fondo y deja la fuente de verdad duplicada).
2. **La ruta usa la variable SQLCMD `$(RepoRoot)` directamente en el literal de `BULK INSERT`** (`'$(RepoRoot)\translation-seeds\base.es.json'`): SQLCMD la sustituye antes de enviar el lote, por lo que no hace falta SQL dinamico. `setup-database.ps1` y `prepare-test-database.ps1` siguen reemplazando `:setvar RepoRoot "."` por la ruta absoluta.
3. **Se mantienen los codigos de error 50000-50005 y sus mensajes**, con `BEGIN TRY`/`CATCH` alrededor de cada `BULK INSERT`, de modo que los chequeos existentes (vacio, sin traducciones, claves incompatibles) no cambian.
4. **Se elimina `emit-seed-json-sqlcmd-path.ps1`** junto con las constantes de `C:\Users\Public` del generador, y la advertencia de `sqlcmd` ODBC 15.x por la linea `!!` en README y skill.

## Risks / Trade-offs

- [`BULK INSERT` con 65001 en el servidor del laboratorio no verificado; documentado desde SQL Server 2016] → el usuario lo comprueba en el laboratorio; si falla, el error 50000 nombra el archivo, y la alternativa es `SINGLE_BLOB` con decodificacion.
- [La cuenta del servicio SQL no puede leer la carpeta del repo (p. ej. un perfil de usuario)] → mismo requisito que hoy para la copia; se documenta en el README: poner el repo en una ruta legible por el servicio.
- [Una ruta con comilla simple rompe el literal] → se documenta; el TP no usa rutas asi.
- [`RepoRoot` relativo (`.`) se resolveria contra el servidor] → los wrappers siempre lo reemplazan por la ruta absoluta; el README lo indica para SSMS.
