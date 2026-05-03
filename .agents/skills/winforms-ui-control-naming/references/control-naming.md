# WinForms Naming Convention

## Control prefixes

- `TextBox` -> `txtb`
- `DataGridView` -> `dgv`
- `Label` -> `lbl`
- `Button` -> `btn`
- `PictureBox` -> `pb`

## Repository rule

Para mantener formularios WinForms legibles y consistentes, usar:

- nombre del control: `<prefijo><Descripcion>`
- nombre del handler: `<nombreDelControl>_<Evento>`

## Examples

- `txtbNickname`
- `txtbPassword`
- `dgvUsuarios`
- `lblSesionActual`
- `btnIngreso`
- `pbPassword`

Handlers:

- `btnIngreso_Click`
- `btnRegistro_Click`
- `dgvUsuarios_SelectionChanged`
- `pbPassword_Click`

## Renaming checklist

- Actualizar nombre de campo en `.Designer.cs`
- Actualizar `Name = "..."`
- Actualizar `InitializeComponent`
- Actualizar referencias en `.cs`
- Actualizar handlers y sus suscripciones de eventos
- Recompilar la solucion al terminar
