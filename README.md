# TpIDS

## Preparar la base de datos (PC local o laboratorio)

La app usa solo la cadena `TpIDS` del `App.config` (`.\SQLEXPRESS` por defecto); no prueba
otras instancias. La base se crea en la misma instancia que esa cadena.

1. Verificá los permisos del login en la instancia (necesita `sysadmin`, o `dbcreator` y `bulkadmin`):
   `sqlcmd -S .\SQLEXPRESS -E -Q "SELECT IS_SRVROLEMEMBER('sysadmin'), IS_SRVROLEMEMBER('bulkadmin')"`.
2. Desde la raíz del repo: `.\setup-database.ps1`. Si la base ya existe, agregá `-Force`
   (la borra y recrea). Si la instancia es otra, `-SqlServer .` (o la que corresponda), y
   cambiá `Data Source` en el `.exe.config` para que la app apunte a la misma.
3. Cargá los datos de demostración (ver "Datos de prueba").

Alternativa sin PowerShell: abrir `script_bd.sql` en SSMS con **SQLCMD Mode** activado
y cambiar `:setvar RepoRoot "."` por la ruta absoluta del repo. El script lee
`translation-seeds\base.es.json` y `base.in.json` directo desde esa ruta, por lo que la cuenta del
servicio de SQL Server debe poder leer esa carpeta (por ejemplo, un repo fuera del perfil de otro usuario).
`setup-database.ps1` verifica al final que el idioma y las traducciones hayan quedado cargados.

## Exportar movimientos a PDF

1. En **Movimientos**, combiná los filtros de fechas, categoría y tipo con el campo
   **Descripción**. Presioná **Buscar** o Enter desde ese campo. La búsqueda por texto
   se aplica solo a la descripción; `%`, `_` y `[` se buscan literalmente.
2. Marcá las casillas de los movimientos que querés exportar, o usá **Seleccionar
   todos** para marcar todos los resultados, incluidos los que quedan fuera de la
   parte visible de la grilla. **Desmarcar todos** limpia las marcas.
3. Presioná **Exportar a PDF** y elegí carpeta y nombre en **Guardar como**.

El reporte incluye las seis columnas del listado y los filtros de la última búsqueda
exitosa. Editar filtros sin volver a buscar no cambia ese contexto. Una nueva búsqueda
o recarga exitosa reinicia las marcas; cancelar o guardar el PDF las conserva.

El PDF usa A4 horizontal, encabezados repetidos, páginas numeradas y continuaciones
para las descripciones extensas. Las bibliotecas del generador y sus licencias se
distribuyen en `lib/pdfsharp-migradoc/6.2.4/`; su inventario está en `manifest.md`.

## Tareas programadas (CRONS)

`CRONS.exe` es el programa de consola que dispara el Programador de tareas de Windows. Una
invocación ejecuta una sola tarea, indicada por argumento: `CRONS.exe tasas` (actualización
automática de tasas) o `CRONS.exe notificaciones` (notificaciones automáticas diarias). Sin
argumento o con una tarea desconocida informa las tareas válidas por stderr y sale con `2`.
Ambas tareas comparten `CRONS/App.config` (cadena de conexión `TpIDS`, copiada como
`CRONS.exe.config`; debe apuntar a la misma base que la aplicación) y `CRONS/secrets.config`.

Preparación común:

1. Copiá `CRONS/secrets.config.example` como `CRONS/secrets.config` y completá las claves de
   las tareas que uses (`OpenExchangeRatesApiKey` y `GeminiApiKey`). El archivo está ignorado
   por git. Si lo creás después de compilar, copialo también junto al `.exe`. Si venís de los
   programas anteriores (`ActualizadorTasas.exe` y `GeneradorNotificaciones.exe`), fusioná sus
   `secrets.config` en este y eliminá sus tareas registradas antes de registrar las nuevas.
