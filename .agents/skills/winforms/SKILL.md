---
name: winforms
description: WinForms UI patterns for data binding, async operations, validation and thread safety. Use when trabajando con controles, BindingSource, ErrorProvider, operaciones async en la UI o validacion de formularios. Source: skills.managed-code.com/skills/winforms.
---

# WinForms

Skill de patrones UI para aplicaciones Windows Forms. Cubre data binding, operaciones asincronicas, validacion de formularios y thread safety.

## CONFLICTOS CON ESTE PROYECTO — LEER ANTES DE APLICAR

Este skill proviene de un contexto generico y promueve tres patrones que estan **explicitamente prohibidos** en esta arquitectura. Ignorar esas secciones por completo:

### 1. MVP / Passive View / MVVM — PROHIBIDO

El skill recomienda que cada `Form` implemente una interfaz `IView` y delegue toda logica a una clase `Presenter`. En este proyecto eso no aplica:

- La capa UI habla directamente con BLL. No hay Presenter.
- Agregar un Presenter significaria crear una capa nueva (violacion de la arquitectura de 6 capas) o meterlo en UI (violacion de responsabilidad de UI).
- Ver reglas transversales en `AGENTS.md`: "sin Unit of Work y sin Composition Root; esa simplicidad es una decision de diseno".

**No sugerir, no implementar, no mencionar MVP ni Presenter en este proyecto.**

### 2. Inyeccion de Dependencias (DI) — PROHIBIDO

El skill recomienda wiring de servicios via `IServiceCollection` desde `Program.cs`. En este proyecto:

- Las BLLs construyen sus dependencias con `new` directo en el bootstrap de la aplicacion.
- No hay `IServiceCollection`, ni `IServiceProvider`, ni ningun contenedor DI.
- Ver skill `architecture-dependency-guard` y reglas transversales: "sin Composition Root".

**No sugerir DI, contenedores IoC ni constructor injection en BLL/DAL.**

### 3. Patrones .NET 6+ — NO APLICAN

Varias guias del skill asumen .NET 6+ (`Program.cs` con top-level statements, button commands, stock icons de .NET 8). Este proyecto corre en **.NET Framework 4.7.2**. Ignorar cualquier referencia a:
- `Microsoft.Extensions.DependencyInjection`
- `IHostBuilder`, `CreateDefaultBuilder`
- Button commands (.NET 8+)

---

## Que SÍ aplica a este proyecto

### Data binding

- Usar `BindingSource` para desacoplar datos del control: evita populacion manual de controles en eventos.
- Implementar `INotifyPropertyChanged` en los BE (Business Entities) si se necesita two-way binding reactivo.
- Preferir `BindingSource.DataSource` sobre asignar directamente a `DataGridView.DataSource` cuando hay filtrado o navegacion.

### Validacion de formularios

- Usar `ErrorProvider` con el evento `Validating` de cada control para mostrar errores inline.
- Llamar `ValidateChildren()` antes de cualquier operacion de persistencia (submit a BLL).
- No mezclar validacion de UI con logica de negocio: la UI valida formato y presencia; la BLL valida reglas de negocio.

### Async y thread safety

- Nunca bloquear el hilo de UI con `.Result` o `.Wait()` sobre Tasks.
- Usar `async/await` para cualquier operacion I/O (llamadas a DAL, lectura de archivos).
- Deshabilitar controles durante cargas async; rehabilitarlos en `finally`.
- Para reportar progreso: `Progress<T>` con callback en el contexto de UI.
- Actualizaciones de controles desde otro hilo: `control.Invoke(() => ...)` (en .NET Framework no hay `SynchronizationContext` automatico en todos los casos).

### Diseñador y controles

- No editar `.Designer.cs` manualmente; usar el Designer de Visual Studio.
- El codigo de logica va en el `.cs` del form, nunca en el `.Designer.cs`.
- Ver skill `winforms-ui-control-naming` para la nomenclatura obligatoria de controles en este proyecto.

## Recursos del skill original

- `references/patterns.md`: leer SOLO las secciones de data binding, threading y validacion. Ignorar MVP, DI y .NET 6+.
- `references/migration.md`: referencia futura si se decide migrar a .NET moderno. No aplica hoy.

## Related

- [`../winforms-build-verification/SKILL.md`](../winforms-build-verification/SKILL.md): compilacion obligatoria antes de cerrar tarea.
- [`../winforms-ui-control-naming/SKILL.md`](../winforms-ui-control-naming/SKILL.md): nomenclatura de controles WinForms.
- [`../architecture-dependency-guard/SKILL.md`](../architecture-dependency-guard/SKILL.md): matriz de dependencias entre capas.
