# Gestión de usuarios

_Especificaciones de casos de uso_

---

**ID y Nombre:** CUN01 Consultar usuarios  
**Estado:** Implementado  
**Descripcion:** Permite al Administrador consultar los usuarios registrados y visualizar los datos y los permisos asignados de un usuario seleccionado.  
**Actor Principal:** Administrador  
**Actor Secundario:** -  
**Precondiciones:** El Administrador tiene una sesión iniciada y cuenta con autorización para acceder a la gestión de usuarios.  
**Puntos de Extension:** No aplica.  
**Condicion:** El Administrador desea consultar los usuarios registrados en el sistema.  

**Escenario Principal:**
1. El Administrador selecciona la opción «Usuarios» del menú de administración.
2. El sistema presenta la pantalla «Gestión de usuarios» con el listado de usuarios, indicando para cada uno su identificador, nombre de usuario, estado activo y estado de bloqueo.
3. El Administrador selecciona un usuario del listado.
4. El sistema muestra el nombre y el estado activo del usuario seleccionado en la sección «Datos del usuario» y presenta el árbol «Permisos y grupos asignados», con las asignaciones marcadas. El campo de contraseña permanece vacío.

**Flujos Alternativos:**
2a. Si no hay usuarios para mostrar, el sistema presenta el listado vacío. El caso de uso finaliza sin selección de usuario.
2b. Si ocurre un error al cargar el listado, el sistema muestra un mensaje de error. El caso de uso finaliza sin completar la consulta.
3a. Si el Administrador solo necesita consultar el listado, omite la selección de un usuario. El caso de uso finaliza con el listado disponible.
4a. Si ocurre un error al cargar los permisos del usuario seleccionado, el sistema muestra un mensaje de error. El listado y los datos del usuario permanecen visibles, pero la consulta de sus permisos no se completa.

**Postcondiciones:** El Administrador dispone del listado de usuarios y, si seleccionó uno, de sus datos y permisos asignados. La consulta no modifica los usuarios ni sus asignaciones.

---

**ID y Nombre:** CUN02 Alta de usuario  
**Estado:** Implementado  
**Descripcion:** Permite al Administrador dar de alta un nuevo usuario informando su nombre de usuario y su contraseña inicial.  
**Actor Principal:** Administrador  
**Actor Secundario:** -  
**Precondiciones:** El Administrador tiene una sesión iniciada y cuenta con autorización para gestionar usuarios.  
**Puntos de Extension:** No aplica.  
**Condicion:** El Administrador necesita incorporar un nuevo usuario al sistema.  

**Escenario Principal:**
1. El Administrador selecciona la opción «Usuarios» del menú de administración.
2. El sistema presenta la pantalla «Gestión de usuarios» con el listado de usuarios y la sección «Datos del usuario».
3. El Administrador ingresa el nombre del nuevo usuario en el campo «Nombre de usuario:» y su contraseña en el campo «Contraseña (obligatoria al agregar, opcional al editar):».
4. El Administrador presiona el botón «Agregar usuario».
5. El sistema registra el usuario y muestra el mensaje «Usuario creado correctamente.».
6. El sistema limpia los campos de la sección «Datos del usuario» y actualiza el listado, que incorpora al nuevo usuario activo y sin bloqueo.

**Flujos Alternativos:**
4a. Si el nombre de usuario o la contraseña están vacíos, el sistema muestra el mensaje «Para agregar un usuario debe completar el Nombre de usuario y la Contraseña.» con el título «Datos incompletos». El usuario no se crea y los datos ingresados permanecen en pantalla.
5a. Si ya existe un usuario con el mismo nombre, el sistema muestra el mensaje «El nombre de usuario ya existe. Por favor elegi otro.». El usuario no se crea.
5b. Si ocurre un error al registrar el usuario, el sistema muestra el mensaje «No se pudo registrar el usuario.». El usuario no se crea.

**Postcondiciones:** El nuevo usuario figura en el listado como activo y sin bloqueo, y puede iniciar sesión con las credenciales informadas. Si el caso de uso no se completa, el listado de usuarios no cambia.

