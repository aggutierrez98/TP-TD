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
   Si se cambian firmas entre capas, ABSTRACCIONES o entidades BE, ejecutar tambien `.agents\\scripts\\verify-contratos-capas.ps1` (regla "Contratos entre clases" de `.agents/02-architecture-7-layers.md`).
5. Tratar cualquier violacion de la matriz como bloqueante.
6. Cerrar la tarea solo despues de que la guardia y el build queden en verde.

## Forbidden patterns

- agregar una referencia `SERVICIOS -> BE`, `SERVICIOS -> DAL` o `SERVICIOS -> BLL`
- agregar una referencia `BE -> SERVICIOS`, `BE -> DAL`, `BE -> BLL` o `BE -> UI`
- agregar una referencia `BLL -> DAL`
- invocar `DAL` directamente desde `UI`
- invocar `DAL` directamente desde `BLL`
- usar `ABSTRACCIONES` para alojar implementaciones concretas: solo admite interfaces y enums (ni DTOs, ni excepciones, ni atributos)
- recibir IDs (`int xxxId`, `List<int> xxxIds`) en firmas no privadas de `BLL`, `REPOSITORIO`, `DAL`, `SERVICIOS` o `ABSTRACCIONES` en lugar de la entidad; unica excepcion `ObtenerPorId(int id)` en `REPOSITORIO`/`DAL`
- armar entidades cascaron `new X { Id = ... }` en `BLL` o exponer FKs `int xxxId` en `BE`
- leer `ConfigurationManager` fuera de `SERVICIOS` y `DAL` (incluido `CompositionRoot`)
- crear bypasses a `BLL` para resolver casos de uso desde `UI`

## Related

- Esta skill vela por la matriz de referencias entre `.csproj`. La disciplina de COMO se construyen e inyectan las instancias dentro de esas capas (Composition Root, sin `new` de BLL fuera del root, sin Service Locator escondido) la cubre [composition-root-guard](../composition-root-guard/SKILL.md). Aplicar ambas cuando se agregan BLLs o servicios.

## Response rule

- Informar si la guardia de arquitectura fue ejecutada o quedo validada via build.
- Si una propuesta requiere romper la matriz, detenerse y explicitar que la arquitectura del repo lo prohibe.
