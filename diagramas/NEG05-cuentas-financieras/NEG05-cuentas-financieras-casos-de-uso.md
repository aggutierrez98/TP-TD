# Gestión de cuentas financieras

_Especificaciones de casos de uso_

---

**ID y Nombre:** CUN19 Consultar mis cuentas financieras  
**Estado:** Implementado  
**Descripcion:** Permite al actor consultar sus cuentas financieras y visualizar los datos de una cuenta seleccionada.  
**Actor Principal:** Usuario, Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada y cuenta con autorización para acceder a la gestión de cuentas financieras.  
**Puntos de Extension:** Desde el listado, el actor puede continuar con CUN20 Alta de cuenta financiera, CUN21 Modificar cuenta financiera y CUN22 Baja de cuenta financiera.  
**Condicion:** El actor desea consultar las cuentas financieras que tiene registradas.  

**Escenario Principal:**
1. El actor selecciona la opción «Mis cuentas financieras» del menú «Finanzas».
2. El sistema presenta la pantalla «Mis cuentas financieras» con el listado de las cuentas del actor, ordenadas por alias, indicando para cada una su alias, entidad, tipo y medios de pago contratados.
3. El actor selecciona una cuenta del listado.
4. El sistema muestra en la sección de datos de la cuenta la entidad financiera, el alias y el tipo de la cuenta seleccionada, y presenta en «Medios de pago contratados:» los medios que ofrece la entidad, con los contratados por la cuenta marcados.

**Flujos Alternativos:**
2a. Si el actor no tiene cuentas registradas, el sistema presenta el listado vacío.
2b. Si ocurre un error al cargar el listado, el sistema muestra un mensaje de error. El caso de uso finaliza sin completar la consulta.
3a. Si el actor solo necesita consultar el listado, omite la selección de una cuenta. El caso de uso finaliza con el listado disponible.
4a. Si ocurre un error al cargar los datos de la cuenta seleccionada, el sistema muestra un mensaje de error. El listado permanece visible.

**Postcondiciones:** El actor dispone del listado de sus cuentas financieras y, si seleccionó una, de sus datos. La consulta no modifica las cuentas.

---

**ID y Nombre:** CUN20 Alta de cuenta financiera  
**Estado:** Implementado  
**Descripcion:** Permite al actor registrar una nueva cuenta financiera indicando su entidad, alias, tipo y los medios de pago que tiene contratados.  
**Actor Principal:** Usuario, Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada y cuenta con autorización para acceder a la gestión de cuentas financieras. Existe al menos una entidad financiera disponible.  
**Puntos de Extension:** No aplica.  
**Condicion:** El actor desea registrar una cuenta financiera que utiliza.  

**Escenario Principal:**
1. El actor selecciona la opción «Mis cuentas financieras» del menú «Finanzas».
2. El sistema presenta la pantalla «Mis cuentas financieras» con el listado de cuentas y la sección «Nueva cuenta financiera», que incluye los campos «Entidad financiera:», «Alias:», «Tipo:» y «Medios de pago contratados:».
3. El actor selecciona la entidad financiera.
4. El sistema presenta en «Medios de pago contratados:» los medios de pago que ofrece la entidad seleccionada.
5. El actor ingresa el alias de la cuenta, selecciona el tipo («Billetera digital», «Caja de ahorro» o «Cuenta corriente») y, si lo desea, marca los medios de pago que tiene contratados.
6. El actor presiona el botón «Agregar».
7. El sistema crea la cuenta, limpia el formulario y actualiza el listado, donde la nueva cuenta queda seleccionada.
8. El sistema muestra el mensaje «Cuenta financiera creada con éxito.».

**Flujos Alternativos:**
4a. Si la entidad seleccionada no ofrece medios de pago, el sistema presenta la lista de medios vacía. El escenario continúa en el paso 5 sin medios contratados.
6a. Si no hay una entidad financiera seleccionada, el sistema muestra el mensaje «Debe seleccionar una entidad financiera.». La cuenta no se crea.
6b. Si no hay un tipo seleccionado, el sistema muestra el mensaje «Debe seleccionar un tipo de cuenta.». La cuenta no se crea.
7a. Si el alias está vacío, el sistema muestra el mensaje «El alias de la cuenta es obligatorio.». La cuenta no se crea.
7b. Si ocurre un error al crear la cuenta, el sistema muestra el mensaje «No se pudo crear la cuenta financiera.». La cuenta no se crea.

