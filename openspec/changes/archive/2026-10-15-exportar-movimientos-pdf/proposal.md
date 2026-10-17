# Proposal

## Why

El usuario necesita guardar y compartir un listado de movimientos elegidos desde Gestion de movimientos, con la identidad visual de FINAZAPP y los criterios que originaron la consulta. La pantalla no permite marcar filas para exportacion ni generar ese reporte, y tampoco expone el filtro por descripcion que ya admite la consulta.

## What Changes

- Agregar un buscador de coincidencias parciales exclusivamente en Descripcion, combinado con fecha desde/hasta, categoria y tipo, aplicado al presionar Buscar.
- Agregar una casilla por movimiento, Seleccionar todos, Desmarcar todos y un contador de seleccionados respecto del total de resultados. Cada busqueda o recarga exitosa reinicia las marcas.
- Incorporar Exportar a PDF, habilitado cuando hay movimientos marcados, con dialogo Guardar como.
- Exportar las columnas Fecha, Tipo, Categoria, Comercio, Monto y Descripcion, respetando el orden y formato del listado y excluyendo la columna de casillas.
- Incluir el logo basado en la imagen aportada por el usuario (marca violeta oscuro, lema y detalle verde), filtros de la ultima busqueda ejecutada, fecha de generacion y cantidad exportada.
- Usar A4 horizontal, encabezados de tabla repetidos, descripciones completas y paginas numeradas.
- Distribuir las DLL de PDFsharp/MigraDoc compatibles con .NET Framework 4.7.2 y sus dependencias/licencias dentro del repo, con referencias relativas directas y sin restore NuGet productivo. El usuario autorizo expresamente esta distribucion.

## Capabilities

### New Capabilities

- `consulta-movimientos`: busqueda por descripcion y representacion de los filtros correspondientes al resultado vigente.
- `exportacion-movimientos-pdf`: seleccion explicita de resultados y generacion de un reporte PDF identificable, legible y guardable por el usuario.

### Modified Capabilities

Ninguna. No hay una capacidad vigente de consulta o exportacion de movimientos en `openspec/specs/`.

## Impact

- `UI/FormMovimientos.{cs,Designer.cs,resx}`: buscador, marcas independientes de la fila activa, captura de la consulta, acciones y dialogo.
- `BLL/MovimientoBLL.cs` y `BLL/CompositionRoot.cs`: coordinacion y validacion del reporte, inyeccion del generador PDF.
- Contratos de reporte en `ABSTRACCIONES`, modelo de solicitud de negocio en `BE` y generador concreto en `SERVICIOS`; referencias explicitas en los `.csproj` afectados.
- `DAL/MovimientoDAL.cs` solo si la busqueda necesita escapar comodines para implementar coincidencias parciales literales; consulta parametrizada, sin cambios de esquema.
- Nuevo recurso grafico del reporte, `translation-seeds/base.es.json`, tests unitarios/integracion y pruebas UI posteriores al desarrollo.
- Bibliotecas versionadas bajo `lib/`, licencias y manifiesto de procedencia. La portabilidad integral depende ademas de resolver los `PackageReference` productivos preexistentes; este cambio no los elimina por su cuenta.
- Paridad documental del TP: una tarea de este cambio coordinara su reflejo con `actualizar-diagramas-tp` y los artefactos de entrega correspondientes.
