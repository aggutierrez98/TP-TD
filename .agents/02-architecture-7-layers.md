# Arquitectura de 6 Capas

Este proyecto implementa rigurosamente un modelo simple de 6 capas, sin capa REPOSITORIO, sin Unit of Work y sin Composition Root: `BLL` coordina directamente `DAL` y `SERVICIOS`. Esa simplicidad es una decision de diseno de esta rama y debe preservarse. La matriz de dependencias entre proyectos es obligatoria y debe respetarse siempre.

## Matriz obligatoria de dependencias

Solo se permiten estas dependencias de compilacion:

1. **UI/PRESENTACION (`TpIDS`)** -> `ABSTRACCIONES`, `BLL`, `BE`, `SERVICIOS`
2. **BLL** -> `ABSTRACCIONES`, `DAL`, `BE`, `SERVICIOS`
3. **DAL** -> `ABSTRACCIONES`, `BE`, `SERVICIOS`
4. **BE** -> `ABSTRACCIONES`
5. **SERVICIOS** -> `ABSTRACCIONES`
6. **ABSTRACCIONES** -> sin dependencias

Cualquier referencia fuera de esa matriz esta prohibida. Todas las capas pueden depender de `ABSTRACCIONES` (nucleo de contratos, sin dependencias) para consumir tipos por interfaz (p. ej. `IPermiso`) y evitar `object`/`dynamic` en `UI`/`BLL`/`DAL`. Esto es solo tipado por contrato: no habilita a saltear `BLL` en los flujos de negocio ni a introducir capa REPOSITORIO, Unit of Work o Composition Root.

## Regla de uso

1. La UI puede conocer `SERVICIOS` y `BE`, pero los casos de uso y flujos de negocio deben entrar por `BLL`.
2. `BLL` centraliza validaciones, reglas de negocio y coordinacion entre `DAL` y `SERVICIOS`.
3. `DAL` maneja persistencia y no debe ser invocada desde `UI`.
4. `BE` contiene entidades de negocio y solo puede depender de contratos definidos en `ABSTRACCIONES`.
5. `SERVICIOS` es la capa concreta reusable y solo puede depender de `ABSTRACCIONES`.
6. `ABSTRACCIONES` debe contener solo interfaces o contratos. No debe contener implementaciones concretas. Cualquier capa puede referenciarla para tipar por contrato; `ABSTRACCIONES` no depende de nada ni contiene tipos del dominio ni concretos.

## Guard rail automatica

La solucion incorpora una validacion automatica durante el build para revisar esta matriz sobre los `.csproj`. Si una capa referencia a otra no permitida, el build debe fallar.
