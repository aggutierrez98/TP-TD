# Tasas de conversión

_Especificaciones de casos de uso_

---

**ID y Nombre:** CUN37 Consultar tasas de conversion  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador consultar las tasas de conversión definidas entre pares de monedas, su fecha de actualización y si fueron fijadas manualmente.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada y cuenta con autorización para acceder a la gestión de monedas.  
**Puntos de Extension:** No aplica.  
**Condicion:** El actor desea conocer las tasas de conversión vigentes entre las monedas del sistema.  

**Escenario Principal:**
1. El actor selecciona la opción «Monedas» del menú «Operador».
2. El sistema presenta la pantalla «Gestion de Monedas» con la sección «Tasas de Conversion», que contiene el listado de tasas indicando para cada una su «Moneda Base», «Moneda Conversion», «Calculo», «Fecha Actualizacion» y «Manual» (casilla marcada si la tasa fue fijada manualmente).
3. El actor selecciona una tasa del listado.
4. El sistema muestra la moneda base, la moneda de conversión y el cálculo de la tasa seleccionada en los campos «Moneda Base:», «Moneda Conversion:» y «Calculo:». Si la tasa es manual, el botón «Liberar» queda habilitado.

**Flujos Alternativos:**
2a. Si no hay tasas registradas, el sistema presenta el listado vacío. El caso de uso finaliza sin selección.
2b. Si ocurre un error al cargar el listado, el sistema muestra un mensaje de error. El caso de uso finaliza sin completar la consulta.
3a. Si el actor solo necesita consultar el listado, omite la selección de una tasa. El caso de uso finaliza con el listado disponible.
4a. Si la tasa seleccionada no es manual, el botón «Liberar» permanece deshabilitado.

**Postcondiciones:** El actor dispone del listado de tasas de conversión y, si seleccionó una, de sus datos. La consulta no modifica las tasas.

---

**ID y Nombre:** CUN38 Alta de tasa de conversion  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador registrar la tasa de conversión entre un par de monedas.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada y cuenta con autorización para acceder a la gestión de monedas. Existen al menos dos monedas registradas. No existe una tasa para el mismo par de monedas.  
**Puntos de Extension:** Si las monedas necesarias no existen, se deben registrar previamente mediante CUN34 Alta de moneda.  
**Condicion:** El actor desea definir cuántas unidades de una moneda equivalen a una unidad de otra.  

**Escenario Principal:**
1. El actor selecciona la opción «Monedas» del menú «Operador».
2. El sistema presenta la pantalla «Gestion de Monedas» con la sección «Tasas de Conversion» y su listado de tasas.
3. El actor selecciona una moneda en el campo «Moneda Base:».
4. El actor selecciona una moneda distinta en el campo «Moneda Conversion:».
5. El actor ingresa el valor en el campo «Calculo:» (mayor a cero, hasta seis decimales).
6. El actor presiona el botón «Agregar» de la sección «Tasas de Conversion».
7. El sistema registra la tasa con la fecha actual y sin marca de manual, limpia los campos, actualiza el listado con la nueva tasa seleccionada y muestra el mensaje «Tasa de conversion creada con exito.».

**Flujos Alternativos:**
6a. Si el actor no seleccionó la moneda base, el sistema muestra el mensaje «Debe seleccionar una moneda base.». No se registra la tasa.
6b. Si el actor no seleccionó la moneda de conversión, el sistema muestra el mensaje «Debe seleccionar una moneda de conversion.». No se registra la tasa.
6c. Si la moneda base y la de conversión son la misma, el sistema muestra el mensaje «La moneda base y la moneda de conversion deben ser distintas.». No se registra la tasa.
6d. Si el cálculo es cero, el sistema muestra el mensaje «El calculo de conversion debe ser mayor a cero.». No se registra la tasa.
6e. Si ya existe una tasa para ese par de monedas, el sistema muestra el mensaje «Ya existe una tasa de conversion para ese par de monedas.». No se registra la tasa.
6f. Si alguna de las monedas ya no existe, el sistema muestra el mensaje «No se encontro la moneda base indicada.» o «No se encontro la moneda de conversion indicada.», según corresponda. No se registra la tasa.
6g. Si ocurre un error al registrar la tasa, el sistema muestra un mensaje de error. No se registra la tasa.

