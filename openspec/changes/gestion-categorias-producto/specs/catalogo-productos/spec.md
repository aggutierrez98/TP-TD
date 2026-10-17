# Spec Delta

## ADDED Requirements

### Requirement: Orientacion cuando no hay categorias
La pantalla Productos SHALL informar al operador, cuando no existe ninguna categoria de producto, que primero debe dar de alta una categoria en la pantalla Categorias de producto, indicando el camino de menu. Mientras no haya categorias, el alta de productos MUST NOT ser posible.

#### Scenario: Sin categorias cargadas
- **WHEN** el operador abre la pantalla Productos y no existe ninguna categoria de producto
- **THEN** la pantalla muestra una indicacion de que debe crear una categoria en la pantalla Categorias de producto, dentro del menu Operador

#### Scenario: Con categorias cargadas
- **WHEN** el operador abre la pantalla Productos y existe al menos una categoria de producto
- **THEN** la pantalla no muestra la indicacion y el combo de categorias ofrece las categorias existentes
