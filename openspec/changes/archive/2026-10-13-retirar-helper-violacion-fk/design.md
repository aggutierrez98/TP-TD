# Design

## Context

Ver `proposal.md` (Why). Estado actual observado en `BLL/`:

- `RubroComercioBLL`, `MedioPagoBLL` y `EntidadFinancieraBLL.Eliminar` ya validan el uso de antemano con la UoW y dejan el helper como red de seguridad ante carreras.
- `EntidadFinancieraBLL.QuitarMedioPagoEntidad`, `ComercioBLL.Eliminar` y `CategoriaProductoBLL.Eliminar` no validan: dependen solo del helper. Sus comentarios en el codigo lo justifican con repositorios que "todavia no forman parte de la UoW", cosa que ya no es cierta para `Productos`, `Movimientos`, `Promociones`, `PromocionComercio` y `ComercioReglaCategorizacion` (todos expuestos en `IUnidadDeTrabajo`).
- `desacoplar-producto-de-comercio` (en curso, 0/22) ya retira el helper de `ProductoBLL`, reemplaza `Producto.ComercioId` por `ComercioProducto` y tiene la tarea 4.3 de revisar `ComercioBLL.Eliminar`.

## Goals / Non-Goals

**Goals:** una sola forma de bloquear bajas (consulta previa + `ExcepcionNegocio`); borrar el helper sin dejar referencias.

**Non-Goals:** cerrar la ventana de carrera entre la consulta y el `DELETE` (ver Riesgos); tocar `AccesoDatosSqlServer`; cambiar esquema.

## Decisions

1. **Consulta previa dentro de la misma UoW que el borrado.** Se reutiliza la unidad de trabajo ya abierta en cada `Eliminar`, igual que `RubroComercioBLL`. Alternativa descartada: una consulta en otra conexion, que no aporta nada y rompe el patron.
2. **Un chequeo por referencia, en el orden de las tablas que apuntan a la entidad.** Se prefieren metodos de repositorio `ObtenerPorX(id)` ya usados (`.Count > 0`) a inventar un contrato nuevo. Si el metodo no existe, se agrega al repositorio con su test de integracion; si existe, se reutiliza. Alternativa descartada: un metodo generico `EstaEnUso(tabla, id)`, que mezcla SQL de varias tablas en una sola DAL y sale caro de testear.
3. **Un mensaje de error por caso.** Se conserva la clave actual cuando ya existe (`RubroInUse`, `InUse`, `EntityPaymentMethodInUse`, `ComercioInUse`, `CategoriaProductoInUse`) aunque un caso haya varias causas; no hay una clave por referencia. Alternativa descartada: una clave por tabla referenciada, que multiplica traducciones sin que el usuario necesite el detalle.
4. **El `catch (Exception)` queda reducido** a relanzar `ExcepcionTecnica` o envolver con la clave `...DeleteFailed`. No se interpreta ningun `SqlException`.
5. **Orden de aplicacion: despues de `desacoplar-producto-de-comercio`.** El chequeo de `ComercioBLL` usa `ComercioProductos` (existe recien con ese change) y el de `CategoriaProductoBLL` usa `Productos` ya sin `ComercioId`. Si se invirtiera el orden, el chequeo del comercio habria que escribirlo contra `Producto.ComercioId` y rehacerlo despues.

## Risks / Trade-offs

- [Carrera: otro proceso inserta una referencia entre la consulta y el `DELETE`] → la FK real sigue protegiendo los datos; el usuario recibe la falla tecnica de baja en lugar del mensaje "en uso". Es un caso de un solo usuario operando en un TP, aceptado por el usuario al pedir este criterio.
- [Una referencia olvidada en una baja que antes cubria el helper] → la tarea de cada BLL parte de la lista de FKs del esquema (`script_bd.sql`) y cada una lleva su test rojo; la suite de integracion cubre el caso contra la base real.
- [Mas consultas por baja] → son conteos sobre tablas chicas y la baja es una operacion rara de operador.

## Migration Plan

Sin cambio de esquema ni de datos. Rollback: revertir el commit; el helper vuelve junto con sus usos.
