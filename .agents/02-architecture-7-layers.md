# Arquitectura por Capas

Este proyecto implementa rigurosamente un modelo por capas con `REPOSITORIO` entre `BLL` y `DAL`. La matriz de dependencias entre proyectos es obligatoria y debe respetarse siempre.

## Matriz obligatoria de dependencias

Solo se permiten estas dependencias de compilacion:

1. **UI/PRESENTACION (`TpIDS`)** -> `ABSTRACCIONES`, `BLL`, `BE`, `SERVICIOS`
2. **BLL** -> `ABSTRACCIONES`, `REPOSITORIO`, `BE`, `SERVICIOS`
3. **REPOSITORIO** -> `ABSTRACCIONES`, `DAL`, `BE`, `SERVICIOS`
4. **DAL** -> `ABSTRACCIONES`, `BE`, `SERVICIOS`
5. **BE** -> `ABSTRACCIONES`
6. **SERVICIOS** -> `ABSTRACCIONES`
7. **ABSTRACCIONES** -> sin dependencias

Cualquier referencia fuera de esa matriz esta prohibida.

## Regla de uso

1. La UI puede conocer `SERVICIOS` y `BE`, pero los casos de uso y flujos de negocio deben entrar por `BLL`.
2. `BLL` centraliza validaciones, reglas de negocio y coordinacion entre `REPOSITORIO` y `SERVICIOS`.
3. `REPOSITORIO` adapta los casos de uso de `BLL` a operaciones persistentes y unidades de trabajo, sin reglas de negocio.
4. `DAL` maneja SQL, mapeos y acceso tecnico a datos; no debe ser invocada desde `UI` ni desde `BLL`.
5. `BE` contiene entidades de negocio y solo puede depender de contratos definidos en `ABSTRACCIONES`.
6. `SERVICIOS` es la capa concreta reusable y solo puede depender de `ABSTRACCIONES`.
7. `ABSTRACCIONES` debe contener solo interfaces o contratos. No debe contener implementaciones concretas.
8. Todas las capas pueden depender de `ABSTRACCIONES` (es el nucleo de contratos). Asi `BLL`/`UI` consumen las utilidades de `SERVICIOS` por interfaz (DIP) y las implementaciones concretas de `SERVICIOS` quedan intercambiables sin tocar a los consumidores. `ABSTRACCIONES` no depende de nada y no debe contener tipos del dominio ni concretos; solo contratos estables.

## Guard rail automatica

La solucion incorpora una validacion automatica durante el build para revisar esta matriz sobre los `.csproj`. Si una capa referencia a otra no permitida, el build debe fallar.

Ademas de la matriz de `.csproj`, un segundo guard (`verify-composition-root.ps1`, tambien en el build) vela por la disciplina de composicion/inyeccion: los BLL se construyen solo desde `CompositionRoot` y ninguno actua como Service Locator. Ver la skill `composition-root-guard`.
