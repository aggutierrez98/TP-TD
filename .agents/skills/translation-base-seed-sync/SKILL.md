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

1. Ejecutar `scripts/sync-i18n-base-seed.ps1` desde la raiz del repo.
2. Revisar si el script agrego nuevas keys con valor placeholder igual a la key en `translation-seeds/base.es.json`.
3. Completar o corregir esos valores en `translation-seeds/base.es.json`.
4. Compilar `TpIDS.sln`.

## Reglas

- `translation-seeds/base.es.json` es la fuente de verdad del idioma base; `script_bd.sql` se regenera desde ese archivo.
- `script_bd.sql` no debe editarse a mano dentro del bloque auto-generado.
- Si una key nueva aparece en UI/BLL/SERVICIOS, debe terminar en `translation-seeds/base.es.json` y en el bloque auto-generado de `script_bd.sql` en el mismo cambio.
- Las keys base deben seguir persistidas en BD con `Palabra.EsBase = 1`.
