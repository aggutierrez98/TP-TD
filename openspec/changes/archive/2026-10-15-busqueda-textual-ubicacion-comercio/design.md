# Design

## Context

Ver proposal.md para la motivacion. El formulario usa GMap.NET 2.1.7/OpenStreetMap, un solo overlay y ColocarMarcador; devuelve coordenadas solo al aceptar. `FormGestionComercios` lo construye con las coordenadas temporales del editor. El tamaño cliente actual es 584 x 542, el mapa tiene anclajes a los cuatro bordes y los seis botones son hijos del mapa. SERVICIOS ya referencia System.Net.Http, System.Xml y System.Xml.Linq; el root manual compone servicios y expone contratos a UI.

El cambio `navegacion-visible-mapa-comercios` esta completo y no archivado. Se agrega `busqueda-ubicaciones` como capacidad propia sin duplicar los requisitos de navegacion. Debe archivarse la navegacion antes del archivo de este cambio; no es necesario bloquear el desarrollo del buscador por ese tramite.

### Evidencia de viabilidad

Consultas reales el 2026-10-14 desde el entorno de desarrollo devolvieron HTTP 200 para Obelisco, Avenida Santa Fe 3253, Cafe Tortoni y Carrefour en Buenos Aires. Tortoni devolvio Avenida de Mayo 829; Carrefour devolvio supermercados y otros objetos homonimos. `layer=address,poi` mejoro la pertinencia pero no elimino todos los predios no comerciales. Estas pruebas acreditan casos concretos, no cobertura completa ni conectividad del ejecutable Windows. Se consulto la API en JSON durante la investigacion; el adaptador productivo usara XML, formato documentado que debe probarse explicitamente.

