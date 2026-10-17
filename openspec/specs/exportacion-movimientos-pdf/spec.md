# exportacion-movimientos-pdf Specification

## Purpose
Permitir elegir movimientos del resultado vigente y guardarlos en un PDF legible que identifique a FINAZAPP y conserve el contexto de la consulta realizada.

## Requirements

### Requirement: Seleccion explicita para exportacion
El sistema SHALL ofrecer una casilla por fila, Seleccionar todos, Desmarcar todos y un contador de seleccionados respecto del total de resultados. Las marcas SHALL ser independientes de la fila activa usada para editar o eliminar movimientos.

#### Scenario: Marcas individuales y seleccion masiva
- **WHEN** el usuario marca dos movimientos
- **THEN** el contador indica dos seleccionados y solo esos movimientos son candidatos a exportacion
- **WHEN** presiona Seleccionar todos
- **THEN** se marcan todos los resultados de la busqueda vigente, incluidos los que requieren desplazarse por la grilla
- **WHEN** presiona Desmarcar todos
- **THEN** ninguna fila queda marcada y el contador indica cero

#### Scenario: Fila activa y orden de filas
- **WHEN** el usuario activa una fila para editar o cambia el orden de la grilla
- **THEN** las marcas siguen asociadas a los mismos movimientos y no se confunden con la fila activa

### Requirement: Reinicio de seleccion al recargar resultados
El sistema SHALL iniciar sin marcas cada nueva busqueda o recarga exitosa de movimientos, incluyendo las posteriores a altas, modificaciones y bajas. Con cero seleccionados, Exportar a PDF SHALL estar deshabilitado.

#### Scenario: Nueva busqueda y resultado vacio
- **WHEN** el usuario ejecuta una nueva busqueda exitosa despues de marcar movimientos
- **THEN** la seleccion vuelve a cero y Exportar a PDF queda deshabilitado
- **AND** si no hay resultados, el contador indica cero de cero y Seleccionar todos queda deshabilitado

### Requirement: Contenido del listado exportado
El PDF SHALL contener exclusivamente los movimientos marcados de la consulta del usuario actual, en el orden vigente de la grilla, con las columnas Fecha, Tipo, Categoria, Comercio, Monto y Descripcion. SHALL conservar sus valores y formatos visibles, sin incluir las casillas ni identificadores internos.

#### Scenario: Exportacion de un subconjunto
- **WHEN** el usuario exporta tres filas marcadas de un resultado mayor
- **THEN** el PDF contiene esas tres filas en el orden vigente y las seis columnas de datos
- **AND** no incluye filas sin marcar ni la columna de seleccion

### Requirement: Identidad y contexto del reporte
El PDF SHALL incluir el logo basado en la referencia aportada por el usuario, con marca violeta oscuro, lema "Tus finanzas en un solo lugar" y detalle verde, el titulo del listado, los filtros de la consulta vigente incluyendo el texto buscado, la fecha y hora de generacion y la cantidad exportada.

#### Scenario: Reporte identificable
- **WHEN** el usuario abre el PDF generado
- **THEN** puede identificar FINAZAPP, los criterios usados para obtener el listado y cuantos movimientos fueron exportados
- **AND** los filtros sin restriccion se indican explicitamente como tales

### Requirement: Reporte multipagina legible
El PDF SHALL usar A4 horizontal, repetir los encabezados de la tabla en cada pagina y numerar las paginas. SHALL mostrar descripciones completas mediante ajuste de lineas y, cuando sea necesario, continuaciones identificables, sin recortar texto por los limites de la grilla.

#### Scenario: Muchas filas y descripcion extensa
- **WHEN** el listado requiere varias paginas o contiene descripciones largas
- **THEN** el reporte distribuye el contenido sin superposiciones, repite los encabezados y muestra "Pagina X de Y"
- **AND** todo el texto de cada descripcion permanece legible, incluso si necesita continuar en otra pagina

### Requirement: Guardado elegido por el usuario
Exportar a PDF SHALL abrir Guardar como para elegir carpeta y nombre con extension .pdf. Cancelar SHALL conservar la seleccion sin crear archivos. Un fallo de generacion o escritura SHALL informar el error, conservar la seleccion y no presentar la operacion como exitosa ni dejar un PDF parcial en el destino.

#### Scenario: Guardado y cancelacion
- **WHEN** el usuario confirma una ruta valida
- **THEN** se guarda alli un PDF y se informa el resultado exitoso
- **WHEN** cancela Guardar como
- **THEN** no se genera ningun archivo y las marcas permanecen

#### Scenario: Destino no escribible
- **WHEN** el usuario confirma un destino sin permisos o un archivo bloqueado
- **THEN** se informa el error, se conservan las marcas y un archivo preexistente no se reemplaza por un archivo parcial

### Requirement: Distribucion compatible con el laboratorio
El componente de exportacion SHALL compilar y ejecutarse con .NET Framework 4.7.2 utilizando bibliotecas y dependencias incluidas con el codigo fuente, sin instalar o restaurar NuGet ni depender de su cache, de una impresora PDF, de Office o de servicios externos.

#### Scenario: Componente PDF en entorno limpio
- **WHEN** el componente PDF se compila en un entorno compatible con .NET Framework 4.7.2, sin cache ni restauracion NuGet
- **THEN** resuelve todas sus referencias desde los archivos distribuidos y genera el reporte localmente
