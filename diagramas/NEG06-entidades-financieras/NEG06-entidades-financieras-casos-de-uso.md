# Entidades financieras

_Especificaciones de casos de uso_

---

**ID y Nombre:** CUN23 Consultar entidades financieras  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador consultar el catálogo de entidades financieras (bancos, billeteras virtuales) y los medios de pago que ofrece cada una.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada y cuenta con autorización para acceder a la gestión de entidades financieras.  
**Puntos de Extension:** Desde el detalle de una entidad se extiende a CUN27 Asociar medio de pago a entidad financiera y CUN28 Quitar medio de pago de entidad financiera.  
**Condicion:** El actor desea conocer las entidades financieras registradas y los medios de pago que ofrece cada una.  

**Escenario Principal:**
1. El actor selecciona la opción «Entidades financieras» del menú «Operador».
2. El sistema presenta la pantalla «Gestión de entidades financieras» con el listado de entidades financieras ordenado alfabéticamente, indicando para cada una su «Nombre».
3. El actor selecciona una entidad del listado.
4. El sistema muestra el nombre de la entidad seleccionada en el campo «Nombre:».
5. El actor presiona el botón «Ver detalle».
6. El sistema presenta la ventana «Detalle de la entidad financiera», con el nombre de la entidad junto a «Entidad:» y, en la sección «Medios de pago de esta entidad», el listado de medios de pago que ofrece, indicando para cada uno su «Tipo», «Marca» y «Segmento».

**Flujos Alternativos:**
2a. Si no hay entidades registradas, el sistema presenta el listado vacío. El caso de uso finaliza sin selección de entidad.
2b. Si ocurre un error al cargar el listado, el sistema muestra un mensaje de error. El caso de uso finaliza sin completar la consulta.
3a. Si el actor solo necesita consultar el listado, omite la selección de una entidad. El caso de uso finaliza con el listado disponible.
5a. Si el actor presiona «Ver detalle» sin haber seleccionado una entidad, el sistema muestra el mensaje «Debe seleccionar una entidad financiera.» y no abre la ventana de detalle.
6a. Si la entidad no ofrece medios de pago, el sistema presenta el listado de la sección «Medios de pago de esta entidad» vacío.
6b. Si ocurre un error al cargar los medios de pago de la entidad, el sistema muestra un mensaje de error. La ventana permanece abierta sin el listado.

**Postcondiciones:** El actor dispone del listado de entidades financieras y, si abrió el detalle, de los medios de pago que ofrece la entidad seleccionada. La consulta no modifica las entidades ni sus medios de pago.

---

**ID y Nombre:** CUN24 Alta de entidad financiera  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador registrar una nueva entidad financiera en el catálogo.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada y cuenta con autorización para acceder a la gestión de entidades financieras. No existe otra entidad con el mismo nombre (sin distinguir mayúsculas de minúsculas).  
**Puntos de Extension:** No aplica.  
**Condicion:** El actor desea incorporar una entidad financiera que aún no figura en el catálogo.  

**Escenario Principal:**
1. El actor selecciona la opción «Entidades financieras» del menú «Operador».
2. El sistema presenta la pantalla «Gestión de entidades financieras» con el listado de entidades registradas.
3. El actor ingresa el nombre de la nueva entidad en el campo «Nombre:».
4. El actor presiona el botón «Agregar».
5. El sistema registra la entidad, vacía el campo «Nombre:», actualiza el listado con la nueva entidad seleccionada y muestra el mensaje «Entidad financiera creada con éxito.».

**Flujos Alternativos:**
4a. Si el nombre está vacío o solo contiene espacios, el sistema muestra el mensaje «El nombre de la entidad financiera es obligatorio.». No se registra la entidad.
4b. Si ya existe una entidad con ese nombre, el sistema muestra el mensaje «Ya existe una entidad financiera con ese nombre.». No se registra la entidad.
4c. Si ocurre un error al registrar la entidad, el sistema muestra un mensaje de error. No se registra la entidad.

**Postcondiciones:** La nueva entidad financiera figura en el catálogo, sin medios de pago asociados, y queda disponible en el listado.

---

**ID y Nombre:** CUN25 Modificar entidad financiera  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador modificar el nombre de una entidad financiera existente.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada y cuenta con autorización para acceder a la gestión de entidades financieras. Existe al menos una entidad financiera registrada.  
**Puntos de Extension:** No aplica.  
**Condicion:** El actor desea corregir o actualizar el nombre de una entidad financiera.  

