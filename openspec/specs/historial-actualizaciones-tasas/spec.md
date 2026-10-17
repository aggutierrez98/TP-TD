# historial-actualizaciones-tasas Specification

## Purpose
Dar visibilidad al operador de las ultimas actualizaciones de tasas y del consumo del limite mensual de la API, y permitirle disparar una actualizacion manual acotada.

## Requirements

### Requirement: Registro de cada ejecucion
Cada ejecucion, programada o manual, SHALL registrar fecha, origen, resultado (exitosa, fallida u omitida), si consumio una request a la API, un mensaje y, por cada par, el valor anterior, el nuevo y su estado (actualizado, saltado por manual o saltado por falta de dato).

#### Scenario: Ejecucion exitosa
- **WHEN** una ejecucion actualiza dos pares y saltea uno manual
- **THEN** el historial muestra la ejecucion exitosa con los tres pares y su estado

#### Scenario: Ejecucion omitida por tope
- **WHEN** una ejecucion se omite por el tope horario
- **THEN** se registra como omitida y sin consumo de request

### Requirement: Pantalla de ultimas actualizaciones
El sistema SHALL ofrecer una pantalla "Actualizacion de Tasas" que liste las ejecuciones mas recientes, primero la mas nueva, y el detalle de los pares de la ejecucion seleccionada.

#### Scenario: Ver el historial
- **WHEN** el operador abre la pantalla
- **THEN** ve las ultimas ejecuciones ordenadas de la mas reciente a la mas antigua

### Requirement: Requests consumidas en el mes
La pantalla SHALL mostrar las requests consumidas en el mes calendario actual frente al limite mensual del plan. Toda consulta intentada, exitosa o fallida, MUST contarse como consumida.

#### Scenario: Consulta fallida
- **WHEN** una consulta llega a la API y esta responde con error
- **THEN** el contador del mes aumenta en uno

#### Scenario: Cambio de mes
- **WHEN** comienza un mes calendario nuevo
- **THEN** el contador del mes parte de cero

### Requirement: Actualizar ahora con tope
La pantalla SHALL ofrecer un boton "Actualizar ahora". Si la ultima ejecucion exitosa tiene menos de una hora, MUST NOT consultar a la API, informar al operador y mostrar los datos ya guardados.

#### Scenario: Actualizacion reciente
- **WHEN** el operador pulsa el boton y la ultima actualizacion exitosa fue hace 20 minutos
- **THEN** no se consulta a la API y se informa cuando podra volver a actualizar

#### Scenario: Actualizacion vencida
- **WHEN** el operador pulsa el boton y pasaron mas de 60 minutos
- **THEN** se consulta a la API, se actualizan los pares y se refresca la pantalla sin bloquear la interfaz

### Requirement: Acceso a la pantalla
El acceso a la pantalla SHALL requerir un permiso propio, otorgado al grupo Operador y al administrador, y todos sus textos MUST estar traducidos.

#### Scenario: Usuario sin permiso
- **WHEN** un usuario sin el permiso inicia sesion
- **THEN** la opcion de menu no esta disponible
