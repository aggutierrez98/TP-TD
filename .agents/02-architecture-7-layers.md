# Arquitectura por Capas

Este proyecto implementa rigurosamente un modelo por capas con `REPOSITORIO` entre `BLL` y `DAL`. La matriz de dependencias entre proyectos es obligatoria y debe respetarse siempre.

## Matriz obligatoria de dependencias

Solo se permiten estas dependencias de compilacion:

1. **UI/PRESENTACION (`TpIDS`)** -> `ABSTRACCIONES`, `BLL`, `BE`, `SERVICIOS`
2. **CRONS** (consola de tareas programadas; segundo punto de entrada, presentacion como `TpIDS`) -> `ABSTRACCIONES`, `BLL`, `BE`, `SERVICIOS`
3. **BLL** -> `ABSTRACCIONES`, `REPOSITORIO`, `BE`, `SERVICIOS`
4. **REPOSITORIO** -> `ABSTRACCIONES`, `DAL`, `BE`, `SERVICIOS`
5. **DAL** -> `ABSTRACCIONES`, `BE`, `SERVICIOS`
6. **BE** -> `ABSTRACCIONES`
7. **SERVICIOS** -> `ABSTRACCIONES`
8. **ABSTRACCIONES** -> sin dependencias

Cualquier referencia fuera de esa matriz esta prohibida. Todas las capas pueden depender de `ABSTRACCIONES` (nucleo de contratos, sin dependencias) para consumir tipos por interfaz (p. ej. `IPermiso`) y evitar `object`/`dynamic` en `UI`/`BLL`/`REPOSITORIO`/`DAL`. Esto es solo tipado por contrato: no habilita a saltear `BLL` en los flujos de negocio.

## Regla de uso

1. La UI y CRONS pueden conocer `SERVICIOS` y `BE`, pero los casos de uso y flujos de negocio deben entrar por `BLL`.
2. `BLL` centraliza validaciones, reglas de negocio y coordinacion entre `REPOSITORIO` y `SERVICIOS`.
3. `REPOSITORIO` adapta los casos de uso de `BLL` a operaciones persistentes y unidades de trabajo, sin reglas de negocio.
4. `DAL` maneja SQL, mapeos y acceso tecnico a datos; no debe ser invocada desde `UI` ni desde `BLL`.
5. `BE` contiene entidades de negocio y solo puede depender de contratos definidos en `ABSTRACCIONES`.
6. `SERVICIOS` es la capa concreta reusable y solo puede depender de `ABSTRACCIONES`.
7. `ABSTRACCIONES` contiene solo interfaces y enums: contratos sin comportamiento. Ninguna clase, struct ni record, ni siquiera DTOs, excepciones o atributos. Un DTO que comparten capas es una interfaz `I*` en `ABSTRACCIONES` con su implementacion en `SERVICIOS` (p. ej. `IUsuarioSnapshot` / `UsuarioHistorialSnapshot`); una excepcion va en la capa que la lanza.
8. Todas las capas pueden depender de `ABSTRACCIONES` (es el nucleo de contratos). Asi `BLL`/`UI` consumen las utilidades de `SERVICIOS` por interfaz (DIP) y las implementaciones concretas de `SERVICIOS` quedan intercambiables sin tocar a los consumidores. `ABSTRACCIONES` no depende de nada y no contiene tipos del dominio.
9. Solo `SERVICIOS` y `DAL` leen configuracion (`ConfigurationManager`). Cada servicio lee la suya con un ctor que recibe la funcion de configuracion y otro por defecto; ni `BLL` ni `CompositionRoot` leen `App.config`.

## Contratos entre clases: entidades, no IDs

Las clases de todas las capas se comunican pasando entidades (o interfaces de `ABSTRACCIONES`), nunca sus IDs. Es uso de POO, no estilo.

- Las firmas no privadas reciben la entidad: `Actualizar(Categoria categoria)`, `Eliminar(Comercio comercio)`, `GuardarPermisosUsuario(Usuario usuario, IEnumerable<IPermiso> permisos)`. Nada de `int categoriaId` ni `List<int> comercioIds`.
- Los metodos de alta devuelven la entidad creada con su `Id` cargado, no el `int`.
- `BLL` no arma entidades "cascaron" (`new Categoria { Id = categoriaId }`): recibe la entidad real.
- Las entidades de `BE` se relacionan con objetos (`Movimiento.Categoria`), no con FKs `int xxxId`. La propiedad `Id` propia si va.
- La UI y CRONS conservan la entidad en el control (`DataBoundItem`, `Tag` o un campo de la fila) y la pasan tal cual al `BLL`.
- El usuario en sesion se obtiene como `Usuario` desde el punto de acceso unico de `BLL`, no como `int`.
- `DAL` es el unico lugar donde `entidad.Id` se convierte en un `SqlParameter`.
- **Unica excepcion:** `ObtenerPorId(int id)` en `REPOSITORIO` y `DAL`, para rehidratar una entidad cuyo `Id` viene de la base o de la sesion, o para que `BLL` vuelva a leer una entidad recibida antes de validarla.
- Los metodos `private` quedan fuera: la regla es sobre contratos entre clases.

## Guard rail automatica

La solucion incorpora una validacion automatica durante el build para revisar esta matriz sobre los `.csproj`. Si una capa referencia a otra no permitida, el build debe fallar.

Ademas de la matriz de `.csproj`, un segundo guard (`verify-composition-root.ps1`, tambien en el build) vela por la disciplina de composicion/inyeccion: los BLL se construyen solo desde `CompositionRoot` y ninguno actua como Service Locator. Ver la skill `composition-root-guard`.

Un tercer guard (`verify-contratos-capas.ps1`, tambien en el build) vela por las reglas 7 y 9 y por los contratos entre clases: falla si `ABSTRACCIONES` declara una clase, struct o record; si una firma no privada de `BLL`, `REPOSITORIO`, `DAL`, `SERVICIOS` o `ABSTRACCIONES` recibe IDs (salvo `ObtenerPorId`); si `BLL` arma entidades cascaron; si `BE` expone FKs `int xxxId`; o si se lee `ConfigurationManager` fuera de `SERVICIOS` y `DAL`. La validacion es estricta: no admite excepciones por linea base.
