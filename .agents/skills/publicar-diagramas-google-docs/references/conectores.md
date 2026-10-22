# Conectores: prioridad y capacidades

## MCP nativos primero, por operacion

Descubrir herramientas reales del runtime (busqueda de herramientas/catalogo).
Una skill instalada no demuestra que su MCP este conectado. Buscar capacidades:

| Plataforma | Operaciones necesarias |
|---|---|
| Drive | listar hijos de carpeta, paginar, metadatos nombre/id/MIME/MD5/dimensiones |
| Docs | lectura estructural con pestañas y revision, estilo de pagina, batchUpdate |

Intentar las lecturas del conector nativo disponible. Para escritura usarlo
si soporta indices, tabId y control de revision; si no, usar One para ESA
operacion. No sustituir la lectura estructural por texto plano porque pierde
indices, objetos y distincion entre encabezado real e indice.

Si el conector nativo falla por disponibilidad o no soporta una operacion,
informar brevemente el fallback. Un rechazo de permisos no se evita con otro
conector. Si una escritura tiene resultado incierto, releer antes de decidir.
Mantener el mismo lector y escritor para un lote siempre que sea posible; ante
un cambio de conector, releer estructura/revision con el escritor antes de editar.

## One como fallback

1. `list_one_integrations`: obtener conexiones actuales de Docs y Drive.
2. `find_one_actions`: descubrir de una vez las operaciones faltantes (listar
   archivos, leer documento, batchUpdate). Enviar modelo actual y descripcion
   generica del trabajo. Cargar documentacion omitida con `load` si hace falta.
3. `execute_one_action`: usar action_id y connection_key devueltos, no IDs
   copiados de ejemplos o conversaciones previas. Enviar el modelo actual.
4. Revisar **status y responseData.error**; recibir un objeto de resultado no
   significa que la operacion haya sido exitosa.

Operaciones de Google usadas como contrato, no nombres de herramientas MCP:

- Drive `files.list`: `q="'<folderId>' in parents and trashed=false"`,
  `pageSize=1000`, campos `nextPageToken,files(id,name,mimeType,md5Checksum,size,
  imageMediaMetadata,webContentLink,permissions)`. Seguir nextPageToken hasta
  terminar y reunir todas las paginas en `files.json`.
- Docs `documents.get`: `includeTabsContent=true`, `SUGGESTIONS_INLINE`.
  Si la lectura devuelve sugerencias que afectan el destino, no normalizarlas
  a texto de forma silenciosa: resolver con el usuario antes de publicar.
- Estilo de pagina: si no viene en la respuesta tab-aware, leer
  `fields=documentStyle,revisionId` **sin** includeTabsContent. Solo unir estilo
  y estructura cuando las revisiones coincidan; guardar en `doc.json`.
- Docs `documents.batchUpdate`: enviar `plan.body` con
  `writeControl.requiredRevisionId`. Usar el documento exacto de `plan.documentId`.

Los scripts no acceden a credenciales. El agente transfiere las respuestas JSON
de los MCP a los scripts locales, normalizando envoltorios `responseData` o
`document` sin perder campos. Si el runtime no permite archivos desde Code Mode,
ejecutar la logica pura de `documento.mjs` con los datos en memoria usando las
herramientas disponibles; no inferir imports/herramientas inexistentes. Las
funciones plan/verify no dependen de I/O; solo el adaptador CLI usa Node fs.

## URLs de imagen

Para archivos publicos por enlace, el enlace de descarga de Drive funciona:
`https://drive.google.com/uc?export=download&id=<fileId>`.
Los enlaces `/file/d/.../view` o de carpetas son paginas HTML, no imagenes.

Google Docs requiere imagen recuperable sin sesion: PNG/JPEG/GIF, menos de 50 MB,
no mas de 25 MP. El preparador ya deja margen con 24 MP y 45 MB. Para PNG
preexistentes mayores, el planificador usa el thumbnail de Drive con un ancho
calculado bajo 24 MP (maximo 5000 px). Comprobar respuesta MIME/dimensiones
antes de publicar; thumbnails pueden tardar en estar disponibles y sus URLs
no se consideran sustituto de verificar acceso.

La carpeta puede estar privada aunque el MCP la lea. En caso de `forbidden`,
el batch es atomico y no se aplica; informar y pedir al usuario acceso por
enlace para las imagenes necesarias. No modificar permisos, ni del Doc ni de
la carpeta, como parte automatica de esta skill.

## Reanudacion y lotes

No guardar un token de revision para otra sesion. Leer estado real, comparar
imagenes presentes y placeholders, y regenerar indices. Despues de timeout,
revisar sourceUri/fileId: si ya se inserto, verificar sin reinsertar; si hay
estado parcial o anomalo, detener ese destino y pedir aclaracion. Una respuesta
de revision obsoleta no se soluciona quitando writeControl.
