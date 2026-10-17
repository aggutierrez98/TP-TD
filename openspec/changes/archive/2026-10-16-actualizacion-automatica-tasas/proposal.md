# Proposal

## Why

Hoy las tasas de conversion se cargan y editan a mano desde Gestion de Monedas. Es una tarea repetitiva para el operador y los valores quedan desactualizados. Se busca que se actualicen solas cada hora desde una API externa gratuita, y que el operador solo intervenga en casos especiales.

## What Changes

- Nuevo proceso `ActualizadorTasas` (ejecutable de consola) que una tarea programada de Windows dispara cada hora: consulta Open Exchange Rates (plan gratuito, base USD, 1000 requests/mes) y vuelca el resultado en la base. Los formularios no conocen la API: solo leen de la base.
- Una sola request por corrida trae todas las monedas; los cruces entre pares se calculan localmente.
- El proceso actualiza solo los pares que el operador ya creo y saltea los marcados como manuales. No crea pares.
- Nueva proteccion manual: editar una tasa a mano la marca como manual y la automatica la saltea hasta que el operador la libere con un boton.
- Nuevo historial de ejecuciones y una pantalla "Actualizacion de Tasas" que lo muestra, con las requests consumidas en el mes y un boton "Actualizar ahora" que no consulta a la API si la ultima actualizacion exitosa tiene menos de una hora.
- La API key vive en un `secrets.config` propio del ejecutable (no versionado) con su plantilla `.example`.
- Un script `schtasks` versionado registra la tarea programada en la maquina.

## Capabilities

### New Capabilities
- `actualizacion-automatica-tasas`: obtencion horaria de tasas desde la API externa, calculo de cruces, actualizacion de pares existentes, tope de requests y manejo de fallas.
- `historial-actualizaciones-tasas`: registro de cada ejecucion y pantalla que lo muestra, con requests del mes y el boton "Actualizar ahora".
- `proteccion-tasas-manuales`: marca de tasa manual al editarla a mano, exclusion de la actualizacion automatica y liberacion explicita.

### Modified Capabilities

Ninguna. No existe un spec vigente de monedas o tasas; el comportamiento previo de Gestion de Monedas queda salvo por la marca manual, que se describe en `proteccion-tasas-manuales`. Los requisitos de `configuracion-secretos` y `conexion-base-datos` se cumplen sin cambiarlos.

## Impact

- Esquema: columnas de proteccion manual en `TasaConversion` y tablas nuevas de historial; `script_bd.sql` actualizado.
- Capas: BE, REPOSITORIO, DAL, BLL, ABSTRACCIONES (contrato del proveedor de tasas), SERVICIOS (cliente de la API), `CompositionRoot`, y un proyecto nuevo `ActualizadorTasas` con rol de capa de presentacion, incluido en `TpIDS.sln`.
- UI: nueva pantalla, ajuste de `FormGestionMonedas` (estado manual y boton de liberar), permiso nuevo para Operador y administrador, traducciones.
- Dependencias: HTTP con `System.Net.Http` del framework.
- Documentacion: fuera de este cambio. Los diagramas, casos de uso y el documento de la catedra los cubre `actualizar-diagramas-tp`.
