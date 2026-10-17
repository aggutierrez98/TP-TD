# busqueda-ubicaciones Specification

## Purpose
Permitir encontrar direcciones, comercios y lugares mediante texto y usar una coincidencia como ubicacion editable del comercio antes de confirmar sus coordenadas.

## Requirements

### Requirement: Busqueda textual explicita
El selector SHALL ofrecer un campo unico para direcciones, comercios o lugares y ejecutar la busqueda solo mediante Buscar o Enter en ese campo. SHALL rechazar texto vacio sin consultar al proveedor y MUST NOT consultar mientras el usuario escribe.

#### Scenario: Buscar direccion o nombre
- **WHEN** el usuario introduce una direccion o el nombre de un comercio/lugar y pulsa Buscar o Enter en el campo
- **THEN** se consulta ese texto y se muestran las coincidencias disponibles sin confirmar el dialogo.

#### Scenario: Texto vacio
- **WHEN** el usuario intenta buscar solo espacios
- **THEN** se informa que debe introducir un texto y no se envia una peticion.

#### Scenario: Escritura sin envio
- **WHEN** el usuario escribe o modifica el texto sin ejecutar Buscar
- **THEN** no se consulta al proveedor ni se modifica el marcador.

### Requirement: Coincidencias descriptivas y contexto geografico
La busqueda SHALL presentar hasta cinco coincidencias con descripcion de nombre/direccion disponible, sin elegir automaticamente ninguna. SHALL favorecer la zona visible sin excluir resultados externos ni imponer un pais fijo. SHALL informar que la cobertura de comercios depende de los datos del proveedor.

#### Scenario: Cadena con varias sucursales
- **WHEN** el proveedor devuelve varias ubicaciones para un nombre comercial
- **THEN** el usuario puede distinguirlas por sus descripciones y elegir explicitamente una, sin que cargar la lista modifique la seleccion.

#### Scenario: Resultado unico
- **WHEN** la consulta devuelve una sola coincidencia
- **THEN** se presenta para eleccion explicita y no se coloca el marcador automaticamente.

#### Scenario: Lugar fuera de la vista
- **WHEN** el texto identifica un lugar fuera del mapa visible
- **THEN** ese lugar puede aparecer porque la zona visible es una preferencia y no una restriccion.

### Requirement: Aplicacion de una coincidencia al marcador
Al elegir una coincidencia valida el selector SHALL centrar el mapa en sus coordenadas, ajustar el zoom dentro de 2–18 y colocar o reemplazar el unico marcador seleccionado. SHALL habilitar Aceptar y permitir ajustar manualmente el punto antes de devolver las coordenadas finales.

#### Scenario: Elegir sin marcador previo
- **WHEN** el usuario elige una coincidencia sin ubicacion seleccionada
- **THEN** el mapa se centra, aparece un marcador en sus coordenadas y Aceptar queda habilitado.

#### Scenario: Reemplazar y ajustar
- **WHEN** el usuario elige otra coincidencia y despues arrastra el marcador o hace clic en el mapa
- **THEN** queda un solo marcador y Aceptar devuelve las coordenadas ajustadas, sin guardar por el mero hecho de buscar.

#### Scenario: Cancelar la eleccion
- **WHEN** el usuario elige una coincidencia y cancela el dialogo
- **THEN** las coordenadas del comercio en el formulario llamador no son reemplazadas.

### Requirement: Consulta sin bloqueo y respuestas vigentes
Durante la consulta el selector SHALL mostrar un estado de busqueda y permanecer utilizable para navegar y cerrar. SHALL evitar envios duplicados, cancelar al cerrar y descartar respuestas de consultas cuyo texto ya cambio o cuyo formulario se cerro. Una respuesta SHALL modificar solo la lista, nunca el marcador.

#### Scenario: Proveedor lento y doble envio
- **WHEN** una consulta esta en curso y el usuario vuelve a pulsar Buscar o Enter
- **THEN** no se inicia otra peticion y el mapa y Cancelar siguen respondiendo.

