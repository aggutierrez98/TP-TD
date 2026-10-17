# Design

## Context

Hoy `TasaConversion` guarda una fila por par (`MonedaBaseId`, `MonedaConversionId`, `CalculoConversion`, `FechaActualizacion`) con unicidad de par, y `FormGestionMonedas` la edita a mano a traves de `TasaConversionBLL`. No hay historial ni marca de origen. Ningun otro codigo consume `CalculoConversion`, por lo que su significado no esta fijado. Motivacion en proposal.md.

Restricciones: .NET Framework 4.7.2 sin NuGet productivo, matriz de capas, BLL solo desde `CompositionRoot`, secretos en `secrets.config` no versionado, HTTP en `async/await`.

## Goals / Non-Goals

**Goals:**
- Que los formularios solo lean de la base y que un proceso aparte sea el unico que escriba tasas automaticas.
- Cumplir el limite de 1000 requests/mes con margen (~720 a 744 por mes con una por hora).

**Non-Goals:**
- Crear pares o monedas automaticamente.
- Reintentos, colas o servicios residentes; servicio de Windows.
- Cotizaciones por tipo de dolar (blue, oficial): se usa la cotizacion unica de la API.

## Decisions

### 1. Proceso aparte disparado por el Programador de tareas
`ActualizadorTasas` es un ejecutable de consola, incluido en `TpIDS.sln`, que reusa BLL y `CompositionRoot`. En la matriz de capas se trata como la UI: depende de ABSTRACCIONES, BLL, BE y SERVICIOS. Un script `schtasks` versionado lo registra cada hora con ejecucion al arrancar si se perdio una corrida.
Alternativas: timer dentro de la app (solo corre con la app abierta y mezcla escritura en los forms), servicio de Windows (requiere instalacion y permisos), cron externo (no llega a SQL Server local).

### 2. Una request en base USD y cruces locales
Una respuesta trae todas las monedas, asi que el costo no crece con la cantidad de pares. El plan gratuito solo permite base USD. Formula: `valor = cotizacion(conversion) / cotizacion(base)`.
**Decision confirmada por el usuario:** `CalculoConversion` significa "1 unidad de la moneda base = N unidades de la moneda de conversion" (por ejemplo, base USD, conversion ARS, valor 1500: un dolar vale 1500 pesos). El valor que escribe el actualizador debe tener esa misma direccion.

### 3. Logica en BLL, contrato en ABSTRACCIONES
`ActualizacionTasasBLL` orquesta la corrida y es la misma para el ejecutable y para el boton. El contrato `IProveedorTasasCambio` vive en ABSTRACCIONES y su implementacion `OpenExchangeRatesProveedor` en SERVICIOS, cableada en `CompositionRoot`. Permite cambiar de proveedor y usar un doble en tests. El ejecutable no tiene `async Main` en net472: espera la tarea con `GetAwaiter().GetResult()`; el boton usa `async/await` para no bloquear la UI. Codigos de salida del ejecutable (decision mia): 0 = exitosa u omitida por el tope, 1 = fallida registrada en el historial (por ejemplo clave faltante o rechazo de la API), 2 = error inesperado antes o fuera del registro (base inaccesible, cancelacion); en consola solo se informa el mensaje, sin stack trace. No usa Mutex: el tope se revalida dentro de la transaccion con bloqueo, asi que dos corridas superpuestas no pueden consultar ambas a la API.

### 4. Tope unificado y consumo contado de forma conservadora
Una corrida consulta el ultimo exito del historial y se omite si desde el inicio de ese ultimo exito pasaron menos de 60 minutos, tanto para la programada como para el boton. Se compara contra la hora de inicio de la corrida (no la de fin) para que la tarea horaria no se omita por los segundos que tarda la consulta. La verificacion se repite dentro de la transaccion con bloqueo para que el ejecutable y el boton no consulten a la vez. Toda consulta intentada cuenta como consumida, aunque falle. El mes es el calendario; la facturacion real de la API puede cortar en otra fecha, por eso el contador es una estimacion. Referenciar el inicio de la corrida es decision mia.

### 5. Modelo de datos
- `TasaConversion`: columnas `EsManual BIT NOT NULL DEFAULT 0`, `ProtegidaPorUsuarioId INT NULL`, `FechaProteccion DATETIME NULL` (esquema en `script_bd.sql`).
- `EjecucionActualizacionTasas`: fecha, origen (Programada o Manual), resultado, `ConsumioRequest`, mensaje.
- `ActualizacionTasaDetalle`: ejecucion, par, valor anterior, valor nuevo, estado.
La actualizacion automatica escribe con una sentencia que no toca `EsManual`, y salta los manuales en la BLL. Editar a mano pasa por `TasaConversionBLL.Actualizar`, que ahora fija `EsManual = 1`. Crear un par lo deja en 0 (decision mia: el par nuevo debe poder actualizarse solo).

### 6. Trazabilidad de la proteccion
Proteger y liberar se registran en la bitacora existente con el usuario de la sesion. **Suposicion mia:** alcanza la bitacora y no hace falta una tabla propia; las ejecuciones programadas no usan bitacora porque no tienen usuario, su rastro es el historial.

### 7. Secretos y configuracion
El ejecutable tiene su `App.config` con la cadena `TpIDS` y un `secrets.config` junto al `.exe` (ignorado por git) con plantilla `secrets.config.example`; falta de clave se registra como falla sin consultar. El JSON de la API se lee con `System.Text.Json`, igual que el resto de `SERVICIOS` (por ejemplo `GeminiChatClient`). Se fuerza TLS 1.2 en el cliente HTTP.

### 8. Permiso y traducciones
Nuevo permiso para la pantalla, asignado al grupo Operador y al administrador desde el inicio, con sus textos en `translation-seeds/base.es.json` y `base.in.json` y la sincronizacion de traducciones a ambas bases.

## Risks / Trade-offs

- [Tarea no registrada o maquina apagada] → el historial muestra la fecha de la ultima actualizacion y el boton sirve de respaldo.
- [Margen de ~250 requests] → un solo tope de 60 minutos para ejecutable y boton, sin reintentos.
- [Precision `DECIMAL(18,6)` en cruces de valores muy chicos] → se redondea a 6 decimales; un valor que redondea a cero se salta por no positivo.
- [Dos procesos escribiendo a la vez] → verificacion del tope dentro de la transaccion con bloqueo.
- [Plan gratuito sujeto a cambios del proveedor] → proveedor detras de un contrato intercambiable.

## Migration Plan

Aplicar el esquema con `ALTER TABLE` y `CREATE TABLE` en `TpCatedraIDS` y `TpCatedraIDS_Test`, y reflejarlo en `script_bd.sql`; los pares existentes quedan con `EsManual = 0`. Registrar la tarea con el script en cada maquina. Reversion: eliminar la tarea programada y las tablas nuevas; el resto sigue funcionando.

## Open Questions

- Ninguna.
