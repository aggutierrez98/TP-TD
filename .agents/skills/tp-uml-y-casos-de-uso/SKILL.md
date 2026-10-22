---
name: tp-uml-y-casos-de-uso
description: Punto central para los diagramas UML (PlantUML/UML2) y las especificaciones de casos de uso del TP. Orquesta las subskills de PlantUML y de especificacion de casos de uso, define que artefactos exige cada requisito (T0X tecnicos, G0X de integracion, N0X de negocio), su ubicacion bajo diagramas/<requisito>/ y su nomenclatura, sincroniza con el documento formal y valida que los artefactos esperados existan. Usar cuando haya que crear, actualizar o validar diagramas o especificaciones de casos de uso del TP.
---

# TP UML y Casos de Uso

Skill **central** de diagramado y especificacion del TP. Es el unico punto de entrada para
crear, actualizar o validar diagramas UML y especificaciones de casos de uso, y delega el
trabajo concreto en dos subskills.

## Cuando ejecutar (a demanda)

- El diagramado y la especificacion NO se hacen automaticamente despues de cada desarrollo.
- Se ejecutan solo a demanda, cuando el desarrollo del requisito esta completo.
- Antes de generar o actualizar diagramas o especificaciones, **preguntar primero al usuario**.
- Durante el desarrollo basta con anotar que artefactos quedarian impactados; recien
  diagramar/especificar cuando el usuario lo confirme.

## Subskills que orquesta

Para preparar PNG y publicarlos en Google Docs con carga manual a Drive, cargar
[`publicar-diagramas-google-docs`](../publicar-diagramas-google-docs/SKILL.md).
Aplica a cualquier tipo de diagrama, incluidos requisitos tecnicos T0X; conserva
imagenes existentes y prioriza MCP nativos antes del fallback One.

Cargar la subskill que corresponda segun el artefacto a producir:

1. **PlantUML (UML2)** — para todo `.puml`:
   [`../tp-delivery-standards/embedded-skills/plantuml/SKILL.md`](../tp-delivery-standards/embedded-skills/plantuml/SKILL.md)
   y el entorno del repo en
   [`../tp-delivery-standards/embedded-skills/plantuml/references/tpids-environment.md`](../tp-delivery-standards/embedded-skills/plantuml/references/tpids-environment.md).
2. **Especificacion de casos de uso** — para cada CU (texto + formato estandar):
   [`../tp-use-case-spec/SKILL.md`](../tp-use-case-spec/SKILL.md).

Esta skill manda el *que* y el *donde* (catalogo, ubicacion, nomenclatura, balanceo, validacion);
las subskills aportan el *como* (sintaxis PlantUML, toolchain, formato de la especificacion).

## Read first

1. `.agents/01-stack-dotnet-winforms.md`
2. `.agents/02-architecture-7-layers.md`
3. [references/catalogo-requisitos.md](references/catalogo-requisitos.md): convenciones de
   carpeta/nomenclatura de archivo y procedimiento de alta de un requisito nuevo. La numeracion ya
   esta reconciliada contra la Carpeta Modelo 2023 vigente; para saber que artefactos exige cada
   requisito concreto (mapeo item oficial <-> carpeta del repo) ver la skill `tp-carpeta-modelo`.
4. [references/der-desde-mssql.md](references/der-desde-mssql.md): como inspeccionar la base real
   en SQL Server para construir los DER fielmente.
5. [references/docs-sync-sources.md](references/docs-sync-sources.md): fuentes de verdad y
   sincronizacion con el documento formal.
6. La skill `google-drive:google-docs` cuando haya que editar el documento formal.
7. La skill `../tp-carpeta-modelo/SKILL.md` (y su
   [references/mapeo-repo.md](../tp-carpeta-modelo/references/mapeo-repo.md)) cuando haya duda de
   que exige textualmente la catedra (numeracion de un grupo G0X/N0X/T0X/A0X/D0X, alcance de un
   requisito, formato de entrega) o de en que carpeta del repo vive cada item — es la fuente
   oficial vigente, por encima de lo que asuma este catalogo.

