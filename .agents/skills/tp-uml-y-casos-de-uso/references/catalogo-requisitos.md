# Catalogo de requisitos: diagramas y casos de uso

Convenciones de carpeta/nomenclatura de archivo y procedimiento de alta de un requisito nuevo.
Usar junto con [SKILL.md](../SKILL.md). Los `.puml` se hacen con la subskill PlantUML; las
especificaciones de CU con la subskill `tp-use-case-spec`.

Para saber que artefactos exige cada requisito concreto del TP (mapeo item oficial <-> carpeta
real del repo, con su numeracion y sus CU), ver
`.agents/skills/tp-carpeta-modelo/references/mapeo-repo.md`.

## Convenciones generales

- Carpeta por requisito: `diagramas/<requisito>/`. **No** crear carpetas por adelantado; crear
  la del requisito recien cuando se va a diagramar/especificar.
- `<requisito>` = codigo + slug (ej. `T05-multidioma`). Slugs canonicos en
  `tp-carpeta-modelo/references/mapeo-repo.md`.
- El tipo de artefacto va en el nombre del archivo. Nombres ordenables y descriptivos.
- Familias: `T0X` tecnicos, `G0X` globales de integracion, `N0X` de negocio.

## Nomenclatura

Diagramas generales del modulo:

| Artefacto | Patron |
| --- | --- |
| Diagrama de clases | `<requisito>-clases.puml` (o `<requisito>-<patron>-clases.puml`, ej. `...-composite-clases.puml`) |
| DER | `<requisito>-datos.puml` (construir inspeccionando la base real, ver [der-desde-mssql.md](der-desde-mssql.md)) |
| Secuencia general del modulo | `<requisito>-secuencia.puml` |
| Diagrama de componentes (T01) | `<requisito>-componentes.puml` |
| Mapa de navegacion (T01) | `<requisito>-navegacion.puml` |

Especificaciones de casos de uso (una por requisito, con todos sus CU adentro):

| Artefacto | Patron |
| --- | --- |
| Especificacion (fuente) | `<requisito>-casos-de-uso.json` (un JSON por requisito, lista `casos_de_uso`) |
| Especificacion (generados) | `<requisito>-casos-de-uso.docx` y `.md` |

Diagramas de secuencia (uno por CU, `NN` correlativo dentro del requisito, `<cu-slug>` descriptivo):

| Artefacto | Patron |
| --- | --- |
| Diagrama de secuencia | `<requisito>-CUNN-<cu-slug>-secuencia.puml` |

Reglas:
- **Especificaciones: un JSON por requisito** (con todos sus CU), no un archivo por CU. Cada CU
  se numera localmente dentro del requisito (`CU01`, `CU02`, ...) y esa numeracion local es la
  que coincide con los `.puml` de secuencia.
- **Diagramas de secuencia: uno por CU** (un `.puml` por cada caso de uso).
- El documento **consolidado** (todas las especificaciones en un solo `.docx`, separadas por el
  titulo del requisito) se arma con la subskill `tp-use-case-spec` (ver su `SKILL.md`) y ahi los
  CU se **renumeran de forma global y correlativa** (CU01, CU02, ... de corrido). Ubicacion:
  `diagramas/TP-especificaciones-casos-de-uso.docx` (+ `.md`).

Ejemplos:
- `diagramas/T02-login-logout/T02-login-logout-casos-de-uso.json` (especificacion del requisito, 3 CU)
- `diagramas/T02-login-logout/T02-login-logout-CU01-login-secuencia.puml` (secuencia del CU01)
- `diagramas/T05-multidioma/T05-multidioma-CU04-importar-json-secuencia.puml` (secuencia del CU04)

## Alta de un requisito nuevo sin regenerar el resto

Aplica tanto a T0X como a N0X:

1. Crear la carpeta `diagramas/<requisito>/` y su `<requisito>-casos-de-uso.json`.
2. Crear los `.puml` de secuencia por CU (subskill PlantUML) y los diagramas generales que aplique.
3. Generar **solo** ese requisito con la subskill `tp-use-case-spec` (modo puntual):
   `--input diagramas/<requisito>/<requisito>-casos-de-uso.json`. No toca los demas.
4. Regenerar el documento consolidado cuando se quiera (comando aparte y opcional):
   `--input diagramas` (autodescubre el nuevo requisito y renumera los CU de forma global).