---

**ID y Nombre:** CUN03 Modificar usuario  
**Estado:** Implementado  
**Descripcion:** Permite al Administrador modificar el nombre de usuario, la contraseña o el estado activo de un usuario existente.  
**Actor Principal:** Administrador  
**Actor Secundario:** -  
**Precondiciones:** El Administrador tiene una sesión iniciada y cuenta con autorización para gestionar usuarios. Existe al menos un usuario registrado.  
**Puntos de Extension:** No aplica.  
**Condicion:** El Administrador necesita corregir los datos de un usuario, cambiar su contraseña o activarlo o desactivarlo.  

**Escenario Principal:**
1. El Administrador selecciona la opción «Usuarios» del menú de administración.
2. El sistema presenta la pantalla «Gestión de usuarios» con el listado de usuarios.
3. El Administrador selecciona un usuario del listado.
4. El sistema muestra el nombre y el estado activo del usuario en la sección «Datos del usuario». El campo de contraseña permanece vacío.
5. El Administrador modifica el nombre de usuario, ingresa una nueva contraseña si desea cambiarla, o marca o desmarca la opción «Usuario activo».
6. El Administrador presiona el botón «Editar usuario».
7. El sistema aplica los cambios y muestra el mensaje «Usuario actualizado correctamente.». Si el cambio activó al usuario, el mensaje es «Usuario actualizado y activado correctamente.»; si lo desactivó, «Usuario actualizado y desactivado correctamente.».
8. El sistema limpia la selección y actualiza el listado con los datos modificados.

**Flujos Alternativos:**
6a. Si no hay un usuario seleccionado, el sistema muestra el mensaje «No hay ningún usuario seleccionado. Selecciona uno de la lista para editarlo.» con el título «Sin selección». No se realizan cambios.
6b. Si el nombre de usuario está vacío, el sistema muestra el mensaje «El nombre de usuario no puede estar vacío.» con el título «Datos incompletos». No se realizan cambios.
6c. Si el Administrador desmarcó «Usuario activo» en un usuario que estaba activo, el sistema solicita confirmación con el título «Confirmar desactivación» y el mensaje «Esta acción desactivará al usuario seleccionado, por lo que no podrá utilizarse hasta que vuelva a activarse. ¿Desea continuar?». Si el Administrador responde afirmativamente, el escenario continúa en el paso 7. Si responde negativamente, el caso de uso finaliza sin cambios.
7a. Si el Administrador intenta desactivar al usuario que tiene la sesión activa, el sistema muestra el mensaje «No se puede desactivar al usuario que tiene la sesión activa.». No se realizan cambios.
7b. Si ya existe otro usuario con el nombre ingresado, el sistema muestra el mensaje «El nombre de usuario ya existe. Por favor elegi otro.». No se realizan cambios.
7c. Si ocurre un error al actualizar el usuario, el sistema muestra el mensaje «No se pudo actualizar el usuario.», limpia la selección y recarga el listado.

**Postcondiciones:** El usuario figura en el listado con los datos modificados. Si se ingresó una nueva contraseña, el usuario deberá utilizarla en su próximo inicio de sesión. Si el caso de uso no se completa, los datos del usuario no cambian.

---

**ID y Nombre:** CUN04 Eliminar usuario  
**Estado:** Implementado  
**Descripcion:** Permite al Administrador eliminar definitivamente un usuario del sistema junto con su historial asociado.  
**Actor Principal:** Administrador  
**Actor Secundario:** -  
**Precondiciones:** El Administrador tiene una sesión iniciada y cuenta con autorización para gestionar usuarios. Existe al menos un usuario distinto del que tiene la sesión activa.  
**Puntos de Extension:** No aplica.  
**Condicion:** El Administrador necesita quitar definitivamente a un usuario del sistema.  

