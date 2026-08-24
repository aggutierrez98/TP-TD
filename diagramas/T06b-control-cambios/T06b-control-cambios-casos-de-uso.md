# T06b. Gestion de Control de Cambios

_Especificaciones de casos de uso_

A continuacion se especifican los casos de uso del control de cambios (historial de estados de usuario) segun la implementacion vigente: la consulta del historial de versiones con sus diferencias y la restauracion de un usuario a una version historica.

---

**ID y Nombre:** CU03 Consultar historial de usuario  
**Estado:** Implementado  
**Descripcion:** Permite al administrador ver las versiones historicas de un usuario y las diferencias entre una version seleccionada y su version anterior.  
**Actor Principal:** Administrador  
**Actor Secundario:** -  
**Precondiciones:** La aplicacion se encuentra iniciada, hay una sesion activa y se selecciono un usuario en la gestion de usuarios para abrir su historial.  
**Puntos de Extension:** Al seleccionar una version se dispara el calculo y la visualizacion de las diferencias contra la version anterior.  
**Condicion:** El administrador necesita revisar la evolucion del estado de un usuario a lo largo del tiempo.  

**Escenario Principal:**
1. El administrador abre la pantalla de historial de un usuario desde la gestion de usuarios.
2. El sistema obtiene las versiones historicas del usuario ordenadas por fecha descendente.
3. El sistema muestra las versiones en la grilla de historial.
4. El administrador selecciona una version del historial.
5. El sistema obtiene la version seleccionada y su version anterior.
6. El sistema compara ambas versiones campo a campo: usuario, contrasena, activo, idioma y permisos.
7. El sistema muestra las diferencias detectadas en la grilla de cambios de estado.

**Flujos Alternativos:**
4a. Si no hay ninguna version seleccionada, el sistema limpia la grilla de cambios de estado.
6a. Si la version seleccionada es la mas antigua, se compara contra un estado anterior nulo y todos los campos figuran como alta inicial.
6b. Si no hay diferencias entre ambas versiones, el sistema muestra una fila indicando que no hubo cambios.
5a. Si ocurre una excepcion durante la consulta, el sistema muestra el error mediante el manejador de excepciones y finaliza el caso de uso.

**Postcondiciones:** Las grillas presentan el historial de versiones y las diferencias de la version seleccionada. El caso de uso es de solo lectura y no modifica el estado del sistema.

---

**ID y Nombre:** CU04 Restaurar version de usuario  
**Estado:** Implementado  
**Descripcion:** Restaura el estado de un usuario a una version historica seleccionada, reescribiendo sus datos, permisos y preferencia de idioma, recalculando su integridad y dejando traza en historial y bitacora.  
**Actor Principal:** Administrador  
**Actor Secundario:** -  
**Precondiciones:** La aplicacion se encuentra iniciada, hay una sesion activa, existe historial para el usuario y se selecciono una version a restaurar.  
**Puntos de Extension:** Si el usuario restaurado es el de la sesion actual, se refrescan nombre, permisos e idioma de la sesion.  
**Condicion:** El administrador necesita revertir el estado de un usuario a una version anterior.  

**Escenario Principal:**
1. El administrador selecciona una version del historial y presiona restaurar.
2. El sistema solicita confirmacion de la restauracion.
3. El administrador confirma la operacion.
4. El sistema obtiene el snapshot de la version a restaurar.
5. El sistema abre una transaccion y obtiene el usuario actual.
6. El sistema aplica el snapshot: reescribe los datos del usuario, reemplaza sus permisos y su preferencia de idioma.
7. El sistema recalcula el digito verificador horizontal del usuario restaurado y actualiza el digito verificador vertical de la tabla.
8. El sistema guarda una nueva version de historial que registra la restauracion y deja traza en la bitacora.
9. El sistema confirma la transaccion.
10. Si el usuario restaurado es el de la sesion actual, el sistema refresca nombre, permisos e idioma de la sesion.
11. El sistema informa que la version fue restaurada y refresca las grillas.

**Flujos Alternativos:**
1a. Si no hay ninguna version seleccionada, el sistema advierte que debe seleccionar una version y finaliza el caso de uso.
3a. Si el administrador no confirma, el caso de uso finaliza sin cambios.
4a. Si la version no existe, el sistema lanza una excepcion de negocio y finaliza.
4b. Si el usuario de la sesion actual intenta restaurar una version inactiva de si mismo, el sistema rechaza la operacion.
5a. Si el usuario del snapshot ya no existe, el sistema lanza una excepcion de negocio y revierte la transaccion.
9a. Si ocurre una excepcion durante la restauracion, el sistema revierte la transaccion y propaga el error.

**Postcondiciones:** El usuario queda con el estado (datos, permisos e idioma) de la version restaurada, se registra una nueva version de historial y un evento de bitacora, y la integridad del usuario queda recalculada.
