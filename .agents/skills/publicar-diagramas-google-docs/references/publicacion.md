# Entradas y planificacion

## Archivos de trabajo

Guardar snapshots y planes temporales en `/tmp/opencode/` (o temporal local
equivalente en Windows), no en un archivo de estado del repo. El progreso de
desarrollo sigue en tasks.md de OpenSpec. `manifest.json` va con los PNG como
inventario reproducible, sin checkboxes ni tokens de conexiones.

- `doc.json`: Document de Google Docs con tabs, documentId, revisionId,
  inlineObjects y documentStyle paginado en PT.
- `files.json`: `{ "files": [...] }` de TODAS las paginas de la carpeta Drive.
- `targets.json`: arreglo de destinos acordados. Ejemplo (titulos ilustrativos;
  usar los textos exactos de la lectura real):

```json
[
  {
    "image": "T02-login-clases.png",
    "tabId": "t.0",
    "headingPath": ["T. Aspectos tecnicos", "T02. Login"],
    "slotLabel": "Diagrama de clases:",
    "placeholder": "[Pegar aquí el diagrama de clases]",
    "expectedMd5": "md5-del-manifest"
  },
  {
    "image": "CUN20-alta-secuencia.png",
    "tabId": "t.0",
    "headingPath": ["N02. Casos de uso", "N02.1 Operaciones", "CUN20 Alta"],
    "slotLabel": "Diagrama de secuencia:",
    "placeholder": "[Pegar aquí el diagrama de secuencia]",
    "expectedMd5": "md5-del-manifest"
  }
]
```

No asumir N01 = N02: las carpetas locales y los encabezados formales pueden
usar numeraciones diferentes. Vincularlos explicitamente, como T0X o cualquier
titulo. Para un diagrama sin rotulo, omitir slotLabel solo si toda esa seccion
identifica un unico destino. Si existen varios tipos o imagenes, usar un
rotulo de slot antes del placeholder para acotar el espacio relevante.

Un slot termina ante el siguiente parrafo con rotulo acabado en `:` (incluye
`Componentes:` o `DER:`). Para contenido con otros rotulos intermedios, indicar
`slotEndLabel` con el texto exacto del limite y revisar el plan.

`headingPath` puede ser un sufijo de la ruta de encabezados; debe resultar
unico en la pestaña/documento. Incluir ancestros para resolver homonimos.
El parser ignora tablas de contenidos y menciones dentro del texto; acepta
subsecciones dentro de un encabezado, hasta el siguiente de nivel igual o menor.
No hay dependencia de prefijos CUN/N/T ni del tipo de diagrama.

Los placeholders con estilos divididos en varios textRuns siguen siendo
localizables. Los indices son UTF-16 (JS String.length), incluidos emojis.
El texto del placeholder se elimina sin eliminar el salto de parrafo.

## Ejecutar

```bash
node .agents/skills/publicar-diagramas-google-docs/scripts/documento.mjs \
  plan /tmp/opencode/doc.json /tmp/opencode/files.json \
  /tmp/opencode/targets.json /tmp/opencode/plan.json
```

El script no escribe en Google. Devuelve jobs, skipped y body para el MCP.
Las solicitudes estan ordenadas por indice descendente dentro de cada pestaña
y condicionadas a la revision leida. El ancho surge de pageSize menos margenes
(4 pt de tolerancia); la altura tambien se limita al espacio vertical disponible.
Se rechazan destinos en tablas porque requieren un ancho de celda que no debe
suponerse igual al de pagina. Para estos casos acordar el ajuste con el usuario
y usar una operacion especifica del conector, con verificacion posterior.
Tambien se rechazan documentos con columnas multiples o margenes de seccion
distintos de los globales: acordar el area efectiva antes de editar esos destinos.

`skipped` significa que se conservo una imagen encontrada en el slot: no
acredita que sea nueva ni que coincida con el archivo. No borrar placeholders
residuales junto a imagenes sin consultar. Si el usuario pide reemplazar, usar
un flujo explicito de replaceImage fuera del planificador de faltantes V1.

Los archivos de salida se crean con modo exclusivo; elegir otro nombre o
retirar solo un temporal propio antes de regenerar. No editar indices a mano.

## Verificar una escritura

Guardar el documento releido y verificar los targets de los jobs publicados:

```bash
node .agents/skills/publicar-diagramas-google-docs/scripts/documento.mjs \
  verify /tmp/opencode/doc-despues.json /tmp/opencode/files.json \
  /tmp/opencode/targets-insertados.json /tmp/opencode/verificacion.json
```

Exige exactamente una imagen inline, sourceUri con fileId correcto, contentUri
presente, alineacion centrada y ausencia del placeholder en el mismo slot.
Comprueba ademas ancho/alto esperados en PT (tolerancia de 1 PT), para detectar
deformaciones y desbordes tras la insercion.
Una respuesta 200 no sustituye esta comprobacion. Si no se puede verificar
porque el conector omite sourceUri/objetos, obtener una lectura estructural
completa por el fallback y reportar cualquier limitacion restante.

La estructura no demuestra que el diagrama sea legible al imprimir. Inspeccionar
PNG locales y revisar muestra en Google Docs/exportacion cuando haya acceso.

## Comprobaciones locales de la skill

```bash
node --test .agents/skills/publicar-diagramas-google-docs/scripts/documento.test.mjs
python -m unittest discover -s .agents/skills/publicar-diagramas-google-docs/scripts -p 'test_*.py'
```

Los escenarios de evaluacion del agente estan en `evals/evals.json`: ejecutarlos
en seco, sin MCP de escritura, con y sin skill. No usar el TP real como prueba.
