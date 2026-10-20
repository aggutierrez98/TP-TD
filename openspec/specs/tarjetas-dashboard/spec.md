# tarjetas-dashboard Specification

## Purpose
Definir cómo se disponen y se leen las tarjetas del dashboard: ancho responsivo, gráficos legibles sin superposiciones ni ejes distorsionados, y toda la información de cada tarjeta visible sin interacción del mouse.

## Requirements

### Requirement: Disposicion responsiva de las tarjetas
El dashboard SHALL mostrar las tarjetas con un ancho calculado a partir del ancho disponible: MUST mostrar 2 tarjetas por fila cuando el ancho alcanza para dos tarjetas de ancho minimo (420 px cada una, mas margenes), repartiendo todo el ancho entre ambas, y MUST mostrar 1 tarjeta por fila ocupando todo el ancho cuando no alcanza. Las tarjetas MUST tener una altura minima de 380 px. Cuando las tarjetas no entran en la altura visible, el dashboard SHALL ofrecer scroll vertical y MUST NOT mostrar scroll horizontal.

#### Scenario: Ventana ancha
- **WHEN** el area del dashboard mide 1200 px de ancho y hay 4 tarjetas
- **THEN** se muestran 2 tarjetas por fila con el mismo ancho, que entre ambas ocupan el ancho disponible

#### Scenario: Ventana angosta
- **WHEN** el area del dashboard mide 700 px de ancho
- **THEN** se muestra 1 tarjeta por fila ocupando el ancho disponible y el resto se ve con scroll vertical

#### Scenario: Cambio de tamano de la ventana
- **WHEN** el usuario redimensiona o maximiza la ventana del dashboard
- **THEN** las tarjetas se reacomodan al nuevo ancho sin recargar los datos ni mostrar scroll horizontal

#### Scenario: Cantidad impar de tarjetas
- **WHEN** hay una cantidad impar de tarjetas con 2 por fila
- **THEN** la ultima tarjeta queda sola en su fila con el mismo ancho que las demas

### Requirement: Legibilidad de los graficos
Los graficos MUST ser legibles sin superposiciones: con margen interno entre el borde de la tarjeta, el grafico y sus etiquetas; el eje vertical de barras y area MUST empezar en 0 para no distorsionar las proporciones; y los montos MUST mostrarse en moneda local con separador de miles y sin decimales irrelevantes (por ejemplo `$ 3.068.500`), abreviando (`3,1 M`) solo en las marcas del eje vertical. Ningun valor MUST aparecer con una cantidad de decimales distinta a la de los demas.

#### Scenario: Eje desde cero
- **WHEN** se muestra el comparativo mensual con ingresos de 3,0 M y egresos de 1,7 M
- **THEN** el eje vertical empieza en 0 y las barras guardan la proporcion real entre los valores

#### Scenario: Montos uniformes
- **WHEN** se muestran los montos de un tooltip o de las etiquetas de datos
- **THEN** todos tienen el mismo formato de moneda, por ejemplo `$ 3.068.500` y `$ 1.722.409`

#### Scenario: Sin superposicion
- **WHEN** se muestra una tarjeta con grafico, leyenda y etiquetas
- **THEN** el titulo, el grafico, la leyenda y las etiquetas no se tapan entre si ni tocan el borde de la tarjeta

### Requirement: Informacion de cada tarjeta
Cada tarjeta SHALL mostrar su titulo, el periodo que cubre y el total del periodo en moneda local, y su grafico MUST mostrar toda su informacion de forma permanente, sin requerir pasar el mouse ni seleccionar partes del grafico: leyenda siempre visible y valores escritos sobre el grafico. El tooltip MAY complementar pero MUST NOT ser la unica forma de ver un valor. Las tarjetas por categoria MUST listar cada categoria con su monto y su porcentaje sobre el total; las tarjetas mensuales MUST mostrar ejes con montos formateados en moneda local. Una tarjeta sin datos en el periodo SHALL mostrar un texto traducido que lo indica en lugar de un grafico vacio.

#### Scenario: Tarjeta de gastos por categoria
- **WHEN** hay egresos en el mes en curso
- **THEN** la tarjeta muestra el mes, el total de egresos y una leyenda con cada categoria, su monto y su porcentaje

#### Scenario: Informacion sin interaccion
- **WHEN** se abre el dashboard sin mover el mouse
- **THEN** cada categoria de una torta se ve con su monto y porcentaje, sin seleccionarla

#### Scenario: Tarjeta comparativa mensual
- **WHEN** hay movimientos en los ultimos meses
- **THEN** la tarjeta muestra leyenda de Ingresos y Egresos, etiquetas de mes, montos formateados en el eje vertical y el valor de cada barra o punto escrito sobre el grafico

#### Scenario: Sin datos
- **WHEN** no hay movimientos del tipo correspondiente en el periodo
- **THEN** la tarjeta muestra el texto de "sin datos" traducido

#### Scenario: Cambio de idioma
- **WHEN** el usuario cambia el idioma con el dashboard abierto
- **THEN** los titulos, el periodo, el total y los textos de la tarjeta se muestran en el idioma elegido
