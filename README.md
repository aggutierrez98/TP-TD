# TpIDS

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

## Actualización automática de tasas

Las tasas de conversión se actualizan cada hora desde Open Exchange Rates (plan gratuito,
1.000 requests por mes). Lo hace `ActualizadorTasas.exe`, un programa de consola que
dispara el Programador de tareas de Windows; los formularios solo leen de la base.

1. Creá una cuenta gratuita en <https://openexchangerates.org/signup/free> y copiá el
   App ID.
2. Copiá `ActualizadorTasas/secrets.config.example` como `ActualizadorTasas/secrets.config`
   y completá `OpenExchangeRatesApiKey`. El archivo está ignorado por git. Si lo creás
   después de compilar, copialo también junto al `.exe`.
3. Compilá `TpIDS.sln` (el `.exe` queda en `ActualizadorTasas/bin/<Configuración>/`).
4. Registrá la tarea horaria desde `ActualizadorTasas/scripts/`:
   `.\registrar-tarea.ps1` (usa Release; con `-Configuracion Debug` toma el build de Debug).
   La tarea se llama `FINAZAPP-ActualizadorTasas`, corre con tu usuario y solo mientras
   tenés la sesión iniciada. Para quitarla: `.\eliminar-tarea.ps1`.

El `.exe` solo actualiza los pares de monedas que ya existen y saltea los editados a mano
hasta que se liberan. No consulta la API si la última corrida exitosa empezó hace menos de
60 minutos. Cada corrida queda en el historial de la pantalla **Actualización de Tasas**.
Códigos de salida: `0` exitosa u omitida por el tope, `1` falla registrada en el historial,
`2` error inesperado. Sin la clave, la corrida se registra como falla y no consume requests.

## Datos de prueba

`script_datos_prueba.sql` carga un conjunto de datos de demostración del mercado argentino
(relevado en octubre de 2026): rubros, categorías, entidades financieras, medios de pago,
comercios, productos con precios, promociones y, para el usuario `user`, cuentas, categorías
propias, movimientos y reglas de categorización.

1. Ejecutá `script_bd.sql` (crea la base y los usuarios).
2. Ejecutá `script_datos_prueba.sql`. Es idempotente: se puede repetir sin duplicar filas.
   Con `sqlcmd`: `sqlcmd -S .\SQLEXPRESS -E -C -b -i script_datos_prueba.sql`.

No lo ejecutes sobre `TpCatedraIDS_Test`: algunos tests de integración parten de catálogos
vacíos. El script no toca `Usuarios`, así que no afecta los dígitos verificadores.

El script se edita directamente; al modificarlo, mantené la idempotencia (cada inserción
resuelve sus relaciones por nombre y no duplica filas).
