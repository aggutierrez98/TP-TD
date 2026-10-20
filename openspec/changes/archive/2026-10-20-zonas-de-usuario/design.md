# Design

## Context

Ver `proposal.md` (Why). Estado observado:

- `FormPrincipal.Designer` tiene `mnuSesion` (Tag `FormMainShell.Menu.Session`) con `mnuMiConfiguracion`, un separador y `mnuCerrarSesion`. Los permisos se resuelven con `TienePermiso` y constantes `Permiso*`.
- `FormSeleccionarUbicacionComercio` usa GMap.NET con `GMapProviders.OpenStreetMap`, overlays y `MouseClick`; la busqueda textual pasa por `CoordinadorBusquedaUbicacion` (UI) sobre `IBusquedaUbicacionesService` (Nominatim) de `CompositionRoot.BusquedaUbicaciones`.
- `CuentaFinancieraBLL` es el modelo de entidad propia: toma el usuario de `_sesion` en cada operacion y filtra por `UsuarioId`.
- `Comercio` guarda `Latitud`/`Longitud` como `DECIMAL(9,6) NULL`.

## Goals / Non-Goals

**Goals:**
- Persistir zonas por usuario con radio propio y exponer la distancia a ellas para el change de compras.

**Non-Goals:**
- Edicion de zonas (decision del usuario: solo alta y baja).
- Geolocalizacion automatica o calculo de distancia en SQL.

## Decisions

**1. Radio por zona (decision del usuario, opcion A).** Cada `ZonaUsuario` lleva `RadioMetros`; no hay radio global en la configuracion.

**2. Tabla `ZonaUsuario` (propuesta aprobada).** `Id INT IDENTITY PK`, `UsuarioId INT NOT NULL FK Usuarios`, `Nombre VARCHAR(50) NOT NULL`, `Latitud DECIMAL(9,6) NOT NULL`, `Longitud DECIMAL(9,6) NOT NULL`, `RadioMetros INT NOT NULL` con `CHECK` de rango, `UNIQUE (UsuarioId, Nombre)` e indice por `UsuarioId`. Se agrega a `script_bd.sql` y a las dos bases de desarrollo por DDL directo.

**3. Reglas en `ZonaUsuarioBLL` (valores mios).** Operaciones `ObtenerMisZonas`, `Crear` y `Eliminar`, todas con el usuario de `_sesion`; `Eliminar` verifica que la zona sea del usuario. Nombre obligatorio, hasta 50 caracteres, unico por usuario sin distinguir mayusculas (criterio de `CategoriaProductoBLL`). Radio entre 100 y 20000 m, 1000 por defecto, paso de 100. Coordenadas dentro de los rangos validos. Maximo 10 zonas por usuario para acotar el calculo y el mapa.

**4. Permiso `gestionzonasusuario` (decision mia en el nombre).** Funcionalidad del usuario final: se asigna al grupo `basico` y a `admin` (convencion del proyecto de que admin tenga todo lo nuevo). Se siembra en `script_bd.sql` y en las dos bases.

**5. Menu "Mi usuario".** `mnuSesion` se renombra a `mnuMiUsuario` con Tag nuevo `FormMainShell.Menu.MyUser`; se agrega `mnuMisZonas` entre Mi configuracion y el separador. La key vieja se retira de seeds y bases si queda sin uso. Los tests UI que abren el menu por nombre se ajustan.

**6. `FormMisZonas`.** Hijo MDI unico, `FormTraducibleBase`, tema Krypton y nomenclatura de `winforms-ui-control-naming`. Reutiliza el mismo proveedor de mapas, zoom y `CoordinadorBusquedaUbicacion` de `FormSeleccionarUbicacionComercio`; la coincidencia elegida o el clic fijan un marcador provisorio. Cada zona se dibuja como `GMapPolygon` aproximando el circulo con puntos calculados en UI (helper visual local, no regla de negocio), mas marcador con etiqueta.

**7. Distancia Haversine en BLL (decision mia sobre ubicacion).** Clase estatica pura `DistanciaGeografica` en BLL (radio terrestre 6371008.8 m) y un metodo de `ZonaUsuarioBLL` que, dado un punto, devuelve la zona mas cercana, su distancia y si cae dentro del radio. Vive en BLL porque su consumidor es la recomendacion de compras; no hace falta en ABSTRACCIONES ni en SERVICIOS.

**8. Decisiones de implementacion (tomadas sin consulta, criterio conservador).**
- El paso de 100 m es el `Increment` del `NumericUpDown`; `ZonaUsuarioBLL` valida solo el rango 100-20000.
- La falta de punto se valida en BLL (`Crear(nombre, decimal? latitud, decimal? longitud, radio)` lanza `Error.MyZones.PointRequired`); las coordenadas se redondean a 6 decimales, la escala de la columna.
- API de distancia para `recomendacion-compras-asistente`: `DistanciaGeografica.CalcularMetros(decimal, decimal, decimal, decimal)` y en `ZonaUsuarioBLL` `CalcularDistanciasAMisZonas`, `ObtenerZonaMasCercana`, `ObtenerZonaQueContiene` (la mas cercana entre las que contienen el punto, que puede no ser la mas cercana) y el estatico `CalcularDistancias(zonas, lat, lon)` para evaluar muchos puntos con una sola lectura. Resultado `DistanciaZonaUsuario` (`Zona`, `DistanciaMetros`, `EstaDentroDelRadio`); el borde del radio cuenta como dentro.
- `ZonaUsuario` no lleva DVH/DVV (solo `Usuarios` los tiene) y la baja de un usuario no borra sus zonas, igual que `CuentaFinanciera`.
- La pantalla reutiliza las keys `FormMapLocationPicker.*` para estados de busqueda, zoom y atribucion; abre ocupando el area MDI y resalta la zona seleccionada. El separador del menu pasa a `toolStripSeparatorMiUsuario` y `FormMainShell.Menu.Session` se retiro de seeds y bases.

## Risks / Trade-offs

- [Renombrar `mnuSesion` rompe tests UI existentes] → Se buscan con `rg` y se ajustan en la misma tarea.
- [Nominatim tiene politica de uso estricta] → Se reutiliza el servicio existente, que ya la respeta.
- [Aproximar el circulo con poligono] → 64 puntos alcanzan visualmente para radios de hasta 20 km.
- [DDL a mano en dos bases] → Tarea con verificacion por consulta en ambas.
