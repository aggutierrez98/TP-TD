# WinForms Naming Convention

## Control prefixes

- `TextBox` -> `txtb`
- `DataGridView` -> `dgv`
- `Label` -> `lbl`
- `ListBox` -> `lstb`
- `Button` -> `btn`
- `PictureBox` -> `pb`
- `ComboBox` -> `cmb`
- `CheckBox` -> `chkb`
- `DateTimePicker` -> `dtp`
- `GroupBox` -> `groupBox`
- `TreeView` -> `tvw`
- `CheckedListBox` -> `clb`
- `NumericUpDown` -> `nud`
- `RichTextBox` -> `rtb`
- `FlowLayoutPanel` -> `flp`
- `Panel` -> `pnl`
- `TableLayoutPanel` -> `tlp`
- `ProgressBar` -> `prgb`

## Controles Krypton (tema de la UI)

La UI usa Krypton Toolkit: el prefijo es el del control equivalente, no cambia con el tipo.

| Se usa | En lugar de | Prefijo |
|--------|-------------|---------|
| `KryptonLabel` | `Label` | `lbl` |
| `KryptonButton` (con `ButtonStyle = Custom1`) | `Button` | `btn` |
| `KryptonTextBox` | `TextBox` | `txtb` |
| `KryptonComboBox` | `ComboBox` | `cmb` |
| `KryptonDataGridView` | `DataGridView` | `dgv` |
| `KryptonGroupBox` (hijos en `.Panel.Controls`) | `GroupBox` | `groupBox` |
| `KryptonListBox` | `ListBox` | `lstb` |
| `TpIDS.CheckBoxAccesible` | `CheckBox` | `chkb` |
| `TpIDS.DateTimePickerAccesible` | `DateTimePicker` | `dtp` |

Quedan nativos (tematizados por `TemaFinanzapp`): `TreeView`, `CheckedListBox`, `NumericUpDown`, `RichTextBox` y `MenuStrip`.

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
- `cmbIdiomas`

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
