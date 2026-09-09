---
name: tp-carpeta-modelo
description: Fuente oficial vigente (2023) de la Carpeta Modelo de la catedra (Ingenieria de Software / Trabajo de Diploma): formato de entrega del proyecto, estructura completa de requisitos (G0X, N0X, T0X, A0X, D0X) y la plantilla oficial de especificacion de casos de uso. Consultar cuando otra skill del TP necesite saber que exige la catedra textualmente (numeracion de un grupo de requisitos, campos de una especificacion, formato de entrega) o para resolver una duda de alcance/numeracion contra la fuente formal.
---

# TP Carpeta Modelo

Referencia de solo lectura. Esta skill no genera ni edita artefactos: es la fuente de verdad
textual de lo que exige la catedra, para que las skills que sí producen artefactos
(`tp-uml-y-casos-de-uso`, `tp-use-case-spec`, `tp-delivery-standards`) la consulten en vez de
asumir o duplicar su contenido.

## Read first

[references/carpeta-modelo-2023.md](references/carpeta-modelo-2023.md) — copia integra en
Markdown de "Carpeta modelo IS y TD 2023" (aportada por el usuario en `.docx`; este `.md` es el
texto completo, incluida la simbologia de DER como imagen embebida). Es la version **vigente**;
prevalece sobre cualquier mencion suelta a una "Carpeta modelo TC y TD 2020" que pueda aparecer
en otros documentos del repo (esa version ya no esta disponible).

[references/mapeo-repo.md](references/mapeo-repo.md) — mapeo concreto item oficial <-> carpeta
real de `diagramas/` (con notas de implementacion como el detalle de CU por requisito). Consultar
junto con `carpeta-modelo-2023.md` cuando haga falta saber en que carpeta del repo vive cada item.

## Estructura de la carpeta de proyecto

Cinco grandes grupos, cada uno con su propia numeracion interna:

- **G00. Descripcion global del producto** — G01 Proposito, G02 Descripcion funcional y Alcance,
  G03 Planificacion del proyecto, G04 Definiciones/Acronimos/Abreviaturas, G05 Roles
  (participantes del desarrollo + usuarios del sistema), G06 Otros Requisitos, **G07** Diagrama
  de clases parcial de todos los modulos implementados, **G08** Modelo de datos parcial de todos
  los modulos implementados (DER).
- **N00. Procesos de negocio** — N01 Especificacion funcional por proceso de negocio (roles
  intervinientes, descripcion funcional entrada/comportamiento/salida, diagrama de proceso por
  actividad, modelo conceptual/dominio por clases; uno por cada proceso de negocio), N02
  Especificaciones de Casos de Uso (ver plantilla abajo).
- **T00. Documentos de aspectos tecnicos** — T01 Arquitectura Base, T02 Login/Logout, T03
  Encriptado, T04 Perfiles de Usuario, T05 Multiples Idiomas, **T06 Gestion de Bitacora y Control
  de cambios** (un solo item, con dos sub-aspectos T06a bitacora / T06b control de cambios
  descriptos adentro, no como items numerados propios), T07 Digitos Verificadores. Cada gestion
  tecnica exige: Objetivo, Descripcion detallada de como funciona, Diagrama de clases, DER (si es
  necesario), Secuencia (si es necesario). **No hay T08 formal**: backup solo se nombra como
  ejemplo generico de "requerimiento de caracteristicas tecnicas" (junto con encriptacion), sin
  numero ni seccion propia.
- **A00. Caracteristicas y funcionalidades adicionales** — A01 Instalador, A02 Informe y
  exportacion en PDF, A03 Serializacion.
- **D00. Documentacion adicional** — D01 Manual de instalacion, D02 Ayuda en linea, D03 Material
  de apoyo al usuario final.

## Plantilla oficial de especificacion de caso de uso (N02)

14 items, en este orden (los marcados con `*` en el original arrancan carilla nueva): Caratula,
Historial de revision, Objetivo, Precondiciones, Poscondiciones, Eventos disparadores del caso de
uso, Puntos de extension y condiciones, Grafico del caso de uso, Descripcion analitica (flujo
normal / flujo alternativo), Diagrama de flujo (solo si el algoritmo es complejo), Diagrama de
clase (clases afectadas), Diagrama de secuencia (objetos afectados), DER (entidades afectadas),
Prototipo de interfaz de usuario.

Esto **no coincide exactamente** con el formato que usa hoy `tp-use-case-spec`
(`ID y Nombre, Estado, Descripcion, Actor Principal, Actor Secundario, Precondiciones, Puntos de
Extension, Condicion, Escenario Principal, Flujos Alternativos, Postcondiciones`): faltan ahi
caratula/historial de revision como campos propios, y sobran "Estado"/"Actor
Principal"/"Actor Secundario" que no estan en la plantilla oficial. No se resuelve esta skill —
es una discrepancia para decidir con el usuario si corresponde reconciliar.

## Formato de entrega (bibliorato)

Hoja A4 a una carilla, Arial (12 cuerpo, 13 subtitulos, 14 titulos), margenes 3/2/4/1.5 cm
(izq/der/sup/inf), encabezado con universidad/facultad, materia, docente, localizacion/comision/
turno, año, titulo del trabajo, alumno, legajo, seccion/etapa, numero de pagina. Documento
inicial de la carpeta: caratula, indice, historial de revision.

## Discrepancias conocidas vs. el catalogo actual del repo

`diagramas/` y `.agents/skills/tp-uml-y-casos-de-uso/references/catalogo-requisitos.md` heredaban
la numeracion de una "Carpeta modelo TC y TD 2020" ya no disponible. Esa numeracion ya fue
reconciliada contra esta version vigente (G06/G07 del repo -> **G07/G08**; `T06a-bitacora` +
`T06b-control-cambios` -> **T06-bitacora-control-cambios** unico; ver
[references/mapeo-repo.md](references/mapeo-repo.md) para el mapeo completo). Quedan dos puntos
abiertos, ninguno bloqueante:

- **`T08-backup-restore`**: es una extension propia del equipo, no un item formal de esta fuente
  (que solo menciona "backup" como ejemplo generico, sin numero ni seccion propia). Es una nota
  informativa, no un problema: el usuario ya decidio mantenerlo con ese nombre y numeracion T0X
  propia.
- **Plantilla N02 vs. formato de `tp-use-case-spec`**: la plantilla oficial de especificacion de
  caso de uso (14 items, ver arriba) no coincide exacto con el formato que usa hoy
  `tp-use-case-spec` (`ID y Nombre, Estado, Descripcion, Actor Principal, Actor Secundario,
  Precondiciones, Puntos de Extension, Condicion, Escenario Principal, Flujos Alternativos,
  Postcondiciones`): faltan caratula/historial de revision como campos propios, y sobran
  "Estado"/"Actor Principal"/"Actor Secundario" que no estan en la plantilla oficial. Sigue sin
  resolverse — es decision pendiente del usuario si corresponde reconciliar.
