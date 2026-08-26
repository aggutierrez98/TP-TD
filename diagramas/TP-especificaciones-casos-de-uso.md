# Especificaciones de Casos de Uso

## T02. Gestion de Login y Logout

_Especificaciones de casos de uso_

Casos de uso de autenticacion del sistema (login, registro y logout), segun la implementacion vigente en UsuarioBLL y las pantallas de la capa UI.

---

**ID y Nombre:** CU01 Login  
**Estado:** Implementado  
**Descripcion:** Permite que un usuario registrado y activo inicie sesion en el sistema mediante su nombre de usuario y contrasena, quedando disponibles sus permisos e idioma.  
**Actor Principal:** Usuario  
**Actor Secundario:** -  
**Precondiciones:** La aplicacion esta iniciada y supero la verificacion de integridad de arranque (T07). El usuario ya existe en la base de datos y se encuentra activo.  
**Puntos de Extension:** CU02 Registro (se accede desde el boton Registrarse de la pantalla de login).  
**Condicion:** El usuario desea autenticarse para acceder a la gestion de usuarios del sistema.  

**Escenario Principal:**
1. El usuario ingresa su nombre de usuario y su contrasena en la pantalla de login.
2. El usuario presiona el boton de ingreso.
3. El sistema valida que el nombre de usuario y la contrasena no esten vacios.
4. El sistema busca al usuario por su nombre de usuario a traves del repositorio.
5. El sistema verifica que el usuario este activo.
6. El sistema verifica la contrasena ingresada contra el hash almacenado (PBKDF2).
7. El sistema inicia la sesion del usuario.
8. El sistema obtiene los permisos directos del usuario y actualiza los permisos efectivos de la sesion.
9. El sistema aplica el idioma configurado para el usuario.
10. El sistema registra en la bitacora la operacion de login exitoso.
11. El sistema limpia los campos del formulario y abre la pantalla de gestion de usuarios con la sesion iniciada.

**Flujos Alternativos:**
3a. Si el nombre de usuario o la contrasena estan vacios, el sistema registra un fallo de negocio en la bitacora, informa que se deben completar los campos y finaliza el caso de uso.
4a. Si el usuario no existe, el sistema registra el fallo en la bitacora, informa credenciales incorrectas y finaliza el caso de uso.
5a. Si el usuario existe pero esta inactivo, el sistema registra el fallo en la bitacora, informa credenciales incorrectas y finaliza el caso de uso.
6a. Si la contrasena no coincide con el hash almacenado, el sistema registra el fallo en la bitacora, informa credenciales incorrectas y finaliza el caso de uso.
11a. Si ocurre una excepcion tecnica durante el proceso, el sistema muestra el error de login al usuario y finaliza el caso de uso.

**Postcondiciones:** La sesion del usuario queda iniciada con sus permisos efectivos e idioma cargados, la pantalla de gestion de usuarios queda visible y el login exitoso queda registrado en la bitacora.

---

**ID y Nombre:** CU02 Registro  
**Estado:** Implementado  
**Descripcion:** Permite crear un nuevo usuario en el sistema a partir de un nombre de usuario y una contrasena, dejandolo activo y con el perfil basico de permisos.  
**Actor Principal:** Usuario  
**Actor Secundario:** -  
**Precondiciones:** La aplicacion esta iniciada y el usuario aun no esta autenticado.  
**Puntos de Extension:** Es invocado desde CU01 Login mediante el boton Registrarse.  
**Condicion:** Una persona desea crear una cuenta para poder autenticarse en el sistema.  

**Escenario Principal:**
1. El usuario presiona el boton Registrarse desde la pantalla de login.
2. El sistema abre el formulario de registro en una ventana modal.
3. El usuario ingresa nombre de usuario y contrasena.
4. El usuario presiona el boton de registro.
5. El sistema valida que el nombre de usuario y la contrasena no esten vacios.
6. El sistema genera el hash seguro de la contrasena (PBKDF2).
7. El sistema verifica que no exista otro usuario con el mismo nombre de usuario.
8. El sistema calcula el digito verificador horizontal (DVH) del nuevo usuario y lo inserta en la base.
9. El sistema asigna al nuevo usuario el perfil basico de permisos.
10. El sistema guarda la version inicial en el historial del usuario y registra la operacion en la bitacora.
11. El sistema actualiza el digito verificador vertical (DVV) de la tabla de usuarios y confirma la transaccion.
12. El sistema informa que el usuario fue registrado con exito y cierra el formulario de registro.