**Postcondiciones:** La tasa de conversión figura en el listado como no manual, por lo que queda alcanzada por la actualización automática de tasas.

---

**ID y Nombre:** CUN39 Modificar tasa de conversion  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador modificar el par de monedas o el cálculo de una tasa de conversión existente.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada y cuenta con autorización para acceder a la gestión de monedas. Existe al menos una tasa de conversión registrada.  
**Puntos de Extension:** Una tasa modificada queda como manual; para volver a incluirla en la actualización automática se utiliza CUN41 Liberar tasa de conversion para actualizacion automatica.  
**Condicion:** El actor desea corregir o fijar el valor de una tasa de conversión.  

**Escenario Principal:**
1. El actor selecciona la opción «Monedas» del menú «Operador».
2. El sistema presenta la pantalla «Gestion de Monedas» con la sección «Tasas de Conversion» y su listado de tasas.
3. El actor selecciona del listado la tasa a modificar.
4. El sistema muestra la moneda base, la moneda de conversión y el cálculo de la tasa en los campos «Moneda Base:», «Moneda Conversion:» y «Calculo:».
5. El actor modifica la moneda base, la moneda de conversión y/o el cálculo.
6. El actor presiona el botón «Actualizar» de la sección «Tasas de Conversion».
7. El sistema guarda los cambios con la fecha actual, actualiza el listado manteniendo seleccionada la tasa, marca la tasa como manual si hubo cambios (casilla de la columna «Manual») y muestra el mensaje «Tasa de conversion actualizada con exito.».

**Flujos Alternativos:**
6a. Si el actor presiona «Actualizar» sin haber seleccionado una tasa, el sistema muestra el mensaje «Debe seleccionar una tasa de conversion.». No se realizan cambios.
6b. Si falta seleccionar la moneda base o la de conversión, el sistema muestra el mensaje «Debe seleccionar una moneda base.» o «Debe seleccionar una moneda de conversion.», según corresponda. No se realizan cambios.
6c. Si la moneda base y la de conversión son la misma, el sistema muestra el mensaje «La moneda base y la moneda de conversion deben ser distintas.». No se realizan cambios.
6d. Si el cálculo es cero, el sistema muestra el mensaje «El calculo de conversion debe ser mayor a cero.». No se realizan cambios.
6e. Si ya existe otra tasa para ese par de monedas, el sistema muestra el mensaje «Ya existe una tasa de conversion para ese par de monedas.». No se realizan cambios.
6f. Si la tasa seleccionada ya no existe, el sistema muestra el mensaje «No se encontro la tasa de conversion indicada.». No se realizan cambios.
6g. Si ocurre un error al guardar, el sistema muestra un mensaje de error. No se realizan cambios.
7a. Si el actor no cambió ningún valor, el sistema actualiza la fecha de la tasa pero no la marca como manual.

**Postcondiciones:** La tasa figura en el listado con los nuevos valores y su fecha de actualización vigente. Si hubo cambios, queda marcada como manual y la actualización automática no la sobrescribe.

---

**ID y Nombre:** CUN40 Baja de tasa de conversion  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador eliminar la tasa de conversión de un par de monedas.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada y cuenta con autorización para acceder a la gestión de monedas. Existe al menos una tasa de conversión registrada.  
**Puntos de Extension:** No aplica.  
**Condicion:** El actor desea retirar una tasa de conversión que ya no corresponde.  

**Escenario Principal:**
1. El actor selecciona la opción «Monedas» del menú «Operador».
2. El sistema presenta la pantalla «Gestion de Monedas» con la sección «Tasas de Conversion» y su listado de tasas.
3. El actor selecciona del listado la tasa a eliminar.
4. El actor presiona el botón «Eliminar» de la sección «Tasas de Conversion».
5. El sistema solicita confirmación mediante el mensaje «Se eliminara la tasa de conversion seleccionada. Desea continuar?», titulado «Confirmar eliminacion».
6. El actor confirma la eliminación.
7. El sistema elimina la tasa, limpia los campos, actualiza el listado sin la tasa eliminada y muestra el mensaje «Tasa de conversion eliminada con exito.».

