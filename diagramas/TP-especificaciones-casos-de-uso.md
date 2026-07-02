# Especificaciones de Casos de Uso

## T02. Gestion de Login y Logout

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

## T04. Gestion de Perfiles de Usuario

_Especificaciones de casos de uso_

Casos de uso de administracion de permisos (patron Composite: familias y patentes), asignacion de perfiles a usuarios y control de acceso, segun la implementacion vigente en PermisoBLL, PermisoService y AutorizacionService.

---

**ID y Nombre:** CU01 Gestionar permisos  
**Estado:** Implementado  
**Descripcion:** Permite al administrador definir y mantener el catalogo de permisos del sistema, organizado como familias (grupos) y patentes (permisos simples) mediante una estructura Composite.  
**Actor Principal:** Administrador  
**Actor Secundario:** -  
**Precondiciones:** El administrador tiene una sesion iniciada y cuenta con el permiso de gestion de permisos.  
**Puntos de Extension:** El catalogo definido aqui es la base de la asignacion de perfiles (CU02) y del control de acceso (CU03).  
**Condicion:** El administrador necesita crear o modificar las familias y patentes que componen el catalogo de permisos.  

**Escenario Principal:**
1. El administrador abre la pantalla de gestion de permisos.
2. El sistema carga el catalogo de permisos y lo muestra como un arbol de familias y patentes.
3. El administrador crea familias nuevas y agrega familias o patentes hijas a una familia (composicion).
4. El administrador presiona guardar.
5. El sistema valida que el catalogo no este vacio.
6. El sistema valida que no existan ciclos entre las familias.
7. El sistema valida cada nombre (no vacio y sin punto), lo normaliza y verifica que no haya nombres duplicados en un mismo nivel.
8. El sistema valida que ninguna familia quede vacia.
9. El sistema guarda los nodos, reemplaza las relaciones padre-hijo y elimina los permisos que ya no forman parte del catalogo.
10. El sistema registra la operacion en la bitacora y recarga el catalogo actualizado.

**Flujos Alternativos:**
5a. Si el catalogo esta vacio, el sistema registra el fallo en la bitacora, informa el error y finaliza el caso de uso.
6a. Si se detecta un ciclo entre familias, el sistema informa el error y finaliza el caso de uso.
7a. Si un nombre esta vacio o contiene un punto, el sistema informa el error y finaliza el caso de uso.
7b. Si hay nombres duplicados en un mismo nivel, el sistema informa el error y finaliza el caso de uso.
8a. Si una familia queda sin hijos, el sistema informa que no se permiten grupos vacios y finaliza el caso de uso.
9a. Si ocurre una excepcion tecnica durante el guardado, el sistema informa el error y finaliza el caso de uso.

**Postcondiciones:** El catalogo de permisos queda persistido (nodos, relaciones de composicion y eliminaciones) y la operacion queda registrada en la bitacora.

---

**ID y Nombre:** CU02 Asignar perfil(es) a usuario  
**Estado:** Implementado  
**Descripcion:** Permite al administrador asignar o quitar permisos (familias y patentes) a un usuario, definiendo su perfil de acceso.  
**Actor Principal:** Administrador  
**Actor Secundario:** -  
**Precondiciones:** El administrador tiene una sesion iniciada con el permiso de gestion de usuarios, existe al menos un usuario y el catalogo de permisos (CU01) esta definido.  
**Puntos de Extension:** Los permisos asignables provienen del catalogo gestionado en CU01 Gestionar permisos.  
**Condicion:** El administrador necesita definir que permisos tiene un usuario.  

**Escenario Principal:**
1. El administrador selecciona un usuario en la grilla de usuarios.
2. El sistema carga el catalogo de permisos y marca los que el usuario ya tiene asignados.
3. El administrador marca o desmarca permisos en el arbol.
4. El administrador presiona guardar los permisos del usuario.
5. El sistema verifica que el usuario exista.
6. El sistema reemplaza los permisos del usuario por la seleccion, sin duplicados.
7. El sistema guarda una version en el historial del usuario y registra la operacion en la bitacora.
8. Si el usuario modificado es el de la sesion actual, el sistema actualiza sus permisos efectivos.
9. El sistema confirma que los permisos del usuario fueron guardados.

**Flujos Alternativos:**
4a. Si no hay ningun usuario seleccionado, el sistema informa que se debe seleccionar un usuario y finaliza el caso de uso.
5a. Si el usuario no existe, el sistema informa el error y finaliza el caso de uso.
7a. Si ocurre una excepcion tecnica durante el guardado, el sistema informa el error y finaliza el caso de uso.

**Postcondiciones:** Los permisos del usuario quedan actualizados, con una version registrada en su historial y en la bitacora. Si es el usuario de la sesion actual, sus permisos efectivos quedan refrescados.

---

**ID y Nombre:** CU03 Controlar acceso de usuario  
**Estado:** Implementado  
**Descripcion:** El sistema habilita o restringe las acciones y pantallas disponibles para el usuario segun sus permisos efectivos, expandiendo las familias hasta sus patentes.  
**Actor Principal:** Usuario  
**Actor Secundario:** -  
**Precondiciones:** El usuario tiene una sesion iniciada y sus permisos efectivos fueron cargados durante el login.  
**Puntos de Extension:** Los permisos efectivos se derivan del catalogo (CU01) y de la asignacion al usuario (CU02).  
**Condicion:** El usuario abre una pantalla con acciones protegidas o intenta ejecutar una accion protegida.  

**Escenario Principal:**
1. El usuario abre una pantalla que contiene acciones protegidas.
2. El sistema, para cada accion de la pantalla, consulta si el usuario tiene el permiso requerido.
3. El sistema calcula los permisos efectivos expandiendo las familias del usuario hasta sus patentes (hojas del Composite).
4. El sistema habilita o deshabilita cada control segun el usuario tenga o no el permiso.
5. El usuario ejecuta una accion protegida.
6. El sistema vuelve a validar el permiso correspondiente a esa accion.
7. El sistema ejecuta la accion solicitada.

**Flujos Alternativos:**
6a. Si el usuario no tiene el permiso requerido para la accion, el sistema informa que no esta permitido y no ejecuta la accion.

**Postcondiciones:** El usuario solo puede visualizar y ejecutar las acciones para las que posee permiso efectivo.
