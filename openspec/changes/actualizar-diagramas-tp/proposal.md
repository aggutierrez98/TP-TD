# Proposal

## Why

Los diagramas del TP (`diagramas/`) y el Word de la catedra quedaron atrasados respecto del codigo. Desde la revision del 27/08/2026 se sumaron modulos (entidades financieras, medios de pago, reglas de categorizacion, comercios, promociones, monedas, notificaciones), se desacoplo `Producto` de `Comercio` y se renumeraron carpetas (G06/G07 -> G07/G08, T06a+T06b -> T06). Los pendientes se anotaban en `diagramas/TODO-actualizacion-diagramas.md`, fuera de OpenSpec. Este change los reemplaza y pasa a ser el unico lugar donde se registran.

## What Changes

- Se actualizan G07 (clases por capa) y G08 (DER integrado) contra el codigo y la base actuales, incluidos los cambios ya identificados por `desacoplar-producto-de-comercio` y `gestion-promociones`.
- Se actualiza G00 (modelo de dominio): la relacion Comercio-Producto pasa a N:M con precio y moneda.
- Se verifican los DER contra la base SQL Server real, no solo contra `script_bd.sql`.
- Se decide con el usuario si T02, T04, T05 y T06 llevan secuencia general.
- Se sincroniza el Word de la catedra: imagenes regeneradas y numeracion nueva.
- Se elimina `diagramas/TODO-actualizacion-diagramas.md`.

## Capabilities

### New Capabilities

### Modified Capabilities

(Ninguna: el change solo toca documentacion y diagramas, sin requisitos de comportamiento. Declara `skip_specs: true`.)

## Impact

- `diagramas/G00-*`, `G07-*`, `G08-*` y, segun la decision del usuario, `T02`, `T04`, `T05`, `T06`.
- Documento externo `C:\Users\agust\Desktop\Trabajo Final TD Gutierrez.docx` (fuera del repo).
- Sin cambios de codigo.
