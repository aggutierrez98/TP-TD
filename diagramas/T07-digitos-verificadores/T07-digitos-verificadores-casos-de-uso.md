# T07. Gestion de Digitos Verificadores

_Especificaciones de casos de uso_

Caso de uso de verificacion de integridad de los datos mediante digitos verificadores horizontal (DVH) y vertical (DVV), ejecutado durante el arranque de la aplicacion, segun la implementacion vigente en IntegridadBLL, IntegridadService y el arranque en Program.

---

**ID y Nombre:** CU01 Verificar integridad de datos  
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
