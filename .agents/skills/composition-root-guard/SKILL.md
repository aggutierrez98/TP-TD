---
name: composition-root-guard
description: Enforce the Composition Root discipline for TpIDS. Use when creating or modifying a BLL, adding a transversal SERVICIOS service, changing constructors/dependencies of a BLL, wiring dependencies, or touching CompositionRoot / application startup. Complements architecture-dependency-guard.
---

# Composition Root Guard

Aplicar esta skill cada vez que se cree o modifique un BLL, se agregue un servicio de `SERVICIOS`, se cambien constructores o dependencias, o se toque el arranque de la aplicacion. Es complementaria a [architecture-dependency-guard](../architecture-dependency-guard/SKILL.md): esa vela por la matriz de `.csproj`; esta vela por COMO se construyen e inyectan las instancias dentro de esa matriz.

## Required inputs

Leer siempre antes de tocar composicion:

1. [BLL/CompositionRoot.cs](../../../BLL/CompositionRoot.cs) — la unica fuente y via de construccion.
2. [BLL/ContratosInternosBLL.cs](../../../BLL/ContratosInternosBLL.cs) — contratos `internal` de composicion entre BLL.
3. Esta skill (reglas y flujos de alta).

## Principio

`CompositionRoot` es la **unica fuente y via de construccion** de servicios transversales y de los BLL de caso de uso. Se dispara una vez desde `Program.Main` (y los getters auto-inicializan). Cada BLL tiene **un solo constructor `internal`** que recibe todas sus dependencias por parametro; **no** tiene constructor por defecto y **no** nombra a `CompositionRoot`. Asi:

- hay una sola instancia compartida de cada servicio y BLL (no se reconstruyen arboles),
- las dependencias son explicitas (inyeccion por constructor, no Service Locator escondido),
- el compilador garantiza que nada construya un BLL por fuera del root (ctor `internal`).

## Como se usa (Do)

- Para obtener un BLL o servicio ya compuesto: `CompositionRoot.<Getter>` (p. ej. `CompositionRoot.Usuario`, `CompositionRoot.Permiso`, `CompositionRoot.Idioma`, `CompositionRoot.Integridad`, `CompositionRoot.GestorBitacora`, `CompositionRoot.Sesion`, `CompositionRoot.Autorizacion`).
- En la UI, inicializar los campos de BLL con el getter: `private readonly UsuarioBLL _bll = CompositionRoot.Usuario;`.
- Bitacora tiene dos getters, misma instancia: `Bitacora` (contrato `IBitacora`, para consumo transversal) y `GestorBitacora` (tipo concreto `BitacoraBLL`, necesario para `FormBitacora` que usa `Buscar(...)`, ausente en `IBitacora`).
- El arranque funcional (pasos de negocio al iniciar) va en [BLL/InicializadorAplicacionBLL.cs](../../../BLL/InicializadorAplicacionBLL.cs), consumiendo instancias del root — nunca en `Program.cs` ni dentro de `CompositionRoot.Inicializar()`.

## Que NO se puede hacer (Forbidden patterns)

- **`new <X>BLL(...)` fuera de `BLL/CompositionRoot.cs`.** Ningun Form, ningun BLL, ninguna otra clase construye un BLL. Se obtiene por getter.
- **Agregar un constructor por defecto (`public XxxBLL()`) a un BLL.** El unico ctor es el `internal` con dependencias.
- **Nombrar `CompositionRoot` dentro de un BLL de caso de uso** (`CompositionRoot.Sesion`, etc. en `UsuarioBLL`/`PermisoBLL`/`UsuarioEstadoBLL`/`IdiomaBLL`/`IntegridadBLL`/`BitacoraBLL`). Eso reintroduce el Service Locator. Las dependencias entran por el ctor.
- **Usar `CompositionRoot` como localizador dentro de metodos** (resolver dependencias en medio de la logica). Los getters son para armar el grafo/consumir, no para pedir servicios ad hoc en cada metodo.
- **Meter arranque funcional o acceso a base en `CompositionRoot.Inicializar()`.** El root solo compone (sin efectos de negocio); `Inicializar()` se dispara desde getters y desde `ManejadorExcepcionesBLL`, y un efecto colateral ahi puede lanzar excepciones desde el manejador de excepciones.
- **Construir el mismo BLL/servicio dos veces.** Reutilizar el campo ya creado (`_bitacoraBLL`, `_usuarioEstadoBLL`, ...).

Excepcion consciente (allow-list): [BLL/ManejadorExcepcionesBLL.cs](../../../BLL/ManejadorExcepcionesBLL.cs) puede usar `CompositionRoot.Bitacora` en un metodo — es un helper estatico sin ctor donde inyectar. [BLL/InicializadorAplicacionBLL.cs](../../../BLL/InicializadorAplicacionBLL.cs) tambien puede nombrar el root (consume instancias compuestas).

## Como agregar cosas al grafo de dependencias

