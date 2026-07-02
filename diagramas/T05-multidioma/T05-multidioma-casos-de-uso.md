# T05. Gestion de Multiples Idiomas

_Especificaciones de casos de uso_

Casos de uso de administracion de idiomas y traducciones, y de configuracion del idioma preferido del usuario, segun la implementacion vigente en IdiomaBLL, IdiomaCatalogoService y el patron Observer (IdiomaService).

---

**ID y Nombre:** CU01 Agregar idioma  
**Estado:** Implementado  
**Descripcion:** Permite al administrador incorporar un nuevo idioma al catalogo del sistema.  
**Actor Principal:** Administrador  
**Actor Secundario:** -  
**Precondiciones:** El administrador tiene una sesion iniciada con el permiso de gestion de idiomas.  
**Puntos de Extension:** No aplica.  
**Condicion:** El administrador necesita incorporar un nuevo idioma al catalogo.  

**Escenario Principal:**
1. El administrador abre la pantalla de gestion de idiomas.
2. El administrador ingresa el nombre del nuevo idioma.
3. El administrador presiona agregar idioma.
4. El sistema normaliza el nombre y valida que no exista otro idioma con el mismo nombre.
5. El sistema crea el idioma como no predeterminado y registra la operacion en la bitacora.
6. El sistema recarga la lista de idiomas y, si corresponde, refresca el idioma en ejecucion.
7. El sistema informa que el idioma fue creado.

**Flujos Alternativos:**
4a. Si ya existe un idioma con ese nombre, el sistema informa el error y finaliza el caso de uso.
5a. Si ocurre una excepcion tecnica durante el alta, el sistema informa el error y finaliza el caso de uso.

**Postcondiciones:** El nuevo idioma queda en el catalogo (aun sin traducciones) y la operacion queda registrada en la bitacora.

---

**ID y Nombre:** CU02 Actualizar idioma  
**Estado:** Implementado  
**Descripcion:** Permite al administrador modificar el nombre de un idioma existente.  
**Actor Principal:** Administrador  
**Actor Secundario:** -  
**Precondiciones:** El administrador tiene una sesion iniciada con el permiso de gestion de idiomas y existe al menos un idioma.  
**Puntos de Extension:** Definir un idioma como predeterminado es una accion separada (boton Predeterminado, IdiomaBLL.HacerIdiomaDefault).  
**Condicion:** El administrador necesita corregir o cambiar el nombre de un idioma.  

**Escenario Principal:**
1. El administrador selecciona un idioma de la lista.
2. El administrador modifica el nombre del idioma.
3. El administrador presiona actualizar idioma.
4. El sistema normaliza el nombre y valida que no colisione con otro idioma existente.
5. El sistema actualiza el nombre del idioma y registra la operacion en la bitacora.
6. El sistema recarga la lista y, si corresponde, refresca el idioma en ejecucion.
7. El sistema informa que el idioma fue actualizado.

**Flujos Alternativos:**
1a. Si no hay un idioma seleccionado, el sistema informa que se debe seleccionar un idioma y finaliza el caso de uso.
4a. Si el nombre ya pertenece a otro idioma, el sistema informa el error y finaliza el caso de uso.

**Postcondiciones:** El idioma queda con su nombre actualizado y la operacion queda registrada en la bitacora.

---

**ID y Nombre:** CU03 Eliminar idioma  
**Estado:** Implementado  
**Descripcion:** Permite al administrador eliminar un idioma del catalogo junto con sus traducciones, siempre que no sea el idioma predeterminado.  
**Actor Principal:** Administrador  
**Actor Secundario:** -  
**Precondiciones:** El administrador tiene una sesion iniciada con el permiso de gestion de idiomas y existe el idioma a eliminar.  
**Puntos de Extension:** No aplica.  
**Condicion:** El administrador necesita quitar un idioma del catalogo.  

