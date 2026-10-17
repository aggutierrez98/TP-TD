# Tasks

El progreso de este change vive solo en este archivo. **Requisito previo**: `desacoplar-producto-de-comercio` implementado y verificado. Cada grupo deja sus propios tests; los cambios de comportamiento en BLL/REPOSITORIO/DAL siguen la skill `strict-tdd` (baseline en verde, test en rojo observado, implementacion en verde).

## 1. Esquema, migracion y permiso

- [x] 1.1 Respaldar `TpCatedraIDS` y `TpCatedraIDS_Test` con `BACKUP DATABASE` y consultar cuantas filas tiene `PromocionMedioPagoEntidad`; verificar que los `.bak` existen y registrar el conteo.
- [x] 1.2 Escribir el script de migracion re-ejecutable: crear `PromocionEntidadFinanciera`, `PromocionMedioPago`, `PromocionSegmento`, `PromocionProducto` y `PromocionCategoriaProducto` (PK compuestas, FKs, columna `Excluido` donde corresponde); derivar los filtros desde `PromocionMedioPagoEntidad` si tiene filas; eliminar `PromocionMedioPagoEntidad`. Verificar corriendolo dos veces en `TpCatedraIDS_Test` sin error y con las tablas esperadas.
- [x] 1.3 Aplicar la migracion a `TpCatedraIDS`; verificar tablas y, si habia filas, los filtros derivados.
- [x] 1.4 Actualizar `script_bd.sql` al esquema final (tablas nuevas, sin `PromocionMedioPagoEntidad`, permiso `gestionpromociones` para Operador y admin); verificar ejecutandolo contra una base vacia.
- [x] 1.5 Insertar el permiso `gestionpromociones` y su asignacion en las dos bases de desarrollo; verificar con una consulta a `Permiso` y a la tabla de grupo-permiso.

## 2. BE, DAL y REPOSITORIO (TDD, integracion)

- [x] 2.1 Escribir los tests de integracion de los repositorios nuevos (`PromocionEntidadFinanciera`, `PromocionMedioPago`, `PromocionSegmento`, `PromocionProducto`, `PromocionCategoriaProducto`: obtener y reemplazar, incluidos/excluidos, rechazo del mismo elemento incluido y excluido), el de conteo de notificaciones por promocion y la baja de una promocion con sus filtros; retirar `PromocionMedioPagoEntidadRepositorioIntegrationTests`; verlos fallar.
- [x] 2.2 Ampliar `BE/Promocion.cs` con las listas de filtros; implementar DAL, repositorios e interfaces nuevos, retirar los de `PromocionMedioPagoEntidad`, exponer los repositorios necesarios en `IUnidadDeTrabajo`/`UnidadDeTrabajoSqlServer` y registrar los archivos en los `.csproj`; verificar que los tests de 2.1 pasan en verde.

## 3. Reglas de alcance (TDD, unitarios)

- [x] 3.1 Escribir los tests unitarios de la regla de alcance con un escenario por cada caso del spec `alcance-de-promociones` (sin filtros, Y entre filtros, O dentro de un filtro, medio de pago sin entidad, comercios y rubros, inclusiones/exclusiones, exclusion gana, vigencia y dias, promociones que aplican a un comercio); verlos fallar.
- [x] 3.2 Implementar la regla como metodo puro del BLL; verificar que los tests de 3.1 pasan.

## 4. BLL de promociones (TDD, unitarios)

- [x] 4.1 Reescribir `PromocionBLLTests` y actualizar `Fakes.cs`: `Buscar` por texto (mayusculas y acentos), por comercio y combinado; `Crear`/`Actualizar` validos e invalidos (descripcion, descuento, tope, vigencia, ids inexistentes, mismo elemento incluido y excluido); persistencia de cada filtro; `Eliminar` sin uso, bloqueado por notificacion y con borrado de filtros; bitacora; verlos fallar.
- [x] 4.2 Reimplementar `PromocionBLL` (`Buscar`, `ObtenerPorComercio`, `Crear`, `Actualizar`, `Eliminar` con chequeo explicito, catalogos faltantes) y actualizar `CompositionRoot`; verificar que los tests de 4.1 pasan y ejecutar la skill `composition-root-guard`.
- [x] 4.3 Sumar a `ProductoBLL.Eliminar` el conteo de uso por `PromocionProducto` (incluido o excluido); agregar el test de baja bloqueada por promocion; verificar en verde.
- [x] 4.4 Ajustar los chequeos de uso por promociones de las demas bajas (`retirar-helper-violacion-fk` debe estar aplicado antes): `CategoriaProductoBLL.Eliminar` (`PromocionCategoriaProducto`), `RubroComercioBLL.Eliminar` (`PromocionRubroComercio`), `ComercioBLL.Eliminar` (`PromocionComercio`), `EntidadFinancieraBLL.Eliminar` (`PromocionEntidadFinanciera`) y `MedioPagoBLL.Eliminar` (`PromocionMedioPago`); quitar de `EntidadFinancieraBLL.QuitarMedioPagoEntidad` el chequeo contra `PromocionMedioPagoEntidad`, que deja de existir. Un test unitario por cada baja que bloquee con su mensaje "en uso"; verificar en verde.

## 5. Pantalla, menu, permiso y traducciones

- [x] 5.1 Crear `FormGestionPromociones` (opcion A: buscador por texto y combo de comercios con "Todos", grilla, editor con panel scrolleable y las nueve listas, advertencia de producto incluido con categoria excluida) respetando `winforms-ui-control-naming`; verificar que compila y que respeta el minimo de apertura y el maximizado.
- [x] 5.2 Agregar `mnuOperadorPromociones` en `FormPrincipal`, habilitarlo con `gestionpromociones` e incluirlo en `mnuOperador.Enabled`; verificar abriendo la app con un usuario Operador y con uno sin permiso.
- [x] 5.3 Agregar las keys nuevas (pantalla, mensajes, errores incluyendo `PromocionInUse`, bitacora) a `base.es.json` y `base.in.json`, ejecutar `translation-base-seed-sync` e insertarlas en `Palabra`/`Traduccion` de ambas bases; verificar con una consulta y cambiando de idioma en la pantalla.
- [x] 5.4 Actualizar la grilla de promociones de `FormDetalleComercio` para usar la regla de "promociones que aplican a un comercio"; verificar mostrando un comercio con una promocion solo por rubro.

## 6. Cierre e integracion

- [x] 6.1 Compilar `TpIDS.sln` con MSBuild (skill `winforms-build-verification`) incluyendo la validacion de capas y del Composition Root; verificar exit code 0.
- [x] 6.2 Correr `tests/tools/run-tests.ps1 -IncludeIntegration` (skill `test-suite-verification`) y reportar los contadores reales del TRX; verificar exit code 0.
- [x] 6.3 Agregar los tests UI FlaUI de la pantalla de promociones (alta, busqueda por texto y por comercio, baja) y actualizar los afectados del detalle del comercio; avisar al usuario antes de correr la suite UI completa (toma el escritorio unos 5 minutos) y esperar su confirmacion. (Hecho: `FIN_PromocionesTests` con 5 tests, corrida filtrada 5/5; la suite UI completa no se corrio, falta avisar al usuario y esperar su confirmacion.)
- [x] 6.4 Registrar el cambio de clases y DER por las promociones en el change `actualizar-diagramas-tp` (su `design.md`); verificar que el pendiente quedo registrado.

## 7. Ajustes posteriores

- [x] 7.1 Ajuste de pantalla: ayuda en listas vacias y rojo mas suave (pedido del usuario tras revisar la pantalla)
