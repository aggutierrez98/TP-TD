# T02. Gestion de Login y Logout

_Especificaciones de casos de uso_

Casos de uso de autenticacion del sistema (login, registro y logout), segun la implementacion vigente en UsuarioBLL y las pantallas de la capa UI.

---

**ID y Nombre:** CU01 Login  
**Estado:** Implementado  
**Descripcion:** Permite que un usuario registrado y activo inicie sesion en el sistema mediante su nombre de usuario y contrasena, quedando disponibles sus permisos e idioma.  
**Actor Principal:** Usuario  
**Actor Secundario:** -  
**Precondiciones:** La aplicacion esta iniciada y supero la verificacion de integridad de arranque (T07). El usuario ya existe en la base de datos y se encuentra activo.  
**Puntos de Extension:** CU02 Registro (se accede desde el boton Registrarse de la pantalla de login).  
**Condicion:** El usuario desea autenticarse para acceder a la gestion de usuarios del sistema.  

**Escenario Principal:**
1. El usuario ingresa su nombre de usuario y su contrasena en la pantalla de login.
2. El usuario presiona el boton de ingreso.
3. El sistema valida que el nombre de usuario y la contrasena no esten vacios.
4. El sistema busca al usuario por su nombre de usuario a traves del repositorio.
5. El sistema verifica que el usuario este activo.
6. El sistema verifica la contrasena ingresada contra el hash almacenado (PBKDF2).
7. El sistema inicia la sesion del usuario.
8. El sistema obtiene los permisos directos del usuario y actualiza los permisos efectivos de la sesion.
9. El sistema aplica el idioma configurado para el usuario.
10. El sistema registra en la bitacora la operacion de login exitoso.
11. El sistema limpia los campos del formulario y abre la pantalla de gestion de usuarios con la sesion iniciada.

**Flujos Alternativos:**
3a. Si el nombre de usuario o la contrasena estan vacios, el sistema registra un fallo de negocio en la bitacora, informa que se deben completar los campos y finaliza el caso de uso.
4a. Si el usuario no existe, el sistema registra el fallo en la bitacora, informa credenciales incorrectas y finaliza el caso de uso.
5a. Si el usuario existe pero esta inactivo, el sistema registra el fallo en la bitacora, informa credenciales incorrectas y finaliza el caso de uso.
6a. Si la contrasena no coincide con el hash almacenado, el sistema registra el fallo en la bitacora, informa credenciales incorrectas y finaliza el caso de uso.
11a. Si ocurre una excepcion tecnica durante el proceso, el sistema muestra el error de login al usuario y finaliza el caso de uso.

**Postcondiciones:** La sesion del usuario queda iniciada con sus permisos efectivos e idioma cargados, la pantalla de gestion de usuarios queda visible y el login exitoso queda registrado en la bitacora.
