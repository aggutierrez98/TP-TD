# Mis notificaciones

_Especificaciones de casos de uso_

---

**ID y Nombre:** CUN85 Consultar mis notificaciones  
**Estado:** Implementado  
**Descripcion:** Permite al Usuario, Operador o Administrador consultar sus propias notificaciones, filtrarlas por fecha y recorrerlas por páginas, así como ver el detalle de cada una.  
**Actor Principal:** Usuario, Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada.  
**Puntos de Extension:** Se extiende con CUN86 Marcar notificación propia como leída, que se inicia desde la misma pantalla. Ver el detalle de una notificación no leída la marca como leída.  
**Condicion:** El actor desea revisar los avisos que recibió.  

**Escenario Principal:**
1. El actor selecciona la opción «Mis notificaciones» del menú principal, que indica la cantidad de notificaciones no leídas.
2. El sistema presenta la pantalla «Mis notificaciones» ocupando el área de trabajo, con la sección «Filtros» («Desde:», «Hasta:» y el botón «Buscar»), el listado de las notificaciones del actor con las columnas «Estado», «Fecha», «Tipo» y «Mensaje», el indicador «Página 1 de N · M notificaciones» y los botones «Anterior», «Siguiente», «Ver detalle» y «Marcar como leída».
3. El actor activa el filtro «Desde:» y/o «Hasta:», elige las fechas deseadas y selecciona el botón «Buscar».
4. El sistema presenta la primera página de las notificaciones comprendidas en el rango de fechas elegido.
5. El actor selecciona los botones «Siguiente» y «Anterior» para recorrer las páginas.
6. El sistema presenta la página solicitada y actualiza el indicador de páginas.
7. El actor selecciona una notificación y el botón «Ver detalle».
8. El sistema presenta el diálogo «Detalle de notificación» con la fecha, el tipo y el mensaje completo de la notificación, y el botón «Cerrar». La notificación queda marcada como «Leída» en el listado.
9. El actor selecciona el botón «Cerrar».
10. El sistema cierra el diálogo y vuelve al listado.

**Flujos Alternativos:**
2a. Si el actor no tiene notificaciones en el rango consultado, el sistema muestra el mensaje «No hay notificaciones para estas fechas.».
2b. Si ocurre un error al cargar las notificaciones, el sistema muestra un mensaje de error. El caso de uso finaliza sin completar la consulta.
3a. Si el actor deja desactivados los filtros de fecha, el sistema considera todas las notificaciones del actor.
3b. Si la fecha «Desde:» es posterior a la fecha «Hasta:», el sistema muestra el mensaje «La fecha Desde no puede ser posterior a Hasta.» y conserva el listado anterior.
5a. Si el actor se encuentra en la primera página, el botón «Anterior» está deshabilitado; si se encuentra en la última, el botón «Siguiente» está deshabilitado.
7a. Si el actor hace doble clic sobre una notificación, el sistema procede como si hubiera seleccionado el botón «Ver detalle».
7b. Si el actor no seleccionó ninguna notificación, el botón «Ver detalle» está deshabilitado.
8a. Si ocurre un error al leer la notificación, el sistema muestra un mensaje de error y no presenta el detalle.

**Postcondiciones:** El actor dispone de sus notificaciones según el filtro aplicado. Las notificaciones cuyo detalle consultó quedan marcadas como leídas y el contador del menú principal se actualiza.

---

**ID y Nombre:** CUN86 Marcar notificación propia como leída  
**Estado:** Implementado  
**Descripcion:** Permite al Usuario, Operador o Administrador marcar como leída una de sus notificaciones no leídas, sin abrir su detalle.  
**Actor Principal:** Usuario, Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada y tiene abierta la pantalla «Mis notificaciones» con al menos una notificación en estado «No leída».  
**Puntos de Extension:** Extiende a CUN85 Consultar mis notificaciones.  
**Condicion:** El actor desea descartar un aviso pendiente sin leer su detalle.  

**Escenario Principal:**
1. El actor selecciona una notificación con estado «No leída» del listado.
2. El actor selecciona el botón «Marcar como leída».
3. El sistema registra la notificación como leída, muestra su estado como «Leída» en el listado y actualiza la cantidad de notificaciones no leídas indicada en el menú principal.

**Flujos Alternativos:**
1a. Si el actor no seleccionó ninguna notificación o seleccionó una que ya figura como «Leída», el botón «Marcar como leída» está deshabilitado.
3a. Si la notificación ya no existe o no pertenece al actor, el sistema muestra el mensaje «No se encontró la notificación indicada.» y no realiza cambios.
3b. Si ocurre un error al actualizar la notificación, el sistema muestra el mensaje «No se pudo actualizar la notificación.». El estado de la notificación permanece sin cambios.
3c. Mientras el sistema procesa la solicitud, los filtros y el listado permanecen deshabilitados.

**Postcondiciones:** La notificación queda marcada como leída y el contador de no leídas del menú principal se reduce en una unidad. Si el caso de uso no se completó, el estado de la notificación permanece sin cambios.
