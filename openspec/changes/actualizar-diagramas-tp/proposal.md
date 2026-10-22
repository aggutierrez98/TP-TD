# Proposal

## Why

Los diagramas del TP (`diagramas/`) y el Word de la catedra quedaron atrasados respecto del codigo. Desde la revision del 27/08/2026 se sumaron modulos (entidades financieras, medios de pago, reglas de categorizacion, comercios, promociones, monedas, notificaciones), se desacoplo `Producto` de `Comercio` y se renumeraron carpetas (G06/G07 -> G07/G08, T06a+T06b -> T06). Los pendientes se anotaban en `diagramas/TODO-actualizacion-diagramas.md`, fuera de OpenSpec. Este change los reemplaza y pasa a ser el unico lugar donde se registran.

## What Changes

- Se incorpora la skill `publicar-diagramas-google-docs` aprobada por el usuario: preparacion local de PNG de cualquier tipo de PlantUML, carga manual a Drive y publicacion verificable en secciones tecnicas, de negocio u otras. Prioriza MCP nativos de Docs/Drive y usa One como fallback por operacion.
- Se actualizan G07 (clases por capa) y G08 (DER integrado) contra el codigo y la base actuales, incluidos los cambios ya identificados por `desacoplar-producto-de-comercio` y `gestion-promociones`.
- Se actualiza G00 (modelo de dominio): la relacion Comercio-Producto pasa a N:M con precio y moneda.
- Se verifican los DER contra la base SQL Server real, no solo contra `script_bd.sql`.
- Se decide con el usuario si T02, T04, T05 y T06 llevan secuencia general.
- Se sincroniza el Word de la catedra: imagenes regeneradas y numeracion nueva.
- Se elimina `diagramas/TODO-actualizacion-diagramas.md`.
- Se documentan los 87 casos de uso de negocio aprobados por el usuario (CUN01-CUN87), con un Word por cada uno de los 22 grupos y fuentes JSON/Markdown regenerables, siguiendo la muestra CUN01 aprobada.
- La entrega inicial elaboro 164 diagramas de negocio: una secuencia y un diagrama de clases por CUN06-CUN87, agrupados en siete carpetas por proceso N01.1-N01.7, con sus SVG y balanceo contra codigo y especificaciones existentes.
- Por correccion posterior aprobada por el usuario, la entrega de clases de negocio pasa a siete diagramas integrados, uno por N01.1-N01.7, con las capas diferenciadas y cobertura de todos los CU asignados a cada proceso. Se generan fuentes PlantUML, SVG y siete PNG de alta resolucion juntos en `diagramas/exportaciones/clases-negocio-png/`.

## Capabilities

### New Capabilities

### Modified Capabilities

(Ninguna: el change solo toca documentacion y diagramas, sin requisitos de comportamiento. Declara `skip_specs: true`.)

## Impact

- `diagramas/G00-*`, `G07-*`, `G08-*` y, segun la decision del usuario, `T02`, `T04`, `T05`, `T06`.
- Siete carpetas `diagramas/N01.*` con los pares de diagramas CUN06-CUN87 y sus SVG.
- Documento externo `C:\Users\agust\Desktop\Trabajo Final TD Gutierrez.docx` (fuera del repo).
- Sin cambios de codigo productivo; herramientas de desarrollo bajo `.agents/skills/publicar-diagramas-google-docs/`.
