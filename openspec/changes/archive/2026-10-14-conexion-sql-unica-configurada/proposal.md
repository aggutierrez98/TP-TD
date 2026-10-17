# Proposal

## Why

El acceso a datos intenta dos conexiones y puede terminar usando una instancia distinta de la elegida en la configuracion. Se necesita que el destino SQL sea exclusivamente el indicado en `UI/App.config`, tanto al arrancar como durante el uso de la aplicacion.

## What Changes

- Usar exclusivamente la cadena `TpIDS` de la seccion `connectionStrings` del config del ejecutable, cuyo origen es `UI/App.config`.
- **BREAKING**: eliminar el intento alternativo de `TpIDS-UAI`; una conexion principal no disponible debe producir el error existente, sin cambiar de destino.
- Eliminar la entrada alternativa de la configuracion productiva y de la suite de integracion.
- Alinear backup/restore con la misma seleccion unica y rechazar una configuracion principal ausente o vacia.

## Capabilities

### New Capabilities

- `conexion-base-datos`: seleccion del destino SQL mediante una unica cadena configurada y comportamiento ante configuracion ausente o destino no disponible.

### Modified Capabilities

Ninguna.

## Impact

- `DAL/AccesoDatosSqlServer.cs`, `DAL/BackupDAL.cs` y `UI/App.config`.
- `tests/TpIDS.IntegrationTests/App.config` y pruebas de regresion de conexion.
- Arranque: la carga de idioma e integridad depende de la disponibilidad de la conexion configurada.
- Sin nuevas dependencias productivas; se conserva .NET Framework 4.7.2 y el esquema de capas.
