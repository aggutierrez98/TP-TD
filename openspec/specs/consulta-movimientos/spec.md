# consulta-movimientos Specification

## Purpose
Permitir consultar los movimientos propios mediante filtros combinados, incluida la descripcion, y asociar cada resultado con los criterios que efectivamente lo produjeron.

## Requirements

### Requirement: Busqueda parcial por descripcion
El sistema SHALL ofrecer un campo de texto que, al ejecutar Buscar, encuentre coincidencias parciales literales exclusivamente en Descripcion y se combine con fecha desde, fecha hasta, categoria y tipo. Un texto vacio o compuesto solo por espacios SHALL omitir este filtro.

#### Scenario: Coincidencia en descripcion y filtros combinados
- **WHEN** el usuario busca "super" con tipo Egreso y un rango de fechas
- **THEN** se muestran exclusivamente sus movimientos que cumplen esos filtros y cuya descripcion contiene "super"
- **AND** un movimiento con "super" solamente en Comercio o Categoria no se incorpora por ese motivo

#### Scenario: Texto vacio y comodines literales
- **WHEN** el usuario busca un texto vacio o compuesto solo por espacios
- **THEN** la consulta aplica solamente los demas filtros
- **WHEN** busca una descripcion que contiene "%", "_" o "["
- **THEN** esos caracteres se interpretan como texto y no como comodines de busqueda

### Requirement: Criterios asociados al resultado vigente
El sistema SHALL conservar los criterios de la ultima consulta exitosa junto con sus resultados. Editar filtros sin ejecutar Buscar SHALL mantener los criterios asociados al listado. Una consulta fallida SHALL conservar el resultado anterior y sus criterios juntos.

#### Scenario: Campos editados sin nueva consulta
- **WHEN** el usuario cambia el texto o las fechas sin ejecutar Buscar
- **THEN** los movimientos visibles siguen asociados a los criterios de la consulta anterior
- **AND** un reporte de esos movimientos muestra esos criterios anteriores

#### Scenario: Consulta fallida
- **WHEN** una nueva busqueda falla
- **THEN** se informa el error y se conservan juntos el listado anterior, sus marcas y sus criterios
