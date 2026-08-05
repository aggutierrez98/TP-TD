# T06a. Gestion de Bitacora

_Especificaciones de casos de uso_

A continuacion se especifican los casos de uso de la bitacora del sistema segun la implementacion vigente: la consulta filtrada de eventos y el registro centralizado de operaciones, fallos de negocio y excepciones tecnicas.

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
**Precondiciones:** La bitacora fue configurada durante el arranque con el registrador de persistencia y los proveedores de identidad de la sesion.  
**Puntos de Extension:** Incluido por los casos de uso que ejecutan operaciones auditables (Login, Registro, Alta/Edicion/Baja de usuario, Restauracion de version, entre otros).  
**Condicion:** Una operacion de negocio auditable finaliza con exito, con un fallo de negocio o con una excepcion tecnica y debe dejar traza.  

**Escenario Principal:**
1. Una operacion de la capa de negocio invoca el registro de un evento indicando actividad, informacion y origen.
2. El sistema determina el nivel del evento segun el tipo de registro: operacion, fallo de negocio o excepcion tecnica.
3. El sistema obtiene el identificador y el nombre del usuario de la sesion actual.
4. El sistema construye el evento de bitacora con fecha y hora actual, identidad, actividad, informacion, nivel, origen y marca tecnica.
5. El sistema persiste el evento, participando de la transaccion de la operacion cuando se proporciona un contexto de persistencia.
6. El sistema devuelve el identificador del evento registrado.

**Flujos Alternativos:**
1a. Si la bitacora no fue configurada, el sistema lanza una excepcion de operacion invalida.
3a. Si no hay sesion iniciada, la identidad del usuario queda nula y el evento se registra igual.
5a. Si el registro corresponde a una excepcion tecnica, la informacion se compone con el tipo y el mensaje de la excepcion y el nivel es Error.

**Postcondiciones:** El evento queda persistido en la tabla Bitacora, disponible para su consulta posterior.
