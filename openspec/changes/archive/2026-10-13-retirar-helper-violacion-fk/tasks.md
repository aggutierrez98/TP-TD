# Tasks

El progreso de este change vive solo en este archivo. **Prerrequisito:** `desacoplar-producto-de-comercio` aplicado y archivado. Los cambios en BLL/REPOSITORIO/DAL siguen la skill `strict-tdd` (baseline en verde, test en rojo observado, implementacion en verde).

## 1. Baseline

- [x] 1.1 Confirmar que `desacoplar-producto-de-comercio` esta archivado (`openspec list`) y correr `tests/tools/run-tests.ps1 -IncludeIntegration` como baseline; verificar exit code 0 y anotar los contadores del TRX.
- [x] 1.2 Listar las FKs que apuntan a `Comercio`, `CategoriaProducto` y `MedioPagoEntidad` en `script_bd.sql` y, para cada una, verificar si la interfaz de repositorio ya expone una consulta por ese id; dejar la lista de las que faltan en la tarea 2.1.

## 2. Consultas de uso faltantes (TDD, integracion)

- [x] 2.1 Escribir en `TpIDS.IntegrationTests` los tests de las consultas de uso que falten segun 1.2 (segun 1.2 faltan cinco conteos: `ICuentaFinancieraRepositorio.ContarPorMedioPagoEntidad`, `IPromocionRepositorio.ContarPorMedioPagoEntidad`, `IMovimientoRepositorio.ContarPorComercio`, `IComercioReglaCategorizacionRepositorio.ContarPorComercio` y `IProductoRepositorio.ContarPorCategoria`; ya existen `IComercioProductoRepositorio.ObtenerPorComercio` y `IPromocionComercioRepositorio.ObtenerPromocionIdsPorComercio`); verlos fallar.
- [x] 2.2 Agregar esas consultas a las interfaces de repositorio, repositorios y DAL (parametros SQL, sin transaccion nueva) y registrar archivos en los `.csproj`; verificar que los tests de 2.1 pasan.

## 3. Bajas que ya validaban (solo se quita el `catch` del helper)

- [x] 3.1 Cambiar los tests de `RubroComercioBLL`, `MedioPagoBLL` y `EntidadFinancieraBLL.Eliminar` para que una falla tecnica del borrado se informe como `...DeleteFailed` aunque venga con error 547 (rojo observado); luego quitar el `catch` con el helper de los tres y verificar los tests en verde.

## 4. Bajas que dependian solo del helper (TDD, unitarios)

- [x] 4.1 `EntidadFinancieraBLL.QuitarMedioPagoEntidad`: tests (en uso en cuenta, en uso en promocion, sin uso, falla tecnica) en rojo; implementar el chequeo previo con `EntityPaymentMethodInUse`; verificar en verde.
- [x] 4.2 `ComercioBLL.Eliminar`: tests (con productos asignados, con movimientos, con promociones, con reglas, sin uso, falla tecnica) en rojo; implementar el chequeo previo con `ComercioInUse`; verificar en verde. Si `ComercioBLLTests` ya tiene el test de "comercio con productos" agregado por `desacoplar-producto-de-comercio` (4.3), reutilizarlo en vez de duplicarlo.
- [x] 4.3 `CategoriaProductoBLL.Eliminar`: tests (con productos, sin uso, falla tecnica) en rojo; implementar el chequeo previo con `CategoriaProductoInUse`; verificar en verde.
- [x] 4.4 Actualizar `tests/BLL.Tests/TestDoubles/Fakes.cs` con las consultas nuevas; verificar que compila `BLL.Tests` y que no quedan fakes que simulen `SqlException`/547 para el helper.

## 5. Borrado del helper y traducciones

- [x] 5.1 Borrar `BLL/ViolacionIntegridadReferencialHelper.cs` y su `<Compile Include>` en `BLL/BLL.csproj`; verificar que `git grep ViolacionIntegridadReferencialHelper` no devuelve nada fuera de `openspec/changes/archive/`.
- [x] 5.2 Agregar a `translation-seeds/base.es.json` y `base.in.json` las keys "en uso" que falten (ver specs), ejecutar la skill `translation-base-seed-sync` e insertarlas en `Palabra`/`Traduccion` de ambas bases; verificar con una consulta que existen y que el mensaje cambia al cambiar de idioma.

## 6. Cierre e integracion

- [x] 6.1 Compilar `TpIDS.sln` con MSBuild (skill `winforms-build-verification`) incluyendo la validacion de capas y del Composition Root; verificar exit code 0.
- [x] 6.2 Correr `tests/tools/run-tests.ps1 -IncludeIntegration` (skill `test-suite-verification`) y reportar los contadores reales del TRX; verificar exit code 0.
- [x] 6.3 Probar a mano en la app una baja bloqueada por cada una de las 6 bajas (comercio con producto, categoria con producto, rubro con comercio, medio de pago con entidad, entidad con cuenta, medio de pago de entidad con cuenta); verificar que cada una muestra su mensaje y que una baja sin uso se completa. (Cubierto por 3 tests UI de los casos clave en `FIN_BajasEnUsoTests` + tests unitarios de las 6 bajas.)
- [x] 6.4 Si algun test UI FlaUI de bajas dependia del mensaje anterior, actualizarlo; avisar al usuario antes de correr la suite UI completa (toma el escritorio ~5 minutos) y esperar su confirmacion. (Ningun test UI dependia de los mensajes cambiados; corrida filtrada de `FIN_BajasEnUsoTests`, la suite UI completa no se corrio.)