**Postcondiciones:** La nueva cuenta figura en el listado de cuentas del actor con sus medios de pago contratados. Si el caso de uso no se completa, el listado no cambia.

---

**ID y Nombre:** CUN21 Modificar cuenta financiera  
**Estado:** Implementado  
**Descripcion:** Permite al actor modificar la entidad, el alias, el tipo o los medios de pago contratados de una cuenta financiera propia.  
**Actor Principal:** Usuario, Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada y cuenta con autorización para acceder a la gestión de cuentas financieras. El actor tiene al menos una cuenta financiera registrada.  
**Puntos de Extension:** No aplica.  
**Condicion:** El actor necesita corregir los datos de una cuenta o cambiar los medios de pago que tiene contratados.  

**Escenario Principal:**
1. El actor selecciona la opción «Mis cuentas financieras» del menú «Finanzas».
2. El sistema presenta la pantalla «Mis cuentas financieras» con el listado de las cuentas del actor.
3. El actor selecciona la cuenta a modificar.
4. El sistema muestra la entidad financiera, el alias y el tipo de la cuenta seleccionada y presenta los medios de pago de la entidad, con los contratados marcados.
5. El actor modifica los datos que desea cambiar. Si cambia la entidad, el sistema presenta los medios de pago de la nueva entidad sin ninguno marcado.
6. El actor presiona el botón «Actualizar».
7. El sistema aplica los cambios y actualiza el listado, donde la cuenta queda seleccionada con sus datos modificados.
8. El sistema muestra el mensaje «Cuenta financiera actualizada con éxito.».

**Flujos Alternativos:**
6a. Si no hay una cuenta seleccionada, el sistema muestra el mensaje «Debe seleccionar una cuenta financiera.». No se realizan cambios.
6b. Si no hay una entidad financiera seleccionada, el sistema muestra el mensaje «Debe seleccionar una entidad financiera.». No se realizan cambios.
6c. Si no hay un tipo seleccionado, el sistema muestra el mensaje «Debe seleccionar un tipo de cuenta.». No se realizan cambios.
7a. Si el alias está vacío, el sistema muestra el mensaje «El alias de la cuenta es obligatorio.». No se realizan cambios.
7b. Si ocurre un error al actualizar la cuenta, el sistema muestra el mensaje «No se pudo actualizar la cuenta financiera.». La cuenta conserva sus datos anteriores.

**Postcondiciones:** La cuenta figura en el listado con los datos y los medios de pago contratados modificados. Si el caso de uso no se completa, la cuenta conserva sus datos anteriores.

---

**ID y Nombre:** CUN22 Baja de cuenta financiera  
**Estado:** Implementado  
**Descripcion:** Permite al actor eliminar una cuenta financiera propia que ya no utiliza.  
**Actor Principal:** Usuario, Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada y cuenta con autorización para acceder a la gestión de cuentas financieras. El actor tiene al menos una cuenta financiera registrada.  
**Puntos de Extension:** No aplica.  
**Condicion:** El actor desea quitar una cuenta financiera de su lista.  

**Escenario Principal:**
1. El actor selecciona la opción «Mis cuentas financieras» del menú «Finanzas».
2. El sistema presenta la pantalla «Mis cuentas financieras» con el listado de las cuentas del actor.
3. El actor selecciona la cuenta a eliminar.
4. El sistema muestra los datos de la cuenta seleccionada.
5. El actor presiona el botón «Eliminar».
6. El sistema solicita confirmación con el título «Confirmar eliminación» y el mensaje «Se eliminará la cuenta» seguido del alias de la cuenta y la pregunta «¿Desea continuar?».
7. El actor confirma la eliminación.
8. El sistema elimina la cuenta, actualiza el listado, que ya no incluye la cuenta eliminada, y limpia el formulario.
9. El sistema muestra el mensaje «Cuenta financiera eliminada con éxito.».

**Flujos Alternativos:**
5a. Si no hay una cuenta seleccionada, el sistema muestra el mensaje «Debe seleccionar una cuenta financiera.». No se realizan cambios.
7a. Si el actor no confirma la eliminación, el caso de uso finaliza sin cambios.
8a. Si ocurre un error al eliminar la cuenta, el sistema muestra el mensaje «No se pudo eliminar la cuenta financiera.». La cuenta permanece en el listado.

**Postcondiciones:** La cuenta eliminada no figura en el listado del actor. Si el caso de uso no se completa, la cuenta permanece sin cambios.