## Modelo del catalogo

Cada requisito tiene cero o mas **casos de uso**. La **especificacion** de todos los CU de un
requisito vive en **un unico JSON por requisito** (`<requisito>-casos-de-uso.json`); cada CU
exige ademas **1 diagrama de secuencia** propio (`-CUNN-<cu-slug>-secuencia.puml`). Ademas cada
modulo tiene **diagramas generales** (segun el requisito: clases, DER, secuencia general,
componentes, mapa de navegacion). El catalogo completo, con la lista exacta de CU y artefactos
por requisito, esta en [references/catalogo-requisitos.md](references/catalogo-requisitos.md).

Familias de requisitos:

- `T0X` — requisitos tecnicos (T01..T07).
- `G0X` — entregables globales de integracion (G07 clases por capa, G08 DER integrado).
- `N0X` — requisitos de negocio (mismo patron; confirmar el set vigente con el usuario).

## Ubicacion y nomenclatura

- Guardar siempre el `.puml` y la especificacion bajo `diagramas/<requisito>/`.
- **No** crear carpetas por adelantado: crear la carpeta del requisito recien cuando se va a
  diagramar/especificar ese requisito, y verificar que sea la carpeta correcta del catalogo.
- Si un diagrama existente esta en una carpeta vieja, moverlo primero a la estructura del
  catalogo y luego editarlo ahi.
- Nomenclatura (detalle y ejemplos en `references/catalogo-requisitos.md`):
  - Diagramas generales: `<requisito>-clases.puml`, `<requisito>-datos.puml` (DER),
    `<requisito>-secuencia.puml` (secuencia general), `<requisito>-componentes.puml`,
    `<requisito>-navegacion.puml`.
  - Especificaciones: **un JSON por requisito** `<requisito>-casos-de-uso.json` (con todos sus
    CU en la lista `casos_de_uso`; + `.docx`/`.md` generados). No un archivo por CU.
  - Diagramas de secuencia: **uno por CU** `<requisito>-CUNN-<cu-slug>-secuencia.puml`.
  - Documento consolidado de todas las especificaciones (un solo `.docx`, separadas por titulo de
    requisito, con CU renumerados de forma global): `diagramas/TP-especificaciones-casos-de-uso.docx`
    (+ `.md`), generado con la subskill `tp-use-case-spec`. La numeracion local (por requisito) es
    la que coincide con los `.puml` de secuencia; la global solo aplica al consolidado.

## Reglas de los diagramas de secuencia por CU

- Seguir el estandar UML2 correctamente (lifelines, activaciones, mensajes sincronos/asincronos,
  `alt`/`opt`/`loop`, `return`).
- La interaccion **usuario <-> Form** debe seguir el flujo tal cual lo describe la especificacion
  del CU (escenario principal y, si aplica, flujos alternativos).
- A partir del Form hacia el resto del sistema (BLL, DAL, Servicios, BE, etc.), la interaccion
  ya no replica los pasos del CU, pero debe **balancear exacto** con las clases, metodos,
  parametros y colaboraciones reales del codigo y del diagrama de clases del modulo.
- Una secuencia y su especificacion son un par 1:1: si una cambia, actualizar la otra.

## Code to docs workflow

Ejecutar solo cuando el desarrollo este completo y el usuario haya confirmado que quiere
diagramar/especificar.

1. Identificar el requisito del TP impactado y revisar el catalogo para saber que artefactos
   exige (generales + CU).
2. Revisar el codigo real para que diagramas y especificaciones reflejen el comportamiento
   vigente (no inventar pasos ni metodos).
3. Escribir/actualizar el **unico JSON del requisito** (`<requisito>-casos-de-uso.json`, con todos
   sus CU) y generar la especificacion con la subskill `tp-use-case-spec`; crear/actualizar el
   diagrama de secuencia **por CU** con la subskill PlantUML. Para dar de alta un requisito nuevo
   sin regenerar el resto, generar solo su JSON en modo puntual (`--input <ese JSON>`).
