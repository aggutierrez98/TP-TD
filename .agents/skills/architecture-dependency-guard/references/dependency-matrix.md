# Dependency Matrix

## Allowed project references

- `TpIDS` -> `ABSTRACCIONES`, `BLL`, `BE`, `SERVICIOS`
- `BLL` -> `ABSTRACCIONES`, `DAL`, `BE`, `SERVICIOS`
- `DAL` -> `ABSTRACCIONES`, `BE`, `SERVICIOS`
- `BE` -> `ABSTRACCIONES`
- `SERVICIOS` -> `ABSTRACCIONES`
- `ABSTRACCIONES` -> none

## Interpretation rules

- La matriz es estricta: si una referencia no aparece arriba, esta prohibida.
- La UI no debe usar la existencia de la referencia a `SERVICIOS` o `BE` para saltearse `BLL` en los flujos de negocio.
- `SERVICIOS` es la capa reusable concreta; `ABSTRACCIONES` es solo para contratos.
- Todas las capas pueden depender de `ABSTRACCIONES` (nucleo de contratos, sin dependencias) para tipar por interfaz (p. ej. `IPermiso`) y evitar `object`/`dynamic` en `UI`/`BLL`/`DAL`. Referenciar `ABSTRACCIONES` es solo tipado por contrato: no habilita saltear `BLL` ni introducir REPOSITORIO/UoW/Composition Root. `ABSTRACCIONES` no depende de nada y no debe contener tipos del dominio ni concretos.
- Si una clase reusable no encaja como entidad pura de negocio, no debe empujarse a `BE` por conveniencia.
