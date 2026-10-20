# Proposal

## Why

La matriz de referencias entre proyectos se cumple, pero las clases de las capas se comunican pasando IDs en lugar de entidades. Hay unas 94 firmas en BLL, unas 275 en REPOSITORIO y unas 139 en DAL que reciben `int id`/`xxxId`, 22 entidades "cascaron" (`new Categoria { Id = categoriaId }`) armadas en BLL y unas 57 llamadas de la UI que reducen el objeto seleccionado a su `Id`. Ademas, ABSTRACCIONES contiene clases concretas (DTOs, excepciones, un atributo), cuando la regla de la arquitectura es que solo tenga contratos sin comportamiento. Ninguna guardia del build controla estas dos reglas, por eso cada cambio nuevo suma mas violaciones.

## What Changes

- **Regla escrita**: las clases de todas las capas se comunican pasando entidades (o interfaces de ABSTRACCIONES), no IDs. `.agents/02-architecture-7-layers.md`, `AGENTS.md` y la skill `architecture-dependency-guard` la documentan.
- **ABSTRACCIONES solo con interfaces y enums**: se retiran las clases concretas. Las excepciones van a SERVICIOS, `DigitoVerificadorAttribute` va a BE y cada DTO pasa a ser una interfaz `I*` en ABSTRACCIONES con su implementacion en SERVICIOS.
- **Guardia nueva en el build** (`verify-contratos-capas.ps1`): falla si ABSTRACCIONES declara una clase o struct; si una firma no privada de BLL, REPOSITORIO, DAL, SERVICIOS o ABSTRACCIONES recibe IDs; si BLL arma entidades cascaron; si BE expone FKs `int xxxId`; o si se lee `ConfigurationManager` fuera de SERVICIOS y DAL. Funciona como trinquete: una linea base congela las violaciones actuales, rechaza las nuevas y se vacia a medida que se migra cada modulo.
- **Sesion sin IDs**: **BREAKING** (interno) `ISesion` pierde `GetUsuarioActualId()` y `Sesion` pierde `IniciarSesion(int, string)`. BLL obtiene el `Usuario` en sesion desde un unico punto de acceso.
- **CompositionRoot sin `ConfigurationManager`**: el limite de uso del asistente pasa a SERVICIOS detras de una interfaz y lee su propia configuracion, como el resto de los servicios.
- **Migracion por modulos**: DAL, REPOSITORIO, BLL, UI y tests de cada modulo pasan a recibir y devolver entidades. Los metodos de alta devuelven la entidad creada, no su `Id`. Las propiedades FK `int xxxId` de BE pasan a ser referencias a objetos. Se empieza por un modulo piloto (Categorias) que fija el patron.

El comportamiento observable de la aplicacion no cambia: es un refactor de contratos internos con tooling y documentacion.

## Capabilities

### New Capabilities

Ninguna. El change declara `skip_specs: true` porque no cambia el comportamiento del sistema. Las reglas de arquitectura viven en `.agents/`, igual que la matriz de capas vigente.

### Modified Capabilities

Ninguna.

## Impact

- **Codigo**: casi todos los archivos de BLL, REPOSITORIO y DAL; los formularios de UI que llaman a esos BLL; ABSTRACCIONES y SERVICIOS por los DTOs y la sesion; BE por las FKs; `BLL/CompositionRoot.cs`.
- **Tests**: `tests/BLL.Tests`, `tests/SERVICIOS.Tests`, `tests/TpIDS.IntegrationTests` y `tests/TpIDS.UiTests` se adaptan a las firmas nuevas. Los tests de `LimiteUsoAsistente` pasan a `SERVICIOS.Tests`.
- **Build**: `Directory.Build.targets` suma la guardia nueva junto a las dos existentes.
- **Documentacion**: `.agents/02-architecture-7-layers.md`, `AGENTS.md` y la skill `architecture-dependency-guard`. Los diagramas de clases G07/G08 cambian de firmas; ese pendiente se registra en `actualizar-diagramas-tp`.
- **Cambios en curso**: `unificar-crons`, `notificaciones-automaticas-diarias`, `perfil-inversor-usuario` y `listas-filtros-escalables` tocan modulos afectados. El trinquete evita que sumen violaciones nuevas mientras tanto.
- **Sin dependencias nuevas** y sin impacto en la restriccion de compilar sin NuGet.