**Escenario Principal:**
1. El administrador selecciona un idioma de la lista.
2. El administrador presiona eliminar idioma.
3. El sistema solicita confirmacion.
4. El administrador confirma la eliminacion.
5. El sistema verifica que el idioma no sea el predeterminado.
6. El sistema elimina el idioma junto con sus traducciones y registra la operacion en la bitacora.
7. El sistema recarga la lista y, si corresponde, refresca el idioma en ejecucion.
8. El sistema informa que el idioma fue eliminado.

**Flujos Alternativos:**
1a. Si no hay un idioma seleccionado, el sistema informa que se debe seleccionar un idioma y finaliza el caso de uso.
4a. Si el administrador no confirma, el sistema finaliza el caso de uso sin cambios.
5a. Si el idioma es el predeterminado, el sistema informa que no se puede eliminar y finaliza el caso de uso.

**Postcondiciones:** El idioma y sus traducciones quedan eliminados del catalogo y la operacion queda registrada en la bitacora.

---

**ID y Nombre:** CU04 Importar JSON de traducciones  
**Estado:** Implementado  
**Descripcion:** Permite al administrador cargar masivamente las traducciones de un idioma a partir de un archivo JSON de pares clave-valor.  
**Actor Principal:** Administrador  
**Actor Secundario:** -  
**Precondiciones:** El administrador tiene una sesion iniciada con el permiso de gestion de idiomas, existe el idioma destino y las claves del archivo corresponden a palabras existentes en el catalogo.  
**Puntos de Extension:** No aplica.  
**Condicion:** El administrador necesita cargar o actualizar muchas traducciones de un idioma de una sola vez.  

**Escenario Principal:**
1. El administrador selecciona un idioma y presiona importar JSON.
2. El sistema abre el dialogo de seleccion de archivo.
3. El administrador elige un archivo JSON de traducciones.
4. El sistema valida que el archivo exista y deserializa su contenido en pares clave-valor.
5. Por cada par, el sistema busca la palabra por su clave y guarda la traduccion para el idioma.
6. El sistema registra la operacion en la bitacora y confirma la transaccion.
7. El sistema recarga las traducciones del idioma y, si corresponde, refresca el idioma en ejecucion.
8. El sistema informa que las traducciones fueron importadas.

**Flujos Alternativos:**
1a. Si no hay un idioma seleccionado, el sistema informa que se debe seleccionar un idioma y finaliza el caso de uso.
3a. Si el administrador cancela el dialogo, el sistema finaliza el caso de uso sin cambios.
4a. Si el archivo no existe, el sistema informa el error y finaliza el caso de uso.
4b. Si el JSON tiene un formato invalido, el sistema informa el error y finaliza el caso de uso.
4c. Si el JSON esta vacio, el sistema informa el error y finaliza el caso de uso.
5a. Si alguna clave no existe en el catalogo de palabras, el sistema informa el error, revierte la transaccion y finaliza el caso de uso.

**Postcondiciones:** Las traducciones del archivo quedan aplicadas al idioma y la operacion queda registrada en la bitacora.

---

**ID y Nombre:** CU05 Exportar JSON de traducciones  
**Estado:** Implementado  
**Descripcion:** Permite al administrador exportar las traducciones de un idioma a un archivo JSON de pares clave-valor.  
**Actor Principal:** Administrador  
**Actor Secundario:** -  
**Precondiciones:** El administrador tiene una sesion iniciada con el permiso de gestion de idiomas y existe el idioma a exportar.  
**Puntos de Extension:** No aplica.  
**Condicion:** El administrador necesita obtener las traducciones de un idioma en un archivo externo.  

**Escenario Principal:**
1. El administrador selecciona un idioma y presiona exportar JSON.
2. El sistema solicita confirmacion.
3. El administrador confirma y elige la ubicacion y el nombre del archivo.
4. El sistema obtiene el idioma con sus traducciones y arma un diccionario clave-valor ordenado.
5. El sistema serializa el diccionario a JSON y lo escribe en el archivo elegido en UTF-8.
6. El sistema registra la operacion en la bitacora.
7. El sistema informa que las traducciones fueron exportadas.

