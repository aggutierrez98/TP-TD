## Why

La UI de FINAZAPP es WinForms estandar sin tema: aspecto gris por defecto, sin relacion con la identidad de la presentacion del TP (fondo blanco, verde, texto gris-violaceo oscuro). Se quiere un aspecto coherente con esa presentacion sin perder la suite de tests UI FlaUI ni la regla de que el proyecto compile y levante a la primera en el laboratorio de la universidad.

## What Changes

- Incorporar `Krypton.Toolkit` (BSD-3, soporta .NET Framework 4.7.2) al proyecto `UI`, con version fija.
- Definir una paleta propia en modo claro (`PaletteMode.Custom`), inicializada globalmente al arrancar la app.
- Hacer que `FormTraducibleBase` herede de `KryptonForm`, de modo que los 28 formularios hereden el tema desde un unico punto.
- Migrar **todos** los controles a sus equivalentes Krypton, incluidos ComboBox, CheckBox, DateTimePicker y DataGridView, y adaptar los helpers de tests UI (`UiFind`) para operarlos. Un control solo permanece nativo (tematizado con la paleta) como excepcion documentada, si un spike previo no logra automatizarlo de forma fiable.
- Extender `IdiomaFormHelper` para reconocer los equivalentes Krypton (hoy usa `control is ComboBox`).
- Marco de ventana blanco con borde verde fino, menu y area MDI tematizados.
- Conservar los `Name` de los controles (y por tanto los `AutomationId`) y sus prefijos de nomenclatura.
- Actualizar la skill de nomenclatura de controles y la documentacion del proyecto.

## Capabilities

### New Capabilities
- `tema-visual`: tema global de la UI (paleta, fuente, herencia desde el formulario base, tematizado de controles nativos).
- `compatibilidad-tests-ui`: garantia de que la suite UI FlaUI sigue encontrando y operando los controles tras la migracion.

### Modified Capabilities
Ninguna (todavia no existen specs en `openspec/specs/`).

## Impact

- Codigo: `UI/UI.csproj`, `UI/Program.cs`, `UI/FormTraducibleBase.cs`, `UI/FormPrincipal*.cs`, `UI/*.Designer.cs` (por lotes), `UI/PasswordTextBox.cs`, `UI/IdiomaFormHelper.cs`, nueva carpeta `UI/Tema/`.
- Tests: `tests/TpIDS.UiTests/Infrastructure/UiFind.cs` se adapta (combo, checkbox, fecha, seleccion de fila de grilla) para operar los controles Krypton sin perder el soporte de los nativos.
- Dependencias: nuevo paquete NuGet `Krypton.Toolkit` (solo referenciado desde `UI`; la matriz de capas no cambia).
- Documentacion: skill `winforms-ui-control-naming`, `AGENTS.md`, `ESTADO-ACTUAL-Y-PROXIMOS-PASOS.md`, documentos G0X que describan la UI.

## Fuera de alcance

- Modo oscuro.
- Colores de acento (naranja y azul) de los graficos LiveCharts.
- Fuentes Anton/Poppins (se mantiene Segoe UI, que viene con Windows); solo quedan como prueba opcional dentro de la prueba de concepto.
- Cambios de comportamiento en BLL/DAL/REPOSITORIO/SERVICIOS.