**Flujos Alternativos:**
5a. Si el nombre de usuario o la contrasena estan vacios, el sistema informa que se deben completar los campos y finaliza el caso de uso.
7a. Si ya existe un usuario con el mismo nombre de usuario, el sistema registra el fallo en la bitacora, informa que el nombre de usuario ya existe y finaliza el caso de uso.
12a. Si ocurre una excepcion tecnica durante el alta, el sistema informa el error de registro y finaliza el caso de uso.

**Postcondiciones:** El nuevo usuario queda almacenado y activo, con la contrasena hasheada, el perfil basico asignado, la version inicial en su historial, los digitos verificadores (DVH/DVV) actualizados y la operacion registrada en la bitacora. El usuario regresa a la pantalla de login.

---

**ID y Nombre:** CU03 Logout  
**Estado:** Implementado  
**Descripcion:** Permite al usuario cerrar su sesion activa, limpiando sus permisos efectivos y regresando a la pantalla de login.  
**Actor Principal:** Usuario  
**Actor Secundario:** -  
**Precondiciones:** El usuario tiene una sesion iniciada y se encuentra en la pantalla de gestion de usuarios.  
**Puntos de Extension:** Al cerrarse la pantalla de gestion, el sistema reejecuta el cierre de sesion por seguridad (evento FormClosed).  
**Condicion:** El usuario desea terminar su sesion en el sistema.  

**Escenario Principal:**
1. El usuario presiona el boton de cierre de sesion en la pantalla de gestion de usuarios.
2. El sistema solicita confirmacion para cerrar la sesion.
3. El usuario confirma el cierre de sesion.
4. El sistema limpia los permisos efectivos de la sesion.
5. El sistema cierra la sesion del usuario.
6. El sistema cierra la pantalla de gestion de usuarios y regresa a la pantalla de login.

**Flujos Alternativos:**
3a. Si el usuario no confirma, el sistema mantiene la sesion abierta y permanece en la pantalla de gestion de usuarios.

**Postcondiciones:** La sesion del usuario queda cerrada y sin permisos efectivos, y la pantalla de login queda visible.

## T04. Gestion de Perfiles de Usuario

_Especificaciones de casos de uso_

Casos de uso de administracion de permisos (patron Composite: familias y patentes), asignacion de perfiles a usuarios y control de acceso, segun la implementacion vigente en PermisoBLL, PermisoService y AutorizacionService.

---

**ID y Nombre:** CU04 Gestionar permisos  
**Estado:** Implementado  
**Descripcion:** Permite al administrador definir y mantener el catalogo de permisos del sistema, organizado como familias (grupos) y patentes (permisos simples) mediante una estructura Composite.  
**Actor Principal:** Administrador  
**Actor Secundario:** -  
**Precondiciones:** El administrador tiene una sesion iniciada y cuenta con el permiso de gestion de permisos.  
**Puntos de Extension:** El catalogo definido aqui es la base de la asignacion de perfiles (CU02) y del control de acceso (CU03).  
**Condicion:** El administrador necesita crear o modificar las familias y patentes que componen el catalogo de permisos.  

**Escenario Principal:**
1. El administrador abre la pantalla de gestion de permisos.
2. El sistema carga el catalogo de permisos y lo muestra como un arbol de familias y patentes.
3. El administrador crea familias nuevas y agrega familias o patentes hijas a una familia (composicion).
4. El administrador presiona guardar.
5. El sistema valida que el catalogo no este vacio.
6. El sistema valida que no existan ciclos entre las familias.
7. El sistema valida cada nombre (no vacio y sin punto), lo normaliza y verifica que no haya nombres duplicados en un mismo nivel.
8. El sistema valida que ninguna familia quede vacia.
9. El sistema guarda los nodos, reemplaza las relaciones padre-hijo y elimina los permisos que ya no forman parte del catalogo.
10. El sistema registra la operacion en la bitacora y recarga el catalogo actualizado.

