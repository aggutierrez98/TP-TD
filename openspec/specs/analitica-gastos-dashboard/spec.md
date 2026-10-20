# analitica-gastos-dashboard Specification

## Purpose
Definir las tarjetas "Gastos por comercio" y "Gasto acumulado del mes" del dashboard: que el usuario vea en qué comercios gasta más y si su ritmo de gasto del mes en curso supera o no al del mes anterior.

## Requirements

### Requirement: Tarjetas disponibles para agregar
El sistema SHALL ofrecer dos tipos de tarjeta nuevos, "Gastos por comercio" y "Gasto acumulado del mes", en el selector de tarjetas del dashboard, con las mismas reglas que las existentes: cada tipo se puede agregar una sola vez por dashboard y desaparece del selector cuando ya fue agregado.

#### Scenario: Agregar una tarjeta nueva
- **WHEN** el usuario agrega "Gastos por comercio" desde el selector
- **THEN** el dashboard muestra la tarjeta y el tipo deja de ofrecerse en el selector

#### Scenario: Tipo repetido
- **WHEN** el usuario intenta agregar un tipo que ya tiene
- **THEN** el sistema rechaza la alta con el error de tarjeta ya agregada

### Requirement: Gastos por comercio
La tarjeta "Gastos por comercio" SHALL mostrar, para el mes en curso, los 8 comercios con mayor total de egresos del usuario en barras horizontales ordenadas de mayor a menor, con el monto escrito en cada barra en moneda local. Los egresos sin comercio MUST excluirse de las barras y su total MUST informarse en el resumen de la tarjeta. Solo MUST considerarse movimientos del usuario de la sesion.

#### Scenario: Mas de 8 comercios
- **WHEN** el usuario tiene egresos en 12 comercios distintos en el mes
- **THEN** la tarjeta muestra solo los 8 de mayor total, ordenados de mayor a menor

#### Scenario: Egresos de un mismo comercio
- **WHEN** hay varios egresos del mismo comercio en el mes
- **THEN** la tarjeta muestra una sola barra con la suma de sus montos

#### Scenario: Egresos sin comercio
- **WHEN** el usuario tiene egresos sin comercio en el mes
- **THEN** no aparecen como barra y el resumen informa el total sin comercio

#### Scenario: Sin datos
- **WHEN** el usuario no tiene egresos con comercio en el mes
- **THEN** la tarjeta muestra el texto traducido de "sin datos"

### Requirement: Gasto acumulado del mes
La tarjeta "Gasto acumulado del mes" SHALL mostrar dos lineas con el total acumulado de egresos por dia: el mes en curso, hasta el dia de hoy, y el mes anterior completo, sobre un eje con los dias del mes. El resumen MUST indicar el acumulado del mes en curso, el acumulado del mes anterior al mismo dia y la variacion porcentual entre ambos; si el mes anterior al mismo dia no tiene egresos, la variacion MUST omitirse. Los dias sin egresos MUST repetir el acumulado del dia anterior, y el eje vertical MUST empezar en 0.

#### Scenario: Acumulado con dias sin movimientos
- **WHEN** hay egresos los dias 1 y 3 y ninguno el dia 2
- **THEN** el acumulado del dia 2 es igual al del dia 1 y el del dia 3 suma ambos egresos

#### Scenario: Variacion contra el mes anterior
- **WHEN** hoy es el dia 10, el mes en curso acumula 150.000 y el anterior acumulaba 100.000 al dia 10
- **THEN** el resumen muestra ambos montos y una variacion de +50 %

#### Scenario: Mes anterior sin egresos
- **WHEN** el mes anterior no tiene egresos hasta el mismo dia
- **THEN** el resumen muestra el acumulado del mes en curso y no muestra variacion

#### Scenario: Sin datos
- **WHEN** no hay egresos ni en el mes en curso ni en el anterior
- **THEN** la tarjeta muestra el texto traducido de "sin datos"

### Requirement: Informacion visible y textos traducidos
Las dos tarjetas MUST cumplir la disposicion y la legibilidad de las demas tarjetas (informacion visible sin interactuar, montos en moneda local con el mismo formato) y todos sus textos MUST mostrarse en el idioma elegido, reconstruyendose al cambiar el idioma con la pantalla abierta.

#### Scenario: Cambio de idioma
- **WHEN** el usuario cambia el idioma con ambas tarjetas visibles
- **THEN** titulos, resumen y leyendas aparecen en el idioma elegido
