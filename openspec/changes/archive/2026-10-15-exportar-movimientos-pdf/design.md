# Design

## Context

Ver `proposal.md` para la motivacion y `specs/` para los comportamientos. El usuario acordo casillas, seleccion masiva, contador, reinicio al buscar, las seis columnas actuales, PDF A4 horizontal y Guardar como. Solicito ademas un buscador exclusivamente por Descripcion y un logo basado en su imagen; autorizo distribuir DLL compatibles con .NET Framework 4.7.2 dentro del repo.

Estado observado:
- `FormMovimientos.RefrescarDatos` consulta la BLL y proyecta una lista de `FilaMovimiento`; tiene Id interno y las seis columnas visibles. No asigna `FiltroMovimiento.Texto`.
- `FiltroMovimiento.Texto` ya existe y `MovimientoDAL.ObtenerPorUsuario` aplica `Descripcion LIKE '%' + @Texto + '%'`, parametrizado. Hay un test de integracion de busqueda por descripcion. Los metacaracteres LIKE hoy se interpretan como comodines.
- El clic en la grilla carga la fila activa en el formulario de edicion. Debe distinguirse de las marcas para exportar.
- `UI/Resources/logo_finanzapp.png` es una imagen diferente de la referencia del usuario: marca verde y monedas. No sirve como reemplazo de la referencia nueva.
- La solucion productiva tiene `PackageReference` preexistentes en UI y SERVICIOS. `migrar-json-system-text-sin-nuget` aborda JSON, no toda la UI. La autorizacion de DLL para este reporte no cambia otros planes en curso.
- C# 7.3, WinForms/Krypton, .NET Framework 4.7.2 y la matriz de siete capas son obligatorios. SERVICIOS no puede referenciar BE ni UI.

## Goals / Non-Goals

**Goals:**
- Generar un reporte de la consulta que el usuario esta viendo, sin mezclar criterios editados pero no ejecutados.
- Separar seleccion, negocio, generacion PDF y escritura del archivo mediante contratos e inyeccion manual.
- Resolver tablas, Unicode, ajuste de texto y paginacion con una biblioteca adecuada y referencias reproducibles.

**Non-Goals:**
- Reportes de totales/balance, otros formatos, envio por correo o impresion directa.
- Cambiar el logo general de la aplicacion o agregar nuevos permisos/registro de bitacora para exportar.
- Reemplazar las dependencias NuGet preexistentes de mapas, graficos, tema o JSON dentro de este cambio.
- Exportar todos los movimientos historicos cuando el usuario marca todos: el alcance es el resultado vigente.

## Decisions

### 1. PDFsharp + MigraDoc GDI 6.2.4 con distribucion directa

Eleccion tecnica del autor, pendiente de aprobacion del diseño completo: usar `PDFsharp-MigraDoc-GDI` 6.2.4 y `PDFsharp-GDI` 6.2.4, variantes `net462`, compatibles con net472. MigraDoc proporciona tablas, ajuste de parrafos y saltos de pagina; PDFsharp renderiza el PDF con texto seleccionable. Evita un generador PDF casero y la dependencia de una impresora instalada. Frente a PDFsharp solo, MigraDoc reduce el trabajo de maquetacion. La variante GDI coincide con Windows y las fuentes disponibles en WinForms.

Las fuentes verificadas son:
- https://www.nuget.org/packages/PDFsharp-MigraDoc-GDI/6.2.4
- https://www.nuget.org/packages/PDFsharp-GDI/6.2.4
- https://docs.pdfsharp.net/General/Overview/Introduction.html

Ambos paquetes declaran licencia MIT y activos net462. Declaran dependencias `Microsoft.Extensions.Logging.Abstractions >= 8.0.3` y `System.Security.Cryptography.Pkcs >= 8.0.1`; MigraDoc depende ademas de PDFsharp-GDI. Durante la incorporacion se fijaran esas versiones y se cerrara el grafo transitivo inspeccionando los ensamblados efectivos: el inventario no se limita a las DLL principales. Registrar versiones resueltas, origen, licencias y hashes SHA-256 en `lib/pdfsharp-migradoc/6.2.4/manifest.md`, y guardar todas las DLL necesarias en esa carpeta, incluyendo las transitivas compatibles elegidas.

Se pueden obtener los binarios del paquete oficial durante el desarrollo, sin añadir PackageReference ni un paso de restore. `SERVICIOS.csproj` utilizara `Reference` y `HintPath` relativos con copia local; los proyectos que necesiten ensamblados transitivos los referenciaran tambien desde `lib/`. Ninguna referencia apuntara al cache del desarrollador. No usar variantes WPF ni binarios net8/net10.

### 2. Buscar por descripcion reutilizando el filtro existente

Agregar `txtbDescripcionFiltro` y su etiqueta traducible al grupo de filtros. Buscar combina el texto con los cuatro filtros actuales; Enter en el campo ejecuta la misma accion. Recortar espacios exteriores; vacio o blanco equivale a null. La coincidencia de mayusculas y acentos conserva la collation actual de SQL Server: no se agrega una nueva promesa de insensibilidad a acentos que el usuario no definio.