**Escenario Principal:**
1. El Administrador selecciona la opción «Usuarios» del menú de administración.
2. El sistema presenta la pantalla «Gestión de usuarios» con el listado de usuarios.
3. El Administrador selecciona el usuario a eliminar.
4. El sistema muestra los datos del usuario seleccionado en la sección «Datos del usuario».
5. El Administrador presiona el botón «Eliminar usuario».
6. El sistema solicita confirmación con el título «Confirmar borrado físico» y el mensaje «Esta es una acción crítica. Se eliminará definitivamente al usuario seleccionado del sistema y todo su historial asociado. Esta operación no podrá deshacerse. ¿Desea continuar?».
7. El Administrador confirma la eliminación.
8. El sistema elimina al usuario y muestra el mensaje «Usuario eliminado definitivamente.».
9. El sistema limpia la selección y actualiza el listado, que ya no incluye al usuario eliminado.

**Flujos Alternativos:**
5a. Si no hay un usuario seleccionado, el sistema muestra el mensaje «No hay ningún usuario seleccionado. Selecciona uno de la lista para eliminarlo.» con el título «Sin selección». No se realizan cambios.
7a. Si el Administrador no confirma la eliminación, el caso de uso finaliza sin cambios.
8a. Si el usuario seleccionado es el que tiene la sesión activa, el sistema muestra el mensaje «No se puede eliminar al usuario que tiene la sesión activa.». El usuario no se elimina.
8b. Si la eliminación dejaría permisos sin asignar a ningún otro usuario activo, el sistema muestra el mensaje «No se puede quitar el permiso porque dejaría patentes huérfanas.». El usuario no se elimina.
8c. Si ocurre un error al eliminar, el sistema muestra el mensaje «No se pudo eliminar el usuario.». El usuario no se elimina.

**Postcondiciones:** El usuario eliminado no figura en el listado y ya no puede iniciar sesión. Si el caso de uso no se completa, el usuario permanece sin cambios.

---

**ID y Nombre:** CUN05 Desbloquear usuario  
**Estado:** Implementado  
**Descripcion:** Permite al Administrador desbloquear a un usuario bloqueado por intentos fallidos de inicio de sesión, para que pueda volver a ingresar al sistema.  
**Actor Principal:** Administrador  
**Actor Secundario:** -  
**Precondiciones:** El Administrador tiene una sesión iniciada y cuenta con autorización para gestionar usuarios. Existe al menos un usuario bloqueado.  
**Puntos de Extension:** No aplica.  
**Condicion:** Un usuario no puede iniciar sesión porque su cuenta está bloqueada y el Administrador decide habilitarla nuevamente.  

**Escenario Principal:**
1. El Administrador selecciona la opción «Usuarios» del menú de administración.
2. El sistema presenta la pantalla «Gestión de usuarios» con el listado de usuarios, donde la columna «Bloqueado» identifica a los usuarios bloqueados.
3. El Administrador selecciona un usuario bloqueado.
4. El sistema muestra los datos del usuario y habilita el botón «Desbloquear».
5. El Administrador presiona el botón «Desbloquear».
6. El sistema solicita confirmación con el título «Confirmar desbloqueo» y el mensaje «Esta acción desbloqueará al usuario seleccionado y reiniciará su contador de intentos fallidos. ¿Desea continuar?».
7. El Administrador confirma el desbloqueo.
8. El sistema desbloquea al usuario y muestra el mensaje «Usuario desbloqueado correctamente.».
9. El sistema limpia la selección y actualiza el listado, donde el usuario ya no figura como bloqueado.

**Flujos Alternativos:**
4a. Si no hay un usuario seleccionado o el usuario seleccionado no está bloqueado, el botón «Desbloquear» permanece deshabilitado. El caso de uso finaliza sin cambios.
7a. Si el Administrador no confirma el desbloqueo, el caso de uso finaliza sin cambios.
8a. Si el usuario ya no se encuentra bloqueado, el sistema muestra el mensaje «El usuario seleccionado no está bloqueado.». No se realizan cambios.
8b. Si ocurre un error al desbloquear, el sistema muestra el mensaje «No se pudo desbloquear el usuario.». El usuario permanece bloqueado.

**Postcondiciones:** El usuario figura como no bloqueado, su contador de intentos fallidos se reinicia y puede iniciar sesión nuevamente. Si el caso de uso no se completa, el usuario permanece bloqueado.
