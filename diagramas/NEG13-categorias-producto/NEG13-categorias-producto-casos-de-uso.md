# Categorías de producto

_Especificaciones de casos de uso_

---

**ID y Nombre:** CUN56 Consultar categorías de producto  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador consultar las categorías de producto registradas.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada y cuenta con autorización para acceder a la gestión de productos.  
**Puntos de Extension:** No aplica.  
**Condicion:** El actor desea conocer las categorías de producto disponibles en el sistema.  

**Escenario Principal:**
1. El actor selecciona la opción «Categorías de producto» del menú «Operador».
2. El sistema presenta la pantalla «Gestión de Categorías de producto» con el listado de categorías ordenado por nombre.
3. El actor selecciona una categoría del listado.
4. El sistema muestra el nombre de la categoría seleccionada en el campo «Nombre».

**Flujos Alternativos:**
2a. Si no hay categorías registradas, el sistema presenta el listado vacío. El caso de uso finaliza sin selección de categoría.
2b. Si ocurre un error al cargar el listado, el sistema muestra un mensaje de error. El caso de uso finaliza sin completar la consulta.
3a. Si el actor solo necesita consultar el listado, omite la selección de una categoría. El caso de uso finaliza con el listado disponible.

**Postcondiciones:** El actor dispone del listado de categorías de producto. La consulta no modifica las categorías.

---

**ID y Nombre:** CUN57 Alta de categoría de producto  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador registrar una nueva categoría de producto.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada, cuenta con autorización para acceder a la gestión de productos y la pantalla «Gestión de Categorías de producto» está abierta.  
**Puntos de Extension:** No aplica.  
**Condicion:** El actor desea incorporar una nueva categoría para clasificar productos.  

**Escenario Principal:**
1. El actor ingresa el nombre de la categoría en el campo «Nombre».
2. El actor selecciona la acción «Agregar».
3. El sistema registra la categoría, limpia el campo «Nombre», actualiza el listado seleccionando la categoría recién creada y muestra el mensaje «Categoría creada con éxito.».

**Flujos Alternativos:**
3a. Si el nombre está vacío o contiene solo espacios, el sistema muestra la advertencia «Debe indicar un nombre para la categoría de producto.». La categoría no se registra.
3b. Si ya existe una categoría con ese nombre (sin distinguir mayúsculas de minúsculas), el sistema muestra la advertencia «Ya existe una categoría de producto con ese nombre.». La categoría no se registra.
3c. Si ocurre un error inesperado al registrar la categoría, el sistema muestra un mensaje de error. La categoría no se registra.

**Postcondiciones:** La categoría queda registrada, figura en el listado y está disponible para clasificar productos.

---

**ID y Nombre:** CUN58 Modificar categoría de producto  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador cambiar el nombre de una categoría de producto existente.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada, cuenta con autorización para acceder a la gestión de productos y la pantalla «Gestión de Categorías de producto» está abierta. Existe al menos una categoría registrada.  
**Puntos de Extension:** No aplica.  
**Condicion:** El actor desea corregir o actualizar el nombre de una categoría.  

**Escenario Principal:**
1. El actor selecciona una categoría del listado.
2. El sistema carga el nombre de la categoría en el campo «Nombre».
3. El actor modifica el nombre.
4. El actor selecciona la acción «Actualizar».
5. El sistema guarda el cambio, actualiza el listado manteniendo seleccionada la categoría y muestra el mensaje «Categoría actualizada con éxito.».

**Flujos Alternativos:**
4a. Si el actor no seleccionó ninguna categoría, el sistema muestra la advertencia «Debe seleccionar una categoría.». No se modifica ningún dato.
5a. Si el nombre está vacío o contiene solo espacios, el sistema muestra la advertencia «Debe indicar un nombre para la categoría de producto.». La categoría no se modifica.
5b. Si ya existe otra categoría con ese nombre (sin distinguir mayúsculas de minúsculas), el sistema muestra la advertencia «Ya existe una categoría de producto con ese nombre.». La categoría no se modifica.
5c. Si la categoría ya no existe, el sistema muestra la advertencia «No se encontró la categoría de producto indicada.». No se realiza ningún cambio.
5d. Si ocurre un error inesperado al guardar el cambio, el sistema muestra un mensaje de error. La categoría no se modifica.

**Postcondiciones:** La categoría queda con el nuevo nombre y el listado lo refleja.

---

**ID y Nombre:** CUN59 Baja de categoría de producto  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador eliminar una categoría de producto que no está en uso.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada, cuenta con autorización para acceder a la gestión de productos y la pantalla «Gestión de Categorías de producto» está abierta. Existe al menos una categoría registrada. Para poder eliminarla, la categoría no debe tener productos ni promociones asociados.  
**Puntos de Extension:** No aplica.  
**Condicion:** El actor desea eliminar una categoría que ya no corresponde mantener.  

**Escenario Principal:**
1. El actor selecciona una categoría del listado.
2. El actor selecciona la acción «Eliminar».
3. El sistema solicita confirmación mediante el diálogo «Confirmar eliminación» con el mensaje «Se eliminará la categoría seleccionada. ¿Desea continuar?».
4. El actor confirma la eliminación.
5. El sistema elimina la categoría, limpia el campo «Nombre», actualiza el listado sin la categoría eliminada y muestra el mensaje «Categoría eliminada con éxito.».

**Flujos Alternativos:**
2a. Si el actor no seleccionó ninguna categoría, el sistema muestra la advertencia «Debe seleccionar una categoría.». El caso de uso finaliza sin cambios.
4a. Si el actor no confirma la eliminación, el sistema cierra el diálogo y la categoría se mantiene sin cambios.
5a. Si la categoría tiene productos o promociones asociados, el sistema muestra la advertencia «No se puede eliminar la categoría porque tiene productos o promociones asociados.». La categoría no se elimina.
5b. Si la categoría ya no existe, el sistema muestra la advertencia «No se encontró la categoría de producto indicada.». No se realiza ningún cambio.
5c. Si ocurre un error inesperado al eliminar la categoría, el sistema muestra un mensaje de error. La categoría no se elimina.

**Postcondiciones:** La categoría deja de figurar en el listado y ya no está disponible para clasificar productos.
