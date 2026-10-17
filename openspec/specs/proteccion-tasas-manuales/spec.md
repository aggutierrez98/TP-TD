# proteccion-tasas-manuales Specification

## Purpose
Permitir que el operador fije manualmente el valor de una tasa en casos especiales sin que la actualizacion automatica lo pise, y devolverla al circuito automatico cuando lo decida.

## Requirements

### Requirement: Edicion manual marca la tasa
Cuando el operador modifica el valor de una tasa existente desde Gestion de Monedas, el sistema SHALL marcarla como manual. Crear un par nuevo NO SHALL marcarlo como manual.

#### Scenario: Editar el valor
- **WHEN** el operador cambia el valor de un par existente y guarda
- **THEN** el par queda marcado como manual

#### Scenario: Crear un par
- **WHEN** el operador crea un par con un valor inicial
- **THEN** el par no queda marcado como manual y la actualizacion automatica lo toma

### Requirement: Las tasas manuales no se actualizan
La actualizacion automatica y el boton "Actualizar ahora" MUST NOT modificar el valor de una tasa marcada como manual y SHALL registrarla como saltada por manual.

#### Scenario: Corrida con un par manual
- **WHEN** corre una actualizacion y un par esta marcado como manual
- **THEN** su valor no cambia y el historial lo muestra como saltado por manual

### Requirement: Liberar una tasa manual
El operador SHALL poder liberar una tasa manual con un boton en Gestion de Monedas; desde ese momento la actualizacion automatica la actualiza en la proxima ejecucion. El estado manual MUST ser visible en la grilla.

#### Scenario: Liberar
- **WHEN** el operador libera un par manual
- **THEN** deja de estar marcado y la siguiente ejecucion lo actualiza

### Requirement: Trazabilidad de la proteccion
El sistema SHALL dejar constancia en la bitacora de quien marco como manual y de quien libero cada tasa, con su fecha.

#### Scenario: Marcar y liberar
- **WHEN** un operador edita un par y luego otro lo libera
- **THEN** la bitacora contiene ambos eventos con su usuario y fecha