**Flujos Alternativos:**
1a. Si no hay un idioma seleccionado, el sistema informa que se debe seleccionar un idioma y finaliza el caso de uso.
2a. Si el administrador no confirma o cancela el dialogo, el sistema finaliza el caso de uso sin cambios.
4a. Si el idioma no existe, el sistema informa el error y finaliza el caso de uso.

**Postcondiciones:** Se genera un archivo JSON con las traducciones del idioma y la operacion queda registrada en la bitacora.

---

**ID y Nombre:** CU06 Editar traduccion  
**Estado:** Implementado  
**Descripcion:** Permite al administrador crear o modificar la traduccion de una clave para un idioma.  
**Actor Principal:** Administrador  
**Actor Secundario:** -  
**Precondiciones:** El administrador tiene una sesion iniciada con el permiso de gestion de idiomas, existe el idioma y la clave corresponde a una palabra existente en el catalogo.  
**Puntos de Extension:** No aplica.  
**Condicion:** El administrador necesita definir o corregir el texto de una clave en un idioma.  

**Escenario Principal:**
1. El administrador selecciona un idioma y una clave, y edita su valor.
2. El administrador presiona guardar traduccion.
3. El sistema valida que la clave y el valor no esten vacios.
4. El sistema busca la palabra por su clave.
5. El sistema guarda (inserta o actualiza) la traduccion para el idioma y registra la operacion en la bitacora.
6. El sistema recarga las traducciones y, si corresponde, refresca el idioma en ejecucion.
7. El sistema informa que la traduccion fue guardada.

**Flujos Alternativos:**
1a. Si no hay un idioma seleccionado, el sistema informa que se debe seleccionar un idioma y finaliza el caso de uso.
3a. Si la clave o el valor estan vacios, el sistema informa el error y finaliza el caso de uso.
4a. Si la clave no existe en el catalogo de palabras, el sistema informa el error y finaliza el caso de uso.

**Postcondiciones:** La traduccion del idioma para esa clave queda guardada y la operacion queda registrada en la bitacora.

---

**ID y Nombre:** CU07 Configurar idioma preferido  
**Estado:** Implementado  
**Descripcion:** Permite al usuario elegir el idioma con el que se muestra la aplicacion, guardandolo como su preferencia y aplicandolo de inmediato.  
**Actor Principal:** Usuario  
**Actor Secundario:** -  
**Precondiciones:** El usuario tiene una sesion iniciada con el permiso de configuracion de usuario.  
**Puntos de Extension:** Al aplicar el idioma se dispara el patron Observer, que retraduce todas las pantallas abiertas.  
**Condicion:** El usuario desea cambiar el idioma con el que se muestra la aplicacion.  

**Escenario Principal:**
1. El usuario abre la pantalla de configuracion de usuario.
2. El sistema carga los idiomas disponibles y marca el idioma actual.
3. El usuario selecciona el idioma preferido y presiona guardar.
4. El sistema obtiene el usuario de la sesion actual.
5. El sistema valida que el idioma sea seleccionable.
6. El sistema guarda la preferencia de idioma del usuario, registra la version en su historial y en la bitacora.
7. El sistema aplica el idioma seleccionado y notifica a las pantallas abiertas para que se retraduzcan.
8. El sistema informa que la configuracion fue guardada.

**Flujos Alternativos:**
3a. Si no hay un idioma seleccionado, el sistema informa que se debe seleccionar un idioma y finaliza el caso de uso.
4a. Si no hay una sesion activa, el sistema informa el error y cierra la pantalla de configuracion.
5a. Si el idioma no es seleccionable, el sistema informa el error y finaliza el caso de uso.

**Postcondiciones:** La preferencia de idioma del usuario queda persistida, con version en su historial y en la bitacora, y la interfaz queda mostrada en el nuevo idioma.
