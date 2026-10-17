# catalogo-productos Specification

## Purpose
Define el catalogo de productos como una entidad independiente de los comercios, su administracion (alta, edicion y baja) desde la pantalla Productos y las reglas de acceso y de baja asociadas.

## Requirements

### Requirement: El producto es independiente de los comercios
Un producto del catalogo SHALL estar definido unicamente por su nombre y su categoria de producto. El producto MUST NOT tener comercio, precio ni moneda propios.

#### Scenario: Producto sin comercio
- **WHEN** el operador da de alta un producto en la pantalla Productos
- **THEN** el producto queda en el catalogo con su nombre y categoria, sin asociarse a ningun comercio

### Requirement: Alta de producto
La pantalla Productos SHALL permitir dar de alta un producto con nombre y categoria de producto. El nombre MUST ser obligatorio (se normaliza quitando espacios al inicio y al final) y la categoria MUST existir.

#### Scenario: Alta valida
- **WHEN** el operador ingresa un nombre y elige una categoria existente y confirma el alta
- **THEN** el producto aparece en el listado de la pantalla Productos y se registra la operacion en la bitacora

#### Scenario: Nombre vacio
- **WHEN** el operador intenta dar de alta un producto con el nombre vacio o solo espacios
- **THEN** el sistema rechaza el alta con un mensaje de nombre obligatorio y no crea el producto

### Requirement: Edicion de producto
La pantalla Productos SHALL permitir modificar el nombre y la categoria de un producto existente, con las mismas validaciones que el alta.

#### Scenario: Edicion valida
- **WHEN** el operador selecciona un producto, cambia su nombre o categoria y confirma
- **THEN** el listado muestra los datos actualizados y se registra la operacion en la bitacora

### Requirement: Baja de producto bloqueada si esta en uso
La pantalla Productos SHALL permitir dar de baja un producto que ningun comercio vende. El sistema MUST rechazar la baja de un producto que algun comercio vende, informando con un mensaje claro que el producto esta en uso, y el producto MUST permanecer en el catalogo.

#### Scenario: Baja de un producto sin uso
- **WHEN** el operador confirma la baja de un producto que ningun comercio vende
- **THEN** el producto deja de aparecer en el catalogo y se registra la operacion en la bitacora

#### Scenario: Baja de un producto vendido por un comercio
- **WHEN** el operador intenta dar de baja un producto que al menos un comercio tiene asignado
- **THEN** el sistema muestra un mensaje indicando que el producto esta en uso y no lo elimina

### Requirement: Acceso desde el menu de Operador
La pantalla Productos SHALL abrirse desde un submenu dentro del menu Operador. El acceso MUST requerir el permiso `gestionproductos`, que SHALL estar concedido por defecto a los grupos Operador y admin.

#### Scenario: Usuario con permiso
- **WHEN** un usuario del grupo Operador o admin abre el menu Operador
- **THEN** el submenu Productos esta habilitado y abre la pantalla Productos

#### Scenario: Usuario sin permiso
- **WHEN** un usuario sin el permiso `gestionproductos` abre el menu
- **THEN** el submenu Productos aparece deshabilitado

### Requirement: Textos traducibles
Todos los textos de la pantalla Productos y sus mensajes SHALL estar disponibles en los idiomas del sistema mediante el catalogo de traducciones.

#### Scenario: Cambio de idioma
- **WHEN** el usuario cambia el idioma de la aplicacion
- **THEN** la pantalla Productos muestra sus textos, encabezados de grilla y mensajes en el idioma elegido
