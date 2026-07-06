# Catalogo de requisitos: diagramas y casos de uso

Fuente unica de que artefactos exige cada requisito del TP, donde se guardan y como se nombran.
Usar junto con [SKILL.md](../SKILL.md). Los `.puml` se hacen con la subskill PlantUML; las
especificaciones de CU con la subskill `tp-use-case-spec`.

## Convenciones generales

- Carpeta por requisito: `diagramas/<requisito>/`. **No** crear carpetas por adelantado; crear
  la del requisito recien cuando se va a diagramar/especificar.
- `<requisito>` = codigo + slug (ej. `T05-multidioma`). Slugs canonicos en la tabla de abajo.
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

## Catalogo por requisito

### T01 — Arquitectura base y pantallas
Carpeta: `diagramas/T01-arquitectura-base/`
- Generales: diagrama de componentes; mapa tentativo de navegacion de menus.
- Casos de uso: ninguno.

### T02 — Gestion de login y logout
Carpeta: `diagramas/T02-login-logout/`
- Generales: clases, DER, secuencia general.
- CU01 Login · CU02 Registro · CU03 Logout (cada uno: especificacion + secuencia).

### T03 — Gestion de encriptado
Carpeta: `diagramas/T03-encriptado/`
- Generales: clases, DER, secuencia general.
- Casos de uso: ninguno.

### T04 — Gestion de perfiles / roles
Carpeta: `diagramas/T04-perfiles-usuario/`
- Generales: clases, DER, secuencia general.
- CU01 Gestionar permisos (catalogo) · CU02 Asignar perfil(es) a usuario · CU03 Controlar acceso de usuario.

### T05 — Gestion de multidioma
Carpeta: `diagramas/T05-multidioma/`
- Generales: clases, DER, secuencia general.
- CU01 Agregar idioma · CU02 Actualizar idioma · CU03 Eliminar idioma · CU04 Importar JSON de
  traducciones · CU05 Exportar JSON de traducciones · CU06 Editar traduccion · CU07 Configurar
  idioma preferido.

### T06 — Gestion de bitacora y control de cambios
En el repo este requisito esta dividido en dos carpetas:

- `diagramas/T06a-bitacora/` — Generales: clases, DER, secuencia general.
  - CU01 Consultar bitacora · CU02 Registrar en bitacora.
- `diagramas/T06b-control-cambios/` — Generales: clases, DER, secuencia general.
  - CU03 Consultar historial de usuario · CU04 Restaurar punto del historial de usuario.

### T07 — Gestion de digitos verificadores
Carpeta: `diagramas/T07-digitos-verificadores/`
- Generales: clases, DER, secuencia general.
- CU01 Verificar integridad de datos.

## Entregables globales de integracion

### G06 — Diagrama de clases parcial de todos los modulos
Carpeta: `diagramas/G06-clases-por-capa/`
- Un diagrama de clases exclusivo por cada capa de la arquitectura (minimo 4 en total).
- Nomenclatura: `G06-clases-<capa>.puml` (ej. `G06-clases-bll.puml`, `G06-clases-dal.puml`).

### G07 — Modelo de datos parcial de todos los modulos
Carpeta: `diagramas/G07-modelo-datos-integrado/`
- Un DER unificado que integre las tablas de todos los modulos.
- Nomenclatura: `G07-datos-integrado.puml`.

## Requisitos de negocio (N0X)

Mismo patron que los T0X: carpeta `diagramas/N0X-<slug>/`, diagramas generales segun aplique,
**un JSON de especificacion por requisito** (`<requisito>-casos-de-uso.json` con todos sus CU) y
**un `.puml` de secuencia por cada CU**. El set concreto de requisitos N0X se completa a medida
que se definen (confirmar con el usuario antes de crearlos).

Alta de un requisito nuevo (T0X o N0X) sin regenerar el resto:

1. Crear la carpeta `diagramas/<requisito>/` y su `<requisito>-casos-de-uso.json`.
2. Crear los `.puml` de secuencia por CU (subskill PlantUML) y los diagramas generales que aplique.
3. Generar **solo** ese requisito con la subskill `tp-use-case-spec` (modo puntual):
   `--input diagramas/<requisito>/<requisito>-casos-de-uso.json`. No toca los demas.
4. Regenerar el documento consolidado cuando se quiera (comando aparte y opcional):
   `--input diagramas` (autodescubre el nuevo requisito y renumera los CU de forma global).
