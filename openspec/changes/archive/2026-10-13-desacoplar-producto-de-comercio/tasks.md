# Tasks

El progreso de este change vive solo en este archivo. Cada grupo deja sus propios tests; los cambios de comportamiento en BLL/REPOSITORIO/DAL siguen la skill `strict-tdd` (baseline en verde, test en rojo observado, implementacion en verde).

## 1. Esquema, migracion y permiso

- [x] 1.1 Respaldar `TpCatedraIDS` y `TpCatedraIDS_Test` con `BACKUP DATABASE`; verificar que los archivos `.bak` existen y no estan vacios.
- [x] 1.2 Escribir el script de migracion re-ejecutable (crear `ComercioProducto`, copiar cada `Producto` existente, quitar FKs/CHECK/indice/columnas `ComercioId`, `MonedaId`, `Precio`); verificar corriendolo dos veces sobre `TpCatedraIDS_Test` sin error y contando que las filas de `ComercioProducto` igualan las de `Producto` previas.
- [x] 1.3 Aplicar la migracion a `TpCatedraIDS`; verificar con una consulta que `ComercioProducto` tiene las mismas filas, precios y monedas que tenia `Producto`.
- [x] 1.4 Actualizar `script_bd.sql` al esquema final (tabla `ComercioProducto`, `Producto` sin columnas de comercio, permiso `gestionproductos` asignado a Operador y admin); verificar ejecutandolo contra una base vacia y comprobando tablas y permiso.
- [x] 1.5 Insertar el permiso `gestionproductos` y su asignacion a Operador y admin en las dos bases de desarrollo; verificar con una consulta a `Permiso` y a la tabla de grupo-permiso.

## 2. BE, DAL y REPOSITORIO (TDD, integracion)

- [x] 2.1 Reescribir `ProductoRepositorioIntegrationTests` para el catalogo (obtener por id, obtener todos, insertar, actualizar, eliminar) y escribir `ComercioProductoRepositorioIntegrationTests` (obtener por comercio, obtener por id, insertar, actualizar precio, eliminar, contar por producto, rechazo de duplicado y de precio no positivo); verlos fallar.
- [x] 2.2 Simplificar `BE/Producto.cs` y crear `BE/ComercioProducto.cs`; verificar que `BE` compila.
- [x] 2.3 Adaptar `ProductoDAL`/`ProductoRepositorio`/`IProductoRepositorio` y crear `ComercioProductoDAL`/`ComercioProductoRepositorio`/`IComercioProductoRepositorio`; sumar `ComercioProductos` a `IUnidadDeTrabajo` y `UnidadDeTrabajoSqlServer` y registrar los archivos en los `.csproj`; verificar que los tests de 2.1 pasan en verde.

## 3. BLL del catalogo de productos (TDD, unitarios)

- [x] 3.1 Reescribir `ProductoBLLTests` (alta valida, nombre vacio, categoria inexistente, edicion, baja sin uso, **baja bloqueada con `ProductoInUse` cuando un comercio lo vende**, bitacora en cada operacion) y actualizar `Fakes.cs` con el fake de `ComercioProducto`; verlos fallar.
- [x] 3.2 Reimplementar `ProductoBLL` (`ObtenerTodos`, `Crear`, `Actualizar`, `Eliminar` con conteo explicito de uso, sin `ViolacionIntegridadReferencialHelper`) y actualizar la construccion en `CompositionRoot`; verificar que los tests de 3.1 pasan.

## 4. BLL de productos por comercio (TDD, unitarios)

- [x] 4.1 Escribir `ComercioProductoBLLTests` (asignar valido, precio invalido, moneda inexistente, producto o comercio inexistente, producto repetido en el comercio, mismo producto en dos comercios con precios distintos, actualizar precio, quitar sin eliminar del catalogo, bitacora); verlos fallar.
- [x] 4.2 Implementar `ComercioProductoBLL` (ctor `internal`, `ObtenerPorComercio`, `ObtenerMonedas`, `Asignar`, `ActualizarPrecio`, `Quitar`), darlo de alta en `CompositionRoot` y ejecutar la skill `composition-root-guard`; verificar tests en verde y que el build valida el Composition Root.
- [x] 4.3 Revisar `ComercioBLL.Eliminar` y sus tests: confirmar que un comercio con productos asignados sigue rechazandose con el mismo mensaje; verificar con un test (nuevo o existente) en `ComercioBLLTests`.

## 5. Pantalla Productos, menu, permiso y traducciones

- [x] 5.1 Crear `FormGestionProductos` (Krypton, MDI unico, grilla de catalogo, nombre, categoria, Agregar/Actualizar/Eliminar) respetando la skill `winforms-ui-control-naming`; verificar que compila y que el Designer sigue la nomenclatura.
- [x] 5.2 Agregar `mnuOperadorProductos` en `FormPrincipal`, habilitarlo con `gestionproductos` e incluirlo en el calculo de `mnuOperador.Enabled`; verificar abriendo la app con un usuario Operador y con uno sin permiso.
- [x] 5.3 Agregar las keys nuevas (pantalla, mensajes, errores `ProductoInUse` y bitacora) a `translation-seeds/base.es.json` y `base.in.json`, ejecutar la skill `translation-base-seed-sync` e insertarlas en `Palabra`/`Traduccion` de ambas bases; verificar con una consulta que existen y que la pantalla se traduce al cambiar de idioma.

## 6. Detalle del comercio

- [x] 6.1 Rehacer la seccion Productos de `FormDetalleComercio` (grilla de `ComercioProducto`, combo de productos del catalogo, moneda y precio; asignar, actualizar precio, quitar; sin crear productos), usando `ComercioProductoBLL`; verificar que compila y que el flujo funciona manualmente en la app.
- [x] 6.2 Actualizar las keys de traduccion afectadas del detalle del comercio y sincronizarlas; verificar con la skill `translation-base-seed-sync`.

## 7. Cierre e integracion

- [x] 7.1 Compilar `TpIDS.sln` con MSBuild (skill `winforms-build-verification`) incluyendo la validacion de capas y del Composition Root; verificar exit code 0.
- [x] 7.2 Correr `tests/tools/run-tests.ps1 -IncludeIntegration` (skill `test-suite-verification`) y reportar los contadores reales del TRX; verificar exit code 0.
- [x] 7.3 Actualizar los tests UI FlaUI afectados (detalle del comercio y pantalla Productos); avisar al usuario antes de correr la suite UI completa (toma el escritorio unos 5 minutos) y esperar su confirmacion.
- [x] 7.4 Anotar en `diagramas/TODO-actualizacion-diagramas.md` el cambio de clases (G07) y del DER (G08) por este desacople; verificar que el pendiente quedo registrado.