**Escenario Principal:**
1. El actor selecciona la opción «Entidades financieras» del menú «Operador».
2. El sistema presenta la pantalla «Gestión de entidades financieras» con el listado de entidades registradas.
3. El actor selecciona del listado la entidad a modificar.
4. El sistema muestra el nombre de la entidad seleccionada en el campo «Nombre:».
5. El actor modifica el nombre en el campo «Nombre:».
6. El actor presiona el botón «Actualizar».
7. El sistema guarda el nuevo nombre, actualiza el listado manteniendo seleccionada la entidad y muestra el mensaje «Entidad financiera actualizada con éxito.».

**Flujos Alternativos:**
6a. Si el actor presiona «Actualizar» sin haber seleccionado una entidad, el sistema muestra el mensaje «Debe seleccionar una entidad financiera.». No se realizan cambios.
6b. Si el nombre está vacío o solo contiene espacios, el sistema muestra el mensaje «El nombre de la entidad financiera es obligatorio.». No se realizan cambios.
6c. Si ya existe otra entidad con ese nombre, el sistema muestra el mensaje «Ya existe una entidad financiera con ese nombre.». No se realizan cambios.
6d. Si la entidad seleccionada ya no existe, el sistema muestra el mensaje «No se encontró la entidad financiera indicada.». No se realizan cambios.
6e. Si ocurre un error al guardar, el sistema muestra un mensaje de error. No se realizan cambios.

**Postcondiciones:** La entidad financiera figura en el listado con su nuevo nombre. Sus medios de pago asociados se mantienen sin cambios.

---

**ID y Nombre:** CUN26 Baja de entidad financiera  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador eliminar una entidad financiera del catálogo cuando no tiene datos asociados.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada y cuenta con autorización para acceder a la gestión de entidades financieras. Existe al menos una entidad financiera registrada. La entidad a eliminar no tiene medios de pago asociados, cuentas de usuarios ni promociones que la referencien.  
**Puntos de Extension:** Si la entidad tiene medios de pago asociados, estos deben quitarse previamente mediante CUN28 Quitar medio de pago de entidad financiera.  
**Condicion:** El actor desea retirar del catálogo una entidad financiera que ya no corresponde.  

**Escenario Principal:**
1. El actor selecciona la opción «Entidades financieras» del menú «Operador».
2. El sistema presenta la pantalla «Gestión de entidades financieras» con el listado de entidades registradas.
3. El actor selecciona del listado la entidad a eliminar.
4. El actor presiona el botón «Eliminar».
5. El sistema solicita confirmación mediante el mensaje «Se eliminará la entidad {nombre}. ¿Desea continuar?», titulado «Confirmar eliminación».
6. El actor confirma la eliminación.
7. El sistema elimina la entidad, vacía el campo «Nombre:», actualiza el listado sin la entidad eliminada y muestra el mensaje «Entidad financiera eliminada con éxito.».

**Flujos Alternativos:**
4a. Si el actor presiona «Eliminar» sin haber seleccionado una entidad, el sistema muestra el mensaje «Debe seleccionar una entidad financiera.». No se elimina nada.
6a. Si el actor no confirma la eliminación, el sistema cierra la confirmación y mantiene la entidad en el listado.
7a. Si la entidad tiene medios de pago, cuentas de usuarios o promociones asociados, el sistema muestra el mensaje «No se puede eliminar la entidad porque tiene medios de pago, cuentas o promociones asociados.». La entidad se conserva.
7b. Si la entidad seleccionada ya no existe, el sistema muestra el mensaje «No se encontró la entidad financiera indicada.». No se elimina nada.
7c. Si ocurre un error al eliminar, el sistema muestra un mensaje de error. La entidad se conserva.

**Postcondiciones:** La entidad financiera deja de figurar en el catálogo y en el listado.

---

**ID y Nombre:** CUN27 Asociar medio de pago a entidad financiera  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador asociar a una entidad financiera un medio de pago del catálogo, con un segmento opcional, para que la entidad lo ofrezca.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada y cuenta con autorización para acceder a la gestión de entidades financieras. Existe la entidad financiera y al menos un medio de pago en el catálogo. La entidad no ofrece ya ese medio de pago con el mismo segmento.  
**Puntos de Extension:** Si el medio de pago requerido no existe en el catálogo, se debe registrar previamente mediante CUN30 Alta de medio de pago.  
**Condicion:** El actor desea que una entidad financiera ofrezca un determinado medio de pago.  

