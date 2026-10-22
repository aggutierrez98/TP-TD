---
name: publicar-diagramas-google-docs
description: Use when el usuario pide publicar, pegar o sincronizar diagramas PlantUML o PNG en Google Docs, completar placeholders del TP, preparar un lote para Drive o retomar su publicacion tras compartir una carpeta. Aplica a cualquier tipo de diagrama y seccion tecnica, de negocio o general.
---

# Publicar diagramas en Google Docs

Convertir fuentes locales en imagenes y publicarlas con correspondencia verificable.
La primera version espera que **el usuario suba los PNG a Drive manualmente**.

## 1. Acordar el lote

- Leer `tp-uml-y-casos-de-uso` como entrada del TP y la subskill PlantUML para
  renderizado; `google-workspace` y su referencia Docs antes de editar Google.
- Identificar fuentes, documento, pestaña y destinos. Admitir secuencias, clases,
  CU, actividades, estados, componentes, DER y otros tipos de PlantUML.
- Usar contexto de encabezados T0X, N0X, G0X, A0X, D0X o titulos indicados;
  no inferir un destino a partir de una mencion en un parrafo o indice.
- Confirmar alcance cuando falte. Conservar imagenes existentes por defecto.
  No editar documentos externos durante pruebas de la propia skill.

## 2. Preparar PNG

Usar Python 3.9+ con Pillow en un venv de desarrollo, Java/PlantUML y Graphviz.
Desde la raiz del repo:

```bash
python .agents/skills/publicar-diagramas-google-docs/scripts/preparar_png.py \
  "diagramas/T02-login-logout/*.puml" \
  --output "diagramas/exportaciones/login-lote-01"
```

El script acepta varios archivos/globs sin filtrar por tipo, usa `PLANTUML_JAR`
o `--jar`, conserva fuentes/includes relativos, expande includes/macros y omite
bloques legend en la copia de render, genera a 200 DPI y limita cada PNG a
24 MP y menos de 45 MB.
Rechaza homonimos y varios diagramas dentro del mismo archivo. La carpeta de
salida debe ser nueva: no sobrescribir lotes anteriores.

Revisar visualmente una muestra de cada tipo y los diagramas grandes. Informar
dimensiones y reducciones; si el contenido queda ilegible, consultar antes de
publicar, sin eliminar informacion para hacerlo caber.

**Pausa obligatoria:** indicar ruta absoluta de la carpeta, numero de PNG y
pedir que suba **solo los PNG** a Drive y comparta el enlace. `manifest.json`
describe artefactos y hashes; no es un archivo de progreso. Si ya hay imagenes
subidas, reutilizarlas solo tras comprobar su correspondencia con las fuentes.

## 3. Elegir conectores y leer destinos

Leer [references/conectores.md](references/conectores.md).
Para **cada operacion**, descubrir e intentar primero MCP nativo Docs/Drive;
usar One solo si no esta disponible o carece de esa operacion. No inventar
herramientas ni reutilizar IDs de conexiones de otra sesion.

Listar toda la carpeta (paginando); contrastar nombre y MD5 del PNG con el
manifiesto. Leer documento con pestañas, objetos inline y revision. Obtener
estilo de pagina por lectura separada si la API lo exige.

Preparar destinos explicitos segun [references/publicacion.md](references/publicacion.md).
La ruta de encabezados, rotulo y placeholder distinguen tipos dentro del mismo
CU. Resolver ambiguedades antes de escribir. Si no hay placeholder ni imagen,
acordar la ubicacion y crear un ancla textual unica autorizada; luego releer.

## 4. Planificar, publicar y verificar

```bash
node .agents/skills/publicar-diagramas-google-docs/scripts/documento.mjs \
  plan doc.json files.json targets.json plan.json
```

Revisar `jobs` y `skipped`. Mostrar cantidades y destino; confirmar la escritura
si el pedido no la autoriza aun. Ofrecer prueba de hasta cinco cuando no haya
un formato aprobado. Ejecutar `plan.body` mediante el conector seleccionado,
solo si contiene requests y la revision sigue vigente. Para lotes grandes,
seleccionar un subconjunto de targets y regenerar el plan tras cada escritura.

Tras cualquier exito **o timeout**, releer el documento. Nunca reenviar a ciegas
ni cambiar de conector para repetir una escritura de resultado incierto.

```bash
node .agents/skills/publicar-diagramas-google-docs/scripts/documento.mjs \
  verify doc-despues.json files.json targets-insertados.json verificacion.json
```

Verificar solo los targets insertados; los existentes conservados pueden tener
otra fuente/formato. Reportar insertados, conservados, pendientes y problemas.
La lectura estructural no acredita paginacion visual: pedir revision de la
muestra o revisar exportacion si existe una herramienta disponible.

## Errores frecuentes

- Acceso de imagen prohibido: explicar el bloqueo y pedir al usuario resolver
  acceso; no cambiar permisos ni publicar archivos por iniciativa propia.
- Error de revision: releer y volver a planificar.
- Imagen existente mas placeholder: conservar, informar el residuo y consultar.
- Reemplazo solicitado: acordar objetos concretos y usar replaceImage soportado
  por el conector; el planificador V1 solo completa faltantes.
- No afirmar que One permite carga binaria ni que un enlace de vista es un PNG.
