# Spec Delta

## Purpose

Define como se comportan los formularios de FINAZAPP cuando el usuario cambia el tamaño de su ventana: que formularios se pueden redimensionar, hasta donde se pueden achicar y como se reacomoda su contenido para cubrir el area disponible.

## ADDED Requirements

### Requirement: Formularios redimensionables
Todo formulario de la aplicacion, salvo la ventana principal, SHALL poder redimensionarse arrastrando su borde, tanto si se abre dentro del area MDI como si se abre como dialogo.

#### Scenario: Pantalla MDI
- **WHEN** el usuario arrastra el borde de una pantalla abierta dentro de la ventana principal (por ejemplo, Gestion de movimientos)
- **THEN** la pantalla cambia de tamaño siguiendo el borde

#### Scenario: Dialogo
- **WHEN** el usuario arrastra el borde de un dialogo modal (por ejemplo, Detalle de comercio)
- **THEN** el dialogo cambia de tamaño siguiendo el borde

### Requirement: Tamaño minimo
Cada formulario SHALL tener un tamaño minimo igual a su tamaño de diseño. El usuario MUST NOT poder achicarlo por debajo de ese tamaño, de modo que ningun control quede superpuesto, recortado o fuera de la ventana.

#### Scenario: Intento de achicar por debajo del minimo
- **WHEN** el usuario arrastra el borde para dejar la ventana mas chica que su tamaño de diseño
- **THEN** la ventana se detiene en su tamaño minimo y todos sus controles siguen visibles y sin superponerse

### Requirement: Tamaño inicial sin cambios
Cada formulario SHALL abrirse con el mismo tamaño y la misma disposicion de controles que antes de este cambio. Las pantallas que hoy abren ocupando el area MDI (Balance general y Dashboard) SHALL seguir haciendolo.

#### Scenario: Apertura de una pantalla
- **WHEN** el usuario abre cualquier formulario
- **THEN** la ventana aparece con su tamaño de diseño y el contenido en la misma posicion que antes del cambio

#### Scenario: Pantallas que ocupan el area MDI
- **WHEN** el usuario abre Balance general o Dashboard
- **THEN** la pantalla ocupa el area disponible de la ventana principal y su contenido cubre esa area

### Requirement: Contenido que cubre la ventana
Al agrandar un formulario, su contenido SHALL redistribuirse para cubrir el area nueva. Los controles que muestran datos (grillas, arboles, listas, historial, mapa y area de tarjetas) SHALL absorber el espacio sobrante, sin dejar franjas vacias a la derecha ni abajo. En formularios sin controles de datos (solo campos), los campos SHALL ensancharse y los botones de accion SHALL acompañar el borde inferior.

#### Scenario: Grilla que crece
- **WHEN** el usuario agranda una pantalla de gestion con grilla (por ejemplo, Gestion de comercios)
- **THEN** la grilla crece en ancho y alto, sus columnas se reparten el ancho nuevo y no queda espacio vacio a la derecha ni debajo del contenido

#### Scenario: Pantalla con dos zonas de datos
- **WHEN** el usuario agranda una pantalla con dos zonas de datos (por ejemplo, Gestion de permisos con dos arboles, o Detalle de comercio con dos grillas)
- **THEN** ambas zonas crecen en proporcion y ninguna queda con su tamaño original

#### Scenario: Formulario solo de campos
- **WHEN** el usuario agranda un formulario sin controles de datos (por ejemplo, Configuracion de usuario)
- **THEN** los campos se ensanchan con la ventana y el boton de accion queda junto al borde inferior

#### Scenario: Ventana vuelve a achicarse
- **WHEN** el usuario agranda una pantalla y luego la vuelve a su tamaño de diseño
- **THEN** la disposicion de los controles vuelve a ser la original

### Requirement: Campos de entrada que se ensanchan
Los campos de texto, combos y selectores de fecha SHALL ensancharse junto con su grupo cuando el formulario se agranda. Si varios campos comparten una fila, SHALL repartirse el ancho nuevo en proporcion, sin superponerse.

