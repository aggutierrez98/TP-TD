# Spec Delta

## Purpose

Permitir conversar con el asistente personal sin congelar la interfaz durante el envio, conservando el historial y recuperando los controles al finalizar.

## ADDED Requirements

### Requirement: Envio sin bloqueo de interfaz
El sistema SHALL mantener la interfaz receptiva durante la lectura del contexto, la espera de la respuesta y el guardado del intercambio del asistente personal.

#### Scenario: Lectura de historial lenta
- **WHEN** el usuario envia una pregunta y la consulta del historial demora
- **THEN** la ventana sigue procesando eventos y puede repintarse y desplazarse antes de terminar la consulta

#### Scenario: Respuesta o guardado lento
- **WHEN** el proveedor de IA o el guardado de la respuesta demora
- **THEN** la interfaz sigue procesando eventos durante la espera

### Requirement: Un envio activo por formulario
El sistema SHALL impedir otro envio desde el mismo formulario mientras haya una pregunta en proceso, sin deshabilitar el shell completo.

#### Scenario: Boton y Enter durante un envio
- **WHEN** hay un envio activo y el usuario intenta enviar nuevamente desde el boton o Enter
- **THEN** no se inicia otro intercambio y las demas ventanas siguen disponibles

### Requirement: Resultado y recuperacion del formulario
El sistema SHALL mostrar el intercambio despues de persistirlo, limpiar la pregunta solo ante exito y restablecer los controles del formulario abierto al finalizar; ante un error SHALL conservar la pregunta y mostrarlo mediante el manejo habitual.

#### Scenario: Envio exitoso
- **WHEN** la respuesta se obtiene y el intercambio se guarda correctamente
- **THEN** se agrega una sola vez al historial visible, se limpia la pregunta y se rehabilita el envio

#### Scenario: Envio fallido
- **WHEN** falla la consulta, la respuesta o la persistencia
- **THEN** se informa el error, se conserva la pregunta para reintentar y se rehabilita el envio

### Requirement: Finalizacion con formulario cerrado
El sistema SHALL permitir cerrar el formulario mientras espera y omitir las actualizaciones visuales y los dialogos de esa operacion si el formulario ya se cerro.

#### Scenario: Respuesta tardia
- **WHEN** el usuario cierra el formulario con un envio pendiente y la operacion termina posteriormente
- **THEN** no se accede a controles destruidos ni se muestra un dialogo asociado al formulario cerrado
