# Dashboard y balance

_Especificaciones de casos de uso_

---

**ID y Nombre:** CUN72 Consultar dashboard de analíticas  
**Estado:** Implementado  
**Descripcion:** Permite al Usuario, Operador o Administrador visualizar su dashboard personal de analíticas, compuesto por las tarjetas con gráficos que haya agregado.  
**Actor Principal:** Usuario, Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada y cuenta con autorización para acceder al dashboard de analíticas.  
**Puntos de Extension:** Extendido por CUN73 Agregar tarjeta al dashboard y por CUN74 Quitar tarjeta del dashboard, que se inician desde esta misma pantalla.  
**Condicion:** El actor desea revisar de forma gráfica el estado de sus finanzas personales.  

**Escenario Principal:**
1. El actor selecciona la opción «Dashboard» del menú «Analíticas».
2. El sistema presenta la pantalla «Dashboard» ocupando el área de trabajo, con el botón «Agregar tarjeta» y una tarjeta por cada análisis que el actor haya agregado.
3. El sistema muestra en cada tarjeta su título (por ejemplo «Gastos por categoría», «Ingresos por categoría», «Comparativo mensual», «Evolución de balance», «Gastos por comercio» o «Gasto acumulado del mes»), un resumen con el período y los totales correspondientes («Período», «Total», «Ingresos», «Egresos», «Balance», «Sin comercio» o «Variación», según la tarjeta), el gráfico asociado y el botón «Quitar».
4. El actor revisa los gráficos y los resúmenes de las tarjetas.

**Flujos Alternativos:**
2a. Si el actor aún no agregó ninguna tarjeta, el sistema presenta el dashboard sin tarjetas, con el botón «Agregar tarjeta» disponible. El caso de uso finaliza sin tarjetas para consultar.
2b. Si el actor ya agregó todos los tipos de tarjeta disponibles, el sistema deshabilita el botón «Agregar tarjeta» y muestra el mensaje «Ya agregó todas las tarjetas disponibles.».
2c. Si ocurre un error al cargar el dashboard, el sistema muestra un mensaje de error. El caso de uso finaliza sin completar la consulta.
3a. Si una tarjeta no tiene movimientos en el período que analiza, el sistema muestra en su lugar el texto «No hay datos en el período.».
4a. Si el actor cambia el idioma de la aplicación mientras el dashboard está abierto, el sistema vuelve a presentar las tarjetas con sus textos en el idioma elegido.
4b. Si el actor redimensiona la ventana, el sistema reordena las tarjetas en una o dos columnas según el ancho disponible, con desplazamiento vertical cuando no entran todas.

**Postcondiciones:** El actor dispone de la vista de sus tarjetas de analíticas. La consulta no modifica el dashboard ni los movimientos del actor.

---

**ID y Nombre:** CUN73 Agregar tarjeta al dashboard  
**Estado:** Implementado  
**Descripcion:** Permite al Usuario, Operador o Administrador incorporar a su dashboard personal una nueva tarjeta de analíticas, elegida entre los tipos que aún no tiene.  
**Actor Principal:** Usuario, Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada, cuenta con autorización para acceder al dashboard de analíticas y tiene abierta la pantalla «Dashboard». Existe al menos un tipo de tarjeta que el actor aún no agregó.  
**Puntos de Extension:** Extiende a CUN72 Consultar dashboard de analíticas.  
**Condicion:** El actor desea sumar un nuevo análisis a su dashboard.  

**Escenario Principal:**
1. El actor selecciona el botón «Agregar tarjeta» de la pantalla «Dashboard».
2. El sistema presenta el diálogo «Agregar tarjeta» con la lista de tipos de tarjeta que el actor aún no agregó, con el botón «Aceptar» deshabilitado y el botón «Cancelar».
3. El actor selecciona un tipo de tarjeta de la lista.
4. El sistema habilita el botón «Aceptar».
5. El actor selecciona el botón «Aceptar».
6. El sistema cierra el diálogo y vuelve a presentar el dashboard incluyendo la nueva tarjeta con su gráfico y su resumen.

**Flujos Alternativos:**
1a. Si el actor ya agregó todos los tipos de tarjeta disponibles, el botón «Agregar tarjeta» se encuentra deshabilitado y el sistema muestra el mensaje «Ya agregó todas las tarjetas disponibles.». El caso de uso no puede iniciarse.
2a. Si ocurre un error al cargar los tipos de tarjeta disponibles, el sistema muestra un mensaje de error y la lista queda vacía. El caso de uso finaliza sin agregar la tarjeta.
3a. Si el actor no selecciona ningún tipo de tarjeta, el botón «Aceptar» permanece deshabilitado y no es posible confirmar.
5a. Si el actor selecciona el botón «Cancelar», el sistema cierra el diálogo sin modificar el dashboard. El caso de uso finaliza.
6a. Si el tipo seleccionado ya había sido agregado al dashboard, el sistema muestra el mensaje «Ya agregó una tarjeta de este tipo a su dashboard.» y no agrega la tarjeta.
6b. Si ocurre un error al agregar la tarjeta, el sistema muestra un mensaje de error. El dashboard permanece sin cambios.

