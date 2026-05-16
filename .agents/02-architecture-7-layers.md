# Arquitectura de 7 Capas

Este proyecto implementa rigurosamente el modelo de 7 capas. La matriz de dependencias entre proyectos es obligatoria y debe respetarse siempre.

## Matriz obligatoria de dependencias

Solo se permiten estas dependencias de compilacion:

1. **UI/PRESENTACION (`TpIDS`)** -> `BLL`, `BE`, `SERVICIOS`
2. **BLL** -> `DAL`, `BE`, `SERVICIOS`
3. **DAL** -> `BE`, `SERVICIOS`
4. **BE** -> `ABSTRACCIONES`
5. **SERVICIOS** -> `ABSTRACCIONES`
6. **ABSTRACCIONES** -> sin dependencias

Cualquier referencia fuera de esa matriz esta prohibida.

## Regla de uso

1. La UI puede conocer `SERVICIOS` y `BE`, pero los casos de uso y flujos de negocio deben entrar por `BLL`.
2. `BLL` centraliza validaciones, reglas de negocio y coordinacion entre `DAL` y `SERVICIOS`.
3. `DAL` maneja persistencia y no debe ser invocada desde `UI`.
4. `BE` contiene entidades de negocio y solo puede depender de contratos definidos en `ABSTRACCIONES`.
5. `SERVICIOS` es la capa concreta reusable y solo puede depender de `ABSTRACCIONES`.
6. `ABSTRACCIONES` debe contener solo interfaces o contratos. No debe contener implementaciones concretas.

## Guard rail automatica

La solucion incorpora una validacion automatica durante el build para revisar esta matriz sobre los `.csproj`. Si una capa referencia a otra no permitida, el build debe fallar.
