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
**Puntos de Extension:** CU02 Registro (se accede desde el boton Registrarse de la pantalla de login). Un usuario que queda bloqueado solo puede volver a intentar login cuando el bloqueo temporal vence, o requiere que un administrador lo desbloquee desde la pantalla de Gestion de Usuarios (boton Desbloquear) si el bloqueo es permanente.  
**Condicion:** El usuario desea autenticarse para acceder a la gestion de usuarios del sistema.  

**Escenario Principal:**
1. El usuario ingresa su nombre de usuario y su contrasena en la pantalla de login.
2. El usuario presiona el boton de ingreso.
3. El sistema valida que el nombre de usuario y la contrasena no esten vacios.
4. El sistema busca al usuario por su nombre de usuario a traves del repositorio.
5. El sistema verifica que el usuario este activo.
6. El sistema verifica que la cuenta no tenga un bloqueo activo (o que un bloqueo temporal previo ya haya vencido).
7. El sistema verifica la contrasena ingresada contra el hash almacenado (PBKDF2).
8. El sistema resetea a cero el contador de intentos fallidos y limpia el estado de bloqueo del usuario.
9. El sistema inicia la sesion del usuario.
10. El sistema obtiene los permisos directos del usuario y actualiza los permisos efectivos de la sesion.
11. El sistema aplica el idioma configurado para el usuario.
12. El sistema registra en la bitacora la operacion de login exitoso.
13. El sistema limpia los campos del formulario y abre la pantalla de gestion de usuarios con la sesion iniciada.

**Flujos Alternativos:**
3a. Si el nombre de usuario o la contrasena estan vacios, el sistema registra un fallo de negocio en la bitacora, informa que se deben completar los campos y finaliza el caso de uso.
4a. Si el usuario no existe, el sistema registra el fallo en la bitacora, informa credenciales incorrectas y finaliza el caso de uso.
5a. Si el usuario existe pero esta inactivo, el sistema registra el fallo en la bitacora, informa credenciales incorrectas y finaliza el caso de uso.
6a. Si la cuenta esta bloqueada de forma permanente (5 intentos fallidos consecutivos sin login exitoso), el sistema registra el fallo en la bitacora, rechaza el intento sin verificar la contrasena ni incrementar el contador, informa que la cuenta esta bloqueada de forma permanente y que debe comunicarse con el administrador, y finaliza el caso de uso.
6b. Si la cuenta tiene un bloqueo temporal vigente (tercer o cuarto intento fallido, dentro de la ventana de 1 o 5 minutos), el sistema registra el fallo en la bitacora, rechaza el intento sin verificar la contrasena ni incrementar el contador, informa que la cuenta esta bloqueada temporalmente y que debe reintentar mas tarde, y finaliza el caso de uso.
7a. Si la contrasena no coincide con el hash almacenado y es el primer o el segundo intento fallido consecutivo, el sistema incrementa el contador de intentos fallidos, no aplica ningun bloqueo, registra el fallo en la bitacora, informa credenciales incorrectas (mensaje generico) y finaliza el caso de uso.
7b. Si la contrasena no coincide con el hash almacenado y es el tercer intento fallido consecutivo, el sistema incrementa el contador, aplica un bloqueo temporal de 1 minuto, recalcula el digito verificador horizontal (DVH) del usuario y actualiza el digito verificador vertical (DVV) de la tabla Usuarios dentro de la misma transaccion, registra el fallo en la bitacora, informa credenciales incorrectas (mensaje generico, sin revelar el bloqueo) y finaliza el caso de uso.
7c. Si la contrasena no coincide con el hash almacenado y es el cuarto intento fallido consecutivo (una vez vencido el bloqueo temporal corto), el sistema incrementa el contador, aplica un bloqueo temporal de 5 minutos, recalcula DVH/DVV de la misma forma, registra el fallo en la bitacora, informa credenciales incorrectas (mensaje generico) y finaliza el caso de uso.
7d. Si la contrasena no coincide con el hash almacenado y es el quinto intento fallido consecutivo (una vez vencido el bloqueo temporal largo), el sistema incrementa el contador, aplica un bloqueo permanente (requiere desbloqueo manual de un administrador), recalcula DVH/DVV de la misma forma, registra el fallo en la bitacora, informa credenciales incorrectas (mensaje generico) y finaliza el caso de uso.
13a. Si ocurre una excepcion tecnica durante el proceso (por ejemplo al persistir el contador de intentos, el estado de bloqueo o los digitos verificadores), el sistema muestra el error de login al usuario y finaliza el caso de uso.

**Postcondiciones:** Si el login fue exitoso: la sesion del usuario queda iniciada con sus permisos efectivos e idioma cargados, el contador de intentos fallidos y el estado de bloqueo quedan en cero/limpio, la pantalla de gestion de usuarios queda visible y el login exitoso queda registrado en la bitacora. Si el login fallo por contrasena invalida: el contador de intentos fallidos del usuario queda incrementado y, segun el umbral alcanzado (3, 4 o 5 intentos), el usuario puede quedar con un bloqueo temporal o permanente vigente en la base de datos, con su DVH y el DVV de la tabla Usuarios actualizados de forma consistente.

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
8. El sistema inserta el nuevo usuario en la base, calcula su digito verificador horizontal (DVH) a partir del Id asignado y lo persiste.
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
