# Proposal

## Why

El selector de ubicacion de comercios exige recorrer el mapa manualmente aun cuando el usuario conoce una direccion o el nombre del lugar. Una busqueda textual permitira llegar a una ubicacion candidata, marcarla y ajustarla antes de confirmar.

## What Changes

- Agregar un campo unico «Direccion, comercio o lugar», accion Buscar por boton/Enter y selector de hasta cinco coincidencias descriptivas.
- Buscar direcciones y lugares/comercios registrados en OpenStreetMap mediante Nominatim, con preferencia por la zona visible sin restringir la busqueda a ella ni a un pais fijo.
- Al elegir explicitamente una coincidencia, centrar el mapa, ajustar el zoom y colocar o reemplazar el marcador; permitir su ajuste manual y confirmacion con Aceptar.
- Mantener la seleccion previa durante la consulta y ante ausencia de resultados, errores o cancelacion; ejecutar la consulta sin bloquear el formulario y descartar respuestas obsoletas.
- Respetar la politica de Nominatim: busqueda explicita sin autocompletado, identificacion y atribucion, limitacion de frecuencia, cache y endpoint configurable.
- Incorporar controles Krypton, ayudas y estados traducidos es/en, conservando el tamaño inicial y el comportamiento redimensionable.
- Implementar el acceso HTTP con bibliotecas de .NET Framework 4.7.2, sin nuevas dependencias NuGet ni cambios de persistencia.

## Capabilities

### New Capabilities

- `busqueda-ubicaciones`: consulta textual de direcciones y lugares, eleccion de coincidencias y aplicacion al selector geografico de comercios.

### Modified Capabilities

Ninguna. `seleccion-ubicacion-comercio` esta definida en el cambio completo pero aun no archivado `navegacion-visible-mapa-comercios`; este cambio agrega busqueda, preservando sus requisitos. `ventanas-redimensionables` y `tema-visual` se consumen sin modificar sus requisitos.

## Impact

- UI: `FormSeleccionarUbicacionComercio.cs` y designer; conexion desde `FormGestionComercios.cs`; configuracion no sensible en `UI/App.config`; seeds `translation-seeds/base.es.json` y `base.in.json`.
- Contratos tecnicos en ABSTRACCIONES, servicio Nominatim en SERVICIOS y composicion en `BLL/CompositionRoot.cs`; inclusiones de archivos en los `.csproj` correspondientes. No agregar referencias entre capas ni trasladar GMap.NET al servicio.
- API externa: HTTPS de Nominatim, sin API key en el endpoint publico; limites y cobertura condicionan resultados, sin garantia de todas las sucursales o de precision domiciliaria.
- Tests deterministas de servicio y formulario, comprobacion real acotada de conectividad, build y suite unitaria/integracion. La suite UI completa requiere confirmacion del usuario.
- Dependencia de entrega: el cambio de navegacion debe archivarse antes de archivar este cambio. Registrar la actualizacion de G07 y del CU de comercios en el cambio de diagramas activo una vez implementado.
