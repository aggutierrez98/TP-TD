# Mapeo: item oficial (Carpeta Modelo 2023) <-> carpeta real del repo

Que carpeta de `diagramas/` corresponde a cada item de
[carpeta-modelo-2023.md](carpeta-modelo-2023.md), y notas de implementacion especificas del repo
(CU, artefactos generados) que no estan en la fuente oficial. Para la descripcion textual de que
exige cada item, ver `carpeta-modelo-2023.md`; para convenciones de nomenclatura de archivo y el
procedimiento de alta de un requisito nuevo, ver
`.agents/skills/tp-uml-y-casos-de-uso/references/catalogo-requisitos.md`.

## G00. Descripcion global del producto

Carpeta: `diagramas/G00-descripcion-global-producto/`
- Un unico documento `G00-descripcion-global-producto.md` con las secciones G01 Proposito, G02
  Descripcion funcional y Alcance, G03 Definiciones/Acronimos/Abreviaturas, G04 Roles
  (participantes del desarrollo + usuarios del sistema) y G05 Otros Requisitos.
- **Nota:** esta numeracion interna (G01-G05) no coincide exacto con la oficial (G01 Proposito,
  G02 Alcance, **G03 Planificacion del proyecto**, G04 Definiciones, G05 Roles, G06 Otros
  Requisitos): el documento del repo no tiene una seccion G03 Planificacion propia y el resto
  queda corrido en uno. Esta carpeta esta explicitamente fuera de alcance de la reconciliacion de
  esta sesion (quedo cerrada y aprobada por el usuario el 26/08/2026); queda como nota para una futura decision del
  usuario, no como algo a corregir automaticamente.
- A diferencia de G07/G08, este documento describe **el producto completo** (FINAZAPP terminado),
  no solo lo ya implementado. No anticipar en G07/G08 clases o entidades que no existan todavia en
  el codigo/base real; en G00-G05 si corresponde imaginar y describir gestiones de negocio (N0X) y
  caracteristicas (A0X) que todavia no se construyeron.
- Casos de uso: ninguno (es descripcion narrativa, no diagramas ni CU).

## G07. Diagrama de clases parcial de todos los modulos implementados

Carpeta: `diagramas/G07-clases-por-capa/`
- Un diagrama de clases exclusivo por cada capa de la arquitectura (7 capas en el repo:
  abstracciones, be, bll, dal, repositorio, servicios, ui).
- Nomenclatura: `G07-clases-<capa>.puml` (ej. `G07-clases-bll.puml`, `G07-clases-dal.puml`).

## G08. Modelo de datos parcial de todos los modulos implementados

Carpeta: `diagramas/G08-modelo-datos-integrado/`
- Un DER unificado que integra las tablas de todos los modulos.
- Nomenclatura: `G08-datos-integrado.puml`.

## N00. Procesos de negocio (N01/N02)

**N01 y N02 son dos tipos de artefacto oficiales, no procesos de negocio en si.** La fuente
oficial (`carpeta-modelo-2023.md`) define N01 "Especificacion funcional por proceso de negocio"
y N02 "Especificaciones de Casos de Uso" como dos artefactos que se completan **por cada uno de
los procesos de negocio intervinientes** del proyecto — nunca "N01 = proceso 1", "N02 = proceso
2". La cantidad y el orden de procesos de negocio depende del negocio de cada alumno; la catedra
no los fija de antemano.

Este repo indexa sus procesos de negocio reales con numeracion propia, prefijo **`NEG`** +
numero secuencial de 2 digitos + slug (nunca `N0X` a secas, para no confundir con el N01/N02
oficial): mismo patron `<codigo><NN>-<slug>` que el resto del repo, ej. `NEG01-movimientos`
(Movimientos financieros), `NEG02-categorias` (Categorias).

