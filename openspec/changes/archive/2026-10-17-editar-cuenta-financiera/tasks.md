## 1. BLL de edicion (TDD, unitarios)

- [x] 1.1 Correr `tests/tools/run-tests.ps1 -IncludeIntegration` como baseline y anotar exit code y contadores del TRX.
- [x] 1.2 Agregar a `CuentaFinancieraBLLTests` los tests de `Actualizar` (alias y tipo, alias vacio, reemplazo de medios sin duplicados, medio de otra entidad, cambio de entidad, entidad inexistente, credenciales intactas, cuenta ajena, cuenta inexistente, bitacora y atomicidad ante fallo, error tecnico `UpdateFailed`); verlos fallar.
- [x] 1.3 Implementar `CuentaFinancieraBLL.Actualizar` y actualizar el comentario de cabecera ("no se edita"); verificar los tests de 1.2 en verde y el resto de `BLL.Tests` sin regresiones.

## 2. Pantalla

- [x] 2.1 Cargar en `FormGestionCuentasFinancieras` los datos de la fila seleccionada (entidad sin disparar la recarga, alias, tipo y medios contratados marcados) y actualizar el comentario de cabecera.
- [x] 2.2 Agregar `btnActualizarCuenta` (Designer, Krypton, orden Agregar, Actualizar, Eliminar) con su handler: advertencia sin seleccion, recarga manteniendo la fila y mensaje de exito; verificar que compila.
- [x] 2.3 Agregar las keys nuevas a `translation-seeds/base.es.json` y `base.in.json`, ejecutar la skill `translation-base-seed-sync` e insertarlas en `Palabra`/`Traduccion` de `TpCatedraIDS` y `TpCatedraIDS_Test`; verificar con una consulta que existen en los dos idiomas.

## 3. Tests UI y cierre

- [x] 3.1 Reemplazar `FIN_CuentasFinancieras_Pantalla_SoloPermiteAltaYBaja` por un test que exija los botones Agregar, Actualizar y Eliminar, y agregar tests UI de seleccion que carga datos y de edicion de alias; correr solo esa clase con `-UiFilter` y verificar el TRX.
- [x] 3.2 Cerrar con compilacion de `TpIDS.sln` y `tests/tools/run-tests.ps1 -IncludeIntegration` en verde (exit code 0), informando los contadores reales.
- [x] 3.3 Buscar con `rg` en `diagramas/` y en `openspec/` menciones de que la cuenta financiera "no se edita" o "solo alta y baja" y corregirlas o registrar la tarea pendiente en el change que corresponda.
