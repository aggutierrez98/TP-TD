# Gestión de notificaciones

_Especificaciones de casos de uso_

---

**ID y Nombre:** CUN81 Consultar notificaciones de usuarios  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador consultar las notificaciones de todos los usuarios, indicando destinatario, tipo, mensaje, fecha y estado de lectura.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada y cuenta con autorización para gestionar notificaciones.  
**Puntos de Extension:** Se extiende con CUN82 Crear notificación manual para un usuario, CUN83 Eliminar notificación de usuario y CUN84 Cambiar estado de lectura de una notificación de usuario, que se inician desde la misma pantalla.  
**Condicion:** El actor desea revisar las notificaciones emitidas a los usuarios del sistema.  

**Escenario Principal:**
1. El actor selecciona la opción «Notificaciones» del menú «Operador».
2. El sistema presenta la pantalla «Gestión de Notificaciones» con el listado de todas las notificaciones, ordenadas de la más reciente a la más antigua, y con las columnas «Usuario», «Tipo», «Mensaje», «Fecha» y «Leída». Los mensajes extensos se muestran resumidos en una sola línea.
3. El actor revisa el listado.

**Flujos Alternativos:**
2a. Si no hay notificaciones para mostrar, el sistema presenta el listado vacío. El caso de uso finaliza sin notificaciones para consultar.
2b. Si ocurre un error al cargar las notificaciones, el sistema muestra un mensaje de error. El caso de uso finaliza sin completar la consulta.
2c. Si ocurre un error al cargar la lista de usuarios destinatarios, el sistema muestra un mensaje de error. El listado de notificaciones se presenta igualmente.

**Postcondiciones:** El actor dispone del listado de notificaciones de los usuarios. La consulta no modifica las notificaciones.

---

**ID y Nombre:** CUN82 Crear notificación manual para un usuario  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador enviar una notificación con un mensaje propio a un usuario del sistema.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada, cuenta con autorización para gestionar notificaciones y tiene abierta la pantalla «Gestión de Notificaciones».  
**Puntos de Extension:** Extiende a CUN81 Consultar notificaciones de usuarios. El destinatario podrá consultar la notificación mediante CUN85 Consultar mis notificaciones.  
**Condicion:** El actor desea comunicar un aviso a un usuario.  

**Escenario Principal:**
1. El actor selecciona en el campo «Usuario destino:» de la sección «Nueva notificación» el usuario que recibirá el aviso.
2. El actor escribe el texto de la notificación en el campo «Mensaje:».
3. El actor selecciona el botón «Agregar».
4. El sistema registra la notificación como no leída, limpia el campo «Mensaje:», actualiza el listado incluyendo la nueva notificación y muestra el mensaje «Notificación creada con éxito.».

**Flujos Alternativos:**
3a. Si el actor no seleccionó un usuario destino, el sistema muestra el mensaje «Debe seleccionar un usuario destino.» y no registra la notificación.
3b. Si el mensaje está vacío o contiene solo espacios, el sistema muestra el mensaje «Debe indicar un mensaje para la notificación.» y no registra la notificación.
3c. Si el mensaje supera los 300 caracteres, el sistema muestra el mensaje «El mensaje no puede superar los 300 caracteres.» y no registra la notificación.
3d. Si el usuario destino ya no existe, el sistema muestra el mensaje «No se encontró el usuario destino indicado.» y no registra la notificación.
3e. Si ocurre un error al registrar la notificación, el sistema muestra el mensaje «No se pudo crear la notificación.». El listado permanece sin cambios.

**Postcondiciones:** La notificación queda registrada para el usuario destino con la fecha actual y estado no leído, y figura en el listado. Si el caso de uso no se completó, las notificaciones permanecen sin cambios.

---

**ID y Nombre:** CUN83 Eliminar notificación de usuario  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador eliminar una notificación emitida a un usuario.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada, cuenta con autorización para gestionar notificaciones y tiene abierta la pantalla «Gestión de Notificaciones» con al menos una notificación listada.  
**Puntos de Extension:** Extiende a CUN81 Consultar notificaciones de usuarios.  
**Condicion:** El actor desea retirar una notificación del sistema.  

**Escenario Principal:**
1. El actor selecciona una notificación del listado.
2. El actor selecciona el botón «Eliminar seleccionada».
3. El sistema solicita confirmación con el diálogo «Confirmar eliminación» y el mensaje «Se eliminará la notificación seleccionada. ¿Desea continuar?».
4. El actor confirma la eliminación.
5. El sistema elimina la notificación, actualiza el listado sin ella y muestra el mensaje «Notificación eliminada con éxito.».

**Flujos Alternativos:**
2a. Si el actor no seleccionó ninguna notificación, el sistema muestra el mensaje «Debe seleccionar una notificación.» y no realiza cambios.
4a. Si el actor no confirma la eliminación, el sistema cierra el diálogo sin modificar la notificación. El caso de uso finaliza.
5a. Si la notificación ya no existe, el sistema muestra el mensaje «No se encontró la notificación indicada.» y no realiza cambios.
5b. Si ocurre un error al eliminar la notificación, el sistema muestra el mensaje «No se pudo eliminar la notificación.». El listado permanece sin cambios.

**Postcondiciones:** La notificación eliminada deja de figurar en el listado y deja de estar disponible para su destinatario. Si el caso de uso no se completó, las notificaciones permanecen sin cambios.

---

**ID y Nombre:** CUN84 Cambiar estado de lectura de una notificación de usuario  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador marcar una notificación de un usuario como leída o volver a marcarla como no leída.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada, cuenta con autorización para gestionar notificaciones y tiene abierta la pantalla «Gestión de Notificaciones» con al menos una notificación listada.  
**Puntos de Extension:** Extiende a CUN81 Consultar notificaciones de usuarios.  
**Condicion:** El actor desea corregir el estado de lectura de una notificación.  

**Escenario Principal:**
1. El actor marca o desmarca la casilla de la columna «Leída» de la notificación deseada.
2. El sistema registra el nuevo estado de lectura de la notificación y lo conserva en el listado.

**Flujos Alternativos:**
2a. Si la notificación ya no existe, el sistema muestra el mensaje «No se encontró la notificación indicada.» y vuelve a cargar el listado actualizado.
2b. Si ocurre un error al actualizar la notificación, el sistema muestra el mensaje «No se pudo actualizar la notificación.» y vuelve a cargar el listado con el estado vigente.

**Postcondiciones:** La notificación queda con el estado de lectura elegido por el actor, tanto en el listado de gestión como en las notificaciones del usuario destinatario. Si el caso de uso no se completó, el estado de lectura permanece sin cambios.
