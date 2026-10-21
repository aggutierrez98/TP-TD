# Promociones

_Especificaciones de casos de uso_

---

**ID y Nombre:** CUN64 Consultar promociones  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador consultar las promociones registradas, buscarlas por texto o por comercio y visualizar el detalle de alcance de una promoción seleccionada.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada y cuenta con autorización para acceder a la gestión de promociones.  
**Puntos de Extension:** No aplica.  
**Condicion:** El actor desea conocer las promociones registradas en el sistema.  

**Escenario Principal:**
1. El actor selecciona la opción «Promociones» del menú «Operador».
2. El sistema presenta la pantalla «Gestión de Promociones» con el listado de promociones, indicando para cada una su descripción, descuento (en %), tope, días, vigencia desde y vigencia hasta (el tope se muestra como «-» cuando no tiene y los días como «Todos los días» cuando la promoción aplica a toda la semana), y con los filtros de búsqueda «Texto» y «Comercio» (con el valor «Todos» seleccionado por defecto).
3. El actor ingresa un texto en el campo «Texto», selecciona un comercio en el campo «Comercio», o ambos, y selecciona la acción «Buscar».
4. El sistema actualiza el listado mostrando solo las promociones cuya descripción contiene el texto indicado y que aplican al comercio seleccionado.
5. El actor selecciona una promoción del listado.
6. El sistema carga la promoción en el editor: descripción, descuento, tope, vigencia desde y hasta, días de la semana marcados y las opciones marcadas en cada lista de alcance (comercios, rubros de comercio, entidades financieras, medios de pago, segmentos, productos incluidos y excluidos, categorías incluidas y excluidas).

**Flujos Alternativos:**
2a. Si no hay promociones registradas, el sistema presenta el listado vacío. El caso de uso finaliza sin selección de promoción.
2b. Si ocurre un error al cargar el listado o los catálogos de las listas de alcance, el sistema muestra un mensaje de error. El caso de uso finaliza sin completar la consulta.
2c. Si una lista de alcance no tiene opciones cargadas, el sistema muestra dentro de su recuadro un aviso indicando dónde se dan de alta (por ejemplo, «No hay comercios cargados. Se dan de alta en Operador > Comercios.»).
3a. Si el actor solo necesita consultar el listado completo, omite la búsqueda y continúa en el paso 5.
4a. Si ninguna promoción coincide con los filtros, el sistema presenta el listado vacío.
5a. Si el actor no selecciona ninguna promoción, el caso de uso finaliza con el listado disponible.

**Postcondiciones:** El actor dispone del listado de promociones, filtrado si aplicó una búsqueda, y del detalle de la promoción seleccionada. La consulta no modifica las promociones.

---

**ID y Nombre:** CUN65 Alta de promoción  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador registrar una nueva promoción indicando su descuento, vigencia, días de aplicación y el alcance (comercios, rubros, entidades financieras, medios de pago, segmentos, productos y categorías).  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada, cuenta con autorización para acceder a la gestión de promociones y la pantalla «Gestión de Promociones» está abierta. Los filtros de alcance son opcionales: una lista sin opciones marcadas no restringe la promoción (aplica a todos), y en las listas de excluidos la exclusión prevalece.  
**Puntos de Extension:** No aplica.  
**Condicion:** El actor desea incorporar una nueva promoción al sistema.  

**Escenario Principal:**
1. El actor ingresa la descripción de la promoción en el campo «Descripción».
2. El actor ingresa el porcentaje en el campo «Descuento (%)».
3. El actor ingresa, de forma opcional, el monto máximo de descuento en el campo «Tope ($)».
4. El actor indica las fechas «Vigencia desde» y «Vigencia hasta».
5. El actor marca, de forma opcional, los días de la semana en que aplica la promoción (si no marca ninguno, la promoción aplica todos los días).
6. El actor marca, de forma opcional, las opciones de alcance en las listas «Comercios», «Rubros de comercio», «Entidades financieras», «Medios de pago», «Segmentos», «Productos incluidos», «Productos excluidos», «Categorías incluidas» y «Categorías excluidas».
7. El actor selecciona la acción «Agregar».
8. El sistema registra la promoción, limpia el editor, actualiza el listado seleccionando la promoción recién creada y muestra el mensaje «Promoción creada con éxito.».

**Flujos Alternativos:**
6a. Si el actor marca un producto incluido que pertenece a una categoría marcada como excluida, el sistema muestra en el editor la advertencia «Advertencia: hay un producto incluido que pertenece a una categoría excluida; ese producto quedará fuera.». La advertencia no impide continuar.
7a. Si el descuento no es un número válido, el sistema muestra la advertencia «Ingrese un descuento numérico válido.». La promoción no se registra.
7b. Si el tope se ingresó pero no es un número válido, el sistema muestra la advertencia «Ingrese un tope numérico válido o déjelo vacío.». La promoción no se registra.
8a. Si la descripción está vacía, el sistema muestra la advertencia «La descripción de la promoción es obligatoria.». La promoción no se registra.
8b. Si el descuento no es mayor que 0 y menor o igual que 100, el sistema muestra la advertencia «El descuento debe ser mayor que 0 y menor o igual que 100.». La promoción no se registra.
8c. Si el tope no es mayor que 0, el sistema muestra la advertencia «El tope debe ser mayor que 0.». La promoción no se registra.
8d. Si la vigencia desde es posterior a la vigencia hasta, el sistema muestra la advertencia «La vigencia desde no puede ser posterior a la vigencia hasta.». La promoción no se registra.
8e. Si un producto está marcado a la vez como incluido y excluido, o una categoría a la vez como incluida y excluida, el sistema muestra la advertencia «Un producto no puede estar incluido y excluido a la vez.» o «Una categoría no puede estar incluida y excluida a la vez.». La promoción no se registra.
8f. Si alguno de los elementos de alcance seleccionados ya no existe, el sistema muestra la advertencia correspondiente (por ejemplo, «El comercio seleccionado no existe.»). La promoción no se registra.
8g. Si ocurre un error inesperado al registrar la promoción, el sistema muestra un mensaje de error. La promoción no se registra.

