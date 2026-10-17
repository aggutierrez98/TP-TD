# Spec Delta

## Purpose
Define la administracion del catalogo global de categorias de producto desde su propia pantalla: alta, edicion, baja bloqueada por uso, nombre unico y acceso por menu y permiso.

## ADDED Requirements

### Requirement: Pantalla propia de categorias de producto
El sistema SHALL ofrecer una pantalla Categorias de producto, independiente del detalle de cualquier comercio, que liste todas las categorias de producto existentes. Una categoria de producto MUST NOT pertenecer a ningun comercio.

#### Scenario: Listado de categorias
- **WHEN** el operador abre la pantalla Categorias de producto
- **THEN** ve todas las categorias de producto existentes, ordenadas por nombre

### Requirement: Alta de categoria de producto
La pantalla SHALL permitir dar de alta una categoria con un nombre. El nombre MUST ser obligatorio (se normaliza quitando espacios al inicio y al final) y MUST NOT repetir el nombre de otra categoria, sin distinguir mayusculas de minusculas.

#### Scenario: Alta valida
- **WHEN** el operador ingresa un nombre que no existe y confirma el alta
- **THEN** la categoria aparece en el listado y se registra la operacion en la bitacora

#### Scenario: Nombre vacio
- **WHEN** el operador intenta dar de alta una categoria con el nombre vacio o solo espacios
- **THEN** el sistema rechaza el alta con un mensaje de nombre obligatorio y no crea la categoria

#### Scenario: Nombre repetido
- **WHEN** el operador intenta dar de alta una categoria cuyo nombre ya usa otra categoria
- **THEN** el sistema rechaza el alta con un mensaje de nombre duplicado y no crea la categoria

### Requirement: Edicion de categoria de producto
La pantalla SHALL permitir modificar el nombre de una categoria existente, con las mismas validaciones que el alta. Renombrar una categoria con el mismo nombre que ya tiene MUST estar permitido.

#### Scenario: Edicion valida
- **WHEN** el operador selecciona una categoria, cambia su nombre a uno libre y confirma
- **THEN** el listado muestra el nombre nuevo y se registra la operacion en la bitacora

#### Scenario: Edicion a un nombre ya usado
- **WHEN** el operador cambia el nombre de una categoria al de otra categoria existente
- **THEN** el sistema rechaza el cambio con el mensaje de nombre duplicado y la categoria conserva su nombre

### Requirement: Baja de categoria bloqueada si esta en uso
La pantalla SHALL permitir dar de baja una categoria que ningun producto ni promocion usa. El sistema MUST rechazar la baja de una categoria usada por algun producto o por alguna promocion, informando con un mensaje claro que la categoria esta en uso, y la categoria MUST permanecer en el catalogo.

#### Scenario: Baja de una categoria sin uso
- **WHEN** el operador confirma la baja de una categoria que ningun producto ni promocion usa
- **THEN** la categoria deja de aparecer en el listado y se registra la operacion en la bitacora

#### Scenario: Baja de una categoria con productos
- **WHEN** el operador intenta dar de baja una categoria a la que pertenece al menos un producto
- **THEN** el sistema muestra un mensaje indicando que la categoria esta en uso y no la elimina

### Requirement: Acceso desde el menu de Operador
La pantalla Categorias de producto SHALL abrirse desde un submenu dentro del menu Operador, junto al submenu Productos. El acceso MUST requerir el permiso `gestionproductos`, que ya esta concedido por defecto a los grupos Operador y admin; el sistema MUST NOT introducir un permiso nuevo para esta pantalla.

#### Scenario: Usuario con permiso
- **WHEN** un usuario del grupo Operador o admin abre el menu Operador
- **THEN** el submenu Categorias de producto esta habilitado y abre la pantalla

#### Scenario: Usuario sin permiso
- **WHEN** un usuario sin el permiso `gestionproductos` abre el menu
- **THEN** el submenu Categorias de producto aparece deshabilitado

### Requirement: El detalle del comercio no administra categorias
El detalle de un comercio MUST NOT permitir dar de alta, editar ni dar de baja categorias de producto. La administracion de categorias SHALL hacerse unicamente desde la pantalla Categorias de producto.

#### Scenario: Detalle sin administracion de categorias
- **WHEN** el operador abre el detalle de un comercio
- **THEN** el detalle muestra los productos asignados y las promociones del comercio y no ofrece controles para crear, editar ni eliminar categorias

### Requirement: Textos traducibles
Todos los textos de la pantalla Categorias de producto y sus mensajes SHALL estar disponibles en los idiomas del sistema mediante el catalogo de traducciones.

#### Scenario: Cambio de idioma
- **WHEN** el usuario cambia el idioma de la aplicacion
- **THEN** la pantalla muestra sus textos, encabezados de grilla y mensajes en el idioma elegido
