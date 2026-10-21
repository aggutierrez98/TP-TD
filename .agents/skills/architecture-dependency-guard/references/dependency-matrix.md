# Dependency Matrix

## Allowed project references

- `TpIDS` -> `ABSTRACCIONES`, `BLL`, `BE`, `SERVICIOS`
- `CRONS` (consola de tareas programadas, capa de presentacion como `TpIDS`) -> `ABSTRACCIONES`, `BLL`, `BE`, `SERVICIOS`
- `BLL` -> `ABSTRACCIONES`, `REPOSITORIO`, `BE`, `SERVICIOS`
- `REPOSITORIO` -> `ABSTRACCIONES`, `DAL`, `BE`, `SERVICIOS`
- `DAL` -> `ABSTRACCIONES`, `BE`, `SERVICIOS`
- `BE` -> `ABSTRACCIONES`
- `SERVICIOS` -> `ABSTRACCIONES`
- `ABSTRACCIONES` -> none

## Interpretation rules

- La matriz es estricta: si una referencia no aparece arriba, esta prohibida.
- La UI no debe usar la existencia de la referencia a `SERVICIOS` o `BE` para saltearse `BLL` en los flujos de negocio.
- `BLL` no debe invocar `DAL` directamente; toda persistencia debe pasar por `REPOSITORIO`.
- `SERVICIOS` es la capa reusable concreta; `ABSTRACCIONES` es solo para contratos.
- Todas las capas pueden depender de `ABSTRACCIONES` (nucleo de contratos) para consumir las utilidades por interfaz (DIP); asi las implementaciones de `SERVICIOS` son intercambiables sin tocar a los consumidores. `ABSTRACCIONES` no depende de nada y contiene solo interfaces y enums: ninguna clase, struct ni record.
- Las clases de todas las capas se pasan entidades, no IDs (ver "Contratos entre clases" en `.agents/02-architecture-7-layers.md`).
- Si una clase reusable no encaja como entidad pura de negocio, no debe empujarse a `BE` por conveniencia.
