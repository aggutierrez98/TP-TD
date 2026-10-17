# Design

## Context

Ver `proposal.md` (Why). Estado actual observado en el codigo:

- `CategoriaProductoBLL` ya expone `ObtenerTodas`, `Crear`, `Actualizar` y `Eliminar`. `Eliminar` ya bloquea por uso (productos y `PromocionCategoriaProducto`) con `CategoriaProductoInUse`. Crear y Actualizar solo validan nombre obligatorio: el nombre repetido lo frena el `UNIQUE (Nombre)` de la base y llega al usuario como "no se pudo crear" generico.
- El ABM se usa unicamente desde `FormDetalleComercio` (grupo Categorias: textbox, grilla y tres botones). `FormGestionProductos` es el modelo reciente de pantalla de catalogo (Krypton, MDI unico, permiso `gestionproductos`, `mnuOperadorProductos`).
- Las keys de ese bloque tienen prefijo `FormMerchantDetail.` (pantalla, mensajes) y `FormMerchantDetail.Value.*` (bitacora, usadas por `CategoriaProductoBLL`).
- `FIN_BajasEnUsoTests` (change `retirar-helper-violacion-fk`) prueba la baja bloqueada de una categoria desde el detalle del comercio, que va a dejar de ser posible.

## Goals / Non-Goals

**Goals:**
- Que el ABM de categorias viva en una pantalla propia y facil de encontrar, con el mismo patron de `FormGestionProductos`.
- Que el nombre duplicado se informe con un mensaje de negocio.

**Non-Goals:**
- Permiso propio, jerarquia o subcategorias, y cambios de esquema.
- Reubicar otros bloques del detalle del comercio.

## Decisions

**1. Pantalla nueva con el patron de `FormGestionProductos`.** `FormGestionCategoriasProducto` hereda de `FormTraducibleBase`, abre como hijo MDI unico, tiene un textbox de nombre, grilla de categorias y botones Agregar, Actualizar y Eliminar. Alternativa descartada: dejar el ABM en el detalle y agregar un acceso directo, porque seguiria atado a un comercio que no le corresponde.

**2. Permiso `gestionproductos` reutilizado (decision mia, a confirmar).** Productos y categorias son el mismo catalogo y los mismos roles lo administran; no hay migracion ni sembrado. Alternativa descartada: permiso `gestioncategoriasproducto`, que obligaria a sembrarlo para Operador y admin en `script_bd.sql` y en las dos bases sin aportar una restriccion distinta.

**3. Validacion de nombre duplicado en el BLL (confirmada por el usuario: ignora solo mayusculas, no acentos).** `Crear` y `Actualizar` comparan el nombre normalizado contra `ObtenerTodas()` sin distinguir mayusculas (criterio de `EntidadFinancieraBLL.ValidarNombreUnico`) y lanzan `ExcepcionNegocio("Error.MerchantManagement.CategoriaProductoDuplicateName")`; al actualizar se excluye la propia categoria. No se agregan consultas a REPOSITORIO ni DAL: el volumen es chico. El `UNIQUE` de la base sigue como red de seguridad. Alternativa descartada: capturar el `SqlException` del `UNIQUE`, porque `retirar-helper-violacion-fk` fijo el criterio de preguntar de antemano y no interpretar errores de la base.

**4. Orientacion en Productos cuando no hay categorias (decision mia, a confirmar).** `FormGestionProductos`, al cargar categorias, muestra una etiqueta de ayuda en el area del combo con la key `FormProductManagement.NoCategoriesHint` ("No hay categorias de producto. Crealas en Operador > Categorias de producto.") cuando la lista esta vacia, y la oculta si no. No es un error ni un dialogo: no bloquea abrir la pantalla.

**5. Keys con prefijo propio.** Pantalla y mensajes pasan a `FormProductCategoryManagement.*` y la bitacora a `FormProductCategoryManagement.Value.Activity.*` / `.Info.*`; `CategoriaProductoBLL` las usa. Se retiran de los seeds y de las dos bases las keys `FormMerchantDetail.*Categoria*` y `FormMerchantDetail.Value.*CategoriaProducto*` que queden sin uso en el codigo. Alternativa descartada: conservar el prefijo `FormMerchantDetail.`, que ya no describe a la pantalla y confunde al buscar keys.

**6. Retiro del bloque del detalle.** Se quita de `FormDetalleComercio` el grupo Categorias (controles, handlers, `FilaCategoriaProducto`, carga de categorias, referencia a `CategoriaProductoBLL` si no la usa nada mas) y se reacomoda el layout de lo que queda. La columna Categoria de la grilla de productos asignados se mantiene.

**7. Sin cambios de esquema ni de REPOSITORIO/DAL.** Las operaciones existentes alcanzan.

## Risks / Trade-offs

- [Retirar el bloque del detalle altera el layout del form y sus tests] → Se verifica con captura y con los tests UI del detalle; el caso de categoria de `FIN_BajasEnUsoTests` se migra a la pantalla nueva en la misma tarea que retira el bloque.
- [Renombrar keys deja huerfanas en las bases] → Una tarea cruza el codigo contra el seed y borra solo las que ya nadie usa, verificando por consulta en las dos bases.
- [Comparar nombres en memoria con `ObtenerTodas()` no escala] → El catalogo es chico; si crece, se pasa a una consulta por nombre sin cambiar la API del BLL.
- [Dos operadores crean el mismo nombre a la vez] → La validacion previa no cierra esa carrera; el `UNIQUE` la frena y el usuario ve la falla tecnica generica. Aceptado, igual que en `retirar-helper-violacion-fk`.
