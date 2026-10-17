## Context

`CuentaFinancieraBLL` solo expone `Crear` y `Eliminar`, y su comentario de cabecera y el de `FormGestionCuentasFinancieras` declaran que una cuenta "no se edita". El repositorio ya tiene `Actualizar` (UPDATE de entidad, alias, tipo y credenciales) y `ReemplazarMediosPagoEntidadDeCuenta`, ambos con test de integracion. Falta la regla de negocio y la pantalla.

## Goals / Non-Goals

**Goals:**
- Editar alias, tipo, entidad y medios contratados de una cuenta propia con las reglas del alta.
- Cargar los datos de la fila seleccionada en los selectores y agregar el boton Actualizar.

**Non-Goals:**
- Credenciales cifradas (siguen fuera de alcance).
- Unicidad de alias, nuevos permisos o cambios de esquema.
- Cambiar el comportamiento de Agregar y Eliminar.

## Decisions

- **`Actualizar(int id, int entidadFinancieraId, string alias, TipoCuentaFinanciera tipo, List<int> medioPagoEntidadIds)` en el BLL**, devolviendo la cuenta actualizada. Misma estructura que `Crear`: normaliza alias, valida tipo, abre una unidad de trabajo, valida entidad y catalogo, y registra bitacora antes de `Save`. Alternativa descartada: reutilizar `Crear` borrando y recreando, porque cambia el `Id` y rompe cualquier referencia futura.
- **Las credenciales se conservan leyendo la cuenta existente** dentro de la unidad de trabajo y reenviando su `CredencialesCifradas` al `Actualizar` del repositorio, porque el UPDATE las sobreescribe siempre.
- **La propiedad se valida con el `ValidarPropiedad` existente** (cuenta nula = `NotFound`, otro dueno = `NotOwned`); no se agregan errores nuevos de negocio.
- **Cambio de entidad: los medios se envian completos desde la pantalla.** El BLL valida contra el catalogo de la entidad resultante; la pantalla, al cambiar la entidad, recarga el catalogo con todo desmarcado. Asi no hay logica de "migrar" medios entre entidades.
- **Pantalla: seleccionar una fila rellena el formulario** (entidad sin disparar la recarga automatica, alias, tipo y catalogo con los contratados marcados) y **Actualizar** va entre Agregar y Eliminar con el patron de `FormMovimientos`. Agregar sigue creando una cuenta nueva con lo que haya en el formulario, aunque haya una fila seleccionada. Nombre del control: `btnActualizarCuenta` (skill `winforms-ui-control-naming`).
- **Claves nuevas**: `FormFinancialAccountManagement.UpdateButton`, `.Success.Updated`, `.Value.Activity.Update`, `.Value.Info.Update` y `Error.FinancialAccountManagement.UpdateFailed`, en es/en, con sincronizacion de seeds y de las dos bases (`translation-base-seed-sync`).

## Risks / Trade-offs

- [El test UI `SoloPermiteAltaYBaja` fija la ausencia de edicion] → se reemplaza por uno que exige `btnActualizarCuenta` junto a Agregar y Eliminar.
- [Seleccionar una fila y pulsar Agregar duplica la cuenta con otro `Id`] → comportamiento consistente con Movimientos; no se agrega boton de limpiar porque no se pidio.
- [Cuenta sin medios en la fila (por ejemplo "Naranja X para ahorrar")] → el catalogo se muestra con todo desmarcado y la edicion sigue valida.
