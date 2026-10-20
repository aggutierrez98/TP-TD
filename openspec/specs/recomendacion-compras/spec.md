# recomendacion-compras Specification

## Purpose
Responder en el asistente donde, cuando y con que medio de pago le conviene al usuario comprar los items pendientes de sus listas, usando solo datos calculados por el sistema.

## Requirements

### Requirement: Candidatos calculados por el sistema
Ante una pregunta de compras, el sistema SHALL calcular los candidatos con reglas deterministas a partir de los items pendientes de las listas del usuario, los comercios que venden los productos coincidentes y sus precios, y MUST entregar al modelo solo ese resumen.

#### Scenario: Item con comercios que lo venden
- **WHEN** el usuario pregunta donde comprar lo de su lista y "Cerveza" se vende en dos comercios
- **THEN** la respuesta menciona esos comercios con su precio y ningun otro

### Requirement: Cercania a las zonas del usuario
Un comercio con coordenadas SHALL ser candidato solo si cae dentro del radio de alguna zona del usuario, y la respuesta MUST indicar la distancia y la zona. Un comercio sin ubicacion fisica SHALL ser siempre candidato.

#### Scenario: Comercio cerca del trabajo
- **WHEN** un comercio esta a 600 m de la zona "Trabajo", de radio 1000 m
- **THEN** la respuesta dice que queda a 600 m de Trabajo

#### Scenario: Comercio lejano
- **WHEN** un comercio esta fuera del radio de todas las zonas
- **THEN** no se recomienda

#### Scenario: Comercio online
- **WHEN** un comercio que vende el producto no tiene coordenadas
- **THEN** se recomienda indicando que no tiene ubicacion fisica

### Requirement: Usuario sin zonas
Si el usuario no tiene zonas, el sistema SHALL recomendar sin filtro de cercania y MUST sugerir cargarlas en Mis zonas con un texto fijo traducible.

#### Scenario: Sin zonas cargadas
- **WHEN** un usuario sin zonas pregunta donde comprar
- **THEN** recibe recomendaciones de cualquier comercio y la sugerencia de cargar zonas

### Requirement: Dias y medio de pago con promocion aplicable
El sistema SHALL indicar los dias proximos en que una promocion vigente aplica al comercio y al producto, y el medio de pago y la cuenta del usuario que la habilitan. Solo MUST considerarse promociones que el usuario puede usar con sus cuentas.

#### Scenario: Promocion con cuenta del usuario
- **WHEN** hay un 20% los jueves con Visa Credito de una entidad y el usuario tiene una cuenta con ese medio
- **THEN** la respuesta recomienda el jueves, con ese medio de pago y el alias de esa cuenta

#### Scenario: Promocion que el usuario no puede usar
- **WHEN** la promocion exige una entidad con la que el usuario no tiene cuentas
- **THEN** no se menciona

### Requirement: Items sin coincidencia
Los items que no coinciden con ningun producto vendido SHALL informarse como no encontrados y MUST NOT recibir comercios, precios ni promociones inventados.

#### Scenario: Item desconocido
- **WHEN** la lista tiene "yerba organica de la abuela" y no hay producto parecido
- **THEN** la respuesta dice que no encontro donde se vende ese item

### Requirement: Conversion para descuentos y topes
El sistema SHALL usar las tasas registradas para convertir el importe de un item a ARS, calcular el descuento sin superar el tope en ARS y expresar el ahorro y precio final en la moneda original. MUST informar la tasa y fecha utilizadas y comparar ofertas de distintas monedas por su equivalente en ARS. El ahorro SHALL presentarse como una estimacion por item, sin acumular promociones ni prometer la suma de topes de la lista.

#### Scenario: Precio en USD cuyo descuento supera el tope
- **WHEN** el item cuesta USD 100, la tasa registrada es 1000 ARS por USD y la promocion ofrece 20% con tope de 10000 ARS
- **THEN** el ahorro estimado es 10000 ARS, equivalente a USD 10, y el precio final es USD 90

#### Scenario: No hay tasa registrada
- **WHEN** una oferta esta en una moneda sin tasa directa ni inversa con ARS
- **THEN** se informa el precio original y que falta tasa, sin inventar ahorro ni precio final promocional, y se ordena despues de las ofertas comparables

### Requirement: Respuesta limitada al resumen
La respuesta SHALL citar solo comercios, promociones y cuentas presentes en el resumen calculado; una respuesta que cite otros MUST descartarse segun la validacion de salida del asistente.

#### Scenario: Comercio fuera del resumen
- **WHEN** el modelo menciona un comercio que no estaba en el resumen
- **THEN** la respuesta se descarta y se muestra el texto fijo de respuesta no confiable

### Requirement: Solo datos del usuario actual
El resumen SHALL contener solo listas, zonas y cuentas del usuario de la sesion, y MUST NOT incluir credenciales de cuentas.

#### Scenario: Datos de otro usuario
- **WHEN** otro usuario tiene listas y cuentas cargadas
- **THEN** ninguna aparece en el resumen ni en la respuesta