Dentro de cada carpeta `diagramas/NEGxx-<slug>/` (mismo patron que los T0X: diagramas generales
segun aplique, un JSON de especificacion por proceso — `<proceso>-casos-de-uso.json` con todos
sus CU, siguiendo el formato de `tp-use-case-spec` — ver la discrepancia contra la plantilla N02
oficial en la seccion "Discrepancias conocidas" de `../SKILL.md` — y un `.puml` de secuencia por
cada CU) van sus propios artefactos N01 (especificacion funcional) y N02 (especificacion de CU),
tal como los define la fuente oficial.
- **Estado actual: sin crear.** No hay todavia ninguna carpeta `NEGxx-*` en el repo. El codigo de
  los procesos de negocio "Movimientos financieros" y "Categorias" ya esta implementado, pero sus
  diagramas y especificacion de CU (sus artefactos N01/N02) todavia no se generaron. El set se
  completa a medida que se define, confirmando con el usuario antes de crear cada carpeta.

## T01. Arquitectura Base

Carpeta: `diagramas/T01-arquitectura-base/`
- Generales: diagrama de componentes; mapa tentativo de navegacion de menus.
- Casos de uso: ninguno.

## T02. Gestion de Log In / Log Out

Carpeta: `diagramas/T02-login-logout/`
- Generales: clases, DER, secuencia general.
- CU01 Login · CU02 Registro · CU03 Logout (cada uno: especificacion + secuencia).

## T03. Gestion de Encriptado

Carpeta: `diagramas/T03-encriptado/`
- Generales: clases, DER, secuencia general.
- Casos de uso: ninguno.

## T04. Gestion de Perfiles de Usuario

Carpeta: `diagramas/T04-perfiles-usuario/`
- Generales: clases, DER, secuencia general.
- CU01 Gestionar permisos (catalogo) · CU02 Asignar perfil(es) a usuario · CU03 Controlar acceso
  de usuario.

## T05. Gestion de Multiples Idiomas

Carpeta: `diagramas/T05-multidioma/`
- Generales: clases, DER, secuencia general.
- CU01 Agregar idioma · CU02 Actualizar idioma · CU03 Eliminar idioma · CU04 Importar JSON de
  traducciones · CU05 Exportar JSON de traducciones · CU06 Editar traduccion · CU07 Configurar
  idioma preferido.

## T06. Gestion de Bitacora y Control de cambios

Carpeta unica: `diagramas/T06-bitacora-control-cambios/` (la fuente oficial trata T06a bitacora /
T06b control de cambios como sub-aspectos descriptos dentro de un unico item T06, no como items
numerados propios; el repo refleja eso con una sola carpeta, ya no dos).
- Generales: clases, DER, secuencia general (documenta el lado de control de cambios; bitacora no
  tiene flujo general propio, cubierto al 100% por sus CU01/CU02 — igual que T02/T04/T05).
- CU01 Consultar bitacora · CU02 Registrar en bitacora (sub-aspecto bitacora).
- CU03 Consultar historial de usuario · CU04 Restaurar punto del historial de usuario (sub-aspecto
  control de cambios).
- La numeracion de CU es correlativa dentro del requisito unico (CU01-CU04); no colisiona porque
  nunca fueron numerados por separado.

## T07. Gestion de Digitos Verificadores

Carpeta: `diagramas/T07-digitos-verificadores/`
- Generales: clases, DER, secuencia general.
- CU01 Verificar integridad de datos.

## T08. Backup y restauracion de la base de datos (extension no oficial del equipo)

Carpeta: `diagramas/T08-backup-restore/`
- **No es un item formal de la Carpeta Modelo 2023**: la fuente oficial solo menciona "backup"
  como ejemplo generico de requerimiento de caracteristicas tecnicas (junto con encriptacion), sin
  numero ni seccion propia. El equipo decidio documentarlo igual, con numeracion e items T0X
  propios (mismo patron que las gestiones tecnicas formales: clases, DER, secuencia general, CU),
  como extension del proyecto. Decision ya tomada, no requiere reconciliacion.
- Generales: clases, DER, secuencia general.
- CU01 Crear backup · CU02 Restaurar backup.
