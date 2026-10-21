# Monedas

_Especificaciones de casos de uso_

---

**ID y Nombre:** CUN33 Consultar monedas  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador consultar las monedas registradas en el sistema.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada y cuenta con autorización para acceder a la gestión de monedas.  
**Puntos de Extension:** No aplica.  
**Condicion:** El actor desea conocer las monedas registradas en el sistema.  

**Escenario Principal:**
1. El actor selecciona la opción «Monedas» del menú «Operador».
2. El sistema presenta la pantalla «Gestion de Monedas» con la sección «Monedas», que contiene el listado de monedas ordenado por código, indicando para cada una su «Codigo» y su «Nombre».
3. El actor selecciona una moneda del listado.
4. El sistema muestra el código y el nombre de la moneda seleccionada en los campos «Codigo:» y «Nombre:».

**Flujos Alternativos:**
2a. Si no hay monedas registradas, el sistema presenta el listado vacío. El caso de uso finaliza sin selección.
2b. Si ocurre un error al cargar el listado, el sistema muestra un mensaje de error. El caso de uso finaliza sin completar la consulta.
3a. Si el actor solo necesita consultar el listado, omite la selección de una moneda. El caso de uso finaliza con el listado disponible.

**Postcondiciones:** El actor dispone del listado de monedas y, si seleccionó una, de su código y nombre. La consulta no modifica las monedas.

---

**ID y Nombre:** CUN34 Alta de moneda  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador registrar una nueva moneda indicando su código y su nombre.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada y cuenta con autorización para acceder a la gestión de monedas. No existe otra moneda con el mismo código.  
**Puntos de Extension:** No aplica.  
**Condicion:** El actor desea incorporar una moneda que aún no figura en el sistema.  

**Escenario Principal:**
1. El actor selecciona la opción «Monedas» del menú «Operador».
2. El sistema presenta la pantalla «Gestion de Monedas» con el listado de monedas registradas en la sección «Monedas».
3. El actor ingresa el código de la moneda en el campo «Codigo:» (hasta 10 caracteres).
4. El actor ingresa el nombre de la moneda en el campo «Nombre:» (hasta 100 caracteres).
5. El actor presiona el botón «Agregar» de la sección «Monedas».
6. El sistema registra la moneda, vacía los campos, actualiza el listado con la nueva moneda seleccionada, actualiza las listas de monedas de la sección «Tasas de Conversion» y muestra el mensaje «Moneda creada con exito.».

**Flujos Alternativos:**
5a. Si el código está vacío, el sistema muestra el mensaje «El código de la moneda es obligatorio.». No se registra la moneda.
5b. Si el código supera los 10 caracteres, el sistema muestra el mensaje «El código de la moneda no puede superar los 10 caracteres.». No se registra la moneda.
5c. Si el nombre está vacío, el sistema muestra el mensaje «El nombre de la moneda es obligatorio.». No se registra la moneda.
5d. Si el nombre supera los 100 caracteres, el sistema muestra el mensaje «El nombre de la moneda no puede superar los 100 caracteres.». No se registra la moneda.
5e. Si ya existe una moneda con ese código o si ocurre otro error al registrarla, el sistema muestra el mensaje de error «No se pudo crear la moneda.». No se registra la moneda.

**Postcondiciones:** La nueva moneda figura en el listado y queda disponible para definir tasas de conversión.

---

**ID y Nombre:** CUN35 Modificar moneda  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador modificar el código y el nombre de una moneda existente.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada y cuenta con autorización para acceder a la gestión de monedas. Existe al menos una moneda registrada.  
**Puntos de Extension:** No aplica.  
**Condicion:** El actor desea corregir o actualizar el código o el nombre de una moneda.  

**Escenario Principal:**
1. El actor selecciona la opción «Monedas» del menú «Operador».
2. El sistema presenta la pantalla «Gestion de Monedas» con el listado de monedas registradas.
3. El actor selecciona del listado la moneda a modificar.
4. El sistema muestra el código y el nombre de la moneda en los campos «Codigo:» y «Nombre:».
5. El actor modifica el código y/o el nombre.
6. El actor presiona el botón «Actualizar» de la sección «Monedas».
7. El sistema guarda los cambios, vacía los campos, actualiza el listado manteniendo seleccionada la moneda, actualiza los datos de la sección «Tasas de Conversion» y muestra el mensaje «Moneda actualizada con exito.».

**Flujos Alternativos:**
6a. Si el actor presiona «Actualizar» sin haber seleccionado una moneda, el sistema muestra el mensaje «Debe seleccionar una moneda.». No se realizan cambios.
6b. Si el código o el nombre están vacíos, o superan los 10 y 100 caracteres respectivamente, el sistema muestra el mensaje de validación correspondiente (por ejemplo, «El código de la moneda es obligatorio.»). No se realizan cambios.
6c. Si la moneda seleccionada ya no existe, el sistema muestra el mensaje «No se encontró la moneda indicada.». No se realizan cambios.
6d. Si ya existe otra moneda con ese código o si ocurre otro error al guardar, el sistema muestra el mensaje de error «No se pudo actualizar la moneda.». No se realizan cambios.

**Postcondiciones:** La moneda figura en el listado con su nuevo código y nombre, y así se refleja en las tasas de conversión que la utilizan.

---

**ID y Nombre:** CUN36 Baja de moneda  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador eliminar una moneda registrada en el sistema.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada y cuenta con autorización para acceder a la gestión de monedas. Existe al menos una moneda registrada. La moneda no está en uso en otros datos del sistema (por ejemplo, tasas de conversión).  
**Puntos de Extension:** Si la moneda integra tasas de conversión, estas deben eliminarse previamente mediante CUN40 Baja de tasa de conversión.  
**Condicion:** El actor desea retirar del sistema una moneda que ya no corresponde.  

**Escenario Principal:**
1. El actor selecciona la opción «Monedas» del menú «Operador».
2. El sistema presenta la pantalla «Gestion de Monedas» con el listado de monedas registradas.
3. El actor selecciona del listado la moneda a eliminar.
4. El actor presiona el botón «Eliminar» de la sección «Monedas».
5. El sistema solicita confirmación mediante el mensaje «Se eliminara la moneda '{código}' en forma definitiva. Desea continuar?», titulado «Confirmar eliminacion».
6. El actor confirma la eliminación.
7. El sistema elimina la moneda, vacía los campos, actualiza el listado y la sección «Tasas de Conversion» y muestra el mensaje «Moneda eliminada con exito.».

**Flujos Alternativos:**
4a. Si el actor presiona «Eliminar» sin haber seleccionado una moneda, el sistema muestra el mensaje «Debe seleccionar una moneda.». No se elimina nada.
6a. Si el actor no confirma la eliminación, el sistema cierra la confirmación y mantiene la moneda en el listado.
7a. Si la moneda ya no existe, el sistema muestra el mensaje «No se encontró la moneda indicada.». No se elimina nada.
7b. Si la moneda está en uso o si ocurre otro error al eliminarla, el sistema muestra el mensaje de error «No se pudo eliminar la moneda.». La moneda se conserva.

**Postcondiciones:** La moneda deja de figurar en el listado y deja de estar disponible para definir tasas de conversión.
