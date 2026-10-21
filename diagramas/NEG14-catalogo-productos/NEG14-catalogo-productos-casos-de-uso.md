# Catálogo de productos

_Especificaciones de casos de uso_

---

**ID y Nombre:** CUN60 Consultar catálogo de productos  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador consultar los productos del catálogo con su categoría.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada y cuenta con autorización para acceder a la gestión de productos.  
**Puntos de Extension:** No aplica.  
**Condicion:** El actor desea conocer los productos registrados en el catálogo.  

**Escenario Principal:**
1. El actor selecciona la opción «Productos» del menú «Operador».
2. El sistema presenta la pantalla «Gestión de Productos» con el listado de productos, indicando para cada uno su nombre y su categoría.
3. El actor selecciona un producto del listado.
4. El sistema muestra el nombre y la categoría del producto seleccionado en los campos «Nombre» y «Categoría».

**Flujos Alternativos:**
2a. Si no hay productos registrados, el sistema presenta el listado vacío. El caso de uso finaliza sin selección de producto.
2b. Si ocurre un error al cargar el listado o las categorías, el sistema muestra un mensaje de error. El caso de uso finaliza sin completar la consulta.
3a. Si el actor solo necesita consultar el listado, omite la selección de un producto. El caso de uso finaliza con el listado disponible.

**Postcondiciones:** El actor dispone del listado de productos del catálogo. La consulta no modifica los productos.

---

**ID y Nombre:** CUN61 Alta de producto  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador registrar un nuevo producto en el catálogo, asignándole una categoría.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada, cuenta con autorización para acceder a la gestión de productos y la pantalla «Gestión de Productos» está abierta. Existe al menos una categoría de producto registrada; de lo contrario, la acción «Agregar» permanece deshabilitada y el sistema muestra el aviso «No hay categorías de producto. Créalas en Operador > Categorías de producto.».  
**Puntos de Extension:** No aplica.  
**Condicion:** El actor desea incorporar un nuevo producto al catálogo.  

**Escenario Principal:**
1. El actor ingresa el nombre del producto en el campo «Nombre».
2. El actor selecciona una categoría en el campo «Categoría».
3. El actor selecciona la acción «Agregar».
4. El sistema registra el producto, limpia el formulario, actualiza el listado seleccionando el producto recién creado y muestra el mensaje «Producto creado con éxito.».

**Flujos Alternativos:**
3a. Si el actor no seleccionó una categoría, el sistema muestra la advertencia «Debe seleccionar una categoría.». El producto no se registra.
4a. Si el nombre está vacío o contiene solo espacios, el sistema muestra la advertencia «Debe indicar un nombre para el producto.». El producto no se registra.
4b. Si la categoría seleccionada ya no existe, el sistema muestra la advertencia «No se encontró la categoría de producto indicada.». El producto no se registra.
4c. Si ocurre un error inesperado al registrar el producto, el sistema muestra un mensaje de error. El producto no se registra.

**Postcondiciones:** El producto queda registrado en el catálogo con su categoría, figura en el listado y está disponible para asociarse a comercios y para promociones.

---

**ID y Nombre:** CUN62 Modificar producto  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador modificar el nombre y la categoría de un producto del catálogo.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada, cuenta con autorización para acceder a la gestión de productos y la pantalla «Gestión de Productos» está abierta. Existe al menos un producto registrado.  
**Puntos de Extension:** No aplica.  
**Condicion:** El actor desea corregir o actualizar los datos de un producto.  

**Escenario Principal:**
1. El actor selecciona un producto del listado.
2. El sistema carga el nombre y la categoría del producto en el formulario.
3. El actor modifica el nombre, la categoría o ambos.
4. El actor selecciona la acción «Actualizar».
5. El sistema guarda los cambios, limpia el formulario, actualiza el listado manteniendo seleccionado el producto y muestra el mensaje «Producto actualizado con éxito.».

**Flujos Alternativos:**
4a. Si el actor no seleccionó ningún producto, el sistema muestra la advertencia «Debe seleccionar un producto.». No se modifica ningún dato.
4b. Si el actor no seleccionó una categoría, el sistema muestra la advertencia «Debe seleccionar una categoría.». El producto no se modifica.
5a. Si el nombre está vacío o contiene solo espacios, el sistema muestra la advertencia «Debe indicar un nombre para el producto.». El producto no se modifica.
5b. Si el producto o la categoría ya no existen, el sistema muestra la advertencia correspondiente («No se encontró el producto indicado.» o «No se encontró la categoría de producto indicada.»). El producto no se modifica.
5c. Si ocurre un error inesperado al guardar los cambios, el sistema muestra un mensaje de error. El producto no se modifica.

**Postcondiciones:** El producto queda con el nombre y la categoría modificados y el listado los refleja.

---

**ID y Nombre:** CUN63 Baja de producto  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador eliminar de forma definitiva un producto del catálogo que no está en uso.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada, cuenta con autorización para acceder a la gestión de productos y la pantalla «Gestión de Productos» está abierta. Existe al menos un producto registrado. Para poder eliminarlo, ningún comercio debe venderlo y ninguna promoción debe incluirlo o excluirlo.  
**Puntos de Extension:** No aplica.  
**Condicion:** El actor desea eliminar un producto que ya no corresponde mantener en el catálogo.  

**Escenario Principal:**
1. El actor selecciona un producto del listado.
2. El actor selecciona la acción «Eliminar».
3. El sistema solicita confirmación mediante el diálogo «Confirmar eliminación» con el mensaje «Se eliminará el producto '<nombre>' en forma definitiva. ¿Desea continuar?».
4. El actor confirma la eliminación.
5. El sistema elimina el producto, limpia el formulario, actualiza el listado sin el producto eliminado y muestra el mensaje «Producto eliminado con éxito.».

**Flujos Alternativos:**
2a. Si el actor no seleccionó ningún producto, el sistema muestra la advertencia «Debe seleccionar un producto.». El caso de uso finaliza sin cambios.
4a. Si el actor no confirma la eliminación, el sistema cierra el diálogo y el producto se mantiene sin cambios.
5a. Si algún comercio vende el producto o alguna promoción lo incluye o excluye, el sistema muestra la advertencia «No se puede eliminar el producto porque algún comercio lo vende o alguna promoción lo incluye o excluye. Quítelo primero de esos usos.». El producto no se elimina.
5b. Si el producto ya no existe, el sistema muestra la advertencia «No se encontró el producto indicado.». No se realiza ningún cambio.
5c. Si ocurre un error inesperado al eliminar el producto, el sistema muestra un mensaje de error. El producto no se elimina.

**Postcondiciones:** El producto deja de figurar en el catálogo y ya no está disponible en el sistema.
