---
name: winforms-ui-control-naming
description: Apply the repository naming convention for WinForms visual controls and event handlers in `.cs` and `.Designer.cs` forms. Use when creating, renaming, or wiring `TextBox`, `DataGridView`, `Label`, `Button`, `PictureBox`, `ComboBox`, or WinForms events so control names use the required prefixes and handlers use `<controlName>_<Event>`.
---

# WinForms UI Control Naming

Aplicar esta skill en cualquier cambio de formularios WinForms que agregue, renombre o conecte elementos visuales.

## Repository standard

Para mantener la solucion consistente con WinForms y con la convencion pedida para el TP:

- nombre del control visual: `<prefijo><Descripcion>`
- nombre del handler: `<nombreDelControl>_<Evento>`

La tabla de prefijos y ejemplos concretos esta en [references/control-naming.md](references/control-naming.md).

## Non-negotiable rules

1. Renombrar siempre en pareja: archivo `.cs` y `.Designer.cs`.
2. Actualizar tambien el `Name` del control dentro de `InitializeComponent`.
3. Mantener la descripcion corta, concreta y orientada a funcion.
4. No dejar nombres genericos como `button1`, `label2`, `textBoxPassword`.
5. Si se cambia un handler, actualizar la suscripcion del evento.

## Procedure

1. Identificar controles nuevos o afectados.
2. Aplicar la convencion del repo.
3. Revisar declaraciones de campos, referencias, handlers y `InitializeComponent`.
4. Compilar la solucion al terminar.

## Clarification

La parte con sufijo de evento aplica al handler, no al nombre base del control. Ejemplo correcto:

- control: `btnIngreso`
- handler: `btnIngreso_Click`
