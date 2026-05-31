---
name: architecture-dependency-guard
description: Enforce the mandatory layer dependency matrix for TpIDS. Use when adding features, moving classes between layers, changing project references, introducing new services/entities/repositories, or touching architecture-related code.
---

# Architecture Dependency Guard

Aplicar esta skill cada vez que el cambio pueda afectar dependencias entre capas o mover responsabilidades de una capa a otra.

## Required inputs

Leer siempre:

1. `.agents/02-architecture-7-layers.md`
2. [references/dependency-matrix.md](references/dependency-matrix.md)

## Required workflow

1. Verificar la capa destino antes de mover clases, crear servicios, repositorios, entidades o contratos.
2. Respetar la matriz obligatoria de dependencias entre proyectos sin excepciones ad hoc.
3. Mantener la regla de uso: la UI puede conocer `SERVICIOS` y `BE`, pero los flujos funcionales deben entrar por `BLL`.
4. Si se toca un `.csproj`, se mueve codigo entre proyectos o hay dudas sobre la capa correcta, ejecutar `powershell -NoProfile -ExecutionPolicy Bypass -File .agents\\scripts\\verify-architecture-dependencies.ps1`.
5. Tratar cualquier violacion de la matriz como bloqueante.
6. Cerrar la tarea solo despues de que la guardia y el build queden en verde.

## Forbidden patterns

- agregar una referencia `SERVICIOS -> BE`, `SERVICIOS -> DAL` o `SERVICIOS -> BLL`
- agregar una referencia `BE -> SERVICIOS`, `BE -> DAL`, `BE -> BLL` o `BE -> UI`
- invocar `DAL` directamente desde `UI`
- usar `ABSTRACCIONES` para alojar implementaciones concretas
- crear bypasses a `BLL` para resolver casos de uso desde `UI`

## Response rule

- Informar si la guardia de arquitectura fue ejecutada o quedo validada via build.
- Si una propuesta requiere romper la matriz, detenerse y explicitar que la arquitectura del repo lo prohibe.