4. Crear/actualizar los diagramas generales del modulo (clases, DER, secuencia general, y para
   T01 componentes + navegacion) con la subskill PlantUML. Para los DER, inspeccionar primero la
   base real en SQL Server (ver [references/der-desde-mssql.md](references/der-desde-mssql.md)).
5. Guardar todo bajo `diagramas/<requisito>/` con la nomenclatura del catalogo.
6a. Regenerar el documento consolidado de especificaciones cuando corresponda (comando aparte y
   opcional): subskill `tp-use-case-spec` con `--input diagramas`.
6. Recien despues copiar/exportar las imagenes al Google Doc (ver `docs-sync-sources.md`).
7. Compilar la solucion al final para asegurar que lo documentado coincide con un build valido.
8. Correr el checklist de validacion.

## Docs to code workflow

1. Leer el cambio pedido en el documento formal.
2. Determinar si es aclaracion documental o cambio real de comportamiento.
3. Si impacta comportamiento, actualizar codigo respetando stack y arquitectura.
4. Identificar requisito y archivos destino bajo `diagramas/<requisito>/`.
5. Actualizar PlantUML y especificaciones, y luego el documento formal.
6. Compilar la solucion.

## Source of truth policy

Usar esta prioridad:

1. codigo del repo para comportamiento real
2. `.puml` y JSON de especificacion del repo para UML y CU
3. Google Doc para comunicacion y entrega formal

No actualizar el Google Doc sin antes actualizar el PlantUML/especificacion fuente cuando el
cambio impacta diagramas o casos de uso.

## Reglas de balanceo

- Todo diagrama UML se crea en PlantUML y se guarda como `.puml` en el repo.
- No dejar diagramas solo como imagen embebida en documentos.
- Los diagramas (clases, secuencia, DER), las especificaciones de CU y el codigo deben quedar
  balanceados entre si.
- Si una clase, atributo, metodo o parametro aparece en un diagrama, debe existir con ese mismo
  nombre en el codigo vigente.
- Si un flujo de secuencia invoca un metodo, ese metodo debe existir con ese mismo nombre y
  parametros en el diagrama de clases correspondiente y en el codigo.
- Los DER deben construirse inspeccionando la **base real en SQL Server** (`TpCatedraIDS`) y
  reflejar exactamente su persistencia (tablas, columnas, tipos, nullabilidad, PK, FK). Ver
  [references/der-desde-mssql.md](references/der-desde-mssql.md). Cruzar contra `script_bd.sql`:
  ante diferencias, la base real manda y se deja constancia.

## Checklist de validacion

Antes de cerrar una tarea de diagramado/especificacion, validar contra
[references/catalogo-requisitos.md](references/catalogo-requisitos.md):

- [ ] requisito identificado y carpeta `diagramas/<requisito>/` correcta segun el catalogo
- [ ] diagramas generales que exige el requisito creados (clases, DER, secuencia general; T01: componentes + navegacion)
- [ ] DER construido inspeccionando la base real en SQL Server y cruzado contra `script_bd.sql`
- [ ] existe el JSON unico del requisito (`<requisito>-casos-de-uso.json`) con todos sus CU (+ `.docx`/`.md` generados)
- [ ] por cada CU del requisito: existe su diagrama de secuencia (`-CUNN-<cu-slug>-secuencia.puml`)
- [ ] numeracion local de CU en el JSON coincide con la de los `.puml` de secuencia del requisito
- [ ] si corresponde, documento consolidado regenerado (`TP-especificaciones-casos-de-uso.docx`) con `--input diagramas`
- [ ] usuario <-> Form de cada secuencia alineado con el escenario del CU
- [ ] Form <-> resto del sistema balanceado con clases, metodos y parametros reales
- [ ] nomenclatura segun catalogo en todos los archivos
- [ ] consistencia cruzada clases / secuencias / DER / especificaciones / codigo
- [ ] compilacion exitosa de la solucion
