# T06a. Gestion de Bitacora

_Especificaciones de casos de uso_

Casos de uso de la bitacora del sistema: consulta de los eventos auditados y registro de operaciones, fallos de negocio y excepciones tecnicas, segun la implementacion vigente en BitacoraBLL y FormBitacora.

---

**ID y Nombre:** CU01 Consultar bitacora  
**Estado:** Implementado  
**Descripcion:** Permite al administrador consultar los eventos registrados en la bitacora aplicando filtros por fecha, usuario, nivel, tipo y actividad.  
**Actor Principal:** Administrador  
**Actor Secundario:** -  
**Precondiciones:** El administrador tiene una sesion iniciada con el permiso de ver bitacora.  
**Puntos de Extension:** No aplica.  
**Condicion:** El administrador necesita revisar las operaciones y fallos registrados por el sistema.  

**Escenario Principal:**
1. El administrador abre la pantalla de bitacora.
2. El administrador define los filtros (rango de fechas, usuario, nivel y solo tecnicas) y presiona buscar.
3. El sistema arma el filtro y busca los eventos de bitacora que cumplen los criterios.
4. El sistema aplica ademas el filtro por el texto de actividad ingresado.
5. El sistema muestra los eventos en la grilla, traduciendo actividad, informacion y nivel al idioma actual.

**Flujos Alternativos:**
5a. Si ocurre una excepcion durante la consulta, el sistema informa el error y finaliza el caso de uso.

**Postcondiciones:** Se muestra el listado de eventos de bitacora que cumplen los filtros. El caso de uso no modifica datos.

---

**ID y Nombre:** CU02 Registrar en bitacora  
**Estado:** Implementado  
**Descripcion:** El sistema deja constancia de una operacion, un fallo de negocio o una excepcion tecnica, generando un evento en la bitacora.  
**Actor Principal:** Usuario  
**Actor Secundario:** -  
**Precondiciones:** Hay un caso de uso en ejecucion que necesita auditarse.  
**Puntos de Extension:** Caso de uso incluido (<<include>>) por los demas casos de uso del sistema que registran auditoria (login, registro, gestion de permisos, gestion de idiomas, verificacion de integridad, etc.).  
**Condicion:** Un caso de uso en ejecucion necesita dejar constancia de una operacion, un fallo de negocio o una excepcion tecnica.  

**Escenario Principal:**
1. Un caso de uso en ejecucion solicita registrar un evento (operacion, fallo de negocio o excepcion tecnica).
2. El sistema construye el evento con fecha y hora, el usuario actual de la sesion, la actividad, la informacion, el nivel y el origen.
3. El sistema persiste el evento en la bitacora.
4. Si el registro forma parte de una operacion transaccional, se confirma junto con esa operacion; si es autonomo, se confirma en su propia unidad de trabajo.

**Flujos Alternativos:**
2a. Si no hay una sesion activa (por ejemplo, un login fallido o el arranque del sistema), el evento se registra con usuario nulo, conservando el resto de los datos.

**Postcondiciones:** El evento queda persistido en la bitacora con su nivel (Informacion, Advertencia o Error).
