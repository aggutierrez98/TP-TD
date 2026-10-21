# Gestión de categorías de movimientos

_Especificaciones de casos de uso_

---

**ID y Nombre:** CUN11 Consultar categorías de movimientos  
**Estado:** Implementado  
**Descripcion:** Permite al actor consultar las categorías de movimientos que tiene disponibles, tanto las globales como las propias.  
**Actor Principal:** Usuario, Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada y cuenta con autorización para acceder a la gestión de categorías.  
**Puntos de Extension:** Desde el listado, el actor puede continuar con CUN12 Alta de categoría de movimientos, CUN13 Modificar categoría de movimientos y CUN14 Baja de categoría de movimientos.  
**Condicion:** El actor desea conocer las categorías con las que puede clasificar sus movimientos.  

**Escenario Principal:**
1. El actor selecciona la opción «Categorías» del menú «Finanzas».
2. El sistema presenta la pantalla «Gestión de categorías» con el listado de categorías disponibles, indicando para cada una su nombre y su alcance («Global» o «Personal»). El listado muestra primero las categorías globales y luego las personales, ordenadas por nombre.
3. El actor selecciona una categoría del listado.
4. El sistema muestra el nombre de la categoría seleccionada en el campo «Nombre:» de la sección «Categoría».

**Flujos Alternativos:**
3a. Si el actor solo necesita consultar el listado, omite la selección de una categoría. El caso de uso finaliza con el listado disponible.

**Postcondiciones:** El actor dispone del listado de categorías disponibles y, si seleccionó una, de su nombre. La consulta no modifica las categorías.

---

**ID y Nombre:** CUN12 Alta de categoría de movimientos  
**Estado:** Implementado  
**Descripcion:** Permite al actor crear una categoría personal para clasificar sus movimientos.  
**Actor Principal:** Usuario, Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada y cuenta con autorización para acceder a la gestión de categorías.  
**Puntos de Extension:** No aplica.  
**Condicion:** El actor necesita una categoría que no figura entre las disponibles.  

**Escenario Principal:**
1. El actor selecciona la opción «Categorías» del menú «Finanzas».
2. El sistema presenta la pantalla «Gestión de categorías» con el listado de categorías disponibles y la sección «Categoría».
3. El actor ingresa el nombre de la nueva categoría en el campo «Nombre:».
4. El actor presiona el botón «Agregar».
5. El sistema crea la categoría con alcance «Personal», limpia el campo «Nombre:» y actualiza el listado, donde la nueva categoría queda seleccionada.
6. El sistema muestra el mensaje «Categoría creada con éxito.».

**Flujos Alternativos:**
4a. Si el nombre está vacío, el sistema muestra el mensaje «Debe indicar un nombre para la categoría.». La categoría no se crea.
5a. Si ya existe una categoría con ese nombre entre las disponibles del actor, el sistema muestra el mensaje «Ya existe una categoría con ese nombre.». La categoría no se crea.
5b. Si ocurre un error al crear la categoría, el sistema muestra el mensaje «No se pudo crear la categoría.». La categoría no se crea.

**Postcondiciones:** La nueva categoría figura en el listado con alcance «Personal» y está disponible para el actor al registrar movimientos. Si el caso de uso no se completa, el listado no cambia.

---

**ID y Nombre:** CUN13 Modificar categoría de movimientos  
**Estado:** Implementado  
**Descripcion:** Permite al actor cambiar el nombre de una categoría personal.  
**Actor Principal:** Usuario, Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada y cuenta con autorización para acceder a la gestión de categorías. El actor tiene al menos una categoría personal.  
**Puntos de Extension:** No aplica.  
**Condicion:** El actor necesita corregir o cambiar el nombre de una de sus categorías.  

**Escenario Principal:**
1. El actor selecciona la opción «Categorías» del menú «Finanzas».
2. El sistema presenta la pantalla «Gestión de categorías» con el listado de categorías disponibles.
3. El actor selecciona una categoría personal del listado.
4. El sistema muestra el nombre de la categoría seleccionada en el campo «Nombre:» de la sección «Categoría».
5. El actor modifica el nombre.
6. El actor presiona el botón «Actualizar».
7. El sistema aplica el cambio y actualiza el listado, donde la categoría queda seleccionada con su nuevo nombre.
8. El sistema muestra el mensaje «Categoría actualizada con éxito.».

**Flujos Alternativos:**
6a. Si no hay una categoría seleccionada, el sistema muestra el mensaje «Debe seleccionar una categoría.». No se realizan cambios.
6b. Si la categoría seleccionada es global, el sistema muestra el mensaje «Las categorías globales no se pueden editar.». No se realizan cambios.
7a. Si el nombre está vacío, el sistema muestra el mensaje «Debe indicar un nombre para la categoría.». No se realizan cambios.
7b. Si ya existe otra categoría con ese nombre entre las disponibles del actor, el sistema muestra el mensaje «Ya existe una categoría con ese nombre.». No se realizan cambios.
7c. Si ocurre un error al actualizar la categoría, el sistema muestra el mensaje «No se pudo actualizar la categoría.». La categoría conserva su nombre anterior.

**Postcondiciones:** La categoría figura en el listado con el nombre modificado. Si el caso de uso no se completa, la categoría conserva su nombre anterior.

---

**ID y Nombre:** CUN14 Baja de categoría de movimientos  
**Estado:** Implementado  
**Descripcion:** Permite al actor eliminar una categoría personal que ya no necesita.  
**Actor Principal:** Usuario, Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada y cuenta con autorización para acceder a la gestión de categorías. El actor tiene al menos una categoría personal.  
**Puntos de Extension:** No aplica.  
**Condicion:** El actor desea quitar una de sus categorías.  

**Escenario Principal:**
1. El actor selecciona la opción «Categorías» del menú «Finanzas».
2. El sistema presenta la pantalla «Gestión de categorías» con el listado de categorías disponibles.
3. El actor selecciona la categoría personal a eliminar.
4. El sistema muestra el nombre de la categoría seleccionada en el campo «Nombre:».
5. El actor presiona el botón «Eliminar».
6. El sistema solicita confirmación con el título «Confirmar eliminación» y el mensaje «Se eliminará la categoría seleccionada. ¿Desea continuar?».
7. El actor confirma la eliminación.
8. El sistema elimina la categoría, limpia el campo «Nombre:» y actualiza el listado, que ya no incluye la categoría eliminada.
9. El sistema muestra el mensaje «Categoría eliminada con éxito.».

**Flujos Alternativos:**
5a. Si no hay una categoría seleccionada, el sistema muestra el mensaje «Debe seleccionar una categoría.». No se realizan cambios.
5b. Si la categoría seleccionada es global, el sistema muestra el mensaje «Las categorías globales no se pueden editar.». No se realizan cambios.
7a. Si el actor no confirma la eliminación, el caso de uso finaliza sin cambios.
8a. Si la categoría no puede eliminarse, por ejemplo porque ya está asociada a movimientos o a reglas de categorización, el sistema muestra el mensaje «No se pudo eliminar la categoría.». La categoría permanece en el listado.

**Postcondiciones:** La categoría eliminada no figura en el listado ni está disponible para el actor. Si el caso de uso no se completa, la categoría permanece sin cambios.