#### Scenario: Texto modificado durante la consulta
- **WHEN** el usuario cambia el texto antes de que termine la consulta
- **THEN** se invalida esa consulta y sus resultados no se presentan como coincidencias del nuevo texto.

#### Scenario: Cierre durante la consulta
- **WHEN** el usuario cancela, cierra o acepta una seleccion previa mientras hay una consulta pendiente
- **THEN** la consulta se cancela y una respuesta tardia no actualiza controles ni provoca errores de objeto eliminado.

### Requirement: Conservacion de la seleccion ante resultados inutilizables
El selector SHALL dejar intactos el marcador, centro y zoom actuales si la consulta no devuelve coincidencias utilizables, falla o se cancela, incluyendo cambios manuales realizados durante la espera. SHALL distinguir ausencia de resultados de conexion fallida, timeout, limite del proveedor o respuesta invalida con mensajes traducibles, permitiendo seguir seleccionando manualmente.

#### Scenario: Sin coincidencias
- **WHEN** el proveedor devuelve una lista vacia
- **THEN** se informa que no hay resultados y se sugiere precisar ciudad o direccion sin alterar la ubicacion.

#### Scenario: Error remoto
- **WHEN** falla la conexion, vence el tiempo de espera o el proveedor rechaza la consulta
- **THEN** se informa el problema, termina el estado de busqueda y se conserva la seleccion previa y su disponibilidad de Aceptar.

#### Scenario: Coordenadas invalidas
- **WHEN** una respuesta contiene coordenadas ausentes, no numericas o fuera de rango
- **THEN** esas coincidencias no son elegibles y no colocan un marcador; una respuesta malformada se informa como error.

### Requirement: Uso compatible del proveedor publico
El cliente SHALL identificar la aplicacion, mostrar atribucion a OpenStreetMap, permitir cambiar el endpoint mediante configuracion y reutilizar consultas equivalentes en cache. Para Nominatim publico MUST limitarse a una peticion por segundo en total por aplicacion y respetar rechazos temporales, sin autocompletado, consultas periodicas o reintentos automaticos.

#### Scenario: Busquedas consecutivas
- **WHEN** se solicitan dos consultas no cacheadas en rapida sucesion
- **THEN** sus peticiones se serializan y sus inicios se separan al menos un segundo.

#### Scenario: Consulta repetida
- **WHEN** se repite una consulta con el mismo texto normalizado, idioma, contexto geografico y endpoint dentro de la vigencia del cache
- **THEN** se reutilizan las coincidencias sin peticion externa.

#### Scenario: Limite temporal del servidor
- **WHEN** el proveedor devuelve un rechazo temporal con un periodo de espera
- **THEN** ese periodo se respeta antes de otra consulta externa y no se generan reintentos automaticos.

#### Scenario: Cambio de endpoint
- **WHEN** el operador configura otro endpoint compatible y reinicia la aplicacion
- **THEN** las nuevas consultas usan ese endpoint sin recompilar ni actualizar el ejecutable.

### Requirement: Integracion visual traducible y navegacion conservada
Campo, accion, coincidencias y estados SHALL ser accesibles por teclado y tener textos propios en español e ingles. SHALL permanecer utilizables al tamaño inicial y al redimensionar, con atribucion visible. SHALL conservar pan/zoom, cruceta flotante semitransparente, paso de 48 pixeles, ajuste del marcador y Aceptar/Cancelar.

#### Scenario: Teclado e idiomas
- **WHEN** el usuario busca y elige una coincidencia con teclado en español o ingles
- **THEN** puede completar el flujo con etiquetas y mensajes traducidos y consultar la descripcion completa del resultado.

#### Scenario: Redimensionado y navegacion posterior
- **WHEN** el usuario abre, agranda y restaura el dialogo y navega despues de elegir una coincidencia
- **THEN** los controles permanecen visibles, la atribucion queda libre y navegar no modifica las coordenadas del marcador.
