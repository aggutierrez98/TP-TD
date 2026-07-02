# T06b. Gestion de Control de Cambios

_Especificaciones de casos de uso_

Casos de uso del control de cambios sobre usuarios: consulta del historial de versiones (con sus diferencias) y restauracion de un punto anterior, segun la implementacion vigente en UsuarioBLL, UsuarioEstadoBLL y HistorialEstadosService.

---

**ID y Nombre:** CU03 Consultar historial de usuario  
**Estado:** Implementado  
**Descripcion:** Permite al administrador consultar las versiones historicas de un usuario y ver las diferencias entre una version y la anterior.  
**Actor Principal:** Administrador  
**Actor Secundario:** -  
**Precondiciones:** El administrador tiene una sesion iniciada con el permiso de historial de usuarios y hay un usuario seleccionado.  
**Puntos de Extension:** Desde esta pantalla se puede ejecutar CU04 Restaurar punto del historial.  
**Condicion:** El administrador necesita revisar los cambios que sufrio un usuario a lo largo del tiempo.  

**Escenario Principal:**
1. El administrador selecciona un usuario y abre la pantalla de historial de usuario.
2. El sistema obtiene las versiones del historial del usuario, ordenadas de la mas reciente a la mas antigua, y las muestra.
3. El administrador selecciona una version del historial.
4. El sistema calcula las diferencias de esa version respecto de la version anterior.
5. El sistema muestra los cambios (campo, valor anterior y valor nuevo) traducidos al idioma actual.

**Flujos Alternativos:**
2a. Si ocurre una excepcion durante la consulta del historial, el sistema informa el error y finaliza el caso de uso.
4a. Si ocurre una excepcion durante el calculo de diferencias, el sistema informa el error y finaliza el caso de uso.

**Postcondiciones:** Se muestran las versiones del historial del usuario y las diferencias de la version seleccionada. El caso de uso no modifica datos.

---

**ID y Nombre:** CU04 Restaurar punto del historial  
**Estado:** Implementado  
**Descripcion:** Permite al administrador restaurar el estado de un usuario a una version anterior de su historial, recalculando los digitos verificadores.  
**Actor Principal:** Administrador  
**Actor Secundario:** -  
**Precondiciones:** El administrador tiene una sesion iniciada con el permiso de historial de usuarios y existe la version a restaurar.  
**Puntos de Extension:** Se ejecuta desde la pantalla de historial de usuario (CU03).  
**Condicion:** El administrador necesita volver un usuario a un estado anterior registrado en su historial.  

**Escenario Principal:**
1. El administrador selecciona una version en el historial del usuario.
2. El administrador presiona restaurar version.
3. El sistema solicita confirmacion.
4. El administrador confirma la restauracion.
5. El sistema obtiene el snapshot de la version seleccionada.
6. El sistema aplica el snapshot al usuario (datos, permisos y preferencia de idioma).
7. El sistema recalcula el digito verificador horizontal (DVH) del usuario restaurado.
8. El sistema guarda una nueva version de restauracion en el historial y registra la operacion en la bitacora.
9. El sistema actualiza el digito verificador vertical (DVV) de la tabla de usuarios y confirma la transaccion.
10. Si el usuario restaurado es el de la sesion actual, el sistema refresca su nombre, permisos e idioma.
11. El sistema informa que la version fue restaurada y recarga el historial.

**Flujos Alternativos:**
1a. Si no hay una version seleccionada, el sistema informa que se debe seleccionar una version y finaliza el caso de uso.
4a. Si el administrador no confirma, el sistema finaliza el caso de uso sin cambios.
5a. Si la version no existe, el sistema informa el error y finaliza el caso de uso.
5b. Si se intenta restaurar a inactivo el usuario de la sesion actual, el sistema informa el error y finaliza el caso de uso.
9a. Si ocurre una excepcion tecnica durante la restauracion, el sistema informa el error y finaliza el caso de uso.

**Postcondiciones:** El usuario queda restaurado al estado del snapshot, con los digitos verificadores (DVH/DVV) actualizados, una version de restauracion en su historial y la operacion registrada en la bitacora. Si es el usuario de la sesion actual, su sesion queda refrescada.