Decision del autor: los caracteres `%`, `_` y `[` se trataran literalmente, escapandolos en el parametro de LIKE mediante un escape explicito, incluido el propio caracter de escape. Este ajuste requiere TDD de integracion en DAL/REPOSITORIO. No buscar en comercio o categoria. No introducir busqueda en cada pulsacion ni filtrado adicional en memoria.

### 3. Marcas por identidad, independientes de la fila activa

Agregar una propiedad booleana a la fila o un conjunto de Ids en el formulario; elegir la variante mas sencilla consistente con el binding existente al implementar. La casilla `colMovimientosSeleccionar` es la unica columna editable; las seis columnas de datos siguen siendo de solo lectura. Confirmar el cambio de la casilla al hacer clic, antes de recalcular contador o exportar.

Seleccionar todos recorre todo el resultado cargado, no solamente la zona visible. Desmarcar todos limpia las marcas. El contador muestra N de M. Exportar requiere N > 0; Seleccionar todos requiere M > 0 y Desmarcar todos requiere N > 0. El clic en casillas no debe disparar la carga de campos de edicion; los demas clics conservan el comportamiento de fila activa. Si se reordena la grilla, las marcas siguen a los Ids y el reporte recorre las filas en su orden actual. No agregar un ordenamiento nuevo si el binding actual no lo admite.

Toda consulta/recarga exitosa reemplaza filas y criterios juntos y deja cero marcas, incluso despues de CRUD. Para conservar el resultado ante un error, obtener movimientos, nombres de categorias y la nueva proyeccion antes de sustituir el DataSource. Una busqueda fallida conserva el resultado, criterios y marcas anteriores.

### 4. Captura de resultados y criterios, no una consulta nueva al exportar

Guardar una copia del filtro aplicado y su categoria visible junto a las filas del ultimo resultado exitoso. Al exportar, capturar en el hilo UI las filas marcadas, su orden, valores formateados, encabezados traducidos, criterios aplicados, usuario de la consulta, logo y fecha/hora. No leer controles desde un hilo de trabajo.

La solicitud de reporte de movimientos en BE contiene esta captura y el usuario al que corresponde; no contiene controles ni tipos de PDFsharp. BLL valida que la sesion actual coincide con el usuario de la captura, que existen filas y que la estructura tiene las seis columnas esperadas, y llama al generador por interfaz. La captura deriva exclusivamente del resultado obtenido para el usuario autenticado. No aceptar una ruta de archivo ni consultar movimientos de otro usuario en este flujo.

Esta decision evita reconsultar la base al exportar: una modificacion externa no cambiara silenciosamente el listado que el usuario selecciono. Los criterios se toman del filtro aplicado, no de los controles actualmente editados. El reporte mantiene el idioma y formato visible vigentes al exportar; el texto de marca y lema son identidad grafica fija.

### 5. Contrato tabular y responsabilidades por capa

- UI: filtros, marcas, captura, Guardar como y persistencia del archivo local.
- BLL: caso de uso de exportacion, validaciones anteriores y conversion de la solicitud de negocio al contrato tabular; devuelve bytes PDF.
- BE: modelo de solicitud/captura de movimientos, sin dependencia de SERVICIOS.
- ABSTRACCIONES: `IReporteTabularPdfService` y, si hacen falta, interfaces de entrada tabular compuestas exclusivamente de contratos y tipos del framework. Exponen titulo, encabezados, filas de texto, filtros, imagen y fecha, sin entidades BE ni tipos concretos del motor.
- SERVICIOS: `ReporteTabularPdfService`, implementacion MigraDoc/PDFsharp. Solo conoce ABSTRACCIONES y las bibliotecas externas; devuelve bytes y no abre dialogos ni escribe archivos finales.
- REPOSITORIO/DAL: solamente la consulta existente y el escape literal del filtro; ninguna persistencia nueva para exportar.

Inyectar el servicio en el constructor internal de MovimientoBLL y construirlo desde CompositionRoot. Actualizar las fabricas/fakes de tests del constructor; no introducir constructores por defecto ni service locator en la BLL.

### 6. Identidad visual y maquetacion

Crear un recurso exclusivo del reporte basado en la imagen aportada: marca en violeta oscuro `#3E3A4E`, lettering pesado/condensado, lema en cursiva y detalle organico verde. La referencia dice FINANZAPP; respetar la grafica proporcionada, manteniendo FINAZAPP como nombre del producto en los textos existentes. No sustituir el logo general. La imagen de la conversacion no se encontro como archivo del repo: producir una adaptacion del encabezado y validarla visualmente con el usuario antes de cerrar la implementacion. Este diseño no exige una reproduccion exacta de la textura.

Recurso raster de resolucion suficiente o imagen compatible con el motor, conservando proporcion y fondo blanco/transparente. Pasarlo como bytes desde UI; SERVICIOS no carga recursos de UI ni depende de una ruta absoluta. No depender de la fuente del logo instalada en el laboratorio: el lettering se incorpora como imagen.

