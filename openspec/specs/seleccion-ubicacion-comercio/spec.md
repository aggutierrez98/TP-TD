# seleccion-ubicacion-comercio Specification

## Purpose
Permitir seleccionar la ubicacion geografica de un comercio explorando el mapa mediante controles visibles y gestos de mouse.

## Requirements

### Requirement: Zoom mediante botones visibles
El selector SHALL mostrar botones + y − para variar el zoom un nivel por clic, manteniendo el centro de la vista y respetando los limites minimo y maximo.

#### Scenario: Acercar y alejar
- **WHEN** el usuario pulsa + o − en un nivel intermedio
- **THEN** la vista se acerca o aleja un nivel sin cambiar la ubicacion seleccionada.

#### Scenario: Limites de zoom
- **WHEN** se alcanza el zoom minimo o maximo
- **THEN** el boton que excederia ese limite queda deshabilitado, tambien si se llego al limite con la rueda.

### Requirement: Cruceta de navegacion
El selector SHALL mostrar una cruceta con cuatro botones que desplacen la vista hacia el norte, sur, oeste o este segun la flecha pulsada. Cada clic SHALL desplazar el centro 48 pixeles del mapa en esa direccion, conservando la ubicacion seleccionada y respetando los limites geograficos.

#### Scenario: Navegar en las cuatro direcciones
- **WHEN** el usuario pulsa arriba, abajo, izquierda o derecha
- **THEN** el centro de la vista avanza en esa direccion y el marcador mantiene sus coordenadas.

#### Scenario: Desplazamiento corto independiente del tamano de ventana
- **WHEN** el usuario pulsa una flecha al mismo zoom antes y despues de ampliar el selector
- **THEN** cada clic mantiene el paso de 48 pixeles del mapa y no crece hasta un cuarto del ancho o alto de la vista.

#### Scenario: Navegar antes de seleccionar
- **WHEN** el usuario mueve la vista sin haber colocado un marcador
- **THEN** no se crea una seleccion y Aceptar permanece deshabilitado.

#### Scenario: Extremos geograficos
- **WHEN** el desplazamiento alcanza un limite geografico del mapa
- **THEN** la vista mantiene coordenadas validas sin producir errores.

### Requirement: Instrucciones y gestos del mapa
El selector SHALL explicar el desplazamiento con boton derecho, el zoom con rueda, la seleccion con clic izquierdo y el arrastre del marcador. Estos gestos SHALL seguir disponibles junto con los botones.

#### Scenario: Descubrir la navegacion
- **WHEN** se abre o redimensiona el selector
- **THEN** las instrucciones y los seis botones son visibles y utilizables sin tapar la atribucion del mapa ni Aceptar y Cancelar.

#### Scenario: Seleccionar tras navegar
- **WHEN** el usuario explora otra zona y hace clic izquierdo en el mapa
- **THEN** el marcador se coloca en ese punto y Aceptar devuelve sus coordenadas.

#### Scenario: Ajustar el marcador
- **WHEN** el usuario arrastra el marcador con boton izquierdo
- **THEN** se ajusta su ubicacion sin desplazar simultaneamente la vista y despues puede volver a navegar.

### Requirement: Controles flotantes sobre el mapa
El selector SHALL mostrar la cruceta y una pareja vertical +/− junto a ella sobre la esquina inferior derecha del mapa, como controles flotantes compactos sin reservar espacio lateral. Los controles SHALL conservar su margen a los bordes inferior y derecho al redimensionar y no transmitir sus clics como seleccion de ubicacion.

#### Scenario: Distribucion y redimensionado
- **WHEN** se abre o redimensiona el selector
- **THEN** la cruceta y el zoom permanecen sobre la esquina inferior derecha, con margen a los bordes, visibles y sin tapar la atribucion, mientras las instrucciones quedan fuera del mapa.

#### Scenario: Clic en un control flotante
- **WHEN** el usuario pulsa cualquiera de los seis botones, con o sin marcador previo
- **THEN** solo se ejecuta su accion de navegacion y no se crea ni se modifica la ubicacion seleccionada.

#### Scenario: Repintado durante la navegacion
- **WHEN** el usuario arrastra el mapa o cambia el zoom
- **THEN** los controles siguen visibles, legibles y utilizables sobre el mapa.

### Requirement: Estilos diferenciados de direccion y zoom
El selector SHALL distinguir visualmente los botones direccionales verdes y redondeados de los botones de zoom, con fondo blanco y signos oscuros en una pareja vertical. Los simbolos SHALL permanecer legibles y los estados de foco, hover, pulsado y deshabilitado SHALL ser distinguibles.

#### Scenario: Reconocer las acciones
- **WHEN** el usuario observa los controles flotantes
- **THEN** puede distinguir la cruceta de navegacion de la pareja +/− por su agrupacion y estilo visual.

#### Scenario: Estados de interaccion y limites
- **WHEN** el usuario enfoca, apunta o pulsa un boton, o alcanza un limite de zoom
- **THEN** el estado visual correspondiente es distinguible y el boton de zoom deshabilitado no ejecuta una accion.

### Requirement: Fondos semitransparentes sobre el mapa
Los fondos de los seis botones SHALL ser semitransparentes para mostrar parte del mapa real detras, manteniendo simbolos opacos y legibles. El fondo visible SHALL actualizarse junto con el mapa, sin mostrar una imagen anterior ni un color liso del formulario como sustituto.

#### Scenario: Ver detalles del mapa a traves de los botones
- **WHEN** un boton se encuentra sobre detalles cartograficos
- **THEN** esos detalles son parcialmente visibles a traves de su fondo y el simbolo del boton sigue siendo legible.

#### Scenario: Actualizar la composicion del fondo
- **WHEN** el usuario desplaza, amplifica o redimensiona el mapa, o se cargan nuevos tiles
- **THEN** el fondo de los botones refleja el mapa actual y no conserva detalles cartograficos de una vista anterior.