**Flujos Alternativos:**
4a. Si el actor presiona «Eliminar» sin haber seleccionado una tasa, el sistema muestra el mensaje «Debe seleccionar una tasa de conversion.». No se elimina nada.
6a. Si el actor no confirma la eliminación, el sistema cierra la confirmación y mantiene la tasa en el listado.
7a. Si la tasa ya no existe, el sistema muestra el mensaje «No se encontro la tasa de conversion indicada.». No se elimina nada.
7b. Si ocurre un error al eliminar, el sistema muestra un mensaje de error. La tasa se conserva.

**Postcondiciones:** La tasa de conversión deja de figurar en el listado. Las monedas del par continúan registradas.

---

**ID y Nombre:** CUN41 Liberar tasa de conversion para actualizacion automatica  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador quitar la marca de manual de una tasa de conversión para que vuelva a ser actualizada por la actualización automática de tasas.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada y cuenta con autorización para acceder a la gestión de monedas. Existe una tasa de conversión marcada como manual.  
**Puntos de Extension:** No aplica.  
**Condicion:** El actor desea que una tasa fijada manualmente vuelva a actualizarse con los valores del proveedor externo.  

**Escenario Principal:**
1. El actor selecciona la opción «Monedas» del menú «Operador».
2. El sistema presenta la pantalla «Gestion de Monedas» con la sección «Tasas de Conversion» y su listado de tasas.
3. El actor selecciona del listado una tasa con la columna «Manual» marcada.
4. El sistema habilita el botón «Liberar».
5. El actor presiona el botón «Liberar».
6. El sistema quita la marca de manual de la tasa, actualiza el listado manteniéndola seleccionada, deshabilita el botón «Liberar» y muestra el mensaje «Tasa de conversion liberada con exito.».

**Flujos Alternativos:**
3a. Si el actor selecciona una tasa que no es manual, el sistema mantiene deshabilitado el botón «Liberar» y el caso de uso finaliza sin cambios.
5a. Si el actor presiona «Liberar» sin haber seleccionado una tasa, el sistema muestra el mensaje «Debe seleccionar una tasa de conversion.». No se realizan cambios.
6a. Si la tasa ya no existe, el sistema muestra el mensaje «No se encontro la tasa de conversion indicada.». No se realizan cambios.
6b. Si ocurre un error al liberar la tasa, el sistema muestra un mensaje de error. La tasa conserva su marca de manual.

**Postcondiciones:** La tasa figura en el listado sin la marca de manual y es alcanzada por las próximas actualizaciones automáticas de tasas.

---

**ID y Nombre:** CUN42 Consultar historial de actualizaciones de tasas  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador consultar el historial de las últimas actualizaciones de tasas de conversión, el detalle de cada una y las consultas utilizadas en el mes.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada y cuenta con autorización para acceder a la actualización de tasas.  
**Puntos de Extension:** Desde esta pantalla se extiende a CUN43 Actualizar tasas desde el proveedor externo.  
**Condicion:** El actor desea revisar cuándo y con qué resultado se actualizaron las tasas de conversión.  

**Escenario Principal:**
1. El actor selecciona la opción «Actualizacion de tasas» del menú «Operador».
2. El sistema presenta la pantalla «Actualizacion de tasas» con el listado de las últimas 50 actualizaciones, indicando para cada una su «Fecha de inicio», «Origen» (Manual o Programada), «Resultado» (Exitosa, Fallida u Omitida), «Consumio request» y «Mensaje», y muestra el contador «Requests usadas este mes: {usadas} de {límite}».
3. El actor selecciona una actualización del listado.
4. El sistema presenta, en «Detalle de la ejecucion seleccionada», las tasas procesadas en esa actualización, indicando «Moneda base», «Moneda de conversion», «Valor anterior», «Valor nuevo» y «Estado» (Actualizado, Saltado (manual) o Saltado (sin dato)).