2. Compilá `TpIDS.sln` (el `.exe` queda en `CRONS/bin/<Configuración>/`).
3. Registrá cada tarea desde `CRONS/scripts/`:
   `.\registrar-tarea.ps1 -Tarea tasas` y `.\registrar-tarea.ps1 -Tarea notificaciones` (usan
   Release; con `-Configuracion Debug` toman el build de Debug, con `-RutaExe <ruta>` un `.exe`
   específico; admiten rutas con espacios). Repetir el comando reemplaza la tarea, no la
   duplica. Para quitarlas: `.\eliminar-tarea.ps1 -Tarea tasas|notificaciones`.

## Actualización automática de tasas

Las tasas de conversión se actualizan cada hora desde Open Exchange Rates (plan gratuito,
1.000 requests por mes). Lo hace `CRONS.exe tasas`; los formularios solo leen de la base.

1. Creá una cuenta gratuita en <https://openexchangerates.org/signup/free> y copiá el
   App ID; va en `OpenExchangeRatesApiKey` del `secrets.config` de CRONS (ver arriba).
2. Registrá la tarea horaria con `.\registrar-tarea.ps1 -Tarea tasas`. Se llama
   `FINAZAPP-ActualizadorTasas`, corre con tu usuario y solo mientras tenés la sesión
   iniciada.

La tarea solo actualiza los pares de monedas que ya existen y saltea los editados a mano
hasta que se liberan. No consulta la API si la última corrida exitosa empezó hace menos de
60 minutos. Cada corrida queda en el historial de la pantalla **Actualización de Tasas**.
Códigos de salida: `0` exitosa u omitida por el tope, `1` falla registrada en el historial,
`2` error inesperado. Sin la clave, la corrida se registra como falla y no consume requests.

## Notificaciones automáticas diarias

`CRONS.exe notificaciones` genera, para cada cuenta activa, hasta tres avisos en la bandeja de **Mis notificaciones**: recordatorio de movimientos no
cargados (ayer), compras pendientes consolidadas e inversiones (perfil cómodo o alto, como
máximo una cada siete días). Corre sin sesión de FINAZAPP: la tarea de Windows lo ejecuta aunque
la aplicación esté cerrada.

1. Completar `GeminiApiKey` en el `secrets.config` de CRONS (la misma de `UI/secrets.config`;
   ver arriba). Sin la clave, las compras y las inversiones quedan como fallas en el registro
   y el recordatorio de movimientos se genera igual. El modelo se cambia con `GeminiModel` en
   `CRONS/App.config`.
2. Registrar la tarea con `.\registrar-tarea.ps1 -Tarea notificaciones`.

La tarea se llama `FINAZAPP-GeneradorNotificaciones`: corre todos los días a las 07:00 y al
iniciar sesión, con el usuario actual y sin guardar contraseña, con la carpeta del `.exe`
como directorio de trabajo, y no superpone instancias. Requiere la sesión de Windows
iniciada; con el equipo apagado o sin sesión no corre. Si a las 07:00 el equipo estaba
apagado, la tarea corre cuando Windows puede ejecutarla (`StartWhenAvailable`) o al iniciar
sesión. Solo se procesa la jornada del día; no se recuperan los días perdidos. Antes de las
07:00 el programa no genera nada.

Reintento manual (por ejemplo, tras corregir la clave o la conexión): ejecutar
`Start-ScheduledTask -TaskName FINAZAPP-GeneradorNotificaciones` o directamente `CRONS.exe notificaciones`.
Repetir una corrida es seguro: no duplica avisos de la jornada y reintenta solo lo que falló;
las inversiones respetan los siete días desde la última notificación automática de ese tipo.
Se puede cancelar con Ctrl+C: la corrida se detiene entre unidades de trabajo y sale con `2`.

Códigos de salida (columna *Last Run Result* del Programador de tareas): `0` sin fallas (las
omisiones no son fallas), `1` fallas parciales registradas en la corrida, `2` error que
impidió la corrida (base no disponible, configuración inválida o cancelación; el mensaje sale
por stderr).

