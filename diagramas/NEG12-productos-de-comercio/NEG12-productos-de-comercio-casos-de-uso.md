# Productos de comercio

_Especificaciones de casos de uso_

---

**ID y Nombre:** CUN52 Consultar productos y precios de un comercio  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador consultar los productos que vende un comercio, con su precio y moneda, así como las promociones vigentes asociadas a ese comercio.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada, cuenta con autorización para acceder a la gestión de comercios, la pantalla «Gestión de Comercios» está abierta y existe al menos un comercio registrado.  
**Puntos de Extension:** Se origina como extensión de CUN48 Consultar comercios mediante la acción «Ver detalle». Permite continuar con CUN53, CUN54 y CUN55 desde la misma ventana.  
**Condicion:** El actor desea conocer qué productos vende un comercio y a qué precio.  

**Escenario Principal:**
1. El actor selecciona un comercio del listado de la pantalla «Gestión de Comercios».
2. El actor selecciona la acción «Ver detalle».
3. El sistema abre la ventana modal «Detalle del comercio», que muestra el nombre del comercio junto a «Comercio:», la sección «Productos» con el listado de productos del comercio ordenado por nombre, indicando para cada uno su nombre, categoría, precio y moneda, y la sección «Promociones del comercio» con las promociones del comercio, indicando descripción, descuento, tope, días y vigencia desde y hasta (el tope se muestra como «-» cuando no tiene, y los días como «Todos los días» cuando la promoción aplica a toda la semana).
4. El actor selecciona un producto del listado.
5. El sistema carga en los campos «Producto», «Moneda» y «Precio» los datos del producto seleccionado y deshabilita los campos «Producto» y «Moneda», dejando editable solo el precio.

**Flujos Alternativos:**
2a. Si el actor no seleccionó ningún comercio, el sistema muestra la advertencia «Debe seleccionar un comercio.». El caso de uso finaliza sin abrir la ventana.
3a. Si el comercio no tiene productos o promociones, el sistema presenta el listado correspondiente vacío.
3b. Si ocurre un error al cargar los productos, las monedas, el catálogo o las promociones, el sistema muestra un mensaje de error y la sección afectada queda sin datos.
4a. Si el actor solo necesita consultar los listados, omite la selección de un producto. El caso de uso finaliza con la información disponible.

**Postcondiciones:** El actor dispone de los productos con su precio y moneda y de las promociones del comercio. La consulta no modifica ningún dato.

---

**ID y Nombre:** CUN53 Asociar producto a comercio  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador asignar a un comercio un producto del catálogo, indicando su precio y moneda en ese comercio.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** La ventana «Detalle del comercio» está abierta. Existe al menos un producto en el catálogo y al menos una moneda disponible. No hay un producto del listado seleccionado para edición (en ese caso los campos «Producto» y «Moneda» permanecen deshabilitados).  
**Puntos de Extension:** Se origina desde CUN52 Consultar productos y precios de un comercio.  
**Condicion:** El actor desea que un comercio venda un producto del catálogo.  

**Escenario Principal:**
1. El actor selecciona un producto en el campo «Producto», que presenta los productos del catálogo ordenados por nombre.
2. El actor selecciona una moneda en el campo «Moneda».
3. El actor ingresa el precio en el campo «Precio».
4. El actor selecciona la acción «Agregar».
5. El sistema asigna el producto al comercio, limpia el formulario, actualiza el listado seleccionando el producto asignado y muestra el mensaje «Producto asignado al comercio con éxito.».

**Flujos Alternativos:**
4a. Si el actor no seleccionó un producto, el sistema muestra la advertencia «Debe seleccionar un producto del catálogo.». No se realiza la asignación.
4b. Si el actor no seleccionó una moneda, el sistema muestra la advertencia «Debe seleccionar una moneda.». No se realiza la asignación.
4c. Si el precio está vacío, no es numérico o no es mayor a cero, el sistema muestra la advertencia «Debe indicar un precio válido mayor a cero.». No se realiza la asignación.
5a. Si el producto ya está asignado al comercio, el sistema muestra la advertencia «El producto ya está asignado a este comercio.». No se realiza la asignación.
5b. Si el producto, la moneda o el comercio ya no existen, el sistema muestra la advertencia correspondiente («No se encontró el producto indicado.», «No se encontró la moneda indicada.» o «No se encontró el comercio indicado.»). No se realiza la asignación.
5c. Si ocurre un error inesperado al asignar el producto, el sistema muestra un mensaje de error. No se realiza la asignación.

