# validacion-uso-en-bajas Specification

## Purpose
Garantizar que toda baja de una entidad referenciable decida por una consulta de uso propia, previa al borrado, y no por el error de clave foranea de la base de datos.

## Requirements

### Requirement: Validacion de uso antes de borrar
Toda baja de una entidad que otras entidades pueden referenciar SHALL consultar su uso con una consulta propia, previa a la sentencia de borrado, y bloquear la baja si hay uso. El sistema MUST NOT depender del error de clave foranea de la base de datos para decidir que una entidad esta en uso.

#### Scenario: Baja de una entidad sin uso
- **WHEN** el usuario da de baja una entidad que ninguna otra referencia
- **THEN** la entidad se elimina y la operacion queda registrada en la bitacora

#### Scenario: Baja bloqueada por uso
- **WHEN** el usuario da de baja una entidad que otra referencia
- **THEN** la baja se rechaza con un mensaje de negocio y la entidad permanece sin cambios

#### Scenario: La consulta de uso corre antes del borrado
- **WHEN** se rechaza una baja por uso
- **THEN** no se emitio ninguna sentencia de borrado contra la base de datos

### Requirement: Mensaje de uso especifico por caso
Cada baja bloqueada por uso SHALL informar un mensaje propio de esa entidad, traducible a todos los idiomas vigentes, que diga que la entidad esta en uso.

#### Scenario: Mensaje propio por entidad
- **WHEN** se rechazan por uso la baja de un comercio y la de una categoria de producto
- **THEN** cada rechazo muestra su propio mensaje de "en uso" y no uno generico compartido

#### Scenario: Mensaje en el idioma activo
- **WHEN** la baja se rechaza con el idioma de la interfaz en ingles
- **THEN** el mensaje se muestra en ingles

### Requirement: Bajas cubiertas
La validacion de uso SHALL cubrir estas bajas, con estas referencias:
- Rubro de comercio: comercios que lo usan.
- Medio de pago: entidades financieras que lo ofrecen.
- Entidad financiera: medios de pago propios y cuentas de usuarios.
- Medio de pago de una entidad: cuentas de usuarios y promociones que lo usan.
- Comercio: productos asignados, movimientos, promociones y reglas de categorizacion.
- Categoria de producto: productos de esa categoria.

#### Scenario: Quitar el medio de pago de una entidad usado en una cuenta
- **WHEN** el operador quita de una entidad un medio de pago que una cuenta de usuario tiene contratado
- **THEN** la baja se rechaza con el mensaje de uso del medio de pago de la entidad

#### Scenario: Baja de un comercio con movimientos
- **WHEN** el operador da de baja un comercio al que se asocio un movimiento
- **THEN** la baja se rechaza con el mensaje de uso del comercio

#### Scenario: Baja de una categoria con productos
- **WHEN** el operador da de baja una categoria de producto que tiene productos
- **THEN** la baja se rechaza con el mensaje de uso de la categoria de producto

### Requirement: Fallas tecnicas sin reinterpretacion
Si el borrado falla por una causa tecnica despues de pasar la validacion de uso, el sistema SHALL informar una falla tecnica de baja y MUST NOT convertirla en un mensaje de "en uso".

#### Scenario: Falla de la base durante el borrado
- **WHEN** la validacion de uso no encuentra uso y el borrado falla por un error de la base
- **THEN** se informa la falla tecnica de baja de esa entidad
