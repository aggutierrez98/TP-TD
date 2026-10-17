# conexion-base-datos Specification

## Purpose
Definir el destino unico de persistencia SQL de FINAZAPP mediante la configuracion del ejecutable y evitar cambios automaticos de servidor.

## Requirements

### Requirement: Destino SQL unico configurado
El sistema SHALL usar exclusivamente la cadena `TpIDS` de `connectionStrings` en el config del ejecutable para su acceso a datos, incluido el arranque y backup/restore. El sistema MUST ignorar entradas alternativas aunque esten presentes.

#### Scenario: Conexion configurada disponible
- **WHEN** la cadena `TpIDS` apunta a una base disponible
- **THEN** las operaciones de datos se ejecutan en ese destino

#### Scenario: Cambiar el destino por configuracion
- **WHEN** se cambia el servidor o catalogo de `TpIDS` y se vuelve a iniciar la aplicacion
- **THEN** el sistema usa el nuevo destino sin requerir cambios de codigo

#### Scenario: Backup usa el destino configurado
- **WHEN** se solicita un backup o restore con una cadena `TpIDS` valida
- **THEN** la operacion usa el servidor y catalogo de esa cadena, incluso si existen otras entradas

### Requirement: Fallo de conexion sin destino alternativo
El sistema MUST informar el fallo cuando no puede abrir la conexion `TpIDS`, sin intentar conectarse a otro destino. Si ocurre durante el arranque, SHALL interrumpir ese arranque sin mostrar el login operativo.

#### Scenario: Principal inaccesible y alternativa disponible
- **WHEN** `TpIDS` no esta disponible y existe una entrada alternativa accesible
- **THEN** el acceso falla y no utiliza la alternativa

#### Scenario: Base no disponible al iniciar
- **WHEN** el arranque requiere datos y no puede abrir `TpIDS`
- **THEN** se informa el error mediante el manejo existente y no se muestra el login operativo

### Requirement: Configuracion principal obligatoria
El sistema MUST rechazar operaciones de datos cuando `TpIDS` esta ausente o su cadena esta vacia, sin usar valores predeterminados ni entradas alternativas.

#### Scenario: Cadena ausente
- **WHEN** falta `TpIDS` y hay otra cadena configurada
- **THEN** la operacion informa una configuracion invalida sin conectarse a la otra cadena

#### Scenario: Cadena vacia en backup
- **WHEN** se solicita backup o restore y `TpIDS` contiene una cadena vacia o solo espacios
- **THEN** la operacion falla por configuracion invalida sin seleccionar otra entrada
