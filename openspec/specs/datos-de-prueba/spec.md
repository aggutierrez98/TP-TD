# datos-de-prueba Specification

## Purpose
Permite arrancar una base nueva de FINAZAPP con datos de demostración realistas del mercado argentino, cargados de forma reproducible, para mostrar y probar el sistema sin carga manual.

## Requirements

### Requirement: Carga posterior al esquema base
El script de datos de prueba SHALL ejecutarse sobre una base ya creada con `script_bd.sql` y MUST NOT crear ni modificar tablas, ni insertar o modificar filas de `Usuarios`.

#### Scenario: Base recién creada
- **WHEN** se ejecuta `script_datos_prueba.sql` sobre una base con `script_bd.sql` aplicado
- **THEN** el script termina sin errores y el usuario `user` conserva su fila original en `Usuarios`

### Requirement: Idempotencia
El script SHALL poder ejecutarse repetidas veces sin duplicar filas ni fallar por restricciones de unicidad.

#### Scenario: Segunda ejecución
- **WHEN** se ejecuta el script dos veces seguidas
- **THEN** la cantidad de filas de cada tabla cargada es la misma que tras la primera ejecución

### Requirement: Referencias por clave natural
El script SHALL resolver cada relación entre filas por nombre (comercio, producto, entidad, medio de pago, categoría, rubro, moneda), sin depender de identificadores numéricos fijos.

#### Scenario: Identificadores distintos
- **WHEN** las tablas tienen identificadores distintos a los de otra base
- **THEN** cada precio, promoción y vínculo apunta a las filas correctas por su nombre

### Requirement: Catálogos globales de demostración
El script SHALL cargar rubros, categorías de producto, categorías globales de movimiento adicionales, entidades financieras, medios de pago, relaciones entidad–medio–segmento, comercios, productos y precios, respetando los límites de longitud y restricciones del esquema.

#### Scenario: Catálogo disponible
- **WHEN** se abren las pantallas de comercios, productos y entidades tras la carga
- **THEN** se listan el conjunto completo de filas cargadas y el producto sin precio sigue siendo válido

#### Scenario: Datos excluidos
- **WHEN** se revisa lo cargado
- **THEN** no existe una segunda tasa de conversión USD→ARS, no existe MODO como entidad ni medio de pago, y no existe el precio descartado por outlier

### Requirement: Promociones con vigencias mixtas
El script SHALL cargar las promociones con sus filtros (días, entidades, medios de pago, segmentos, comercios, rubros, productos y categorías, incluidos y excluidos) y con sus vigencias reales, de modo que algunas estén vencidas y la mayoría vigentes a la fecha de la demo.

#### Scenario: Mezcla de vigencias
- **WHEN** se consultan las promociones el 2026-10-16
- **THEN** hay promociones vencidas o por vencer en esa fecha y la mayoría tiene `VigenciaHasta` posterior

#### Scenario: Elemento incluido y excluido
- **WHEN** se carga una promoción
- **THEN** ningún producto ni categoría figura a la vez como incluido y excluido en ella

### Requirement: Datos del usuario de prueba
El script SHALL asignar al usuario `user` cuentas financieras de varios bancos y billeteras virtuales con los medios de pago que ese usuario tiene contratados, categorías de movimiento propias, movimientos de ejemplo de ingreso y egreso, y reglas de categorización.

#### Scenario: Pantallas del usuario de prueba
- **WHEN** se inicia sesión como `user`
- **THEN** se ven sus cuentas, sus categorías propias, sus movimientos y sus reglas de categorización

#### Scenario: Vínculo cuenta–medio inexistente
- **WHEN** una cuenta de ejemplo refiere a una combinación entidad–medio que no existe en el catálogo
- **THEN** la cuenta se carga sin ese medio y el script no falla

### Requirement: Coordenadas de comercios
El script SHALL cargar latitud y longitud de un comercio solo si fueron obtenidas de una geocodificación con coincidencia clara de su dirección; en otro caso quedan en NULL.

#### Scenario: Sin coincidencia clara
- **WHEN** la dirección de un comercio no tiene coincidencia clara o el comercio es online
- **THEN** su latitud y longitud quedan en NULL

### Requirement: Notificaciones de demostracion
El script de datos de prueba SHALL cargar para `user` al menos una recomendacion de compras, una de inversiones sin monto y un recordatorio de movimientos, completos y sin leer en la primera carga. SHALL identificarlos como ejemplos, resolver el usuario por su nombre y permitir consultarlos sin servicios externos. Reejecutarlo MUST NOT duplicarlos ni restablecer su estado de lectura.

#### Scenario: Primera apertura en laboratorio
- **WHEN** se ejecutan el esquema y los datos de prueba y se inicia sesion como `user`
- **THEN** Mis notificaciones muestra los tres ejemplos y permite abrir sus mensajes completos sin ejecutar tareas ni llamadas a APIs

#### Scenario: Carga repetida
- **WHEN** se vuelve a ejecutar el script incluso en otra fecha despues de leer un ejemplo
- **THEN** conserva exactamente un ejemplar de cada ejemplo y su lectura, sin modificar Usuarios

#### Scenario: Contexto de los ejemplos
- **WHEN** se abre una recomendacion de ejemplo
- **THEN** se identifica como demostracion y usa datos compatibles con los catalogos y el perfil sembrados sin prometer precios o rendimientos vigentes
