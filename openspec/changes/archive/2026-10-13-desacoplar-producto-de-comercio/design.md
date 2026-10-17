# Design

## Context

Estado actual (observado en el codigo, ver `proposal.md` para la motivacion):

- `BE/Producto.cs` tiene `Comercio`, `CategoriaProducto`, `Moneda`, `Nombre` y `Precio`. La tabla `dbo.Producto` tiene `ComercioId` (FK a `Comercio`), `MonedaId` (FK a `Moneda`) y `Precio` (`CHECK Precio > 0`), mas un indice `IX_Producto_ComercioId`.
- `ProductoBLL` (Crear/Actualizar/Eliminar/ObtenerPorComercio/ObtenerMonedas) y `FormDetalleComercio` son los unicos consumidores de `Producto`. El detalle del comercio tambien hospeda el ABM de `CategoriaProducto` y de rubros.
- Las bajas de `Comercio` y `CategoriaProducto` detectan "en uso" atrapando la `SqlException` de la FK mediante `ViolacionIntegridadReferencialHelper`, que el usuario va a retirar a futuro.
- El menu Operador (`FormPrincipal`) habilita cada submenu segun un permiso (`gestioncomercios`, `gestionmediospago`, ...) sembrado en `script_bd.sql` y asignado a los grupos Operador y admin.
- Reglas del repo que aplican: capas validadas en build, BLL solo desde `CompositionRoot`, TDD estricto en SERVICIOS/BLL/REPOSITORIO/DAL, nomenclatura de controles, sincronizacion de traducciones.

## Goals / Non-Goals

**Goals:**
- Que `Producto` no dependa de ningun comercio y que el precio/moneda vivan en la relacion comercio-producto.
- Que la futura relacion promocion-producto pueda apuntar al catalogo sin tocar comercios.
- Conservar los datos existentes en la migracion.

**Non-Goals:**
- Promociones (change `gestion-promociones`).
- Mover el ABM de `CategoriaProducto` fuera de `FormDetalleComercio`. Queda donde esta; si resulta incomodo para crear categorias antes de crear productos, se evalua aparte.
- Deduplicar productos migrados con el mismo nombre en distintos comercios, o imponer unicidad de nombre en el catalogo.
- Historial de precios.
- Retirar `ViolacionIntegridadReferencialHelper` (los otros BLL lo siguen usando).

## Decisions

**1. Tabla `ComercioProducto` con clave compuesta.** Columnas `ComercioId`, `ProductoId`, `MonedaId`, `Precio`; PK `(ComercioId, ProductoId)`; FK a `Comercio`, `Producto` y `Moneda`; `CHECK (Precio > 0)`. La PK compuesta hace cumplir "un producto una sola vez por comercio" en la base y no necesita un `Id` propio (la relacion nunca se referencia desde otra tabla). Alternativa descartada: `Id` surrogate con indice unico; agrega una columna sin uso.

**2. `Producto` queda con `Id`, `CategoriaProductoId`, `Nombre`.** Sin `UNIQUE` en `Nombre` (decision mia, marcada como tal): la migracion 1 a 1 produce nombres repetidos y el usuario acepto que el operador los unifique a mano.

**3. Migracion en un script unico y transaccional.** Pasos: crear `ComercioProducto`; insertar una fila por cada `Producto` existente (`ComercioId`, `Id`, `MonedaId`, `Precio`); recien despues quitar `FK_Producto_Comercio`, `FK_Producto_Moneda`, `CK_Producto_Precio`, `IX_Producto_ComercioId` y las columnas `ComercioId`, `MonedaId`, `Precio`. Se escribe de forma re-ejecutable (verifica existencia antes de cada paso) y se aplica con `sqlcmd` a `TpCatedraIDS` y `TpCatedraIDS_Test`. `script_bd.sql` se actualiza al esquema final para instalaciones nuevas.

**4. BE.** `Producto` se simplifica. Nueva `ComercioProducto` con `Comercio`, `Producto`, `Moneda` y `Precio` (los objetos referenciados cargan solo `Id`, igual que los DAL actuales; la UI resuelve nombres con el BLL). Se descarta guardar `Comercio.Productos` como coleccion para no crear referencias circulares.

