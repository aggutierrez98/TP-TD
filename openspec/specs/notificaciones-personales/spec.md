# notificaciones-personales Specification

## Purpose

Permitir al destinatario consultar sus notificaciones y recordatorios personales, leer su contenido completo y conocer la cantidad pendiente de lectura mientras utiliza FINAZAPP.

## Requirements

### Requirement: Acceso a la bandeja personal
El sistema SHALL mostrar «Mis notificaciones» junto a «Sesión» en el menú principal para todo usuario autenticado, sin exigir el permiso de gestión de notificaciones.

#### Scenario: Usuario sin permiso de gestión
- **WHEN** inicia sesión un usuario sin permiso de gestión de notificaciones
- **THEN** puede abrir su bandeja personal desde «Mis notificaciones»

### Requirement: Aislamiento por destinatario
El sistema MUST restringir listado, detalle, contador y marcado personal a las notificaciones del usuario autenticado, incluyendo notificaciones manuales y recordatorios de compra e inversión. Sin sesión activa SHALL rechazar las operaciones personales.

#### Scenario: Dos destinatarios
- **WHEN** un usuario consulta sus notificaciones y existen registros de otro usuario
- **THEN** el listado, total de resultados y contador excluyen los registros del otro usuario

#### Scenario: Detalle o lectura ajena
- **WHEN** se solicita el detalle o el marcado de una notificación de otro destinatario
- **THEN** se rechaza la operación sin mostrar el contenido ni modificar el registro

#### Scenario: Sesión inexistente
- **WHEN** se intenta una operación personal sin sesión activa
- **THEN** se rechaza la operación

### Requirement: Historial paginado
La bandeja SHALL listar fecha, tipo, mensaje y estado de lectura, ordenando por fecha descendente con desempate estable, en páginas de hasta 10 elementos. SHALL mostrar página actual, total de páginas y navegación anterior/siguiente.

#### Scenario: Más de diez notificaciones
- **WHEN** hay 24 notificaciones que cumplen los filtros
- **THEN** se muestran 10 en la primera página, 10 en la segunda y 4 en la tercera, con el total de tres páginas

#### Scenario: Fechas idénticas
- **WHEN** varias notificaciones tienen la misma fecha
- **THEN** mantienen un orden determinista entre páginas mientras los datos no cambien

#### Scenario: Bandeja vacía
- **WHEN** no hay resultados
- **THEN** se muestra un estado vacío, total de cero resultados y navegación deshabilitada

### Requirement: Filtros por fecha
La bandeja SHALL ofrecer únicamente los filtros opcionales Desde/Hasta y el botón Buscar siguiendo la interacción de Movimientos. Hasta SHALL incluir el día completo; Buscar SHALL reiniciar a la primera página. Un rango invertido SHALL informar el error sin ejecutar la búsqueda.

#### Scenario: Límite superior inclusivo
- **WHEN** se busca hasta el 14 de octubre con ese límite activado
- **THEN** se incluyen las notificaciones de cualquier hora de ese día y se excluyen las del día siguiente

#### Scenario: Filtros desactivados
- **WHEN** ambos límites están desactivados y se pulsa Buscar
- **THEN** se consulta todo el historial personal desde la primera página

#### Scenario: Rango invertido
- **WHEN** Desde es posterior a Hasta y ambos están activados
- **THEN** se informa el rango inválido y se conserva el listado mostrado

### Requirement: Detalle y lectura persistente
El sistema SHALL permitir abrir el mensaje completo mediante Ver detalle o doble clic y marcarlo automáticamente como leído. SHALL ofrecer Marcar como leída para la fila seleccionada. El marcado SHALL ser persistente e idempotente y actualizar el estado visible solo tras confirmarse el guardado.

#### Scenario: Apertura de detalle
- **WHEN** el usuario abre una notificación propia no leída
- **THEN** ve su mensaje completo, fecha y tipo, y queda guardada como leída

#### Scenario: Marcado desde la bandeja
- **WHEN** el usuario selecciona una fila no leída y pulsa Marcar como leída
- **THEN** se guarda la lectura sin necesitar abrir el detalle

#### Scenario: Lectura repetida
- **WHEN** se abre o marca una notificación ya leída
- **THEN** sigue leída y el contador no disminuye artificialmente

#### Scenario: Fallo de persistencia
- **WHEN** falla el guardado de lectura
- **THEN** se informa el fallo y la UI no confirma la lectura ni descuenta del contador

### Requirement: Contador de no leídas
El menú SHALL mostrar un contador tipo badge de todas las notificaciones personales no leídas de los tres tipos, independiente de los filtros y página. SHALL consultarlo al iniciar sesión, cada 30 segundos y tras una lectura exitosa.

#### Scenario: Nueva notificación durante la sesión
- **WHEN** el operador crea una notificación para el usuario conectado
- **THEN** en la próxima actualización periódica exitosa el contador refleja la nueva cantidad sin abrir la bandeja

#### Scenario: Lectura con filtros activos
- **WHEN** se marca como leída una notificación del listado filtrado
- **THEN** se actualiza inmediatamente el contador global personal, sin esperar al siguiente intervalo

### Requirement: Actualización sin bloquear la sesión
La actualización del contador SHALL mantener la interfaz disponible, evitar consultas periódicas superpuestas y detenerse al cerrar la sesión o la ventana principal. Una respuesta tardía SHALL descartarse si su sesión terminó. Un fallo periódico SHALL conservar el último valor válido y permitir reintentar sin diálogos repetitivos.

#### Scenario: Consulta lenta
- **WHEN** una consulta tarda más que el intervalo de actualización
- **THEN** el usuario puede seguir usando los formularios y no se inicia otra consulta periódica simultánea

#### Scenario: Cierre con consulta pendiente
- **WHEN** se cierra la sesión mientras hay una consulta pendiente
- **THEN** su respuesta no actualiza controles cerrados ni el contador de una sesión posterior

#### Scenario: Fallo transitorio
- **WHEN** falla una consulta periódica y la siguiente tiene éxito
- **THEN** se conserva el valor previo durante el fallo y luego se muestra el valor actualizado sin interrumpir al usuario con diálogos repetitivos
