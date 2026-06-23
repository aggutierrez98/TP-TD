# Dependency Matrix

## Allowed project references

- `TpIDS` -> `BLL`, `BE`, `SERVICIOS`
- `BLL` -> `REPOSITORIO`, `BE`, `SERVICIOS`
- `REPOSITORIO` -> `DAL`, `BE`, `SERVICIOS`
- `DAL` -> `BE`, `SERVICIOS`
- `BE` -> `ABSTRACCIONES`
- `SERVICIOS` -> `ABSTRACCIONES`
- `ABSTRACCIONES` -> none

## Interpretation rules

- La matriz es estricta: si una referencia no aparece arriba, esta prohibida.
- La UI no debe usar la existencia de la referencia a `SERVICIOS` o `BE` para saltearse `BLL` en los flujos de negocio.
- `BLL` no debe invocar `DAL` directamente; toda persistencia debe pasar por `REPOSITORIO`.
- `SERVICIOS` es la capa reusable concreta; `ABSTRACCIONES` es solo para contratos.
- Si una clase reusable no encaja como entidad pura de negocio, no debe empujarse a `BE` por conveniencia.
