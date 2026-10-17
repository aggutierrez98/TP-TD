# Proposal

## Why

Hoy `Producto` pertenece a un `Comercio` (columna `ComercioId`, propiedad `Producto.Comercio`) y lleva su propio `Precio` y `Moneda`. En la realidad un mismo producto lo venden varios comercios, cada uno con su precio. Ese acoplamiento impide el siguiente change (`gestion-promociones`), donde una promocion debe poder apuntar a un producto sin depender de un comercio, por lo que este desacople se hace primero.

## What Changes

- `Producto` pasa a ser un catalogo independiente: solo `Nombre` y `CategoriaProducto`. Pierde `Comercio`, `Moneda` y `Precio`.
- Nueva relacion `ComercioProducto`: indica que un comercio vende un producto del catalogo y guarda el `Precio` y la `Moneda` para ese comercio.
- Nueva pantalla **Productos** (ABM completo del catalogo) como submenu de Operador, con un permiso nuevo (`gestionproductos`) concedido a los grupos Operador y admin.
- `FormDetalleComercio` deja de crear productos: el operador elige un producto del catalogo, le pone precio y moneda, y lo asigna al comercio (tambien puede editar el precio y quitarlo del comercio).
- La baja de un producto se **bloquea** si algun comercio lo vende, con un chequeo explicito de uso en el BLL (sin depender de `ViolacionIntegridadReferencialHelper`, que se va a retirar a futuro).
- Migracion de datos: cada fila actual de `Producto` se convierte en un producto del catalogo mas su fila en `ComercioProducto` (1 a 1, sin deduplicar nombres).
- **BREAKING**: cambian el esquema SQL (`Producto` pierde columnas, aparece `ComercioProducto`), la API de `ProductoBLL`/`IProductoRepositorio`/`ProductoDAL` y la UI de `FormDetalleComercio`.

## Capabilities

### New Capabilities
- `catalogo-productos`: catalogo de productos independiente de los comercios, su ABM desde la pantalla Productos, el bloqueo de baja cuando esta en uso y el acceso por menu/permiso de Operador y admin.
- `productos-por-comercio`: asignacion de productos del catalogo a un comercio con precio y moneda propios, desde el detalle del comercio.

### Modified Capabilities
<!-- Ninguna: los specs vigentes (tema-visual, ventanas-redimensionables, etc.) no cambian de requisitos. -->

## Impact

- **BE**: `Producto` (se simplifica), nueva `ComercioProducto`.
- **DAL / REPOSITORIO**: `ProductoDAL`/`ProductoRepositorio` (consultas sin comercio, conteo de uso), nuevos `ComercioProductoDAL`/`ComercioProductoRepositorio`, `IUnidadDeTrabajo` y `UnidadDeTrabajoSqlServer` exponen `ComercioProductos`.
- **BLL**: `ProductoBLL` (catalogo), nuevo `ComercioProductoBLL` dado de alta en `CompositionRoot`.
- **UI**: nueva `FormGestionProductos`, cambios en `FormDetalleComercio` y en `FormPrincipal` (submenu y permiso).
- **BD**: `script_bd.sql` (esquema final, permiso `gestionproductos`, asignacion a Operador y admin) y migracion aplicada a `TpCatedraIDS` y `TpCatedraIDS_Test`.
- **Traducciones**: keys nuevas en `translation-seeds/base.es.json` (y `base.in.json`), sincronizadas en las dos bases.
- **Tests**: se reescriben `ProductoBLLTests`, `ProductoRepositorioIntegrationTests` y los fakes; se agregan los de `ComercioProducto`; se actualizan los tests UI FlaUI afectados despues del desarrollo.
- **Documentacion**: queda anotado como pendiente en `diagramas/TODO-actualizacion-diagramas.md` el cambio de clases (G07) y DER (G08).
- **Fuera de alcance**: las promociones (change `gestion-promociones`), mover el ABM de `CategoriaProducto` fuera del detalle del comercio, y deduplicar productos migrados.
