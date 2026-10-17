## ADDED Requirements

### Requirement: Edicion de una cuenta financiera propia
El sistema SHALL permitir al usuario autenticado modificar el alias, el tipo, la entidad financiera y los medios de pago contratados de una cuenta financiera que le pertenece. La edicion MUST aplicar las mismas reglas del alta: alias obligatorio (se recorta de espacios), tipo valido, entidad existente y medios de pago del catalogo de la entidad resultante. La edicion MUST NOT leer ni modificar `CredencialesCifradas`.

#### Scenario: Cambiar alias y tipo
- **WHEN** el usuario edita una cuenta propia con un alias nuevo y otro tipo valido
- **THEN** la cuenta persiste el alias recortado y el tipo nuevo, y conserva su entidad y sus medios de pago

#### Scenario: Alias vacio
- **WHEN** el usuario edita una cuenta propia con un alias vacio o de solo espacios
- **THEN** el sistema rechaza la edicion con el error de alias obligatorio y la cuenta queda sin cambios

#### Scenario: Reemplazar medios de pago
- **WHEN** el usuario edita una cuenta propia con otro conjunto de medios de pago del catalogo de su entidad
- **THEN** la cuenta queda con exactamente ese conjunto, sin duplicados

#### Scenario: Medio de pago de otra entidad
- **WHEN** la edicion incluye un medio de pago que no pertenece al catalogo de la entidad resultante
- **THEN** el sistema rechaza la edicion con el error de medio de pago ajeno a la entidad y la cuenta queda sin cambios

#### Scenario: Cambiar de entidad
- **WHEN** el usuario edita una cuenta propia con otra entidad existente y medios de pago del catalogo de esa entidad
- **THEN** la cuenta queda asociada a la entidad nueva y con esos medios de pago

#### Scenario: Entidad inexistente
- **WHEN** la edicion indica una entidad que no existe
- **THEN** el sistema rechaza la edicion con el error de entidad no encontrada y la cuenta queda sin cambios

#### Scenario: Credenciales intactas
- **WHEN** se edita una cuenta que tiene `CredencialesCifradas`
- **THEN** el valor almacenado de `CredencialesCifradas` no cambia

### Requirement: Solo el dueno edita su cuenta
El sistema MUST rechazar la edicion de una cuenta financiera que no existe o que pertenece a otro usuario, sin modificar ningun dato.

#### Scenario: Cuenta de otro usuario
- **WHEN** un usuario intenta editar una cuenta cuyo dueno es otro usuario
- **THEN** el sistema rechaza la edicion con el error de cuenta ajena y no modifica la cuenta

#### Scenario: Cuenta inexistente
- **WHEN** se intenta editar un identificador que no existe
- **THEN** el sistema rechaza la edicion con el error de cuenta no encontrada

### Requirement: Atomicidad y bitacora de la edicion
La edicion MUST ejecutarse en una sola transaccion que actualiza la cuenta, reemplaza sus medios de pago y registra una entrada de bitacora; si cualquier paso falla, no MUST quedar ningun cambio parcial. Un fallo tecnico MUST informarse con el error `Error.FinancialAccountManagement.UpdateFailed`.

#### Scenario: Edicion exitosa registrada
- **WHEN** la edicion se completa
- **THEN** existe una entrada de bitacora de la actividad "Modificacion de cuenta financiera" asociada al usuario

#### Scenario: Falla al reemplazar los medios
- **WHEN** falla el reemplazo de medios de pago durante la edicion
- **THEN** el alias, tipo y entidad de la cuenta vuelven a sus valores previos y no se registra bitacora

### Requirement: Pantalla de edicion de cuentas
La pantalla "Mis cuentas financieras" SHALL cargar en la entidad, el alias, el tipo y los medios de pago contratados (marcados) los datos de la cuenta seleccionada en la grilla, y SHALL ofrecer un boton Actualizar que aplica la edicion a esa cuenta. Actualizar MUST requerir una cuenta seleccionada, una entidad y un tipo; al finalizar con exito la pantalla MUST recargar la grilla manteniendo la cuenta seleccionada y mostrar un mensaje de confirmacion traducido. Agregar y Eliminar MUST conservar su comportamiento actual.

#### Scenario: Seleccionar una cuenta
- **WHEN** el usuario selecciona una cuenta en la grilla
- **THEN** los selectores muestran su entidad, alias y tipo, y los medios de pago contratados aparecen marcados dentro del catalogo de esa entidad

#### Scenario: Actualizar sin seleccion
- **WHEN** el usuario pulsa Actualizar sin cuenta seleccionada
- **THEN** la pantalla muestra la advertencia de seleccionar una cuenta y no modifica nada

#### Scenario: Actualizar con exito
- **WHEN** el usuario cambia el alias de la cuenta seleccionada y pulsa Actualizar
- **THEN** la grilla muestra el alias nuevo, la cuenta sigue seleccionada y se informa el exito

#### Scenario: Cambiar la entidad en edicion
- **WHEN** el usuario elige otra entidad con una cuenta seleccionada
- **THEN** el selector de medios de pago muestra el catalogo de la entidad nueva con todos los medios desmarcados

#### Scenario: Textos traducidos
- **WHEN** el usuario cambia el idioma con la pantalla abierta
- **THEN** el boton Actualizar y los mensajes de la edicion se muestran en el idioma elegido
