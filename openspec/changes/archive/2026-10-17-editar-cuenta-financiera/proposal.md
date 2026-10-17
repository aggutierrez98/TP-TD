## Why

La pantalla "Mis cuentas financieras" solo permite alta y baja: al seleccionar una cuenta de la grilla no se cargan sus datos en los selectores y no existe una accion de edicion. Corregir un alias, el tipo, la entidad o los medios de pago contratados obliga a eliminar y volver a crear la cuenta. El usuario pidio explicitamente poder editarla.

## What Changes

- `CuentaFinancieraBLL` agrega `Actualizar`: edita alias, tipo, entidad y medios de pago contratados de una cuenta propia, en una sola transaccion, con las mismas validaciones del alta, y registra la operacion en bitacora.
- `FormGestionCuentasFinancieras` carga en los selectores los datos de la cuenta seleccionada y agrega el boton Actualizar.
- Claves de traduccion nuevas (es/en) para el boton, el mensaje de exito, la bitacora y el error tecnico.
- **BREAKING** (solo interno): el test UI `FIN_CuentasFinancieras_Pantalla_SoloPermiteAltaYBaja` deja de ser valido porque la pantalla ahora tiene accion de edicion; se reemplaza.
- `CredencialesCifradas` sigue fuera de alcance: la edicion no la lee ni la modifica.

## Capabilities

### New Capabilities
- `edicion-cuentas-financieras`: edicion de una cuenta financiera propia (campos editables, validaciones, propiedad, atomicidad, bitacora y comportamiento de la pantalla).

### Modified Capabilities

Ninguna. No hay spec vigente de cuentas financieras en `openspec/specs/`.

## Impact

- Codigo: `BLL/CuentaFinancieraBLL.cs`, `UI/FormGestionCuentasFinancieras.cs` y su Designer. El repositorio y el DAL ya exponen `Actualizar` y `ReemplazarMediosPagoEntidadDeCuenta`; no cambian.
- Traducciones: `translation-seeds/base.es.json`, `base.in.json` y `Palabra`/`Traduccion` de `TpCatedraIDS` y `TpCatedraIDS_Test`.
- Tests: `tests/BLL.Tests/CuentaFinancieraBLLTests.cs` y `tests/TpIDS.UiTests/FIN_EntidadesMediosPagoTests.cs`.
- Sin cambios de esquema SQL ni de dependencias.