### A) Nuevo BLL de caso de uso
1. Crear `XxxBLL` en `BLL/` con **un solo ctor `internal`** que reciba sus dependencias por contrato: repositorios (`I...Repositorio` de REPOSITORIO), servicios transversales (contratos de ABSTRACCIONES) y otros BLL (por su interfaz `internal` de `ContratosInternosBLL.cs`). Guardar cada una en un campo `readonly`. **Sin** ctor por defecto y **sin** nombrar `CompositionRoot`.
2. Si otros BLL van a depender de el, declarar `internal interface IXxxBLL` en `ContratosInternosBLL.cs` (solo los metodos de composicion/UoW, con `IUnidadDeTrabajo` donde aplique) y que `XxxBLL` lo implemente.
3. En `CompositionRoot.Inicializar()`, construirlo **una** vez, **despues** de sus dependencias (orden hoja -> raiz), **reutilizando** las instancias ya creadas. Guardarlo en un campo `private static XxxBLL _xxxBLL;`.
4. Si la UI lo necesita, exponer getter concreto: `public static XxxBLL Xxx { get { Inicializar(); return _xxxBLL; } }`.
5. En la UI, obtenerlo con `CompositionRoot.Xxx` (nunca `new`).
6. Correr el guard (build o script) hasta verde.

### B) Nuevo servicio transversal (SERVICIOS)
1. Definir el contrato en `ABSTRACCIONES` (solo interfaz).
2. Implementarlo en `SERVICIOS`.
3. En `CompositionRoot`: campo `private static IXxx _xxx;`, instanciarlo en `Inicializar()` (singleton via `GetInstance()` si tiene estado; `new` si no lo tiene) y exponer `public static IXxx Xxx { get { Inicializar(); return _xxx; } }`.
4. Inyectarlo a los BLL que lo requieran agregando el parametro a su ctor `internal` y pasando `_xxx` desde el root.

### C) Nueva dependencia en un BLL existente
1. Agregar el parametro al ctor `internal` + su campo `readonly`.
2. En `CompositionRoot`, pasar la instancia correspondiente en la llamada de construccion. Si la dependencia es otro BLL, asegurarse de que se construye **antes** (respetar el orden hoja -> raiz).
3. No agregar ctor por defecto ni `CompositionRoot.X` dentro del BLL.

## Otras restricciones de arquitectura de valor

- **Orden hoja -> raiz y grafo aciclico.** El root arma de las hojas hacia arriba. Orden actual: `BitacoraBLL`/`UsuarioEstadoBLL` -> `IntegridadBLL` -> `IdiomaBLL`/`PermisoBLL` -> `UsuarioBLL`. Si aparece un ciclo (A necesita B y B necesita A), es un smell de diseno: romperlo (extraer responsabilidad, mediar por evento/servicio), no forzarlo.
- **Composicion vs arranque, separados.** `CompositionRoot` compone; `InicializadorAplicacionBLL` orquesta pasos de arranque; `Program.cs` solo levanta WinForms, configura excepciones globales y decide el form. En `Program.Main`: `CompositionRoot.Inicializar()` **fuera** del `try` (tecnico, no lanza excepciones de negocio); `InicializadorAplicacionBLL.Inicializar()` **dentro** del `try` (puede lanzar `ExcepcionIntegridad`). El orden interno del arranque importa: idioma **antes** que integridad (para que una `ExcepcionIntegridad` se muestre traducida).
- **Getters concretos, sin contratos publicos nuevos.** Los getters de BLL devuelven el tipo concreto a proposito (la UI ya referencia `BLL`). No se crean interfaces publicas de caso de uso (`IServicioUsuarios`, etc.): eso seria otro diseno ("camino 2") que exige decidir donde viven contratos que dependen de `BE` y NO se hace sin decision explicita, para no ensuciar `ABSTRACCIONES`.
- **Tests (parkeados).** El ctor `internal` permite inyectar dobles desde el proyecto de tests via `[assembly: InternalsVisibleTo("<Tests>")]` en el `AssemblyInfo` de `BLL`. Es la via de testeo prevista; no reintroducir ctores por defecto para "facilitar" tests.

## Guardia automatica

Se ejecuta en cada build via [Directory.Build.targets](../../../Directory.Build.targets) (target `VerifyCompositionRoot`, gateado por `ArchitectureGuardEnabled`). Falla el build ante cualquier violacion. Ejecucion manual:

```
powershell -NoProfile -ExecutionPolicy Bypass -File .agents\scripts\verify-composition-root.ps1
```

Reglas que verifica [.agents/scripts/verify-composition-root.ps1](../../../.agents/scripts/verify-composition-root.ps1):

1. `new-fuera-del-root`: ningun `new <X>BLL(` fuera de `BLL/CompositionRoot.cs`.
2. `service-locator-en-bll`: ningun `.cs` de `BLL/` nombra `CompositionRoot`, salvo la allow-list (`CompositionRoot.cs`, `InicializadorAplicacionBLL.cs`, `ManejadorExcepcionesBLL.cs`).

Si se agrega un archivo legitimamente autorizado a nombrar el root (raro), actualizar `$serviceLocatorAllowList` en el script con justificacion.

## Response rule

- Informar si la guardia del Composition Root fue ejecutada o quedo validada via build.
- Si una propuesta requiere `new` de un BLL fuera del root, un ctor por defecto, o resolver dependencias via `CompositionRoot` dentro de un BLL, detenerse y explicitar que el patron del repo lo prohibe; ofrecer la alternativa correcta (getter / inyeccion por ctor / alta en el grafo).