### Diagnóstico

Cada corrida queda en las tablas `CorridaGeneracionNotificaciones` (cantidades, inicio y fin
UTC) y `DetalleGeneracionNotificacion` (usuario, tipo, resultado `Entregada`/`Omitida`/`Fallida`
y motivo estable). No guardan prompts, secretos ni respuestas completas.

```
sqlcmd -S .\SQLEXPRESS -E -C -d TpCatedraIDS -Q "SELECT TOP 10 * FROM CorridaGeneracionNotificaciones ORDER BY InicioUtc DESC"
sqlcmd -S .\SQLEXPRESS -E -C -d TpCatedraIDS -Q "SELECT UsuarioId, TipoAutomatico, Resultado, Motivo FROM DetalleGeneracionNotificacion WHERE CorridaId = (SELECT MAX(Id) FROM CorridaGeneracionNotificaciones) AND Resultado <> 'Entregada'"
```

Estado de la tarea: `Get-ScheduledTaskInfo -TaskName FINAZAPP-GeneradorNotificaciones`
(`LastRunTime`, `LastTaskResult`).

## Datos de prueba

### Educación y recomendaciones del asistente

«¿Qué es un plazo fijo?» y «¿Qué riesgos tienen las acciones?» son consultas
educativas: no necesitan perfil inversor ni porcentaje. «¿En qué me conviene
invertir?» es una recomendación personal. Las consultas mixtas, como «¿Qué es
un CEDEAR y me conviene comprarlo con mis ahorros?», también siguen el flujo
personal, que exige completar el perfil desde Mi Configuración. Los tests con
respuestas HTTP simuladas verifican el contrato y el ruteo; no prueban la
exactitud semántica del clasificador real.

El porcentaje a invertir es opcional. Sin porcentaje («¿En qué invierto lo que me sobra?»)
el asistente recomienda alternativas compatibles con el perfil y la situación financiera,
sin monto ni porcentaje, y no pide ningún dato adicional. Con porcentaje («quiero invertir
el 20%») debe estar entre 0 (excluido) y 100, y calcula el monto sobre el saldo mensual
promedio positivo. Un valor inválido («150%») pide corregirlo y no calcula nada. La
recomendación de inversiones incluye el aviso legal y las fuentes no disponibles; la de compras
analiza hasta 20 items pendientes e informa cuántos quedaron sin analizar.

`script_datos_prueba.sql` carga un conjunto de datos de demostración del mercado argentino
(relevado en octubre de 2026): rubros, categorías, entidades financieras, medios de pago,
comercios, productos con precios, promociones y, para el usuario `user`, cuentas, categorías
propias, movimientos, reglas de categorización y tres notificaciones de ejemplo.

1. Creá la base con el esquema: `.\setup-database.ps1` (ver "Preparar la base de datos").
2. Ejecutá `script_datos_prueba.sql`. Es idempotente: se puede repetir sin duplicar filas.
   Con `sqlcmd`: `sqlcmd -S .\SQLEXPRESS -E -C -b -i script_datos_prueba.sql`.

Al iniciar sesión como `user`, **Mis notificaciones** muestra tres avisos sin leer
(compras, inversiones y movimientos) rotulados «EJEMPLO DE DEMOSTRACIÓN», con el mensaje
completo. No requieren API keys, red ni haber corrido `CRONS.exe notificaciones`. No
cuentan como notificaciones automáticas: no ocupan el cupo diario ni reinician los siete
días de inversiones. Al repetir el script no se duplican ni se restablece su lectura.

No lo ejecutes sobre `TpCatedraIDS_Test`: algunos tests de integración parten de catálogos
vacíos. El script no toca `Usuarios`, así que no afecta los dígitos verificadores.

El script se edita directamente; al modificarlo, mantené la idempotencia (cada inserción
resuelve sus relaciones por nombre y no duplica filas).
