---
name: translation-base-seed-sync
description: Sincroniza el catalogo base de traducciones de WinForms con el bloque auto-generado de `script_bd.sql`. Usar cuando se agregan, renombran o eliminan textos traducibles de UI, tags de idioma, encabezados de grillas o nuevas keys `Form...`.
---

# Translation Base Seed Sync

Aplicar esta skill cuando se modifique UI traducible o cuando se toque el sistema de idiomas.

## Objetivo

Mantener alineados estos artefactos:

- bloque auto-generado de `script_bd.sql`
- keys `Form...` usadas desde formularios WinForms

## Workflow

1. Ejecutar `scripts/sync-i18n-base-seed.ps1` desde la raiz del repo.
2. Revisar si el script agrego nuevas keys con valor placeholder igual a la key.
3. Completar esos valores directamente en la UI o, si no son inferibles por codigo, ajustar el bloque SQL regenerado.
4. Compilar `TpIDS.sln`.

## Reglas

- `script_bd.sql` no debe editarse a mano dentro del bloque auto-generado.
- Si una key nueva aparece en UI, debe terminar en el bloque auto-generado de `script_bd.sql` en el mismo cambio.
- Las keys base deben seguir persistidas en BD con `Palabra.EsBase = 1`.
