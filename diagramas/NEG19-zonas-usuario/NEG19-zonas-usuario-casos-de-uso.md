# Zonas de usuario

_Especificaciones de casos de uso_

---

**ID y Nombre:** CUN78 Consultar mis zonas  
**Estado:** Implementado  
**Descripcion:** Permite al Usuario, Operador o Administrador consultar las zonas habituales que registró, tanto en un listado como dibujadas sobre un mapa.  
**Actor Principal:** Usuario, Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada y cuenta con autorización para gestionar sus zonas.  
**Puntos de Extension:** Se extiende con CUN79 Alta de zona de usuario y con CUN80 Baja de zona de usuario, que se inician desde la misma pantalla.  
**Condicion:** El actor desea revisar dónde ubicó sus zonas habituales de compra.  

**Escenario Principal:**
1. El actor selecciona la opción «Mis zonas» del menú «Mi usuario».
2. El sistema presenta la pantalla «Mis zonas» ocupando el área de trabajo, con el listado de zonas del actor (columnas «Nombre» y «Radio (m)»), el panel «Nueva zona» y un mapa donde cada zona se dibuja como un círculo de su radio, con un marcador y su nombre.
3. El actor selecciona una zona del listado.
4. El sistema resalta esa zona en el mapa y lo centra sobre ella.
5. El actor puede acercar o alejar el mapa con los botones «+» y «−» o con la rueda del mouse, y desplazarlo arrastrando con el botón derecho.

**Flujos Alternativos:**
2a. Si el actor aún no registró zonas, el sistema presenta el listado vacío y el mapa centrado en una posición por defecto (Buenos Aires). El caso de uso finaliza sin selección de zona.
2b. Si ocurre un error al cargar las zonas, el sistema muestra un mensaje de error y presenta el listado vacío. El caso de uso finaliza sin completar la consulta.
3a. Si el actor solo necesita ver el mapa, omite la selección de una zona. El caso de uso finaliza con el listado y el mapa disponibles.
5a. Si el mapa alcanza el nivel máximo o mínimo de acercamiento, el sistema deshabilita el botón «+» o «−» correspondiente.

**Postcondiciones:** El actor dispone del listado y del mapa de sus zonas. La consulta no modifica las zonas.

---

**ID y Nombre:** CUN79 Alta de zona de usuario  
**Estado:** Implementado  
**Descripcion:** Permite al Usuario, Operador o Administrador registrar una nueva zona habitual, indicando su nombre, un punto en el mapa y su radio.  
**Actor Principal:** Usuario, Operador o Administrador  
**Actor Secundario:** Servicio de búsqueda de ubicaciones (OpenStreetMap)  
**Precondiciones:** El actor tiene una sesión iniciada, cuenta con autorización para gestionar sus zonas y tiene abierta la pantalla «Mis zonas». El actor registró menos de 10 zonas.  
**Puntos de Extension:** Extiende a CUN78 Consultar mis zonas. La búsqueda de una dirección es opcional: el punto también puede elegirse haciendo clic sobre el mapa.  
**Condicion:** El actor desea registrar una zona habitual, para recibir recomendaciones de compra según su cercanía.  

**Escenario Principal:**
1. El actor ingresa en el campo «Dirección» de la sección «Nueva zona» una dirección, comercio o lugar y selecciona el botón «Buscar» (o presiona Enter).
2. El sistema consulta el servicio de búsqueda de ubicaciones y muestra el estado «Buscando...». Al recibir la respuesta, presenta la lista de coincidencias (hasta cinco) y el mensaje con la cantidad de coincidencias encontradas.
3. El actor elige una coincidencia de la lista.
4. El sistema centra el mapa en la ubicación elegida, dibuja el punto provisorio con el círculo del radio indicado, muestra sus coordenadas en el campo «Punto» y muestra el mensaje «Ubicación elegida».
5. El actor ingresa el «Nombre» de la zona y, si lo desea, modifica el «Radio (m)», cuyo valor inicial es 1000.
6. El actor selecciona el botón «Agregar».
7. El sistema registra la zona, limpia los campos de la sección «Nueva zona», agrega la zona al listado seleccionándola, centra el mapa en ella y muestra el mensaje «Zona agregada correctamente.».

