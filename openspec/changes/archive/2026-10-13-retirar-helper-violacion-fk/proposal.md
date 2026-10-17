# Proposal

## Why

`ViolacionIntegridadReferencialHelper` (BLL) atrapa el error 547 de SQL Server despues de que falla un `DELETE` y lo convierte en un error de negocio "en uso". Es una forma indirecta e innecesaria de resolver algo que el BLL puede preguntar de antemano: "esto esta en uso?" con una consulta previa y un mensaje propio. Hoy el codigo mezcla dos estilos (validacion proactiva en unos BLL, helper en otros), y el helper obliga a depender de que `AccesoDatosSqlServer` envuelva el `SqlException` original. El criterio unico que se quiere es: **toda baja valida el uso con una consulta aparte, antes del borrado**.

## What Changes

- Cada `Eliminar`/`Quitar` que hoy usa el helper pasa a validar el uso con una consulta previa y lanza `ExcepcionNegocio` con su propio mensaje de "en uso". Los seis usos actuales:
  - `RubroComercioBLL.Eliminar`, `MedioPagoBLL.Eliminar`, `EntidadFinancieraBLL.Eliminar`: ya validan de antemano; solo se quita el `catch` con el helper.
  - `EntidadFinancieraBLL.QuitarMedioPagoEntidad`: nuevo chequeo de uso en cuentas de usuarios (`CuentaFinancieraMedioPagoEntidad`) y en promociones (`PromocionMedioPagoEntidad`).
  - `ComercioBLL.Eliminar`: nuevo chequeo de uso en productos asignados, movimientos, promociones y reglas de categorizacion.
  - `CategoriaProductoBLL.Eliminar`: nuevo chequeo de uso en productos.
- Se borra `BLL/ViolacionIntegridadReferencialHelper.cs` y su `<Compile Include>` en `BLL.csproj`.
- Los `catch (Exception)` de esos metodos dejan de interpretar `SqlException`: solo relanzan `ExcepcionTecnica` o envuelven con la clave `...DeleteFailed`.
- Los mensajes "en uso" que ya existen no cambian; se agregan los que falten por caso (con traduccion es/in).
- No hay cambios de esquema SQL.

## Capabilities

### New Capabilities
- `validacion-uso-en-bajas`: toda baja de una entidad referenciada por otras se bloquea con un mensaje de negocio especifico, decidido por una consulta de uso previa al borrado y no por la captura del error de la base.

### Modified Capabilities
<!-- Ninguna: los specs vigentes no cubren bajas de catalogos. -->

## Impact

- **BLL**: `CategoriaProductoBLL`, `ComercioBLL`, `EntidadFinancieraBLL`, `MedioPagoBLL`, `RubroComercioBLL` y borrado del helper; `BLL.csproj`.
- **REPOSITORIO / DAL**: consultas de conteo o existencia de uso que hoy no existen (por producto, movimiento, promocion, regla de categorizacion, cuenta financiera), expuestas por las interfaces ya presentes en `IUnidadDeTrabajo`.
- **Traducciones**: keys de error "en uso" nuevas en `translation-seeds/base.es.json` y `base.in.json`, sincronizadas en las dos bases.
- **Tests**: `BLL.Tests` por cada BLL (rojo antes de implementar) y `Fakes.cs`; integracion de las consultas nuevas en `TpIDS.IntegrationTests`.
- **Dependencia de orden**: se aplica **despues** de `desacoplar-producto-de-comercio`, que retira el helper de `ProductoBLL` y crea `ComercioProducto`. Asi el chequeo de `ComercioBLL.Eliminar` consulta `ComercioProducto` y no el vinculo viejo `Producto.ComercioId`.
- **Fuera de alcance**: cambiar el comportamiento de `AccesoDatosSqlServer`, agregar bajas nuevas, y cualquier cambio de esquema.
