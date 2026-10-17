# productos-por-comercio Specification

## Purpose
Define como un comercio vende productos del catalogo, con un precio y una moneda propios por comercio, administrados desde el detalle del comercio.

## Requirements

### Requirement: Un comercio vende productos del catalogo con precio propio
El detalle del comercio SHALL permitir asignar al comercio un producto ya existente en el catalogo, indicando el precio y la moneda con que ese comercio lo vende. El precio MUST ser mayor que cero y la moneda MUST existir.

#### Scenario: Asignacion valida
- **WHEN** el operador elige un producto del catalogo, ingresa un precio mayor que cero, elige una moneda y confirma
- **THEN** el producto aparece en el listado de productos del comercio con ese precio y moneda, y se registra la operacion en la bitacora

#### Scenario: Precio invalido
- **WHEN** el operador intenta asignar un producto con precio cero, negativo o no numerico
- **THEN** el sistema rechaza la asignacion con un mensaje de precio invalido

### Requirement: El mismo producto puede tener distinto precio por comercio
Un mismo producto del catalogo SHALL poder asignarse a varios comercios, cada uno con su precio y moneda. Un producto MUST NOT asignarse mas de una vez al mismo comercio.

#### Scenario: Dos comercios con precios distintos
- **WHEN** el operador asigna el mismo producto a dos comercios con precios distintos
- **THEN** cada comercio muestra su propio precio para ese producto

#### Scenario: Producto repetido en el mismo comercio
- **WHEN** el operador intenta asignar a un comercio un producto que ese comercio ya vende
- **THEN** el sistema rechaza la asignacion indicando que el producto ya esta asignado al comercio

### Requirement: Edicion de precio y baja de la asignacion
El detalle del comercio SHALL permitir modificar el precio de un producto asignado y quitar el producto del comercio. La moneda MUST NOT poder modificarse una vez asignado el producto. Quitar un producto del comercio MUST NOT eliminarlo del catalogo.

#### Scenario: Cambio de precio
- **WHEN** el operador selecciona un producto asignado, ingresa un nuevo precio valido y confirma
- **THEN** el comercio muestra el precio actualizado y los demas comercios no se modifican

#### Scenario: Quitar un producto del comercio
- **WHEN** el operador quita un producto asignado a un comercio
- **THEN** el comercio deja de listarlo, pero el producto sigue existiendo en el catalogo

### Requirement: El detalle del comercio no crea productos
El detalle del comercio MUST NOT permitir crear ni modificar el nombre o la categoria de un producto. Los productos nuevos SHALL crearse unicamente desde la pantalla Productos.

#### Scenario: Producto no existente
- **WHEN** el operador necesita vender en un comercio un producto que no esta en el catalogo
- **THEN** debe darlo de alta primero en la pantalla Productos y luego asignarlo desde el detalle del comercio

### Requirement: Los datos existentes se conservan en la migracion
La migracion al nuevo esquema SHALL conservar todo producto existente, su comercio, su precio y su moneda. Cada producto previo MUST quedar como un producto del catalogo asignado al mismo comercio con el mismo precio y la misma moneda.

#### Scenario: Migracion de datos de desarrollo
- **WHEN** se aplica la migracion sobre una base con productos creados por comercio
- **THEN** cada comercio sigue mostrando los mismos productos con los mismos precios y monedas, y el catalogo contiene un producto por cada fila previa

### Requirement: Un comercio con productos asignados no se puede eliminar
El sistema SHALL seguir rechazando la baja de un comercio que tenga productos asignados, igual que antes del desacople.

#### Scenario: Baja de comercio con productos
- **WHEN** el operador intenta eliminar un comercio que vende al menos un producto
- **THEN** el sistema rechaza la baja con un mensaje de comercio en uso y no elimina el comercio