Hoja A4 horizontal, margenes aproximados de 12 mm y tabla de seis columnas. Distribucion inicial, ajustable por el implementador: Fecha 10%, Tipo 9%, Categoria 15%, Comercio 17%, Monto 13%, Descripcion 36%. Cuerpo Segoe UI o fuente Windows equivalente disponible, 9-10 pt, Unicode; monto alineado a la derecha. Encabezado de marca/titulo y pie con Pagina X de Y; filtros completos y cantidad exportada antes de la tabla. Explicitar Sin limite/Todas/Todos/Sin texto cuando un filtro no restringe el resultado. Repetir encabezados de tabla por pagina.

No truncar descripciones: ajuste de lineas y altura dinamica. Si una fila supera el area imprimible de una pagina, dividir su descripcion en continuaciones identificadas como tales, manteniendo una unica cuenta de movimientos. Esta predivision evita confiar en que MigraDoc pueda dividir automaticamente una fila arbitrariamente alta. Probar esta condicion expresamente.

### 7. Guardado y errores

`SaveFileDialog` con filtro PDF, extension automatica y nombre sugerido `FINAZAPP-movimientos-aaaaMMdd-HHmmss.pdf`. Cancelar no inicia generacion. Mantener las marcas despues del guardado, la cancelacion o un error. No abrir automaticamente un visor; informar exito con la ruta.

Despues de confirmar destino, capturar los datos y generar los bytes en trabajo de fondo para no bloquear el formulario. Deshabilitar temporalmente las acciones que pueden modificar la captura y restaurarlas en finally, siguiendo el patron de estado de escaneo. No añadir cancelacion de proceso ni barras de progreso como nuevo alcance.

Escribir un temporal unico en la misma carpeta y luego moverlo al destino; si ya existe y se confirmo sobrescritura, reemplazarlo atomicamente con el mecanismo compatible de Windows. Ante un error, limpiar el temporal y conservar el original, sin reportar exito. Los errores tecnicos y mensajes visibles siguen los helpers y claves traducibles del proyecto.

### 8. Validacion y paridad documental

TDD estricto para los nuevos comportamientos de BLL/SERVICIOS y el escape de DAL: baseline, fallo observado, implementacion y verde. Unit tests verifican validacion, contenido seleccionado y estructura PDF; integracion verifica coincidencia parcial literal y filtros combinados. Abrir/renderizar un reporte de muestra verifica logo, paginacion y ajuste de texto; no basta comprobar `%PDF`.

Tests FlaUI se escriben despues del desarrollo y cubren buscador, marcas, reinicio, seleccion masiva y coexistencia con edicion; su ejecucion completa requiere el aviso y autorizacion de escritorio definidos por la skill del repo. Cierre con MSBuild de TpIDS.sln y `tests/tools/run-tests.ps1 -IncludeIntegration`.

La dependencia PDF se comprobara adicionalmente sin cache ni restore en un entorno net472 limpio. La portabilidad integral de TpIDS.sln solo se acredita cuando se hayan resuelto tambien las dependencias productivas preexistentes: registrar ese trabajo en un cambio de portabilidad propio al aplicar si no existe uno que lo cubra, sin declarar exito integral por un build con cache.

El reflejo en diagramas/documentacion se coordina mediante la tarea de este cambio y `actualizar-diagramas-tp`, con el flujo TP a demanda y aprobacion del usuario antes de editar esos artefactos.

## Risks / Trade-offs

- [Dependencias transitivas omitidas o variantes incorrectas] → Inventario fijado con hashes/licencias y comprobacion de carga/compilacion sin cache.
- [PDF visualmente distinto de la referencia] → Adaptacion de recurso exclusiva y validacion visual antes del cierre.
- [Casillas y fila activa interfieren] → Ignorar columna de marcas en el handler de edicion y pruebas de coexistencia.
- [Filas muy altas exceden una pagina] → Continuaciones explicitas y prueba de descripcion mayor que una pagina.
- [Cambios concurrentes en CompositionRoot y .csproj] → Revisar el diff existente al aplicar y añadir solamente el wiring/referencias de este cambio.
- [Portabilidad global aun incumplida por dependencias anteriores] → Verificacion separada y seguimiento OpenSpec, sin afirmar que la nueva biblioteca corrige toda la solucion.

## Migration Plan

1. Aprobar este diseño, incorporar y verificar las DLL/licencias y el recurso de marca al implementar.
2. Desarrollar filtro literal y caso de exportacion con TDD, luego controles UI y pruebas de regresion.
3. Verificar build, suite con integracion, reportes de muestra y portabilidad del componente PDF.
4. Coordinar paridad documental y archivar solamente cuando todas las tareas esten verificadas.

No hay migracion de base de datos. Para revertir, retirar solamente el nuevo filtro UI, acciones, contratos, generador y referencias PDF de este cambio; conservar el trabajo ajeno y el filtro Texto previamente existente.
