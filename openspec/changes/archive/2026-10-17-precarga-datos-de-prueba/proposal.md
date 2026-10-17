# Proposal

## Why

`script_bd.sql` crea el esquema y los catálogos mínimos, pero el sistema arranca vacío: sin comercios, productos, promociones ni movimientos no se puede demostrar ni probar nada de FINAZAPP. Se investigó un conjunto de datos reales del mercado argentino (Etapas 1 a 7) y hace falta cargarlo de forma reproducible para que la base nueva arranque lista para la demo.

## What Changes

- Nuevo script `script_datos_prueba.sql`, que se ejecuta **después** de `script_bd.sql` y carga el dataset de demo. `script_bd.sql` no cambia.
- Catálogos globales: rubros, categorías de producto, categorías globales de movimiento adicionales, entidades financieras, medios de pago y su relación con las entidades, comercios (con coordenadas cuando se puedan geocodificar), productos, precios por comercio y promociones con todos sus filtros.
- Datos del usuario de prueba `user` (ya sembrado por `script_bd.sql`): cuentas financieras de distintos bancos y billeteras con sus medios de pago, categorías propias, movimientos de ejemplo y reglas de categorización.
- Las fuentes del dataset (JSON) y el generador del script se versionan en el repositorio para poder regenerarlo.
- El script es idempotente: se puede correr más de una vez sin duplicar filas.

## Capabilities

### New Capabilities
- `datos-de-prueba`: carga reproducible e idempotente del dataset de demostración (catálogos globales y datos del usuario de prueba) sobre una base creada con `script_bd.sql`.

### Modified Capabilities

## Impact

- Archivos nuevos en la raíz: `script_datos_prueba.sql` y una carpeta `datos-de-prueba/` con las fuentes y el generador.
- Sin cambios de código en las capas, ni de esquema, ni de `script_bd.sql`, ni dependencias NuGet.
- No debe ejecutarse sobre la base de tests (`TpCatedraIDS_Test`): algunos tests de integración parten de catálogos vacíos.