Referencias: [Search API](https://nominatim.org/release-docs/latest/api/Search/) y [politica de uso](https://operations.osmfoundation.org/policies/nominatim/), consultadas el 2026-10-14. El fuente publico actual de GMap.NET contiene GetPoint/GetPoints, pero no se verifico esa implementacion en la DLL instalada.

## Goals / Non-Goals

**Goals:** aislar proveedor y transporte de GMap.NET; hacer deterministas las pruebas de HTTP, tiempos y cache; preservar seleccion manual y cierre seguro durante I/O.

**Non-Goals:** autocompletado remoto, geocoding inverso, listado exhaustivo de negocios, rutas, servicio offline, proxy/backend nuevo, cambios SQL o guardado automatico. No migrar dependencias preexistentes de UI en este cambio.

## Decisions

### 1. Servicio tecnico por contrato, compuesto una sola vez

Proponer `IBusquedaUbicacionesService` en ABSTRACCIONES, con `BuscarAsync` que reciba consulta, idioma, limites de preferencia opcionales y CancellationToken, y devuelva una coleccion de contratos de resultado: descripcion, latitud/longitud, limites opcionales y tipo descriptivo disponible. Son datos de intercambio tecnico como TicketEscaneado, sin entidades de negocio, dependencias externas, implementaciones ni tipos GMap.NET. Los tipos de solicitud/resultado pueden llamarse `ConsultaUbicacion`, `ResultadoBusquedaUbicacion` y `LimitesGeograficos`.

`NominatimBusquedaUbicacionesService` en SERVICIOS implementa HTTP, interpretacion XML, validacion y control de uso. `CompositionRoot` construye una sola instancia reutilizable y la entrega por interfaz al constructor del dialogo desde el llamador. No agregar un BLL pasante: la busqueda es una utilidad tecnica del selector, no un flujo persistente de negocio. No usar el root desde el servicio ni introducir HTTP en el formulario. Incluir archivos en los proyectos clasicos; las referencias existentes entre capas son suficientes.

Alternativa descartada: GetPoint/GetPoints de GMap.NET; oculta informacion descriptiva, control del endpoint, errores y politicas de red. Una API comercial es posible si el volumen o cobertura lo exige, pero requiere otra decision de proveedor/credenciales.

### 2. HTTPS y XML con bibliotecas del framework

HttpClient de larga vida, transporte inyectable por HttpMessageHandler y reloj/espera sustituibles internamente para tests. Configuracion `GeocodingSearchEndpoint` en UI/App.config con `https://nominatim.openstreetmap.org/search`, leida por ConfigurationManager; validar URI HTTPS al usar la busqueda, sin bloquear inicializacion del root ni seleccion manual si es invalida, y construir query codificando cada valor con EscapeDataString. No concatenar texto sin escape. El endpoint representa la ruta search, no un servidor arbitrario con protocolo distinto.

Parametros: `q`, `format=xml`, `limit=5`, `accept-language=es|en`, `layer=address,poi`, `viewbox` cuando sea valido, `bounded=0`; sin countrycodes fijo. Capturar limites al iniciar la consulta, con cultura invariante. Si el viewport cruza el antimeridiano o no permite construir un rectangulo valido, omitir viewbox. La preferencia no asegura proximidad ni orden por distancia. No descartar categorias amplias de forma heuristica: podrian ser lugares utiles; mostrar tipo cuando exista y dejar decidir.

Parsear XML con DTD prohibida y resolver externo deshabilitado, leyendo `place` y atributos descriptivos/coordenadas/boundingbox. Validar numeros finitos y rangos latitud [-90,90], longitud [-180,180]; descartar elementos invalidos. Limites invalidos se omiten sin perder una coordenada valida. Una raiz valida sin coincidencias devuelve lista vacia; XML malformado o una respuesta con elementos pero ninguno interpretable se informa como respuesta invalida. Acotar lectura a 1 MiB y espera HTTP a 15 segundos. No incorporar serializadores JSON externos para esta funcion.

### 3. Politica de uso, cache y limites de despliegue

Enviar User-Agent identificable `FinazAppTP-UAI/1.0` y no depender de un Referer local para identificar el geocoder. Mostrar atribucion visible «Datos © OpenStreetMap contributors», con enlace https://www.openstreetmap.org/copyright; comprobar tambien la atribucion del mapa. Ayuda breve: «La cobertura depende de OpenStreetMap. Agregue ciudad o direccion para precisar».

La instancia compartida serializa peticiones mediante SemaphoreSlim y asegura al menos un segundo entre sus inicios con tiempo monotono. Cancelar durante la espera no envia HTTP. Cache en memoria compartido por esa instancia: TTL 24 horas, maximo 100 entradas con eviccion por antiguedad; guardar exitos y listas vacias, nunca errores/cancelaciones. Clave: endpoint, texto trim con espacios consecutivos normalizados (conservar signos/acentos), idioma y viewbox normalizado. Revalidar cache dentro de la exclusion antes de enviar para evitar duplicados concurrentes.

No reintentar automaticamente. Para HTTP 429/503 interpretar Retry-After (segundos o fecha), registrar un cooldown compartido y devolver estado traducible; si falta, usar espera de 60 segundos para 429. Otros errores HTTP se distinguen de ausencia de resultados. Aplicar cooldown antes de una nueva peticion externa; los aciertos del cache siguen disponibles. CancellationToken distingue cancelacion del usuario de timeout.

La politica publica limita el trafico agregado de toda la aplicacion. Un limitador en memoria solo garantiza una instancia/proceso: el alcance de despliegue propuesto para usar el endpoint publico es una instancia activa del TP. No afirmar cumplimiento agregado para instalaciones concurrentes. Antes de distribuir a varios usuarios simultaneos, se necesita un endpoint intermediario con limite/cache compartido o un proveedor cuyo contrato soporte ese volumen, fuera de este alcance. La configuracion facilita sustituirlo. No registrar texto de busqueda ni enviar datos de usuario/comercio adicionales al texto que el usuario busca.

### 4. Controles compactos y eleccion explicita

Conservar 584 x 542 de tamaño cliente. Encima del mapa agregar etiqueta, `txtbBusquedaUbicacion`, `btnBuscarUbicacion`, `cmbResultadosUbicacion` (KryptonComboBox, DropDownList) y `lblEstadoBusquedaUbicacion`. El combo evita reservar una grilla permanente para solo cinco resultados. Campo y combo crecen horizontalmente, Buscar conserva tamaño junto al borde derecho; el mapa absorbe el espacio restante y mantiene un area util para navegar. Las instrucciones pueden reorganizarse sin perder los gestos documentados. Los botones del mapa conservan posiciones relativas a su padre, margen y composicion alfa.

Tras enlazar resultados dejar SelectedIndex=-1 y combo habilitado solo con resultados vigentes. Aplicar exclusivamente una eleccion del usuario (SelectionChangeCommitted o confirmacion equivalente de teclado), nunca SelectedIndexChanged disparado por binding. Mostrar descripcion completa en tooltip/ayuda accesible y ampliar el ancho del desplegable dentro del area de pantalla. Tab order: texto, Buscar, coincidencias, mapa/navegacion, Aceptar, Cancelar. Enter en el texto ejecuta Buscar y suprime la propagacion: no asignar Buscar como AcceptButton global ni aceptar accidentalmente el dialogo.

Errores/estados se expresan por keys `FormMapLocationPicker.Search...`, incluyendo texto requerido, buscando, resultados, sin resultados, conexion, timeout, limite, respuesta invalida y ayuda de cobertura. Usar el catalogo base vigente y seeds es/in (in es el archivo ingles del repo). Actualizar accesibilidad y estados al cambiar idioma; invalidar resultados pendientes del idioma anterior. Mensajes largos deben verse completos o tener ayuda accesible sin desplazar botones.

### 5. Estado asincrono ligado al ciclo de vida

Mantener un CancellationTokenSource por consulta y una version de solicitud. Buscar deshabilitado mientras se consulta; no usar Task.Run para I/O. await actualiza controles en el hilo UI. Durante la consulta se permite pan/zoom, marcar manualmente y Aceptar si hay marcador, asi como Cancelar/cerrar. El transporte captura la vista inicial y su respuesta solo actualiza coincidencias.

Editar el texto, cambiar idioma o cerrar incrementa version, cancela la consulta e invalida lista anterior sin tocar marcador. Una respuesta obsoleta se descarta aun si el transporte no observo cancelacion. El finally restaura el estado solo para la version vigente y si el formulario existe; disponer tokens y retirar suscripciones segun el ciclo de vida. No presentar cancelacion intencional como error.

### 6. Aplicacion del resultado y zoom

Al elegir, interrumpir cualquier arrastre activo, restaurar CanDragMap y limpiar estado hover previo. Asignar Position a la coordenada y reutilizar ColocarMarcador para reemplazar el unico marcador y habilitar Aceptar. Si hay boundingbox valida y no degenerada, calcular zoom de encuadre con la proyeccion/mapa, limitado 2–18; despues reafirmar Position en la coordenada devuelta (el centro de los limites puede diferir). Sin limites utiles usar zoom 16. No usar rangos del proveedor como coordenadas de seleccion.

No confirmar ni persistir al seleccionar. El llamador sigue copiando las coordenadas solo para DialogResult.OK. La seleccion manual despues de buscar es autoritativa; pan/zoom posteriores no alteran marcador. Preferir encuadre sobre zoom fijo para ciudades/barrios; el fallback fijo es apropiado para un punto sin extension.

### 7. Verificacion con aislamiento de red

Aplicar strict-tdd a SERVICIOS: baseline verde, tests en rojo observados para cada comportamiento antes de implementar, y verde despues. Tests MSTest con HTTP falso y reloj controlado cubren encoding, parametros, XML, rangos, timeout/cancelacion, errores HTTP, Retry-After, serializacion, cache/TTL/eviccion y configuracion. No usar API publica como dependencia de la suite ni sleeps reales para probar frecuencia.

Verificar el formulario con servicio falso mediante inyeccion y respuestas controladas: multiples/unica/vacias, marcador previo, texto editado, cierre y errores; las comprobaciones UI se hacen despues del desarrollo, fuera del ciclo TDD del servicio. Agregar cobertura determinista del flujo de seleccion/ajuste/OK/Cancel y regresion de navegacion; no sustituirla por tests que solo miren propiedades del designer. FlaUI completo requiere aviso y confirmacion; filtro solo a pedido explicito. Conservar el test existente de redimensionado y no agregar una puerta de pruebas al ejecutable ni cambiar el harness para consultar Internet.

Hacer una comprobacion manual acotada desde Windows con direccion, lugar y comercio concretos usando XML; registrar respuesta y limitaciones, no exigir igualdad permanente de coordenadas externas. Cierre: build sin restore, guardias arquitectura/composicion, tests -IncludeIntegration y OpenSpec estricto. La solucion tiene dependencias preexistentes de UI por NuGet: este cambio no agrega ninguna ni acredita por si solo la compilacion limpia del laboratorio.

## Risks / Trade-offs

- [Comercio ausente o homonimos no comerciales] → descripcion/tipo, eleccion explicita y alternativa por direccion; no prometer exhaustividad.
- [Limitador local insuficiente para varios equipos] → limitar despliegue publico previsto y requerir endpoint de control agregado antes de ampliar distribucion.
- [Red o servicio publico no disponible] → timeout, mensajes concretos y seleccion manual siempre operativa.
- [Respuesta tardia o binding mueve marcador] → version/cancelacion y evento exclusivo de eleccion del usuario.
- [Menor alto de mapa al tamaño inicial] → combo compacto, layout revisado es/en y prueba de margen/atribucion tras redimensionar.
- [Trabajo concurrente en root, configuracion y seeds] → revisar diffs antes de editar y preservar cambios ajenos; incluir solo modificaciones de este buscador.

## Migration Plan

Distribuir ejecutable/configuracion y seeds actualizados con el mecanismo existente; no hay migracion SQL ni secreto nuevo. El endpoint publico no requiere clave. Revertir servicio/wiring/controles restaura seleccion manual y no cambia coordenadas ya guardadas. Mantener registro de resultados de verificacion en tasks.md. Una vez implementado, registrar en `actualizar-diagramas-tp` tareas para G07 y el CU de ubicacion/comercios; si ese cambio ya fue archivado, crear el cambio documental correspondiente. No alterar G08 porque no cambia el modelo persistente.

## Ajustes verificados del adaptador

Verificados con tests deterministas (HTTP y reloj falsos, sin Internet ni esperas reales) en `SERVICIOS.Tests`; precisan o completan las decisiones 1–3.

- **Contrato de errores:** `ConsultaUbicacion` incluye idioma y zona preferida. Los fallos se informan con `BusquedaUbicacionException` y el enum `EstadoBusquedaUbicacion` (`ConexionFallida`, `TiempoAgotado`, `LimiteExcedido`, `RespuestaInvalida`, `ConfiguracionInvalida`), con `CodigoHttp` y `EsperaSugerida` opcionales; el mensaje nunca incluye el texto buscado. Sin coincidencias es una lista vacia; la cancelacion del llamador es `OperationCanceledException`. La UI mapea cada estado a su key `FormMapLocationPicker.Search...`.
- **Endpoint:** ademas de exigir HTTPS, se rechazan endpoints con credenciales, query o fragmento. Se valida al buscar (`ConfiguracionInvalida`), no al construir el servicio.
- **Viewbox:** `viewbox` y `bounded=0` se envian juntos y solo con una zona valida (finita, en rango, `sur < norte`, `oeste < este`); el antimeridiano y los rectangulos degenerados omiten ambos. Las coordenadas se redondean a 4 decimales con cultura invariante, tanto en la URL como en la clave de cache.
- **XML:** `boundingbox` llega como sur,norte,oeste,este. El tipo es `type` con `class` como respaldo. Se descartan los lugares sin `display_name` o con coordenadas invalidas; limites invalidos o degenerados se omiten sin perder la coordenada. El tope de 1 MiB se aplica tambien cuando falta `Content-Length`. Se devuelven como maximo cinco coincidencias.
- **HTTP:** 403, 429 y 503 son `LimiteExcedido`; cualquier otro estado no exitoso es `ConexionFallida` con su `CodigoHttp`. Solo 429 usa 60 s si falta `Retry-After`; 403 y 503 sin cabecera no imponen cooldown. `Retry-After` (segundos o fecha) se acota a 1 hora. Con cooldown vigente una consulta no cacheada falla sin peticion externa, informando la espera restante; no queda en espera.
- **Tiempo:** el timeout de 15 s y la separacion de 1 s usan el reloj inyectado (`IRelojBusqueda`, monotonico); el timeout cubre el envio y la lectura del cuerpo.
- **Cache:** la clave conserva mayusculas, acentos y signos (solo recorta y colapsa espacios), por lo que "Cafe" y "cafe" son entradas distintas. Una zona no representable equivale a no enviar viewbox. Vencen las entradas con 24 h o mas; el desalojo al llegar a 100 es por antiguedad.
- **Alcance real del limitador:** serializacion, separacion de un segundo, cooldown y cache son **por instancia**, es decir, por proceso. No hay control agregado entre varias instalaciones o equipos; no se afirma cumplimiento agregado de la politica de Nominatim. Para distribuir a varios usuarios simultaneos sigue haciendo falta el endpoint intermediario o proveedor descrito en la decision 3.
- **Politica y dependencias:** se contrasto con los puntos de la decision 3 (identificacion, un pedido por segundo, cache, sin reintentos automaticos); la pagina de politica no se volvio a consultar en esta implementacion. No se agrego `PackageReference`, `packages.config` ni referencias nuevas: solo `System.Net.Http`, `System.Xml` y `System.Xml.Linq`, ya presentes en `SERVICIOS.csproj`. `GeocodingSearchEndpoint` en `App.config` queda para la tarea 3.1.

## Ajustes verificados del formulario

Verificados con una corrida de humo del formulario compilado con servicio falso (sin Internet para la busqueda) y con tests deterministas del coordinador; precisan la decision 4–6.

- **Nombres:** el campo es `txtbBusquedaUbicacion` (prefijo `txtb` de la skill de nomenclatura, en lugar de `txtBusquedaUbicacion`). El estado es un `KryptonWrapLabel` de dos lineas (`lblEstadoBusquedaUbicacion`) y la atribucion un `KryptonLinkLabel` (`lblAtribucionOsmUbicacion`) abajo a la izquierda, en la fila de Aceptar/Cancelar, que abre https://www.openstreetmap.org/copyright.
- **Layout a 584 x 542:** ayuda de gestos de 54 px, fila etiqueta/campo/Buscar, combo de coincidencias y estado; el mapa pasa de 400 a 326 px de alto y los botones de navegacion conservan su posicion relativa al mapa. Al ampliar crecen campo, combo, estado y mapa; al restaurar vuelven exactamente a las posiciones iniciales. Orden de tab: campo, Buscar, coincidencias, mapa, Aceptar, Cancelar, atribucion.
- **Ciclo de vida:** `CoordinadorBusquedaUbicacion` (UI, sin WinForms ni GMap.NET, no es un presenter) concentra version, `CancellationTokenSource`, descarte de respuestas obsoletas y bloqueo de doble envio; el formulario solo refleja su resultado. Se enlaza a `BLL.Tests` como `Compile Link`, igual que `ActualizadorContadorNotificaciones`. Editar el texto, cambiar de idioma, aceptar, cancelar o cerrar invalidan la consulta y la lista; Buscar queda disponible de inmediato. Una excepcion inesperada se informa como conexion fallida.
- **Idioma de la consulta:** `en` si el nombre del idioma activo empieza con "en" (English); cualquier otro usa `es`.
- **Zoom de encuadre:** el mayor zoom entre 2 y 18 en que la extension cabe en el 90 % de la vista segun la proyeccion del proveedor; si no cabe ni en 2 se usa 2. Sin limites se usa 16. Luego se reafirma `Position` en la coordenada devuelta.
- **Estados:** al abrir y al editar se muestra la ayuda de cobertura; sin servicio inyectado la busqueda queda deshabilitada con el mensaje de configuracion y la seleccion manual sigue operativa. La descripcion completa de la coincidencia elegida se muestra en el estado, en el tooltip y en la descripcion accesible del combo; el desplegable se ensancha hasta el borde de la pantalla.
