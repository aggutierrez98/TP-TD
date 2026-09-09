---
name: tp-use-case-spec
description: Genera especificaciones de casos de uso del TP con el formato estandar (ID y Nombre, Estado, Descripcion, Actor Principal, Actor Secundario, Precondiciones, Puntos de Extension, Condicion, Escenario Principal, Flujos Alternativos, Postcondiciones) a partir de un JSON, produciendo un .docx y un .md espejo. Usar cuando haya que documentar, crear o actualizar casos de uso de un requisito.
---

# TP Use Case Spec

Aplicar esta skill cuando haya que crear o actualizar especificaciones de casos de uso del TP.

## Cuando ejecutar (a demanda)

- Solo cuando el comportamiento del caso de uso este definido o implementado.
- No es automatica tras cada cambio de codigo: se ejecuta a pedido para documentar uno o
  mas casos de uso de un requisito (`T02`, `T03`, `T04`, `T05`, `T06a`, `T06b`, `T07`, ...).

## Read first

1. [references/use-case-spec-format.md](references/use-case-spec-format.md): formato y semantica
   exacta de cada campo, convenciones y esquema JSON.
2. [examples/ejemplo-casos-de-uso.json](examples/ejemplo-casos-de-uso.json): ejemplo completo.
3. La skill `../tp-carpeta-modelo/SKILL.md` tiene la plantilla oficial de especificacion de CU
   (item N02 de la Carpeta Modelo vigente). **Ojo:** no coincide campo a campo con el formato de
   esta skill (faltan caratula/historial de revision como campos propios; sobran Estado/Actor
   Principal/Actor Secundario) — discrepancia conocida, no resuelta automaticamente.

## Formato (estandar obligatorio)

Cada caso de uso lleva, en este orden: **ID y Nombre, Estado, Descripcion, Actor Principal,
Actor Secundario, Precondiciones, Puntos de Extension, Condicion, Escenario Principal,
Flujos Alternativos, Postcondiciones**. La definicion de cada campo esta en
`references/use-case-spec-format.md`; respetarla sin agregar ni quitar campos.

## Workflow

1. Determinar el requisito del TP impactado y revisar el codigo real para que los casos de
   uso reflejen el comportamiento vigente (no inventar pasos).
2. Escribir o actualizar el JSON de entrada siguiendo el esquema. Ubicacion sugerida:
   `diagramas/<requisito>/<requisito>-casos-de-uso.json`.
3. Generar los documentos desde la raiz del repo:

   ```
   python .agents/skills/tp-use-case-spec/scripts/build-use-case-spec.py \
       --input diagramas/<requisito>/<requisito>-casos-de-uso.json
   ```

   Por defecto produce `diagramas/<requisito>/<requisito>-casos-de-uso.docx` y `.md`.
   Overrides disponibles: `--output-docx`, `--output-md`, `--no-md`, `--no-docx`.
4. **Documento consolidado (todas las especificaciones en un solo `.docx`)**: pasar varios
   JSON o un directorio a `--input`. El script arma un unico documento con todas las
   especificaciones, un cuadro abajo de otro, separadas por el titulo del requisito (T0X).

   ```
   # Todos los CU bajo diagramas/ (ordenados por ruta: T02, T03, ... T07)
   python .agents/skills/tp-use-case-spec/scripts/build-use-case-spec.py --input diagramas
   ```

   Salida por defecto: `diagramas/TP-especificaciones-casos-de-uso.docx` (y `.md`). Flags:
   `--consolidar` (fuerza el modo con un solo JSON), `--doc-title` (titulo general),
   `--page-break-por-requisito` (cada T0X en pagina nueva; por defecto van seguidos).
   Regenerar este consolidado cada vez que se agrega o edita un CU, para mantenerlo al dia.

   **Numeracion de CU**: en el consolidado los CU se **renumeran global y correlativamente**
   (CU01, CU02, ... de corrido segun el orden de los requisitos), para que no se repitan los
   codigos entre requisitos. Los documentos puntuales por requisito conservan la numeracion
   **local** (CU01, CU02, ... dentro del requisito), que es la que coincide con los `.puml` de
   secuencia por CU. Flags: `--sin-renumerar` (respeta el codigo de cada JSON en el
   consolidado), `--renumerar-global` (fuerza el renumerado tambien en puntual), `--inicio N`
   (numero inicial). El orden de los requisitos en el consolidado surge del orden de `--input`
   (o del orden alfabetico de rutas al pasar un directorio); para un orden distinto, pasar los
   JSON explicitamente en el orden deseado.
5. Revisar el `.md` espejo en el diff antes de incorporar el `.docx` a la entrega formal.

## Reglas

- El JSON es la fuente de verdad de la especificacion; el `.docx` y el `.md` se regeneran,
  no se editan a mano.
- Mantener un JSON por requisito en `diagramas/<requisito>/`; versionar JSON y `.md` en git.
- Numerar IDs de forma correlativa dentro del requisito (`CU-<requisito>-NN`) o global (`CUNN`).
- El escenario principal describe solo el flujo de exito; errores y variantes van en flujos
  alternativos. Las reglas de negocio se integran en precondiciones, pasos de validacion o
  flujos alternativos (no hay campo propio).
- **Actor Secundario = entidad externa, nunca clases internas.** Un actor (RUP/UML) es un rol
  externo al limite del sistema. El actor secundario interviene de forma reactiva (otro
  sistema/servicio, hardware o persona de soporte). Las clases/componentes internos (BLL,
  REPOSITORIO, DAL, SERVICIOS, Forms, `AccesoDatosSqlServer`, singletons como `Sesion`) **no**
  son actores: su colaboracion se muestra en el diagrama de secuencia, no en este campo. En
  este proyecto de escritorio con SQL Server local la mayoria de los CU llevan `actor_secundario: "-"`.
  Detalle en [references/use-case-spec-format.md](references/use-case-spec-format.md).
- Los casos de uso deben mantenerse balanceados contra el codigo real y contra los diagramas
  de la skill [tp-uml-y-casos-de-uso](../tp-uml-y-casos-de-uso/SKILL.md); si el comportamiento
  cambia, actualizar el JSON y regenerar.
- Requisito de entorno: Python 3.8+ con `python-docx` (`pip install python-docx`). El script
  avisa si falta la dependencia.