**Flujos Alternativos:**
5a. Si el catalogo esta vacio, el sistema registra el fallo en la bitacora, informa el error y finaliza el caso de uso.
6a. Si se detecta un ciclo entre familias, el sistema informa el error y finaliza el caso de uso.
7a. Si un nombre esta vacio o contiene un punto, el sistema informa el error y finaliza el caso de uso.
7b. Si hay nombres duplicados en un mismo nivel, el sistema informa el error y finaliza el caso de uso.
8a. Si una familia queda sin hijos, el sistema informa que no se permiten grupos vacios y finaliza el caso de uso.
9a. Si ocurre una excepcion tecnica durante el guardado, el sistema informa el error y finaliza el caso de uso.

**Postcondiciones:** El catalogo de permisos queda persistido (nodos, relaciones de composicion y eliminaciones) y la operacion queda registrada en la bitacora.

---

**ID y Nombre:** CU05 Asignar perfil(es) a usuario  
**Estado:** Implementado  
**Descripcion:** Permite al administrador asignar o quitar permisos (familias y patentes) a un usuario, definiendo su perfil de acceso.  
**Actor Principal:** Administrador  
**Actor Secundario:** -  
**Precondiciones:** El administrador tiene una sesion iniciada con el permiso de gestion de usuarios, existe al menos un usuario y el catalogo de permisos (CU01) esta definido.  
**Puntos de Extension:** Los permisos asignables provienen del catalogo gestionado en CU01 Gestionar permisos.  
**Condicion:** El administrador necesita definir que permisos tiene un usuario.  

**Escenario Principal:**
1. El administrador selecciona un usuario en la grilla de usuarios.
2. El sistema carga el catalogo de permisos y marca los que el usuario ya tiene asignados.
3. El administrador marca o desmarca permisos en el arbol.
4. El administrador presiona guardar los permisos del usuario.
5. El sistema verifica que el usuario exista.
6. El sistema reemplaza los permisos del usuario por la seleccion, sin duplicados.
7. El sistema guarda una version en el historial del usuario y registra la operacion en la bitacora.
8. Si el usuario modificado es el de la sesion actual, el sistema actualiza sus permisos efectivos.
9. El sistema confirma que los permisos del usuario fueron guardados.

**Flujos Alternativos:**
4a. Si no hay ningun usuario seleccionado, el sistema informa que se debe seleccionar un usuario y finaliza el caso de uso.
5a. Si el usuario no existe, el sistema informa el error y finaliza el caso de uso.
7a. Si ocurre una excepcion tecnica durante el guardado, el sistema informa el error y finaliza el caso de uso.

**Postcondiciones:** Los permisos del usuario quedan actualizados, con una version registrada en su historial y en la bitacora. Si es el usuario de la sesion actual, sus permisos efectivos quedan refrescados.

---

**ID y Nombre:** CU06 Controlar acceso de usuario  
**Estado:** Implementado  
**Descripcion:** El sistema habilita o restringe las acciones y pantallas disponibles para el usuario segun sus permisos efectivos, expandiendo las familias hasta sus patentes.  
**Actor Principal:** Usuario  
**Actor Secundario:** -  
**Precondiciones:** El usuario tiene una sesion iniciada y sus permisos efectivos fueron cargados durante el login.  
**Puntos de Extension:** Los permisos efectivos se derivan del catalogo (CU01) y de la asignacion al usuario (CU02).  
**Condicion:** El usuario abre una pantalla con acciones protegidas o intenta ejecutar una accion protegida.  

**Escenario Principal:**
1. El usuario abre una pantalla que contiene acciones protegidas.
2. El sistema, para cada accion de la pantalla, consulta si el usuario tiene el permiso requerido.
3. El sistema calcula los permisos efectivos expandiendo las familias del usuario hasta sus patentes (hojas del Composite).
4. El sistema habilita o deshabilita cada control segun el usuario tenga o no el permiso.
5. El usuario ejecuta una accion protegida.
6. El sistema vuelve a validar el permiso correspondiente a esa accion.
7. El sistema ejecuta la accion solicitada.

