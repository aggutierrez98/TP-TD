# Proposal

## Why

Para recomendar donde comprar (`recomendacion-compras-asistente`) el sistema necesita saber por donde se mueve el usuario. Hoy solo los comercios tienen coordenadas; el usuario no puede registrar sus lugares habituales (casa, trabajo) ni un radio de cercania.

## What Changes

- El menu "Sesion" pasa a llamarse "Mi usuario" y contiene Mi configuracion, Mis zonas (nuevo) y Cerrar sesion.
- Nueva pantalla "Mis zonas" (Krypton): a la izquierda la grilla de zonas del usuario (Nombre, Radio) y un panel de alta con nombre, busqueda de direccion por el proveedor de ubicaciones existente, radio y punto elegible con clic en el mapa, con botones Agregar y Quitar seleccionada; a la derecha un mapa con cada zona dibujada como circulo, marcador y etiqueta.
- Cada zona tiene su propio radio. Solo alta y baja; no hay edicion.
- Nueva tabla `ZonaUsuario`, siempre acotada al usuario de la sesion.
- Nuevo permiso para Mis zonas, asignado al grupo basico y a admin.
- Calculo de distancia geografica (Haversine) en codigo, sin servicios externos, para que lo consuma la recomendacion de compras.

## Capabilities

### New Capabilities
- `zonas-usuario`: registro, consulta y baja de las zonas habituales del usuario con su radio, su visualizacion en mapa, el acceso desde el menu "Mi usuario" y la distancia de un punto a las zonas.

### Modified Capabilities
Ninguna. La busqueda textual se reutiliza tal como la define `busqueda-ubicaciones`.

## Impact

- **BE/DAL/REPOSITORIO/BLL**: `ZonaUsuario`, `ZonaUsuarioDAL`, `IZonaUsuarioRepositorio`/`ZonaUsuarioRepositorio`, `ZonaUsuarioBLL` (alta en `CompositionRoot`) y el calculo de distancia.
- **UI**: `FormPrincipal` (menu renombrado, item nuevo y permiso), `FormMisZonas` nuevo con GMap.NET y la busqueda de `FormSeleccionarUbicacionComercio`.
- **SQL**: `script_bd.sql` (tabla, permiso y asignaciones) y las dos bases de desarrollo.
- **Traducciones**: seeds es/en y `Palabra`/`Traduccion` de ambas bases.
- **Tests**: unitarios de BLL y distancia, integracion de DAL/REPOSITORIO, UI FlaUI de la pantalla y ajuste de los tests que usan el menu Sesion.
- **Fuera de alcance**: edicion de zonas, zonas compartidas entre usuarios, deteccion automatica de ubicacion y radio global unico.
