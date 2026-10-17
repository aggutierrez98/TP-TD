# Proposal

## Why

> **Estado: propuesta pendiente de validacion por el usuario.** No tomar para implementar hasta validar el alcance y las preguntas abiertas de `design.md` (tarea 0.1 de `tasks.md`).

Las nueve listas de filtros de `FormGestionPromociones` (comercios, rubros, entidades, medios de pago, segmentos, productos incluidos y excluidos, categorias incluidas y excluidas) son `CheckedListBox` nativos de unos 290 x 110 px, con unos 8 items visibles. Hoy los catalogos son chicos y alcanza, pero escalan mal en dos casos:

- **Nombres largos**: el control tiene scroll vertical automatico (se verifico: con 5 items no aparece, con 15 si), pero no tiene scroll horizontal. Un nombre mas ancho que el recuadro se corta y no hay forma de leerlo completo.
- **Catalogos grandes**: con cientos de productos, comercios o categorias, encontrar uno obliga a recorrer la lista con la rueda. No hay buscador, y tampoco se ve cuantos items hay marcados.

Se deja planteado ahora, sin urgencia, para tomarlo cuando el catalogo real crezca o el usuario lo priorice. Depende de que `gestion-promociones` este archivado: la pantalla y sus listas viven en ese change.

## What Changes

- Los nombres largos de las listas se pueden leer completos: scroll horizontal, tooltip con el nombre completo, o ambos (decision abierta en `design.md`).
- Cada lista que puede crecer suma una casilla de busqueda que filtra los items visibles por texto, sin distinguir mayusculas ni acentos. Los items marcados se conservan aunque el filtro los oculte.
- Cada lista muestra un contador de items marcados.
- La ayuda gris de lista vacia sigue funcionando: se distingue "el catalogo no tiene items" de "el filtro no encontro nada".
- Sin cambios de BLL, REPOSITORIO, DAL ni esquema: es solo comportamiento de pantalla.

## Capabilities

### New Capabilities
- `listas-de-seleccion-escalables`: comportamiento de las listas de seleccion multiple de los filtros de promociones frente a nombres largos y catalogos grandes (lectura del nombre completo, busqueda por texto que conserva lo marcado, contador de marcados y mensajes de lista vacia o sin resultados).

### Modified Capabilities
<!-- Ninguna: la capacidad `gestion-promociones` todavia no esta en openspec/specs (su change esta pendiente de archivar). Si se tomara este change despues del archivo, evaluar si conviene mover estos requisitos como MODIFIED de esa capacidad. -->

## Impact

- **UI**: `FormGestionPromociones` y su Designer (casillas de busqueda, contadores, tooltip o scroll horizontal). No cambian los nombres de controles existentes.
- **Traducciones**: keys nuevas (placeholder del buscador, contador, mensaje de sin resultados) en `translation-seeds/base.es.json` y `base.in.json` y en `Palabra`/`Traduccion` de las dos bases.
- **Tests**: tests UI FlaUI con listas grandes insertadas por SQL en `FIN_PromocionesTests` o una clase nueva.
- **Fuera de alcance**: cambiar el modelo de filtros de promociones, paginacion desde la base, y reemplazar el `CheckedListBox` por un control de terceros (ver `design.md`).