**Flujos Alternativos:**
6a. Si el usuario no tiene el permiso requerido para la accion, el sistema informa que no esta permitido y no ejecuta la accion.

**Postcondiciones:** El usuario solo puede visualizar y ejecutar las acciones para las que posee permiso efectivo.

## T05. Gestion de Multiples Idiomas

_Especificaciones de casos de uso_

Casos de uso de administracion de idiomas y traducciones, y de configuracion del idioma preferido del usuario, segun la implementacion vigente en IdiomaBLL, IdiomaCatalogoService y el patron Observer (IdiomaService).

---

**ID y Nombre:** CU07 Agregar idioma  
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

**ID y Nombre:** CU08 Actualizar idioma  
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

**ID y Nombre:** CU09 Eliminar idioma  
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

**ID y Nombre:** CU10 Importar JSON de traducciones  
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

**ID y Nombre:** CU11 Exportar JSON de traducciones  
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

**ID y Nombre:** CU12 Editar traduccion  
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

**ID y Nombre:** CU13 Configurar idioma preferido  
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

## T06a. Gestion de Bitacora

_Especificaciones de casos de uso_

A continuacion se especifican los casos de uso de la bitacora del sistema segun la implementacion vigente: la consulta filtrada de eventos y el registro centralizado de operaciones, fallos de negocio y excepciones tecnicas.

---

**ID y Nombre:** CU14 Consultar bitacora  
**Estado:** Implementado  
**Descripcion:** Permite al administrador consultar los eventos registrados en la bitacora aplicando criterios de filtrado por fecha, usuario, actividad, nivel y tipo tecnico.  
**Actor Principal:** Administrador  
**Actor Secundario:** -  
**Precondiciones:** La aplicacion se encuentra iniciada, hay una sesion activa y la pantalla de bitacora es accesible desde la gestion de usuarios.  
**Puntos de Extension:** No aplica.  
**Condicion:** El administrador necesita auditar las operaciones y errores registrados por el sistema.  

**Escenario Principal:**
1. El administrador abre la pantalla de bitacora desde la gestion de usuarios.
2. El sistema carga los niveles de severidad disponibles y muestra los registros existentes ordenados por fecha descendente.
3. El administrador define los criterios de filtro: rango de fechas, usuario, actividad, nivel y si son solo eventos tecnicos.
4. El administrador presiona el boton de busqueda.
5. El sistema arma el filtro con los criterios seleccionados.
6. El sistema consulta los eventos de bitacora que cumplen el filtro.
7. El sistema aplica el filtro adicional por actividad sobre el texto traducido.
8. El sistema muestra los eventos resultantes en la grilla, traduciendo actividad, informacion y nivel.

**Flujos Alternativos:**
6a. Si ocurre una excepcion durante la consulta, el sistema muestra el error mediante el manejador de excepciones y finaliza el caso de uso.
8a. Si ningun evento cumple el filtro, la grilla se muestra vacia.

**Postcondiciones:** La grilla presenta los eventos de bitacora que cumplen los criterios seleccionados. El caso de uso es de solo lectura y no modifica el estado del sistema.

---

**ID y Nombre:** CU15 Registrar en bitacora  
**Estado:** Implementado  
**Descripcion:** Registra de forma centralizada un evento en la bitacora, clasificado como operacion exitosa, fallo de negocio o excepcion tecnica, generado por las operaciones auditables del sistema.  
**Actor Principal:** Sistema  
**Actor Secundario:** -  
**Precondiciones:** La bitacora fue configurada durante el arranque con la fabrica de unidades de trabajo y los proveedores de identidad de la sesion.  
**Puntos de Extension:** Incluido por los casos de uso que ejecutan operaciones auditables (Login, Registro, Alta/Edicion/Baja de usuario, Restauracion de version, entre otros).  
**Condicion:** Una operacion de negocio auditable finaliza con exito, con un fallo de negocio o con una excepcion tecnica y debe dejar traza.  

