# Spec Delta

## Purpose
Define la pantalla de Operador para buscar, dar de alta, editar y dar de baja promociones, con sus validaciones, el bloqueo de baja y el acceso por menu y permiso.

## ADDED Requirements

### Requirement: Buscador de promociones por texto y por comercio
La pantalla Gestion de Promociones SHALL listar las promociones en una grilla con descripcion, descuento, tope, vigencia y dias, y SHALL ofrecer un cuadro de texto y un combo con los comercios existentes para filtrar el listado. El texto MUST buscar sobre la descripcion sin distinguir mayusculas ni acentos. El combo MUST incluir una opcion "Todos". Ambos filtros se combinan con Y.

#### Scenario: Buscar por texto
- **WHEN** el operador escribe "martes" y presiona Buscar
- **THEN** la grilla muestra solo las promociones cuya descripcion contiene "martes"

#### Scenario: Buscar por comercio
- **WHEN** el operador elige un comercio en el combo y presiona Buscar
- **THEN** la grilla muestra las promociones que le aplican a ese comercio segun la regla de alcance

#### Scenario: Sin filtros
- **WHEN** el operador deja el texto vacio y elige "Todos"
- **THEN** la grilla muestra todas las promociones

### Requirement: Alta de promocion
El operador SHALL poder dar de alta una promocion desde el editor de la pantalla. La descripcion MUST ser obligatoria. El descuento MUST ser mayor que 0 y menor o igual que 100. El tope, si se carga, MUST ser mayor que 0, y vacio significa sin tope. La vigencia desde MUST ser menor o igual que la vigencia hasta. Los filtros son opcionales.

#### Scenario: Alta valida
- **WHEN** el operador completa descripcion, descuento y vigencia validos, elige algunos filtros y presiona Agregar
- **THEN** la promocion aparece en el listado con sus filtros y se registra la operacion en la bitacora

#### Scenario: Datos invalidos
- **WHEN** el operador intenta guardar con descripcion vacia, descuento fuera de rango, tope no positivo o vigencia desde posterior a hasta
- **THEN** el sistema rechaza el alta con un mensaje especifico y no crea la promocion

### Requirement: Edicion de promocion
El operador SHALL poder seleccionar una promocion del listado, ver todos sus datos y filtros cargados en el editor, modificarlos y guardar con las mismas validaciones que el alta.

#### Scenario: Editar filtros
- **WHEN** el operador selecciona una promocion, cambia su tope y sus categorias excluidas y presiona Actualizar
- **THEN** la grilla y el editor muestran los datos actualizados y se registra la operacion en la bitacora

### Requirement: Baja de promocion
El operador SHALL poder dar de baja una promocion tras confirmarlo. Al eliminarla se MUST eliminar tambien sus filtros. El sistema MUST rechazar la baja de una promocion referenciada por una notificacion o recordatorio, con un mensaje claro de que esta en uso, y la promocion MUST permanecer.

#### Scenario: Baja sin uso
- **WHEN** el operador confirma la baja de una promocion que ninguna notificacion referencia
- **THEN** la promocion y sus filtros dejan de existir y se registra la operacion en la bitacora

#### Scenario: Baja de una promocion referenciada
- **WHEN** el operador intenta eliminar una promocion referenciada por una notificacion
- **THEN** el sistema muestra un mensaje de promocion en uso y no la elimina

### Requirement: Un producto en uso por una promocion no se puede eliminar
La baja de un producto del catalogo MUST rechazarse tambien cuando alguna promocion lo incluye o lo excluye, con el mismo mensaje de producto en uso.

#### Scenario: Producto usado por una promocion
- **WHEN** el operador intenta eliminar un producto que una promocion incluye o excluye
- **THEN** el sistema rechaza la baja indicando que el producto esta en uso

### Requirement: Acceso desde el menu de Operador
La pantalla SHALL abrirse desde un submenu dentro del menu Operador. El acceso MUST requerir el permiso `gestionpromociones`, concedido por defecto a los grupos Operador y admin.

#### Scenario: Usuario con permiso
- **WHEN** un usuario Operador o admin abre el menu Operador
- **THEN** el submenu Promociones esta habilitado y abre la pantalla

#### Scenario: Usuario sin permiso
- **WHEN** un usuario sin el permiso abre el menu
- **THEN** el submenu Promociones aparece deshabilitado

### Requirement: Textos traducibles
Todos los textos de la pantalla, sus encabezados de grilla y sus mensajes SHALL estar disponibles en los idiomas del sistema mediante el catalogo de traducciones.

#### Scenario: Cambio de idioma
- **WHEN** el usuario cambia el idioma de la aplicacion
- **THEN** la pantalla de promociones muestra todos sus textos en el idioma elegido
