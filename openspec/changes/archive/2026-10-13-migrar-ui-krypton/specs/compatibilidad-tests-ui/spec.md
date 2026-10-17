## Purpose
Define las condiciones que la UI debe cumplir para que la suite FlaUI siga encontrando y operando los controles: AutomationId estables y controles automatizables por UIA.

## ADDED Requirements

### Requirement: AutomationId estables
Cada control migrado SHALL conservar su propiedad `Name` (y por lo tanto su `AutomationId`) y su prefijo de nomenclatura (`btn`, `txtb`, `cmb`, `lbl`, `mnu`, etc.).

#### Scenario: Busqueda por AutomationId
- **WHEN** un test FlaUI busca un control por su `AutomationId` actual
- **THEN** lo encuentra sin modificar el test

### Requirement: Migracion condicionada a automatizabilidad
Un control SHALL migrarse a su equivalente Krypton solo si existe una interaccion de prueba verificada para el: que exponga el mismo `AutomationId` y que sus operaciones (leer/escribir texto, elegir item, marcar, fijar fecha, seleccionar fila) se puedan hacer por un patron UIA, un clic real o teclado. Si no existe, el control SHALL permanecer nativo y tematizado con la paleta, y la excepcion SHALL quedar documentada en `design.md`.

#### Scenario: Control sin patron UIA
- **WHEN** un control Krypton no expone el patron UIA que requiere su helper (por ejemplo `TogglePattern` o `SelectionItemPattern`) pero se verifica una alternativa (clic real o teclado)
- **THEN** el helper de `UiFind` usa la alternativa y el control se migra

#### Scenario: Control no automatizable
- **WHEN** ninguna estrategia de interaccion es fiable para un control Krypton
- **THEN** el control permanece nativo y se tematiza con la paleta

### Requirement: Helpers compatibles con ambas versiones
Los helpers de `UiFind` adaptados SHALL seguir funcionando con la version nativa y con la version Krypton del mismo control, de modo que los tests UI existentes no requieran cambios en su logica.

#### Scenario: Test existente tras migrar un formulario
- **WHEN** un formulario se migra a controles Krypton
- **THEN** sus tests UI existentes pasan sin modificar sus llamadas a `UiFind`

### Requirement: Suite en verde
Cada lote de migracion SHALL cerrar con `TpIDS.sln` compilando y `tests/tools/run-tests.ps1 -IncludeIntegration` en verde; la suite UI completa SHALL ejecutarse solo tras avisar al usuario que toma el escritorio unos 5 minutos y recibir su confirmacion.

#### Scenario: Cierre de un lote
- **WHEN** se termina de migrar un lote de formularios
- **THEN** la compilacion y la suite requerida pasan antes de dar el lote por terminado
