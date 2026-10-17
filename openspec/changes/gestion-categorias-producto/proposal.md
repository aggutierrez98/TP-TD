# Proposal

## Why

Las categorias de producto no tienen pantalla propia: su alta, edicion y baja quedaron dentro del detalle del comercio, un resto del acople viejo entre producto y comercio que `desacoplar-producto-de-comercio` ya retiro para los productos. La categoria es un catalogo global (no pertenece a ningun comercio), y hoy el operador solo la encuentra si abre el detalle de un comercio. Ademas la pantalla Productos exige elegir una categoria y, sin categorias cargadas, no dice donde darlas de alta.

## What Changes

- Nueva pantalla `FormGestionCategoriasProducto` (Krypton, MDI unico) con el ABM completo de categorias de producto: alta, edicion del nombre y baja bloqueada por uso (productos o promociones) con el mensaje que ya existe.
- Nuevo submenu `mnuOperadorCategoriasProducto` dentro de Operador, junto a Productos, habilitado con el permiso `gestionproductos` ya existente (sin permiso ni migracion nuevos).
- **BREAKING (UI)**: el ABM de categorias se retira del detalle del comercio. El detalle queda solo con productos asignados y promociones que le aplican.
- `CategoriaProductoBLL` rechaza el nombre duplicado con un mensaje de negocio propio, antes de insertar o actualizar. Hoy el `UNIQUE` de la base lo frena y el usuario ve "no se pudo crear" generico.
- La pantalla Productos, cuando no hay ninguna categoria, orienta al usuario a la pantalla nueva en lugar de dejar el combo vacio sin explicacion.
- Las keys de traduccion del bloque de categorias del detalle (`FormMerchantDetail.*Categoria*`) pasan a la pantalla nueva con prefijo propio, y las obsoletas se retiran de los seeds y de las dos bases.

## Capabilities

### New Capabilities
- `catalogo-categorias-producto`: administracion del catalogo de categorias de producto desde su propia pantalla (alta, edicion, baja bloqueada por uso, nombre unico), el acceso por menu y permiso, y que el detalle del comercio ya no administra categorias.

### Modified Capabilities
- `catalogo-productos`: se suma el requisito de orientar al usuario cuando no hay categorias de producto cargadas al dar de alta un producto.

## Impact

- **UI**: `FormGestionCategoriasProducto` (nuevo, registrado en `UI.csproj`), `FormPrincipal` (menu y permiso), `FormDetalleComercio` (se retira el grupo Categorias y su codigo), `FormGestionProductos` (orientacion sin categorias).
- **BLL**: `CategoriaProductoBLL` (validacion de nombre duplicado y claves de bitacora con prefijo propio). REPOSITORIO y DAL no cambian: `ObtenerTodas` alcanza para validar duplicados.
- **Traducciones**: `translation-seeds/base.es.json` y `base.in.json` y `Palabra`/`Traduccion` de `TpCatedraIDS` y `TpCatedraIDS_Test`.
- **Tests**: `CategoriaProductoBLLTests` (duplicado); UI FlaUI nuevos para la pantalla; migrar el caso de categoria de `FIN_BajasEnUsoTests`, que hoy usa el detalle del comercio.
- **Diagramas**: tarea en `openspec/changes/actualizar-diagramas-tp/tasks.md` (clases G07: nueva pantalla y validacion de nombres; sin cambios de DER).
- **Fuera de alcance**: permiso propio para categorias, jerarquia de categorias o subcategorias, mover productos entre categorias en lote, y cambios de esquema SQL.