**5. DAL / REPOSITORIO.** `ProductoDAL` pasa a `ObtenerPorId`, `ObtenerTodos`, `InsertarYObtenerId`, `Actualizar`, `Eliminar`. Nuevo `ComercioProductoDAL` con `ObtenerPorComercio`, `ObtenerPorId(comercioId, productoId)`, `Insertar`, `ActualizarPrecio`, `Eliminar` y `ContarPorProducto(productoId)` (para el chequeo de uso). `IUnidadDeTrabajo` expone `ComercioProductos` para participar de las transacciones, igual que `Productos`.

**6. Bloqueo de baja con chequeo explicito.** `ProductoBLL.Eliminar` consulta `ContarPorProducto` dentro de la misma unidad de trabajo y, si es mayor que cero, lanza `ExcepcionNegocio("Error.MerchantManagement.ProductoInUse")`. No atrapa `SqlException` ni usa `ViolacionIntegridadReferencialHelper`. Alternativa descartada: depender de la FK y traducir la excepcion (es justo el patron que se va a retirar). Cuando llegue `gestion-promociones`, ese change suma su propio conteo de uso en el mismo metodo.

**7. BLL.** `ProductoBLL` queda como catalogo: `ObtenerTodos`, `Crear(nombre, categoriaProductoId)`, `Actualizar(id, nombre, categoriaProductoId)`, `Eliminar(id)`. Un `ComercioProductoBLL` nuevo (inyeccion por ctor `internal`, alta en `CompositionRoot`, sin Service Locator) expone `ObtenerPorComercio`, `ObtenerMonedas`, `Asignar(comercioId, productoId, monedaId, precio)`, `ActualizarPrecio(comercioId, productoId, precio)` y `Quitar(comercioId, productoId)`. La moneda no es editable tras asignar, como hoy. `ObtenerMonedas` se mueve de `ProductoBLL` a `ComercioProductoBLL` porque la moneda ahora pertenece a la relacion. Operaciones `ActualizarPrecio` y `Quitar`: son el minimo que implica "elegir un producto y ponerle precio" y fue una inferencia mia que se confirma al revisar este documento.

**8. UI.** `FormGestionProductos` (Krypton, hijo MDI unico, `FormTraducibleBase`, nomenclatura de controles del repo) con grilla del catalogo, nombre, categoria y botones Agregar/Actualizar/Eliminar. `FormDetalleComercio`: la seccion Productos pasa a una grilla de `ComercioProducto` (producto, categoria, moneda, precio) con combo de productos del catalogo, moneda y precio; se quita la creacion de productos. El submenu `mnuOperadorProductos` cuelga de `mnuOperador`, se habilita con `gestionproductos` y se suma al calculo de `mnuOperador.Enabled`.

**9. Permiso.** `gestionproductos` se siembra en `script_bd.sql`, se asigna a Operador y admin (el admin hereda todo permiso nuevo de Operador) y tambien se inserta en las dos bases de desarrollo.

**10. Traducciones.** Keys nuevas para la pantalla, los mensajes y las claves de bitacora en `translation-seeds/base.es.json` y `base.in.json`, mas inserciones manuales en `Palabra`/`Traduccion` de ambas bases (el sync no las toma solo).

## Risks / Trade-offs

- [El esquema pierde columnas y es irreversible sin respaldo] → Respaldar las dos bases antes de migrar (`BACKUP DATABASE` con `sqlcmd`) y escribir el script re-ejecutable.
- [Pruebas existentes de `Producto` quedan obsoletas] → Se reescriben dentro del mismo grupo de tareas que cambia cada capa (TDD), no en un grupo final.
- [Tests UI FlaUI que tocan el detalle del comercio] → Se actualizan despues del desarrollo y la suite UI solo se corre tras avisar al usuario.
- [Productos duplicados tras la migracion] → Aceptado por el usuario; el catalogo permite repetidos y el operador los depura.
- [Crear una categoria exige ir al detalle de un comercio] → Se deja como esta (non-goal); se menciona al usuario como posible mejora.
- [La entrega a la catedra depende de que las bases del laboratorio tengan el esquema nuevo] → `script_bd.sql` queda actualizado y verificado contra una base vacia en la tarea de cierre.