**Escenario Principal:**
1. El actor selecciona la opción «Entidades financieras» del menú «Operador».
2. El sistema presenta la pantalla «Gestión de entidades financieras» con el listado de entidades registradas.
3. El actor selecciona del listado la entidad a la que desea asociar el medio de pago.
4. El actor presiona el botón «Ver detalle».
5. El sistema presenta la ventana «Detalle de la entidad financiera», con los medios de pago que la entidad ya ofrece y el selector «Medio de pago:», que lista los medios del catálogo por su tipo y marca.
6. El actor selecciona un medio de pago en «Medio de pago:».
7. El actor ingresa, si lo desea, un segmento en el campo «Segmento:».
8. El actor presiona el botón «Agregar».
9. El sistema asocia el medio de pago a la entidad, limpia el selector y el campo «Segmento:», agrega el medio al listado de la entidad seleccionándolo y muestra el mensaje «Medio de pago agregado a la entidad.».

**Flujos Alternativos:**
4a. Si el actor presiona «Ver detalle» sin haber seleccionado una entidad, el sistema muestra el mensaje «Debe seleccionar una entidad financiera.» y no abre la ventana de detalle.
8a. Si el actor no seleccionó un medio de pago, el sistema muestra el mensaje «Debe seleccionar un medio de pago.». No se realizan cambios.
8b. Si la entidad ya ofrece ese medio de pago con el mismo segmento (sin distinguir mayúsculas de minúsculas), el sistema muestra el mensaje «La entidad ya ofrece ese medio de pago con ese segmento.». No se realizan cambios.
8c. Si la entidad o el medio de pago seleccionado ya no existen, el sistema muestra el mensaje «No se encontró la entidad financiera indicada.» o «No se encontró el medio de pago indicado.», según corresponda. No se realizan cambios.
8d. Si ocurre un error al asociar el medio de pago, el sistema muestra un mensaje de error. No se realizan cambios.

**Postcondiciones:** La entidad financiera ofrece el medio de pago con el segmento indicado, que figura en el listado de la sección «Medios de pago de esta entidad».

---

**ID y Nombre:** CUN28 Quitar medio de pago de entidad financiera  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador quitar un medio de pago del catálogo de una entidad financiera para que deje de ofrecerlo.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada y cuenta con autorización para acceder a la gestión de entidades financieras. La entidad ofrece al menos un medio de pago. El medio de pago a quitar no está contratado en cuentas de usuarios.  
**Puntos de Extension:** No aplica.  
**Condicion:** El actor desea que una entidad financiera deje de ofrecer un medio de pago.  

**Escenario Principal:**
1. El actor selecciona la opción «Entidades financieras» del menú «Operador».
2. El sistema presenta la pantalla «Gestión de entidades financieras» con el listado de entidades registradas.
3. El actor selecciona del listado la entidad correspondiente.
4. El actor presiona el botón «Ver detalle».
5. El sistema presenta la ventana «Detalle de la entidad financiera» con los medios de pago que la entidad ofrece, indicando su «Tipo», «Marca» y «Segmento».
6. El actor selecciona el medio de pago a quitar.
7. El actor presiona el botón «Quitar».
8. El sistema solicita confirmación mediante el mensaje «Se quitará el medio de pago seleccionado de la entidad. ¿Desea continuar?», titulado «Confirmar baja».
9. El actor confirma la baja.
10. El sistema quita el medio de pago, actualiza el listado de la entidad sin ese medio y muestra el mensaje «Medio de pago quitado de la entidad.».

**Flujos Alternativos:**
4a. Si el actor presiona «Ver detalle» sin haber seleccionado una entidad, el sistema muestra el mensaje «Debe seleccionar una entidad financiera.» y no abre la ventana de detalle.
7a. Si el actor presiona «Quitar» sin haber seleccionado un medio de pago de la grilla, el sistema muestra el mensaje «Debe seleccionar un medio de pago de la grilla.». No se realizan cambios.
9a. Si el actor no confirma la baja, el sistema cierra la confirmación y mantiene el medio de pago en el listado.
10a. Si el medio de pago está contratado en cuentas de usuarios, el sistema muestra el mensaje «No se puede quitar el medio de pago porque esta contratado en cuentas de usuarios.». El medio se conserva.
10b. Si el medio de pago de la entidad ya no existe, el sistema muestra el mensaje «No se encontró el medio de pago de la entidad indicado.». No se realizan cambios.
10c. Si ocurre un error al quitar el medio de pago, el sistema muestra un mensaje de error. El medio se conserva.

**Postcondiciones:** La entidad financiera deja de ofrecer el medio de pago, que ya no figura en su listado. El medio de pago continúa existiendo en el catálogo general.