**Flujos Alternativos:**
2a. Si no hay actualizaciones registradas, el sistema presenta el listado y el detalle vacíos.
2b. Si ocurre un error al cargar el historial o el contador, el sistema muestra un mensaje de error. El caso de uso finaliza sin completar la consulta.
3a. Si el actor solo necesita consultar el listado, omite la selección de una actualización. El caso de uso finaliza con el listado disponible.
4a. Si la actualización seleccionada no procesó tasas (por ejemplo, fue omitida o fallida), el sistema presenta el detalle vacío.
4b. Si ocurre un error al cargar el detalle, el sistema muestra un mensaje de error. El listado permanece visible.

**Postcondiciones:** El actor dispone del historial de actualizaciones, del detalle de la seleccionada y del consumo de consultas del mes. La consulta no modifica tasas ni historial.

---

**ID y Nombre:** CUN43 Actualizar tasas desde el proveedor externo  
**Estado:** Implementado  
**Descripcion:** Permite al Operador o Administrador solicitar la actualización inmediata de las tasas de conversión con las cotizaciones de un proveedor externo de tasas de cambio.  
**Actor Principal:** Operador o Administrador  
**Actor Secundario:** Proveedor externo de tasas de cambio (Open Exchange Rates)  
**Precondiciones:** El actor tiene una sesión iniciada y cuenta con autorización para acceder a la actualización de tasas. Existen tasas de conversión registradas. La última actualización exitosa se realizó hace 60 minutos o más, o no existe ninguna.  
**Puntos de Extension:** Las tasas marcadas como manuales no se actualizan; para incluirlas se utiliza CUN41 Liberar tasa de conversion para actualizacion automatica. El resultado queda disponible en CUN42 Consultar historial de actualizaciones de tasas.  
**Condicion:** El actor desea obtener de inmediato los valores vigentes de las tasas de conversión sin esperar la actualización programada.  

**Escenario Principal:**
1. El actor selecciona la opción «Actualizacion de tasas» del menú «Operador».
2. El sistema presenta la pantalla «Actualizacion de tasas» con el historial de actualizaciones y el botón «Actualizar ahora».
3. El actor presiona el botón «Actualizar ahora».
4. El sistema deshabilita el botón, muestra el cursor de espera y consulta las cotizaciones al proveedor externo.
5. El proveedor externo informa las cotizaciones vigentes.
6. El sistema actualiza el valor de las tasas no manuales para las que dispone de cotización, registra la actualización en el historial y refresca el listado, el detalle y el contador de requests del mes.
7. El sistema vuelve a habilitar el botón y muestra el mensaje «Actualizacion completada: {n} tasas actualizadas, {m} saltadas por ser manuales, {k} saltadas por falta de dato.».

**Flujos Alternativos:**
3a. Si el actor presiona «Actualizar ahora» mientras hay una actualización en curso, el sistema ignora la acción.
4a. Si la última actualización exitosa fue hace menos de 60 minutos, el sistema no consulta al proveedor externo, registra la actualización con resultado «Omitida» y muestra el mensaje «No se consulto la API: la ultima actualizacion fue hace menos de 60 minutos. Proxima consulta permitida: {fecha y hora}.». Las tasas no se modifican.
5a. Si el proveedor externo no responde, rechaza la consulta, informa que la clave es inválida o que se excedió el límite de requests, o devuelve una respuesta inválida, o si falta configurar la clave de acceso, el sistema registra la actualización con resultado «Fallida», rehabilita el botón y muestra el mensaje «La actualizacion fallo: {motivo}», donde el motivo es, por ejemplo, «No se pudo conectar con Open Exchange Rates.» o «Falta configurar la clave de API de Open Exchange Rates.». Las tasas no se modifican.
6a. Si el sistema no puede registrar la actualización, muestra un mensaje de error. El historial no incorpora la actualización.

**Postcondiciones:** El historial incorpora una nueva actualización con su origen «Manual» y su resultado. Si fue exitosa, las tasas no manuales con cotización disponible tienen el valor actualizado y las tasas manuales conservan su valor.