**Postcondiciones:** El producto queda asociado al comercio con el precio y la moneda indicados y figura en el listado de productos del comercio.

---

**ID y Nombre:** CUN54 Modificar precio de producto en comercio  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador modificar el precio de un producto ya asignado a un comercio.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** La ventana «Detalle del comercio» está abierta y el comercio tiene al menos un producto asignado.  
**Puntos de Extension:** Se origina desde CUN52 Consultar productos y precios de un comercio. Solo el precio es modificable: para cambiar la moneda, el producto debe quitarse (CUN55) y volver a asociarse (CUN53).  
**Condicion:** El actor desea actualizar el precio al que un comercio vende un producto.  

**Escenario Principal:**
1. El actor selecciona un producto del listado de productos del comercio.
2. El sistema carga el producto, la moneda y el precio en el formulario, con los campos «Producto» y «Moneda» deshabilitados.
3. El actor modifica el valor del campo «Precio».
4. El actor selecciona la acción «Actualizar».
5. El sistema guarda el nuevo precio, actualiza el listado manteniendo seleccionado el producto y muestra el mensaje «Producto actualizado con éxito.».

**Flujos Alternativos:**
4a. Si el actor no seleccionó ningún producto del listado, el sistema muestra la advertencia «Debe seleccionar un producto.». No se modifica ningún precio.
4b. Si el precio está vacío, no es numérico o no es mayor a cero, el sistema muestra la advertencia «Debe indicar un precio válido mayor a cero.». El precio no se modifica.
5a. Si el producto ya no está asignado al comercio, el sistema muestra la advertencia «El producto no está asignado a este comercio.». El precio no se modifica.
5b. Si ocurre un error inesperado al guardar el precio, el sistema muestra un mensaje de error. El precio no se modifica.

**Postcondiciones:** El producto queda con el nuevo precio en el comercio y el listado lo refleja.

---

**ID y Nombre:** CUN55 Quitar producto de comercio  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador quitar un producto de la oferta de un comercio, sin eliminarlo del catálogo.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** La ventana «Detalle del comercio» está abierta y el comercio tiene al menos un producto asignado.  
**Puntos de Extension:** Se origina desde CUN52 Consultar productos y precios de un comercio.  
**Condicion:** El actor desea que un comercio deje de vender un producto.  

**Escenario Principal:**
1. El actor selecciona un producto del listado de productos del comercio.
2. El actor selecciona la acción «Quitar».
3. El sistema solicita confirmación mediante el diálogo «Confirmar baja del producto» con el mensaje «Se quitará el producto '<nombre>' de este comercio. El producto seguirá existiendo en el catálogo. ¿Desea continuar?».
4. El actor confirma la acción.
5. El sistema quita el producto del comercio, limpia el formulario, actualiza el listado sin ese producto y muestra el mensaje «Producto quitado del comercio con éxito.».

**Flujos Alternativos:**
2a. Si el actor no seleccionó ningún producto, el sistema muestra la advertencia «Debe seleccionar un producto.». El caso de uso finaliza sin cambios.
4a. Si el actor no confirma la acción, el sistema cierra el diálogo y el producto se mantiene asignado al comercio.
5a. Si el producto ya no está asignado al comercio, el sistema muestra la advertencia «El producto no está asignado a este comercio.». No se realiza ningún cambio.
5b. Si ocurre un error inesperado al quitar el producto, el sistema muestra un mensaje de error. El producto se mantiene asignado.

**Postcondiciones:** El producto deja de figurar entre los productos del comercio y sigue disponible en el catálogo de productos.
