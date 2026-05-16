---
name: winforms-build-verification
description: Enforce end-of-task build verification for this repository's .NET Framework 4.7.2 WinForms solution. Use when adding or modifying features, bugfixes, forms, resources, project files, references, or event wiring so Codex always compiles `TpIDS.sln` with MSBuild before handoff and resolves build errors first.
---

# WinForms Build Verification

Aplicar esta skill cada vez que se cambie cualquier proyecto de la solucion.

## Required workflow

1. Leer primero `.agents/01-stack-dotnet-winforms.md` y `.agents/02-architecture-7-layers.md`.
2. Implementar el cambio solicitado.
3. Compilar `TpIDS.sln` antes de responder al usuario. Ese build tambien debe dejar en verde la guardia automatica de arquitectura.
4. Si la compilacion falla, corregir y recompilar.
5. Cerrar la tarea solo con compilacion exitosa.

## Build gate

Usar el comando documentado en [references/build-command.md](references/build-command.md).

Esto es obligatorio especialmente cuando el cambio toca:

- formularios WinForms
- `.Designer.cs`
- `Resources`
- referencias entre proyectos
- cambios de nombres
- eventos
- archivos `.csproj`
- movimiento de clases entre capas

## Response rule

- Informar explicitamente si la solucion compilo.
- Si no fue posible compilar por una limitacion real del entorno, decirlo y explicar por que.
- No asumir que cambios chicos no requieren build.
