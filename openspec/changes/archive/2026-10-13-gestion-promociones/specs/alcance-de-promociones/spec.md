# Spec Delta

## Purpose
Define las reglas que determinan a que comercios, pagos, dias y productos aplica una promocion, y que promociones le aplican a un comercio dado.

## ADDED Requirements

### Requirement: Los filtros de una promocion son opcionales y vacio significa todos
Una promocion SHALL poder definir filtros de comercios, rubros de comercio, entidades financieras, medios de pago, segmentos, dias de la semana, productos y categorias de producto. Un filtro sin valores MUST NOT restringir nada. Una promocion sin ningun filtro SHALL aplicar a cualquier comercio, pago, dia y producto dentro de su vigencia.

#### Scenario: Promocion sin filtros
- **WHEN** se evalua una promocion sin ningun filtro cargado
- **THEN** aplica a cualquier comercio, con cualquier medio de pago, cualquier dia y cualquier producto

### Requirement: Filtros distintos se combinan con Y y opciones de un mismo filtro con O
Para que una promocion aplique, MUST cumplirse a la vez cada uno de los filtros que tengan valores. Dentro de un mismo filtro SHALL bastar con cumplir una de sus opciones.

#### Scenario: Entidad y medio de pago a la vez
- **WHEN** una promocion tiene la entidad "Galicia" y el medio de pago "Visa credito"
- **THEN** aplica a un pago con tarjeta Visa credito emitida por Galicia y no a una Visa credito de otra entidad ni a una Mastercard de Galicia

#### Scenario: Varias opciones del mismo filtro
- **WHEN** una promocion tiene los medios de pago "Visa credito" y "Mastercard credito"
- **THEN** aplica a un pago con cualquiera de los dos

### Requirement: El medio de pago es independiente de la entidad financiera
Una promocion SHALL poder definir un medio de pago sin entidad ni segmento (por ejemplo "Visa credito de cualquier banco" o dinero en cuenta), una entidad sin medio de pago, o ambos. El segmento MUST ser un filtro propio, utilizable junto con una entidad.

#### Scenario: Medio de pago de cualquier banco
- **WHEN** una promocion tiene el medio de pago "Visa credito" y ninguna entidad
- **THEN** aplica a una Visa credito emitida por cualquier entidad

#### Scenario: Promocion propia del comercio
- **WHEN** una promocion no tiene entidad ni medio de pago
- **THEN** aplica con cualquier forma de pago y se entiende como un descuento del propio comercio

### Requirement: Comercios y rubros se combinan con Y
El filtro de comercios y el filtro de rubros de comercio SHALL combinarse con Y. Una promocion con solo rubros aplica a todos los comercios de esos rubros; con solo comercios, a esos comercios; con ambos, a los comercios listados cuyo rubro este entre los rubros listados.

#### Scenario: Promocion por rubro
- **WHEN** una promocion tiene el rubro "Supermercados" y ningun comercio, y se evalua un comercio de ese rubro
- **THEN** la promocion le aplica

#### Scenario: Promocion por rubro y otro rubro
- **WHEN** esa misma promocion se evalua para un comercio de otro rubro
- **THEN** la promocion no le aplica

### Requirement: Inclusiones y exclusiones de productos y categorias, con la exclusion ganando
Productos y categorias de producto SHALL admitir listas de incluidos y de excluidos. Con inclusiones vacias participa todo el catalogo; con inclusiones cargadas participa solo un producto que coincida con algun producto o categoria incluidos. Un producto que coincida con algun producto o categoria excluidos MUST quedar fuera aunque coincida con una inclusion. Un mismo producto o categoria MUST NOT estar a la vez incluido y excluido en la misma promocion.

#### Scenario: Todo menos una categoria
- **WHEN** una promocion excluye la categoria "Carnes" y no tiene inclusiones
- **THEN** participan todos los productos salvo los de la categoria "Carnes"

#### Scenario: Solo una categoria, menos un producto
- **WHEN** una promocion incluye la categoria "Bebidas" y excluye un producto de esa categoria
- **THEN** participan las bebidas salvo ese producto

#### Scenario: Producto incluido y su categoria excluida
- **WHEN** una promocion incluye un producto y excluye la categoria de ese producto
- **THEN** el producto queda excluido y la pantalla de edicion muestra una advertencia al configurarlo

#### Scenario: Mismo elemento incluido y excluido
- **WHEN** el operador intenta cargar el mismo producto como incluido y como excluido en una promocion
- **THEN** el sistema rechaza la combinacion

### Requirement: Dias de la semana y vigencia
Una promocion SHALL aplicar solo dentro de su vigencia (desde y hasta, ambos inclusive) y, si tiene dias de la semana cargados, solo en esos dias. Sin dias cargados aplica todos los dias.

#### Scenario: Fuera de la vigencia
- **WHEN** se evalua una promocion para una fecha posterior a su fecha hasta
- **THEN** la promocion no aplica

### Requirement: Promociones que le aplican a un comercio
Para un comercio dado, el sistema SHALL considerar que le aplican las promociones cuyo filtro de comercios lo incluya, o este vacio, y cuyo filtro de rubros incluya el rubro del comercio, o este vacio. Esta regla MUST usarse tanto en el buscador por comercio de Gestion de Promociones como en las promociones que muestra el detalle del comercio.

#### Scenario: Promocion sin comercio ni rubro
- **WHEN** una promocion no restringe comercio ni rubro
- **THEN** aparece como promocion de cualquier comercio

#### Scenario: Promocion de otro comercio
- **WHEN** una promocion tiene cargado solo el comercio "Carrefour"
- **THEN** no aparece como promocion de "Coto"
