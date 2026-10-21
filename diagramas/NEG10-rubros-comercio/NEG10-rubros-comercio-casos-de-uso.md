# Rubros de comercio

_Especificaciones de casos de uso_

---

**ID y Nombre:** CUN44 Consultar rubros de comercio  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador consultar los rubros de comercio registrados en el sistema.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada y cuenta con autorización para acceder a la gestión de comercios.  
**Puntos de Extension:** No aplica.  
**Condicion:** El actor desea conocer los rubros con los que se clasifican los comercios.  

**Escenario Principal:**
1. El actor selecciona la opción «Comercios» del menú «Operador».
2. El sistema presenta la pantalla «Gestion de Comercios», que incluye la sección «Rubros de comercio» con el listado de rubros ordenado alfabéticamente, indicando para cada uno su «Nombre».
3. El actor selecciona un rubro del listado.
4. El sistema muestra el nombre del rubro seleccionado en el campo «Nombre:» de la sección «Rubros de comercio».

**Flujos Alternativos:**
2a. Si no hay rubros registrados, el sistema presenta el listado vacío. El caso de uso finaliza sin selección.
2b. Si ocurre un error al cargar el listado, el sistema muestra un mensaje de error. El caso de uso finaliza sin completar la consulta.
3a. Si el actor solo necesita consultar el listado, omite la selección de un rubro. El caso de uso finaliza con el listado disponible.

**Postcondiciones:** El actor dispone del listado de rubros y, si seleccionó uno, de su nombre. La consulta no modifica los rubros.

---

**ID y Nombre:** CUN45 Alta de rubro de comercio  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador registrar un nuevo rubro de comercio.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada y cuenta con autorización para acceder a la gestión de comercios. No existe otro rubro con el mismo nombre.  
**Puntos de Extension:** No aplica.  
**Condicion:** El actor desea incorporar un rubro que aún no figura para clasificar comercios.  

**Escenario Principal:**
1. El actor selecciona la opción «Comercios» del menú «Operador».
2. El sistema presenta la pantalla «Gestion de Comercios» con la sección «Rubros de comercio» y su listado de rubros.
3. El actor ingresa el nombre del nuevo rubro en el campo «Nombre:» de la sección «Rubros de comercio».
4. El actor presiona el botón «Agregar» de la sección «Rubros de comercio».
5. El sistema registra el rubro, vacía el campo «Nombre:», actualiza el listado con el nuevo rubro seleccionado, incorpora el rubro al selector «Rubro:» de la sección de comercios y muestra el mensaje «Rubro creado con exito.».

**Flujos Alternativos:**
4a. Si el nombre está vacío o solo contiene espacios, el sistema muestra el mensaje «Debe indicar un nombre para el rubro de comercio.». No se registra el rubro.
4b. Si ya existe un rubro con ese nombre o si ocurre otro error al registrarlo, el sistema muestra el mensaje de error «No se pudo crear el rubro de comercio.». No se registra el rubro.

**Postcondiciones:** El nuevo rubro figura en el listado y queda disponible para clasificar comercios.

---

**ID y Nombre:** CUN46 Modificar rubro de comercio  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador modificar el nombre de un rubro de comercio existente.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada y cuenta con autorización para acceder a la gestión de comercios. Existe al menos un rubro registrado.  
**Puntos de Extension:** No aplica.  
**Condicion:** El actor desea corregir o actualizar el nombre de un rubro.  

**Escenario Principal:**
1. El actor selecciona la opción «Comercios» del menú «Operador».
2. El sistema presenta la pantalla «Gestion de Comercios» con la sección «Rubros de comercio» y su listado de rubros.
3. El actor selecciona del listado el rubro a modificar.
4. El sistema muestra el nombre del rubro en el campo «Nombre:» de la sección «Rubros de comercio».
5. El actor modifica el nombre.
6. El actor presiona el botón «Actualizar» de la sección «Rubros de comercio».
7. El sistema guarda el nuevo nombre, actualiza el listado manteniendo seleccionado el rubro, actualiza el selector «Rubro:» y el rubro mostrado en el listado de comercios, y muestra el mensaje «Rubro actualizado con exito.».

**Flujos Alternativos:**
6a. Si el actor presiona «Actualizar» sin haber seleccionado un rubro, el sistema muestra el mensaje «Debe seleccionar un rubro.». No se realizan cambios.
6b. Si el nombre está vacío o solo contiene espacios, el sistema muestra el mensaje «Debe indicar un nombre para el rubro de comercio.». No se realizan cambios y el rubro conserva su nombre anterior.
6c. Si el rubro seleccionado ya no existe, el sistema muestra el mensaje «No se encontro el rubro de comercio indicado.». No se realizan cambios.
6d. Si ya existe otro rubro con ese nombre o si ocurre otro error al guardar, el sistema muestra el mensaje de error «No se pudo actualizar el rubro de comercio.». No se realizan cambios y el rubro conserva su nombre anterior.

**Postcondiciones:** El rubro figura con su nuevo nombre en el listado de rubros y en los comercios que lo tienen asignado.

---

**ID y Nombre:** CUN47 Baja de rubro de comercio  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador eliminar un rubro de comercio que no está en uso.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada y cuenta con autorización para acceder a la gestión de comercios. Existe al menos un rubro registrado. Ningún comercio ni promoción tiene asociado el rubro.  
**Puntos de Extension:** No aplica.  
**Condicion:** El actor desea retirar un rubro que ya no corresponde.  

**Escenario Principal:**
1. El actor selecciona la opción «Comercios» del menú «Operador».
2. El sistema presenta la pantalla «Gestion de Comercios» con la sección «Rubros de comercio» y su listado de rubros.
3. El actor selecciona del listado el rubro a eliminar.
4. El actor presiona el botón «Eliminar» de la sección «Rubros de comercio».
5. El sistema solicita confirmación mediante el mensaje «Se eliminara el rubro seleccionado. Desea continuar?», titulado «Confirmar eliminacion».
6. El actor confirma la eliminación.
7. El sistema elimina el rubro, vacía el campo «Nombre:», actualiza el listado y el selector «Rubro:», y muestra el mensaje «Rubro eliminado con exito.».

**Flujos Alternativos:**
4a. Si el actor presiona «Eliminar» sin haber seleccionado un rubro, el sistema muestra el mensaje «Debe seleccionar un rubro.». No se elimina nada.
6a. Si el actor no confirma la eliminación, el sistema cierra la confirmación y mantiene el rubro en el listado.
7a. Si el rubro tiene comercios o promociones asociados, el sistema muestra el mensaje «No se puede eliminar el rubro porque tiene comercios o promociones asociados.». El rubro se conserva.
7b. Si el rubro ya no existe, el sistema muestra el mensaje «No se encontro el rubro de comercio indicado.». No se elimina nada.
7c. Si ocurre un error al eliminar, el sistema muestra el mensaje de error «No se pudo eliminar el rubro de comercio.». El rubro se conserva.

**Postcondiciones:** El rubro deja de figurar en el listado y en el selector «Rubro:» de comercios.
