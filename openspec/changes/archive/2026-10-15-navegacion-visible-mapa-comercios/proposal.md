# Proposal

## Why

El selector de ubicacion permite navegar con boton derecho y rueda, pero no lo informa y parece una imagen estatica. Los controles visibles permitiran explorar otras zonas y seleccionar comercios sin depender de descubrir esos gestos.

## What Changes

- Agregar botones visibles + y − para acercar y alejar.
- Agregar una cruceta con botones arriba, abajo, izquierda y derecha para desplazar la vista.
- Presentar la cruceta y el zoom como controles flotantes compactos sobre la esquina inferior derecha del mapa, sin reservar una franja lateral. Diferenciar la cruceta verde redondeada del zoom blanco con simbolos oscuros, organizado como pareja vertical junto a la cruceta.
- Usar fondos semitransparentes que permitan ver el mapa real detras de los botones, con simbolos opacos y legibles; actualizar el fondo al navegar y al cargar tiles.
- Reducir el desplazamiento de las flechas a 48 pixeles del mapa por clic, en lugar de un cuarto del area visible, para evitar saltos grandes al ampliar la ventana.
- Explicar el arrastre con boton derecho, el zoom con rueda, la seleccion con clic izquierdo y el ajuste del marcador.
- Mantener la ubicacion elegida al navegar y respetar los limites de zoom y coordenadas del mapa.
- Evitar que los clics sobre controles flotantes seleccionen puntos del mapa y mantener libre la atribucion de OpenStreetMap.
- Integrar controles Krypton, textos traducibles y disposicion redimensionable.

## Capabilities

### New Capabilities

- `seleccion-ubicacion-comercio`: seleccion geografica de comercios con navegacion visible y gestos de mouse.

### Modified Capabilities

Ninguna.

## Impact

`UI/FormSeleccionarUbicacionComercio.cs`, su designer y `translation-seeds/base.es.json`; catalogo base de traducciones segun el mecanismo vigente. Se reutiliza GMap.NET/OpenStreetMap, sin nuevas dependencias ni cambios de persistencia. Verificacion de build, suite unitaria/integracion y comprobacion visual del dialogo.
