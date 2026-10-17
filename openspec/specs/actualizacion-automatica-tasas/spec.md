# actualizacion-automatica-tasas Specification

## Purpose
Mantener actualizadas las tasas de conversion existentes de FINAZAPP mediante una consulta horaria a una API externa gratuita, sin intervencion del operador y respetando el limite mensual de requests del plan.

## Requirements

### Requirement: Una consulta por ejecucion
Cada ejecucion que actualiza tasas SHALL consultar la API externa una sola vez, con base USD, y obtener de esa respuesta los valores de todas las monedas.

#### Scenario: Ejecucion con varios pares
- **WHEN** existen tres pares de monedas y corre una actualizacion
- **THEN** se realiza una sola request a la API y los tres pares se actualizan con esa respuesta

### Requirement: Calculo de cruces
El valor de un par SHALL calcularse a partir de las cotizaciones en USD de sus dos monedas: una unidad de la moneda base equivale a cotizacion(conversion) / cotizacion(base) unidades de la moneda de conversion.

#### Scenario: Par sin USD
- **WHEN** el par es ARS a EUR y la API informa 1500 ARS y 0,9 EUR por USD
- **THEN** el valor del par es 0,9 / 1500 = 0,0006

#### Scenario: Par con USD como base
- **WHEN** el par es USD a ARS y la API informa 1500 ARS por USD
- **THEN** el valor del par es 1500

### Requirement: Solo pares existentes
La actualizacion automatica MUST actualizar unicamente los pares que ya existen y NO SHALL crear pares nuevos.

#### Scenario: Moneda nueva sin par
- **WHEN** el operador da de alta una moneda y no crea ningun par con ella
- **THEN** la actualizacion no crea pares para esa moneda

### Requirement: Saltear pares sin dato
Si la respuesta de la API no incluye la cotizacion de alguna de las dos monedas de un par, o esta no es positiva, ese par SHALL conservar su valor y quedar registrado como saltado, sin afectar a los demas pares.

#### Scenario: Moneda ausente en la respuesta
- **WHEN** la API no informa la moneda de conversion de un par
- **THEN** el par no cambia, se registra como saltado por falta de dato y los otros pares se actualizan

### Requirement: Frecuencia horaria
El sistema SHALL ejecutar la actualizacion automatica cada hora mediante una tarea programada del sistema operativo, registrada con un script versionado en el repositorio.

#### Scenario: Registrar la tarea
- **WHEN** se ejecuta el script de registro en una maquina
- **THEN** existe una tarea programada que lanza el ejecutable cada hora

### Requirement: Tope de una consulta por hora
Una ejecucion, programada o manual, MUST NOT consultar a la API cuando la ultima ejecucion exitosa inicio hace menos de 60 minutos. En ese caso SHALL terminar sin consultar y sin modificar tasas.

#### Scenario: Ejecucion programada demasiado pronto
- **WHEN** el boton manual actualizo hace 30 minutos y corre la tarea programada
- **THEN** la tarea no consulta a la API y registra que se omitio por tope

#### Scenario: Ejecucion programada normal
- **WHEN** la ultima ejecucion exitosa inicio hace 60 minutos y termino unos segundos despues, y corre la tarea programada a la hora exacta
- **THEN** la tarea programada consulta a la API y actualiza

### Requirement: Fallas sin reintento
Si la consulta falla (red, respuesta invalida, rechazo de la API o falta de clave), la ejecucion SHALL registrar el error en el historial, conservar los valores previos y no reintentar hasta la siguiente ejecucion.

#### Scenario: API no disponible
- **WHEN** la API no responde
- **THEN** no cambia ninguna tasa, el historial registra la falla y no se hace otra consulta en esa hora

#### Scenario: Clave no configurada
- **WHEN** el archivo de secretos no existe o la clave esta vacia
- **THEN** no se consulta a la API y el historial registra el error de clave faltante

### Requirement: Secretos y configuracion del proceso
La clave de la API SHALL leerse de un `secrets.config` junto al ejecutable, excluido del control de versiones, con una plantilla `.example` versionada. La conexion a la base MUST ser la cadena `TpIDS` del config del propio ejecutable.

#### Scenario: Clonar el repositorio
- **WHEN** alguien clona el repositorio
- **THEN** no obtiene la clave de la API y dispone de la plantilla para crearla

### Requirement: Compilacion sin NuGet
El proyecto del proceso SHALL compilar con `TpIDS.sln` sin instalar ni restaurar paquetes NuGet.

#### Scenario: Compilar la solucion productiva
- **WHEN** se compila `TpIDS.sln` en un entorno sin cache NuGet
- **THEN** el ejecutable se genera sin errores