#### Scenario: Fila con varios campos
- **WHEN** el usuario agranda una pantalla cuyo grupo de edicion tiene varios campos en la misma fila (por ejemplo, Categoria, Tipo, Monto y Fecha en Gestion de movimientos)
- **THEN** cada campo se ensancha y la fila sigue ordenada, sin campos superpuestos ni huecos desparejos entre ellos

### Requirement: Maximizar dentro de la ventana principal
Las pantallas abiertas dentro de la ventana principal SHALL tener boton de maximizar. Maximizar MUST ocupar el area de trabajo de la ventana principal conservando la barra de titulo propia de la pantalla (sin fusionarla con la ventana principal), igual que la apertura de Balance general y Dashboard. Los dialogos modales SHALL maximizar de forma normal.

#### Scenario: Maximizar una pantalla MDI
- **WHEN** el usuario presiona maximizar en una pantalla abierta dentro de la ventana principal
- **THEN** la pantalla ocupa toda el area de trabajo de la ventana principal, conserva su barra de titulo con sus botones y su contenido cubre esa area

#### Scenario: Restaurar
- **WHEN** el usuario vuelve a presionar el boton en una pantalla maximizada
- **THEN** la pantalla vuelve al tamaño y la posicion que tenia antes de maximizar

#### Scenario: Ventana principal redimensionada con una pantalla maximizada
- **WHEN** el usuario cambia el tamaño de la ventana principal mientras una pantalla esta maximizada
- **THEN** la pantalla se ajusta al area de trabajo nueva y sigue ocupandola completa

### Requirement: Login y Registro con ancho maximo
En Login y Registro el bloque de campos y botones SHALL ensancharse con la ventana hasta el doble de su ancho de diseño. Pasado ese ancho, el bloque MUST quedar fijo y centrado horizontalmente; en altura el bloque conserva su alto y SHALL quedar centrado verticalmente.

#### Scenario: Ventana un poco mas ancha
- **WHEN** el usuario agranda Login a un ancho menor que el doble del de diseño
- **THEN** el bloque de campos ocupa el ancho de la ventana (con sus margenes) y los campos se ensanchan

#### Scenario: Ventana mas del doble de ancha
- **WHEN** el usuario agranda Registro a mas del doble de su ancho de diseño
- **THEN** el bloque queda con el doble de su ancho original, centrado, y el espacio sobrante se reparte a ambos lados

### Requirement: Botones y textos de tamaño fijo
Botones, etiquetas, casillas y titulos SHALL conservar su tamaño y su tipografia al redimensionar; solo SHALL cambiar su posicion para acompañar al control o grupo al que pertenecen. El texto MUST NOT escalarse con el tamaño de la ventana.

#### Scenario: Botones de accion de un grupo
- **WHEN** el usuario agranda una pantalla con botones de accion debajo de un grupo de edicion
- **THEN** los botones mantienen su tamaño y se reubican junto al grupo, sin quedar flotando lejos de el

#### Scenario: Botones de un dialogo
- **WHEN** el usuario agranda un dialogo con botones Aceptar y Cancelar
- **THEN** los botones mantienen su tamaño y quedan en el borde inferior del dialogo

### Requirement: Compatibilidad con tests y traducciones
El cambio de layout MUST NOT modificar los `AutomationId` (nombres) de los controles ni sus `Tag` de traduccion, y la suite UI FlaUI completa SHALL quedar en verde al tamaño inicial de cada formulario.

#### Scenario: Suite UI tras el cambio
- **WHEN** se ejecuta la suite UI completa despues de aplicar el cambio
- **THEN** todos los tests pasan sin modificar como buscan los controles

#### Scenario: Cambio de idioma con ventana agrandada
- **WHEN** el usuario cambia el idioma con una pantalla agrandada abierta
- **THEN** los textos se traducen y la pantalla conserva la disposicion redimensionada
