## Purpose
Define el aspecto visual comun de FINAZAPP: la paleta propia de Krypton Toolkit en modo claro, como la heredan los formularios y que controles usar por defecto.

## ADDED Requirements

### Requirement: Paleta propia en modo claro
La aplicacion SHALL aplicar globalmente una paleta Krypton personalizada (`PaletteMode.Custom`) en modo claro, con fondo `#FFFFFF`, verde principal `#5DC47D`, verde de acento `#4CDB81`, texto `#3E3A4E` y gris-violaceo `#48465F` para bordes y elementos secundarios.

#### Scenario: Arranque de la aplicacion
- **WHEN** la aplicacion arranca
- **THEN** el tema se inicializa antes de `InicializadorAplicacionBLL.Inicializar()` y todos los formularios, incluido `FormContingenciaIntegridad`, se muestran con la paleta

#### Scenario: Sin modo oscuro
- **WHEN** el usuario usa la aplicacion
- **THEN** no existe ninguna opcion ni paleta oscura

### Requirement: Herencia unica del tema
Todos los formularios SHALL heredar el tema desde `FormTraducibleBase`, que MUST derivar de `KryptonForm`.

#### Scenario: Formulario nuevo
- **WHEN** se crea un formulario que hereda de `FormTraducibleBase`
- **THEN** se muestra con la paleta sin configuracion adicional

### Requirement: Controles Krypton por defecto
Todos los controles de los formularios SHALL ser controles de Krypton (incluidos ComboBox, CheckBox, DateTimePicker y DataGridView). Solo SHALL permanecer nativo un control que el spike de automatizacion declare no automatizable (ademas del `MenuStrip`, que usa el renderer de Krypton), y en ese caso SHALL tomar sus colores de la paleta mediante un helper en `UI/Tema/`.

#### Scenario: Formulario migrado
- **WHEN** se muestra un formulario migrado
- **THEN** sus controles se dibujan con la paleta de Krypton y no hay controles nativos con colores del sistema

#### Scenario: Excepcion nativa
- **WHEN** un control permanece nativo por no ser automatizable
- **THEN** sus colores coinciden con la paleta y la excepcion esta documentada

### Requirement: Marco de ventana
Las ventanas (formularios y hijos MDI) SHALL mostrar un marco blanco con borde verde de 1 px, sin el marco grueso del relleno de Krypton. El `MenuStrip` SHALL usar la paleta (fondo blanco, seleccion verde suave) y el area del contenedor MDI SHALL pintarse con `#F3F4F8`.

#### Scenario: Hijo MDI
- **WHEN** se abre un formulario como hijo del contenedor MDI
- **THEN** su marco es blanco con borde verde fino y el fondo del contenedor es claro

### Requirement: Fuente base compatible con cualquier maquina
La fuente base SHALL ser Segoe UI, de modo que el proyecto compile y se vea igual en cualquier maquina Windows sin instalar fuentes adicionales.

#### Scenario: Maquina de laboratorio
- **WHEN** la solucion se compila y ejecuta en una maquina sin fuentes personalizadas
- **THEN** la UI se muestra con Segoe UI sin errores ni fuentes de reemplazo inesperadas

### Requirement: Dependencia fijada
`Krypton.Toolkit` SHALL referenciarse con una version fija y solo desde el proyecto `UI`.

#### Scenario: Matriz de capas
- **WHEN** se compila `TpIDS.sln`
- **THEN** la validacion de arquitectura pasa sin cambios en la matriz de dependencias