**Escenario Principal:**
1. Una operacion de la capa de negocio invoca el registro de un evento indicando actividad, informacion y origen.
2. El sistema determina el nivel del evento segun el tipo de registro: operacion, fallo de negocio o excepcion tecnica.
3. El sistema obtiene el identificador y el nombre del usuario de la sesion actual.
4. El sistema construye el evento de bitacora con fecha y hora actual, identidad, actividad, informacion, nivel, origen y marca tecnica.
5. El sistema persiste el evento, participando de la unidad de trabajo de la operacion cuando se proporciona una; si no, usa su propia unidad de trabajo.
6. El sistema devuelve el identificador del evento registrado.

**Flujos Alternativos:**
1a. Si la bitacora no fue configurada, el sistema lanza una excepcion de operacion invalida.
3a. Si no hay sesion iniciada, la identidad del usuario queda nula y el evento se registra igual.
5a. Si el registro corresponde a una excepcion tecnica, la informacion se compone con el tipo y el mensaje de la excepcion y el nivel es Error.

**Postcondiciones:** El evento queda persistido en la tabla Bitacora, disponible para su consulta posterior.

## T06b. Gestion de Control de Cambios

_Especificaciones de casos de uso_

A continuacion se especifican los casos de uso del control de cambios (historial de estados de usuario) segun la implementacion vigente: la consulta del historial de versiones con sus diferencias y la restauracion de un usuario a una version historica.

---

**ID y Nombre:** CU16 Consultar historial de usuario  
**Estado:** Implementado  
**Descripcion:** Permite al administrador ver las versiones historicas de un usuario y las diferencias entre una version seleccionada y su version anterior.  
**Actor Principal:** Administrador  
**Actor Secundario:** -  
**Precondiciones:** La aplicacion se encuentra iniciada, hay una sesion activa y se selecciono un usuario en la gestion de usuarios para abrir su historial.  
**Puntos de Extension:** Al seleccionar una version se dispara el calculo y la visualizacion de las diferencias contra la version anterior.  
**Condicion:** El administrador necesita revisar la evolucion del estado de un usuario a lo largo del tiempo.  

**Escenario Principal:**
1. El administrador abre la pantalla de historial de un usuario desde la gestion de usuarios.
2. El sistema obtiene las versiones historicas del usuario ordenadas por fecha descendente.
3. El sistema muestra las versiones en la grilla de historial.
4. El administrador selecciona una version del historial.
5. El sistema obtiene la version seleccionada y su version anterior.
6. El sistema compara ambas versiones campo a campo: usuario, contrasena, activo, idioma y permisos.
7. El sistema muestra las diferencias detectadas en la grilla de cambios de estado.

**Flujos Alternativos:**
4a. Si no hay ninguna version seleccionada, el sistema limpia la grilla de cambios de estado.
6a. Si la version seleccionada es la mas antigua, se compara contra un estado anterior nulo y todos los campos figuran como alta inicial.
6b. Si no hay diferencias entre ambas versiones, el sistema muestra una fila indicando que no hubo cambios.
5a. Si ocurre una excepcion durante la consulta, el sistema muestra el error mediante el manejador de excepciones y finaliza el caso de uso.

**Postcondiciones:** Las grillas presentan el historial de versiones y las diferencias de la version seleccionada. El caso de uso es de solo lectura y no modifica el estado del sistema.

---

**ID y Nombre:** CU17 Restaurar version de usuario  
**Estado:** Implementado  
**Descripcion:** Restaura el estado de un usuario a una version historica seleccionada, reescribiendo sus datos, permisos y preferencia de idioma, recalculando su integridad y dejando traza en historial y bitacora.  
**Actor Principal:** Administrador  
**Actor Secundario:** -  
**Precondiciones:** La aplicacion se encuentra iniciada, hay una sesion activa, existe historial para el usuario y se selecciono una version a restaurar.  
**Puntos de Extension:** Si el usuario restaurado es el de la sesion actual, se refrescan nombre, permisos e idioma de la sesion.  
**Condicion:** El administrador necesita revertir el estado de un usuario a una version anterior.  