**Postcondiciones:** El dashboard del actor incluye la nueva tarjeta, que se mantiene en sus próximas visitas al dashboard. Los tipos de tarjeta ya agregados no vuelven a ofrecerse.

---

**ID y Nombre:** CUN74 Quitar tarjeta del dashboard  
**Estado:** Implementado  
**Descripcion:** Permite al Usuario, Operador o Administrador retirar de su dashboard personal una tarjeta de analíticas que ya no desea visualizar.  
**Actor Principal:** Usuario, Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada, cuenta con autorización para acceder al dashboard de analíticas y tiene abierta la pantalla «Dashboard» con al menos una tarjeta agregada.  
**Puntos de Extension:** Extiende a CUN72 Consultar dashboard de analíticas.  
**Condicion:** El actor desea dejar de ver un análisis en su dashboard.  

**Escenario Principal:**
1. El actor selecciona el botón «Quitar» de la tarjeta que desea retirar.
2. El sistema vuelve a presentar el dashboard sin la tarjeta retirada y deja disponible nuevamente ese tipo de tarjeta para ser agregado.

**Flujos Alternativos:**
1a. Si ocurre un error al quitar la tarjeta, el sistema muestra un mensaje de error. El dashboard permanece sin cambios.
1b. Si la tarjeta ya no pertenece al dashboard del actor o no existe, el sistema muestra un mensaje de advertencia y no realiza cambios.
2a. Si el actor quitó la única tarjeta que tenía, el sistema presenta el dashboard sin tarjetas, con el botón «Agregar tarjeta» disponible.

**Postcondiciones:** El dashboard del actor ya no incluye la tarjeta retirada. Los movimientos del actor no se ven afectados.

---

**ID y Nombre:** CUN75 Consultar balance general  
**Estado:** Implementado  
**Descripcion:** Permite al Usuario, Operador o Administrador consultar el balance de ingresos y egresos de un período y el detalle de los movimientos que lo componen.  
**Actor Principal:** Usuario, Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada y cuenta con autorización para acceder al balance general.  
**Puntos de Extension:** No aplica.  
**Condicion:** El actor desea conocer cuánto ingresó, cuánto gastó y cuál fue su balance en un período.  

**Escenario Principal:**
1. El actor selecciona la opción «Balance General» del menú «Analíticas».
2. El sistema presenta la pantalla «Balance General» ocupando el área de trabajo, con los filtros «Desde:», «Hasta:», «Tipo:» y «Buscar:» y el botón «Buscar». El filtro de fechas aparece preestablecido con el mes actual completo.
3. El sistema muestra los totales «Ingresos del período:», «Egresos del período:» y «Balance del período:» del rango de fechas indicado, y el detalle de los movimientos del período en una grilla con las columnas «Fecha», «Tipo», «Categoría», «Comercio», «Monto» y «Descripción», de a veinte movimientos por página, junto con el indicador «Página 1 de N».
4. El actor modifica el rango de fechas, selecciona un tipo de movimiento («Ingreso» o «Egreso») o ingresa un texto de búsqueda.
5. El actor selecciona el botón «Buscar».
6. El sistema actualiza los totales y la grilla con el nuevo filtro, mostrando la primera página del resultado.
7. El actor selecciona los botones «Siguiente» y «Anterior» para recorrer las páginas del detalle.
8. El sistema presenta la página solicitada y actualiza el indicador «Página n de N».

**Flujos Alternativos:**
2a. Si ocurre un error al cargar los datos, el sistema muestra un mensaje de error. El caso de uso finaliza sin completar la consulta.
3a. Si no hay movimientos para el período, el sistema presenta la grilla vacía con el indicador «Página 1 de 1».
4a. Si el actor deja el filtro «Tipo:» en blanco y no ingresa texto de búsqueda, el sistema considera todos los movimientos del rango de fechas.
6a. Los totales de ingresos, egresos y balance reflejan únicamente el rango de fechas elegido: los filtros «Tipo:» y «Buscar:» solo acotan el detalle de la grilla.
7a. Si el actor se encuentra en la primera página, el botón «Anterior» está deshabilitado; si se encuentra en la última, el botón «Siguiente» está deshabilitado.
8a. Si el actor cambia el idioma de la aplicación, el sistema vuelve a presentar los textos de la pantalla y de la columna «Tipo» en el idioma elegido, sin modificar el filtro aplicado.

**Postcondiciones:** El actor dispone del balance del período consultado y del detalle de sus movimientos. La consulta no modifica ningún movimiento.