**Postcondiciones:** La promoción queda registrada con su alcance y vigencia, y figura en el listado de promociones.

---

**ID y Nombre:** CUN66 Modificar promoción  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador modificar los datos, la vigencia y el alcance de una promoción existente.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada, cuenta con autorización para acceder a la gestión de promociones y la pantalla «Gestión de Promociones» está abierta. Existe al menos una promoción registrada.  
**Puntos de Extension:** No aplica.  
**Condicion:** El actor desea corregir o actualizar una promoción.  

**Escenario Principal:**
1. El actor selecciona una promoción del listado.
2. El sistema carga la promoción en el editor, con sus datos, días y opciones de alcance marcadas.
3. El actor modifica los datos que desea cambiar.
4. El actor selecciona la acción «Actualizar».
5. El sistema guarda los cambios, limpia el editor, actualiza el listado manteniendo seleccionada la promoción y muestra el mensaje «Promoción actualizada con éxito.».

**Flujos Alternativos:**
3a. Si el actor marca un producto incluido que pertenece a una categoría marcada como excluida, el sistema muestra en el editor la advertencia «Advertencia: hay un producto incluido que pertenece a una categoría excluida; ese producto quedará fuera.». La advertencia no impide continuar.
4a. Si el actor no seleccionó ninguna promoción, el sistema muestra la advertencia «Debe seleccionar una promoción.». No se modifica ningún dato.
4b. Si el descuento no es un número válido, el sistema muestra la advertencia «Ingrese un descuento numérico válido.». La promoción no se modifica.
4c. Si el tope se ingresó pero no es un número válido, el sistema muestra la advertencia «Ingrese un tope numérico válido o déjelo vacío.». La promoción no se modifica.
5a. Si la descripción está vacía, el sistema muestra la advertencia «La descripción de la promoción es obligatoria.». La promoción no se modifica.
5b. Si el descuento no es mayor que 0 y menor o igual que 100, el sistema muestra la advertencia «El descuento debe ser mayor que 0 y menor o igual que 100.». La promoción no se modifica.
5c. Si el tope no es mayor que 0, el sistema muestra la advertencia «El tope debe ser mayor que 0.». La promoción no se modifica.
5d. Si la vigencia desde es posterior a la vigencia hasta, el sistema muestra la advertencia «La vigencia desde no puede ser posterior a la vigencia hasta.». La promoción no se modifica.
5e. Si un producto está marcado a la vez como incluido y excluido, o una categoría a la vez como incluida y excluida, el sistema muestra la advertencia «Un producto no puede estar incluido y excluido a la vez.» o «Una categoría no puede estar incluida y excluida a la vez.». La promoción no se modifica.
5f. Si la promoción ya no existe, el sistema muestra la advertencia «La promoción no existe.». No se realiza ningún cambio.
5g. Si ocurre un error inesperado al guardar los cambios, el sistema muestra un mensaje de error. La promoción no se modifica.

**Postcondiciones:** La promoción queda con los datos, la vigencia y el alcance modificados, y el listado los refleja.

---

**ID y Nombre:** CUN67 Baja de promoción  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador eliminar de forma definitiva una promoción que no está referenciada por ninguna notificación.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada, cuenta con autorización para acceder a la gestión de promociones y la pantalla «Gestión de Promociones» está abierta. Existe al menos una promoción registrada. Para poder eliminarla, ninguna notificación debe hacer referencia a ella.  
**Puntos de Extension:** No aplica.  
**Condicion:** El actor desea eliminar una promoción que ya no corresponde mantener.  

**Escenario Principal:**
1. El actor selecciona una promoción del listado.
2. El actor selecciona la acción «Eliminar».
3. El sistema solicita confirmación mediante el diálogo «Confirmar eliminación» con el mensaje «Se eliminará la promoción '<descripción>' en forma definitiva. ¿Desea continuar?».
4. El actor confirma la eliminación.
5. El sistema elimina la promoción, limpia el editor, actualiza el listado sin la promoción eliminada y muestra el mensaje «Promoción eliminada con éxito.».

**Flujos Alternativos:**
2a. Si el actor no seleccionó ninguna promoción, el sistema muestra la advertencia «Debe seleccionar una promoción.». El caso de uso finaliza sin cambios.
4a. Si el actor no confirma la eliminación, el sistema cierra el diálogo y la promoción se mantiene sin cambios.
5a. Si alguna notificación hace referencia a la promoción, el sistema muestra la advertencia «No se puede eliminar la promoción porque una notificación la referencia.». La promoción no se elimina.
5b. Si la promoción ya no existe, el sistema muestra la advertencia «La promoción no existe.». No se realiza ningún cambio.
5c. Si ocurre un error inesperado al eliminar la promoción, el sistema muestra un mensaje de error. La promoción no se elimina.

**Postcondiciones:** La promoción deja de figurar en el listado y ya no está disponible en el sistema.