**Escenario Principal:**
1. El administrador selecciona una version del historial y presiona restaurar.
2. El sistema solicita confirmacion de la restauracion.
3. El administrador confirma la operacion.
4. El sistema obtiene el snapshot de la version a restaurar.
5. El sistema abre una transaccion y obtiene el usuario actual.
6. El sistema aplica el snapshot: reescribe los datos del usuario, reemplaza sus permisos y su preferencia de idioma.
7. El sistema recalcula el digito verificador horizontal del usuario restaurado y actualiza el digito verificador vertical de la tabla.
8. El sistema guarda una nueva version de historial que registra la restauracion y deja traza en la bitacora.
9. El sistema confirma la transaccion.
10. Si el usuario restaurado es el de la sesion actual, el sistema refresca nombre, permisos e idioma de la sesion.
11. El sistema informa que la version fue restaurada y refresca las grillas.

**Flujos Alternativos:**
1a. Si no hay ninguna version seleccionada, el sistema advierte que debe seleccionar una version y finaliza el caso de uso.
3a. Si el administrador no confirma, el caso de uso finaliza sin cambios.
4a. Si la version no existe, el sistema lanza una excepcion de negocio y finaliza.
4b. Si el usuario de la sesion actual intenta restaurar una version inactiva de si mismo, el sistema rechaza la operacion.
5a. Si el usuario del snapshot ya no existe, el sistema lanza una excepcion de negocio y revierte la transaccion.
9a. Si ocurre una excepcion durante la restauracion, el sistema revierte la transaccion y propaga el error.

**Postcondiciones:** El usuario queda con el estado (datos, permisos e idioma) de la version restaurada, se registra una nueva version de historial y un evento de bitacora, y la integridad del usuario queda recalculada.

## T07. Gestion de Digitos Verificadores

_Especificaciones de casos de uso_

Caso de uso de verificacion de integridad de los datos mediante digitos verificadores horizontal (DVH) y vertical (DVV), ejecutado durante el arranque de la aplicacion, segun la implementacion vigente en IntegridadBLL, IntegridadService y el arranque en Program.

---

**ID y Nombre:** CU18 Verificar integridad de datos  
**Estado:** Implementado  
**Descripcion:** Al iniciar la aplicacion, el sistema verifica la integridad de los datos comparando los digitos verificadores recalculados contra los almacenados, e impide el uso normal si detecta corrupcion.  
**Actor Principal:** Usuario  
**Actor Secundario:** -  
**Precondiciones:** La aplicacion se esta iniciando y existe la base de datos con la tabla de usuarios y la de digitos verificadores verticales.  
**Puntos de Extension:** Incluye el registro en bitacora (CU02 de T06a) para la inicializacion de firmas y para los fallos de integridad.  
**Condicion:** El arranque de la aplicacion dispara la verificacion de integridad de los datos.  

**Escenario Principal:**
1. El usuario inicia la aplicacion.
2. El sistema carga el idioma predeterminado y comienza la verificacion de integridad de los datos.
3. El sistema obtiene todos los usuarios y el digito verificador vertical (DVV) almacenado de la tabla de usuarios.
4. El sistema verifica, para cada usuario, que su digito verificador horizontal (DVH) recalculado coincida con el almacenado.
5. El sistema verifica que el DVV recalculado del conjunto de usuarios coincida con el DVV almacenado.
6. El sistema confirma la verificacion y abre la pantalla de login.

**Flujos Alternativos:**
3a. Si la tabla aun no tiene firmas inicializadas (DVV nulo y todos los DVH vacios), el sistema calcula y guarda el DVH de cada usuario y el DVV del conjunto, registra la inicializacion en la bitacora y continua con el arranque normal.
4a. Si el DVH recalculado de un usuario no coincide con el almacenado (corrupcion horizontal), el sistema registra el fallo en la bitacora, lanza una excepcion de integridad y abre la pantalla de contingencia de integridad.
5a. Si el DVV recalculado no coincide con el almacenado (corrupcion vertical), el sistema registra el fallo en la bitacora, lanza una excepcion de integridad y abre la pantalla de contingencia de integridad.
6a. Si ocurre otra excepcion tecnica durante el arranque, el sistema registra la excepcion tecnica y no continua con el inicio normal.

**Postcondiciones:** Si la integridad es valida (o se inicializan las firmas por primera vez), la aplicacion continua hacia la pantalla de login. Si se detecta corrupcion, se muestra la pantalla de contingencia de integridad y no se habilita el uso normal del sistema.
