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

---

**ID y Nombre:** CU02 Registro  
**Estado:** Implementado  
**Descripcion:** Permite crear un nuevo usuario en el sistema a partir de un nombre de usuario y una contrasena, dejandolo activo y con el perfil basico de permisos.  
**Actor Principal:** Usuario  
**Actor Secundario:** -  
**Precondiciones:** La aplicacion esta iniciada y el usuario aun no esta autenticado.  
**Puntos de Extension:** Es invocado desde CU01 Login mediante el boton Registrarse.  
**Condicion:** Una persona desea crear una cuenta para poder autenticarse en el sistema.  

**Escenario Principal:**
1. El usuario presiona el boton Registrarse desde la pantalla de login.
2. El sistema abre el formulario de registro en una ventana modal.
3. El usuario ingresa nombre de usuario y contrasena.
4. El usuario presiona el boton de registro.
5. El sistema valida que el nombre de usuario y la contrasena no esten vacios.
6. El sistema genera el hash seguro de la contrasena (PBKDF2).
7. El sistema verifica que no exista otro usuario con el mismo nombre de usuario.
8. El sistema calcula el digito verificador horizontal (DVH) del nuevo usuario y lo inserta en la base.
9. El sistema asigna al nuevo usuario el perfil basico de permisos.
10. El sistema guarda la version inicial en el historial del usuario y registra la operacion en la bitacora.
11. El sistema actualiza el digito verificador vertical (DVV) de la tabla de usuarios y confirma la transaccion.
12. El sistema informa que el usuario fue registrado con exito y cierra el formulario de registro.

**Flujos Alternativos:**
5a. Si el nombre de usuario o la contrasena estan vacios, el sistema informa que se deben completar los campos y finaliza el caso de uso.
7a. Si ya existe un usuario con el mismo nombre de usuario, el sistema registra el fallo en la bitacora, informa que el nombre de usuario ya existe y finaliza el caso de uso.
12a. Si ocurre una excepcion tecnica durante el alta, el sistema informa el error de registro y finaliza el caso de uso.

**Postcondiciones:** El nuevo usuario queda almacenado y activo, con la contrasena hasheada, el perfil basico asignado, la version inicial en su historial, los digitos verificadores (DVH/DVV) actualizados y la operacion registrada en la bitacora. El usuario regresa a la pantalla de login.

---

**ID y Nombre:** CU03 Logout  
**Estado:** Implementado  
**Descripcion:** Permite al usuario cerrar su sesion activa, limpiando sus permisos efectivos y regresando a la pantalla de login.  
**Actor Principal:** Usuario  
**Actor Secundario:** -  
**Precondiciones:** El usuario tiene una sesion iniciada y se encuentra en la pantalla de gestion de usuarios.  
**Puntos de Extension:** Al cerrarse la pantalla de gestion, el sistema reejecuta el cierre de sesion por seguridad (evento FormClosed).  
**Condicion:** El usuario desea terminar su sesion en el sistema.  

**Escenario Principal:**
1. El usuario presiona el boton de cierre de sesion en la pantalla de gestion de usuarios.
2. El sistema solicita confirmacion para cerrar la sesion.
3. El usuario confirma el cierre de sesion.
4. El sistema limpia los permisos efectivos de la sesion.
5. El sistema cierra la sesion del usuario.
6. El sistema cierra la pantalla de gestion de usuarios y regresa a la pantalla de login.

**Flujos Alternativos:**
3a. Si el usuario no confirma, el sistema mantiene la sesion abierta y permanece en la pantalla de gestion de usuarios.

**Postcondiciones:** La sesion del usuario queda cerrada y sin permisos efectivos, y la pantalla de login queda visible.
