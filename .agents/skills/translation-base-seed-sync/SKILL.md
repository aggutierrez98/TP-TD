---
name: translation-base-seed-sync
description: Sincroniza el catalogo base de traducciones de WinForms con el bloque auto-generado de `script_bd.sql`. Usar cuando se agregan, renombran o eliminan textos traducibles de UI, tags de idioma, encabezados de grillas o nuevas keys `Form...`.
---

# Translation Base Seed Sync

Aplicar esta skill cuando se modifique UI traducible o cuando se toque el sistema de idiomas.

## Objetivo

Mantener alineados estos artefactos:

- `translation-seeds/base.es.json` como fuente de verdad del idioma base
- bloque auto-generado de `script_bd.sql` derivado de ese JSON
- keys `Form...`, `Dialog...` y `Error...` usadas desde UI/BLL/SERVICIOS

## Workflow

1. Ejecutar `.agents/skills/translation-base-seed-sync/scripts/sync-i18n-base-seed.ps1` desde la raiz del repo.
2. Revisar si el script agrego nuevas keys con valor placeholder igual a la key en `translation-seeds/base.es.json`.
3. Completar o corregir esos valores en `translation-seeds/base.es.json`.
4. Compilar `TpIDS.sln`.

## Reglas

- `translation-seeds/base.es.json` es la fuente de verdad del idioma base; el bloque auto-generado de `script_bd.sql` solo debe apuntar a ese archivo via `OPENROWSET`, sin hardcodear el JSON completo.
- `script_bd.sql` no debe editarse a mano dentro del bloque auto-generado.
- Si una key nueva aparece en UI/BLL/SERVICIOS, debe terminar en `translation-seeds/base.es.json` en el mismo cambio. El SQL generado solo debe cambiar si cambia la estructura del bloque o la ruta resuelta al archivo.
- Para que el script sea portable entre clones, `script_bd.sql` se ejecuta desde SSMS 19/22 en SQLCMD Mode (ajustando `RepoRoot`) o con `setup-database.ps1` desde la raiz del repo. NO ejecutarlo directo con `sqlcmd` ODBC 15.x: esa version corta el procesamiento en silencio (exit 0) al encontrar la linea `!!` y la BD queda sin traducciones; el wrapper lo evita ejecutando el helper desde PowerShell y quitando la linea `!!` de una copia temporal.
- El script SQL de base debe asumir recreacion completa de la BD. No debe intentar completar esquemas existentes ni agregar constraints faltantes en caliente.
- Las keys del seed se persisten en `Palabra` (no hay flag `EsBase`: todas las keys son del sistema y solo entran por el seed, nunca desde la UI).
- **Todos los tipos de key pertenecen a `base.es.json`**, sin excepcion: `Dialog.*`, `Error.*`, `Form*.Warning.*`, `Form*.Confirm.*`, `Form*.Success.*`, `Form*.Grid.*`, labels, botones, etc. No hay subtipo de key que quede solo en `UiDialogHelper.TextosBase`; ese diccionario es unicamente el fallback en caso de que el idioma no este cargado desde BD.
