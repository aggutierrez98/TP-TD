# Design

## Context

El dialogo usa GMap.NET WinForms, OpenStreetMap, zoom 2–18 y arrastre con boton derecho. El marcador se arrastra manualmente con el izquierdo, suspendiendo temporalmente CanDragMap. Ver proposal.md para la motivacion. El cambio pertenece a UI y es compatible con .NET Framework 4.7.2/C# 7.3.

## Goals / Non-Goals

**Goals:** separar navegacion de seleccion y hacer explicitos los gestos, con controles que acompanen el redimensionado.

**Non-Goals:** buscador de direcciones, geocoding, cambio de proveedor, nuevos servicios o cambios de persistencia.

## Decisions

1. **Controles flotantes sobre la esquina inferior derecha del mapa**. Conservar seis controles hijos del GMapControl, anclados Bottom/Right, en primer plano y con margen interior de referencia de 12 pixeles. La cruceta queda a la izquierda de una pareja vertical +/−, con separacion entre grupos. El mapa conserva todo su ancho y sus anclajes. Los botones mantienen Custom1, nombres y handlers; la personalizacion se aplica localmente, sin cambiar la paleta global de los formularios. Los eventos de los botones no se reenvian al mapa: pulsarlos no coloca ni arrastra marcadores. La atribucion de OpenStreetMap, abajo a la izquierda, permanece libre al tamano minimo y al ampliar.
2. **Flechas indican la direccion geografica de exploracion, con paso corto fijo**: arriba avanza al norte y derecha al este. Por clic, convertir un punto situado a 48 pixeles del centro en la direccion elegida mediante FromLocalToLatLng y asignarlo como Position. El paso se mide en pixeles del mapa, no en grados ni en una fraccion del ancho/alto. Asi, agrandar la ventana no aumenta el salto y el zoom sigue determinando la distancia geografica recorrida. Normalizar longitud y limitar latitud al rango del proveedor; no tocar el overlay. Se reemplaza el paso inicial de un cuarto de la vista, que el usuario encontro demasiado brusco.
3. **Zoom de un nivel con centro estable**: modificar Zoom limitado a MinZoom/MaxZoom. Sincronizar Enabled de +/− al cargar y en el evento de cambio de zoom de GMap.NET, para cubrir tambien la rueda. Frente a controles personalizados de renderizado, reutilizar botones estandar reduce complejidad.
4. **Instrucciones en varias lineas** encima y fuera del mapa, con espacio suficiente en el tamano minimo. Actualizar HintLabel y registrar textos accesibles/tooltips de navegacion con keys FormMapLocationPicker y sincronizar el seed base. Los simbolos visibles son +, − y flechas; los nombres accesibles describen la accion.
5. **Verificacion proporcional**: comprobar visualmente las direcciones, independencia del marcador, zoom por botones/rueda y redimensionado. Ejecutar build y suite unitaria/integracion obligatorios; la suite FlaUI completa requiere confirmacion explicita porque toma el escritorio. No agregar tests unitarios que solo reflejen asignaciones de propiedades de UI.
6. **Look & feel diferenciado**: cruceta con botones pequenos, verdes y redondeados; zoom en una pareja vertical de fondo blanco y signos oscuros, con mayor presencia visual de +/−. Usar los colores de FinanzappPaleta y StateCommon/estados de Krypton, manteniendo simbolos opacos. Conservar tooltips, nombres accesibles, foco de teclado y estados deshabilitados en los limites de zoom.
7. **Translucidez sobre el mapa real**: referencia inicial de 80% de opacidad para el fondo normal, con mayor opacidad en hover/pressed para reforzar la respuesta visual. Componer el color con alfa sobre el fondo pintado por el GMapControl padre y repintar los botones cuando se actualice la vista o lleguen tiles. Verificar el pipeline de pintura de Krypton en un render aislado antes de extenderlo a los seis botones: un Color con alfa que termina mezclado con el fondo del formulario no cumple. Si hace falta una adaptacion de pintura, mantenerla localizada en UI y basada en Krypton, reutilizando el fondo renderizado del mapa; evitar capturas del escritorio y bitmaps completos por boton. No agregar dependencias. Si el control no permite componer ese fondo con esta estrategia, consultar antes de ampliar el diseno o sustituirlo por botones opacos.

## Risks / Trade-offs

- [Los controles flotantes ocultan una pequena parte del mapa] → usar botones compactos en una sola esquina; permitir navegar para llevar un punto a una zona libre y mantener despejada la atribucion.
- [Repintado o eventos de mouse de controles superpuestos] → usar controles hijos en primer plano y comprobar su visibilidad durante pan/zoom, carga de tiles y redimensionado; los clics no llegan al mapa.
- [Alfa de Krypton mezclado con un fondo liso en vez del mapa] → comprobar composicion con detalles del mapa visibles detras, y actualizacion del fondo al navegar; no dar por cumplida la translucidez solo por configurar un color alfa.
- [Contraste insuficiente sobre tiles claros/oscuros] → simbolos opacos, borde legible y opacidad ajustable de los fondos; revisar ambos tipos de fondo y estados hover/pressed/disabled.
- [La flecha puede confundirse con el movimiento de la imagen] → nombres accesibles y tooltips indican explorar norte/sur/oeste/este.
- [Extremos de proyeccion y antimeridiano] → limitar/normalizar coordenadas y verificar esos bordes.
- [Textos largos o traducidos recortados] → instrucciones multilinea y revision al tamano minimo y al ampliar.

## Migration Plan

Distribuir el ejecutable y el seed de traducciones actualizado mediante el mecanismo existente. No hay migracion de datos; revertir el cambio restaura el dialogo anterior.
