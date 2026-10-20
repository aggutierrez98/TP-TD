# coincidencia-items-producto Specification

## Purpose
Asociar los items de texto libre de las listas de compras con productos del catalogo, sin exigir que el usuario elija un producto al cargar el item.

## Requirements

### Requirement: Coincidencia local primero
El sistema SHALL comparar el texto del item con el nombre del producto (y opcionalmente su categoria) con una similitud local que ignore mayusculas, acentos y plurales, sin servicios externos.

#### Scenario: Diferencia de acentos y plural
- **WHEN** el item es "Galletitas" y existe el producto "galletita"
- **THEN** coinciden sin llamar a ningun servicio externo

### Requirement: Coincidencia semantica de respaldo
Solo cuando la similitud local no supere su umbral, el sistema SHALL comparar el significado del item con el de los productos y aceptar la coincidencia si supera el umbral semantico.

#### Scenario: Sinonimo coloquial
- **WHEN** el item es "birra" y existe el producto "Cerveza"
- **THEN** el item coincide con "Cerveza"

#### Scenario: Servicio semantico no disponible
- **WHEN** el respaldo semantico falla
- **THEN** el item queda como no encontrado y la recomendacion sigue con los demas

### Requirement: Representacion semantica vigente
La representacion semantica guardada de un producto SHALL recalcularse cuando cambia su texto fuente o el modelo que la genero, y MUST NOT usarse una representacion desactualizada.

#### Scenario: Producto renombrado
- **WHEN** un producto cambia de nombre despues de calcular su representacion
- **THEN** la proxima busqueda semantica la recalcula antes de compararla
