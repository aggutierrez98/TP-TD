# T06. Gestion de Bitacora y Control de Cambios

_Especificaciones de casos de uso_

A continuacion se especifican los casos de uso del modulo de bitacora y control de cambios segun la implementacion vigente: la consulta filtrada de eventos de bitacora y el registro centralizado de operaciones, fallos de negocio y excepciones tecnicas (bitacora); y la consulta del historial de versiones de un usuario con sus diferencias y la restauracion a una version historica (control de cambios).

---

**ID y Nombre:** CU01 Consultar bitacora  
**Estado:** Implementado  
**Descripcion:** Permite al administrador consultar los eventos registrados en la bitacora aplicando criterios de filtrado por fecha, usuario, actividad, nivel y tipo tecnico.  
**Actor Principal:** Administrador  
**Actor Secundario:** -  
**Precondiciones:** La aplicacion se encuentra iniciada, hay una sesion activa y la pantalla de bitacora es accesible desde la gestion de usuarios.  
**Puntos de Extension:** No aplica.  
**Condicion:** El administrador necesita auditar las operaciones y errores registrados por el sistema.  

**Escenario Principal:**
1. El administrador abre la pantalla de bitacora desde la gestion de usuarios.
2. El sistema carga los niveles de severidad disponibles y muestra los registros existentes ordenados por fecha descendente.
3. El administrador define los criterios de filtro: rango de fechas, usuario, actividad, nivel y si son solo eventos tecnicos.
4. El administrador presiona el boton de busqueda.
5. El sistema arma el filtro con los criterios seleccionados.
6. El sistema consulta los eventos de bitacora que cumplen el filtro.
7. El sistema aplica el filtro adicional por actividad sobre el texto traducido.
8. El sistema muestra los eventos resultantes en la grilla, traduciendo actividad, informacion y nivel.

**Flujos Alternativos:**
6a. Si ocurre una excepcion durante la consulta, el sistema muestra el error mediante el manejador de excepciones y finaliza el caso de uso.
8a. Si ningun evento cumple el filtro, la grilla se muestra vacia.

**Postcondiciones:** La grilla presenta los eventos de bitacora que cumplen los criterios seleccionados. El caso de uso es de solo lectura y no modifica el estado del sistema.

---

**ID y Nombre:** CU02 Registrar en bitacora  
**Estado:** Implementado  
**Descripcion:** Registra de forma centralizada un evento en la bitacora, clasificado como operacion exitosa, fallo de negocio o excepcion tecnica, generado por las operaciones auditables del sistema.  
**Actor Principal:** Sistema  
**Actor Secundario:** -  
**Precondiciones:** La bitacora fue configurada durante el arranque con la fabrica de unidades de trabajo y los proveedores de identidad de la sesion.  
**Puntos de Extension:** Incluido por los casos de uso que ejecutan operaciones auditables (Login, Registro, Alta/Edicion/Baja de usuario, Restauracion de version, entre otros).  
**Condicion:** Una operacion de negocio auditable finaliza con exito, con un fallo de negocio o con una excepcion tecnica y debe dejar traza.  

**Escenario Principal:**
1. Una operacion de la capa de negocio invoca el registro de un evento indicando actividad, informacion y origen.
2. El sistema determina el nivel del evento segun el tipo de registro: operacion, fallo de negocio o excepcion tecnica.
3. El sistema obtiene el identificador y el nombre del usuario de la sesion actual.
4. El sistema construye el evento de bitacora con fecha y hora actual, identidad, actividad, informacion, nivel, origen y marca tecnica.
5. El sistema persiste el evento, participando de la unidad de trabajo de la operacion cuando se proporciona una; si no, usa su propia unidad de trabajo.
6. El sistema devuelve el identificador del evento registrado.

**Flujos Alternativos:**
1a. Si la bitacora no fue configurada, el sistema lanza una excepcion de operacion invalida.
3a. Si no hay sesion iniciada, la identidad del usuario queda nula y el evento se registra igual.
5a. Si el registro corresponde a una excepcion tecnica, la informacion se compone con el tipo y el mensaje de la excepcion y el nivel es Error.

**Postcondiciones:** El evento queda persistido en la tabla Bitacora, disponible para su consulta posterior.

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

**ID y Nombre:** CU04 Restaurar punto del historial de usuario  
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