**Flujos Alternativos:**
1a. Si el actor prefiere no buscar una dirección, hace clic con el botón izquierdo sobre el mapa. El sistema marca ese punto, muestra sus coordenadas en el campo «Punto» y el caso de uso continúa en el paso 5.
1b. Si el actor selecciona «Buscar» sin ingresar texto, el sistema muestra el mensaje «Introduzca una dirección, comercio o lugar para buscar.» y no realiza la búsqueda.
1c. Si el servicio de búsqueda de ubicaciones no está configurado correctamente, el sistema deshabilita el campo y el botón de búsqueda y muestra el mensaje «La búsqueda no está configurada correctamente. Puede marcar la ubicación en el mapa.». El caso de uso continúa en el paso 1a.
2a. Si la búsqueda no encuentra coincidencias, el sistema muestra el mensaje «No se encontraron coincidencias. Agregue ciudad o dirección para precisar.». El actor puede refinar el texto o continuar en el paso 1a.
2b. Si la búsqueda tarda demasiado, el servicio limita temporalmente las consultas, devuelve una respuesta no válida o no es posible conectarse, el sistema muestra el mensaje correspondiente e indica que puede marcarse la ubicación en el mapa. El caso de uso continúa en el paso 1a.
3a. Si el actor modifica el texto de búsqueda después de obtener coincidencias, el sistema descarta la lista anterior y no cambia el punto elegido.
5a. Si el actor modifica el radio, el sistema redibuja el círculo provisorio con el nuevo radio.
6a. Si el actor no ingresó un nombre, el sistema muestra el mensaje «El nombre de la zona es obligatorio.» y no registra la zona.
6b. Si el nombre supera los 50 caracteres, el sistema muestra el mensaje «El nombre de la zona admite hasta 50 caracteres.» y no registra la zona.
6c. Si el actor no eligió un punto, el sistema muestra el mensaje «Elija un punto: busque una dirección o haga clic en el mapa.» y no registra la zona.
6d. Si el radio no está entre 100 y 20.000 metros, el sistema muestra el mensaje «El radio debe estar entre 100 y 20.000 metros.» y no registra la zona.
6e. Si el actor ya tiene una zona con ese nombre, el sistema muestra el mensaje «Ya tiene una zona con ese nombre.» y no registra la zona.
6f. Si el actor ya registró 10 zonas, el sistema muestra el mensaje «Alcanzó el máximo de 10 zonas. Quite una para agregar otra.» y no registra la zona.
6g. Si ocurre un error al registrar la zona, el sistema muestra el mensaje «No se pudo agregar la zona.». El listado permanece sin cambios.

**Postcondiciones:** La nueva zona queda registrada para el actor y se visualiza en el listado y en el mapa. Si el caso de uso no se completó, las zonas del actor permanecen sin cambios.

---

**ID y Nombre:** CUN80 Baja de zona de usuario  
**Estado:** Implementado  
**Descripcion:** Permite al Usuario, Operador o Administrador quitar una de sus zonas habituales registradas.  
**Actor Principal:** Usuario, Operador o Administrador  
**Actor Secundario:** -  
**Precondiciones:** El actor tiene una sesión iniciada, cuenta con autorización para gestionar sus zonas y tiene abierta la pantalla «Mis zonas» con al menos una zona registrada.  
**Puntos de Extension:** Extiende a CUN78 Consultar mis zonas.  
**Condicion:** El actor desea dejar de considerar una zona habitual.  

**Escenario Principal:**
1. El actor selecciona una zona del listado.
2. El actor selecciona el botón «Quitar seleccionada».
3. El sistema solicita confirmación con el diálogo «Quitar zona» y el mensaje «¿Desea quitar la zona '<nombre>'?».
4. El actor confirma la baja.
5. El sistema quita la zona, actualiza el listado y el mapa sin la zona y muestra el mensaje «Zona quitada correctamente.».

**Flujos Alternativos:**
2a. Si el actor no seleccionó ninguna zona, el sistema muestra el mensaje «Seleccione una zona de la grilla.» y no realiza cambios.
4a. Si el actor no confirma la baja, el sistema cierra el diálogo sin modificar la zona. El caso de uso finaliza.
5a. Si la zona no existe o no pertenece al actor, el sistema muestra el mensaje «No se encontró la zona indicada.» o «Solo puede operar sobre sus propias zonas.» y no realiza cambios.
5b. Si ocurre un error al quitar la zona, el sistema muestra el mensaje «No se pudo quitar la zona.». El listado permanece sin cambios.

**Postcondiciones:** La zona quitada ya no figura en el listado ni en el mapa del actor, y deja de utilizarse en las recomendaciones de compra. Si el caso de uso no se completó, las zonas del actor permanecen sin cambios.
