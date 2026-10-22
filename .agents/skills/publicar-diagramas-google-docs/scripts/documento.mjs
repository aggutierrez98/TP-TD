/** Planificacion offline de Google Docs. Sin autenticacion ni llamadas de red. */
import { readFileSync, writeFileSync } from 'node:fs';
import { pathToFileURL } from 'node:url';

const norm = s => String(s).trim().replace(/\s+/g, ' ');
const fail = message => { throw new Error(message); };

function tabsOf(doc) {
  const result = [];
  function visit(tabs) {
    for (const tab of tabs ?? []) {
      result.push({ id: tab.tabProperties.tabId, ...tab.documentTab });
      visit(tab.childTabs);
    }
  }
  if (!doc.tabs?.length) fail('Leer el documento con includeTabsContent=true');
  visit(doc.tabs);
  return result;
}

function paragraphs(content, path = [], inTable = false) {
  const result = [];
  let headings = [...path];
  for (const block of content ?? []) {
    // No recorrer tableOfContents: sus menciones no son destinos.
    if (block.paragraph) {
      const p = block.paragraph;
      const text = (p.elements ?? []).map(e => e.textRun?.content ?? '').join('');
      const match = p.paragraphStyle?.namedStyleType?.match(/^HEADING_([1-6])$/);
      if (match && !inTable && norm(text)) {
        const level = Number(match[1]);
        headings = headings.filter(h => h.level < level);
        headings.push({ level, text: norm(text) });
      }
      result.push({ start: block.startIndex, end: block.endIndex, text, paragraph: p,
                    path: headings.map(h => h.text), inTable });
    }
    if (block.table) {
      for (const row of block.table.tableRows ?? []) {
        for (const cell of row.tableCells ?? []) result.push(...paragraphs(cell.content, headings, true));
      }
    }
  }
  return result;
}

function locate(doc, target) {
  if (!target.headingPath?.length || !target.placeholder || !target.image) {
    fail('Cada destino requiere image, headingPath y placeholder');
  }
  const candidates = [];
  for (const tab of tabsOf(doc)) {
    if (target.tabId && target.tabId !== tab.id) continue;
    const rows = paragraphs(tab.body?.content);
    const wanted = target.headingPath.map(norm);
    const matches = row => row.path.length >= wanted.length &&
      row.path.slice(-wanted.length).every((h, i) => h === wanted[i]);
    // Separar apariciones del mismo encabezado: no juntar secciones repetidas.
    const runs = [];
    for (let i = 0; i < rows.length; i++) {
      const heading = rows[i].paragraph.paragraphStyle?.namedStyleType?.match(/^HEADING_([1-6])$/);
      if (!heading || rows[i].inTable || !matches(rows[i]) || norm(rows[i].text) !== wanted.at(-1)) continue;
      const start = i;
      let end = start + 1;
      while (end < rows.length) {
        const next = rows[end].paragraph.paragraphStyle?.namedStyleType?.match(/^HEADING_([1-6])$/);
        if (next && !rows[end].inTable && Number(next[1]) <= Number(heading[1])) break;
        end++;
      }
      runs.push(rows.slice(start, end));
    }
    // Subencabezados forman parte de una seccion, pero una ruta exacta repetida
    // sigue siendo ambigua. Usar la ruta completa para desambiguar.
    for (const run of runs) {
      let scope = run;
      if (target.slotLabel) {
        const labels = run.map((r, i) => norm(r.text) === norm(target.slotLabel) ? i : -1).filter(i => i >= 0);
        if (!labels.length) continue;
        if (labels.length !== 1) fail('Rotulo repetido: ' + target.image);
        const start = labels[0] + 1;
        let end = run.length;
        for (let i = start; i < run.length; i++) {
          if (target.slotEndLabel ? norm(run[i].text) === norm(target.slotEndLabel) : /^[^\n:]+:\s*$/u.test(run[i].text)) { end = i; break; }
        }
        scope = run.slice(start, end);
      }
      candidates.push({ tab, rows: scope });
    }
  }
  if (candidates.length !== 1) fail(`Destino ambiguo/ausente (${candidates.length}): ${target.image}`);
  return candidates[0];
}

function inspect(doc, target) {
  const scope = locate(doc, target);
  const images = [], placeholders = [];
  const suggested = value => value && typeof value === 'object' && Object.entries(value).some(([key, v]) =>
    (key.startsWith('suggested') && v && Object.keys(v).length > 0) || suggested(v));
  for (const row of scope.rows) {
    if (suggested(row.paragraph)) fail('Sugerencias sin resolver en destino: ' + target.image);
    for (const element of row.paragraph.elements ?? []) {
      if (element.inlineObjectElement) images.push({ element, row });
      if (element.positionedObjectIds?.length) fail('Imagen posicionada: revisar manualmente ' + target.image);
      const text = element.textRun?.content ?? '';
      let at = text.indexOf(target.placeholder);
      while (at >= 0) {
        placeholders.push({ start: element.startIndex + at, length: target.placeholder.length });
        at = text.indexOf(target.placeholder, at + target.placeholder.length);
      }
    }
    // El texto puede estar dividido entre textRuns por estilos.
    if (!row.paragraph.elements?.some(e => e.textRun?.content?.includes(target.placeholder)) && row.text.includes(target.placeholder)) {
      if (row.paragraph.elements.some(e => !e.textRun)) fail('Placeholder mezclado con objetos: ' + target.image);
      placeholders.push({ start: row.start + row.text.indexOf(target.placeholder), length: target.placeholder.length });
    }
    if (row.paragraph.positionedObjectIds?.length) fail('Imagen posicionada: revisar manualmente ' + target.image);
  }
  return { ...scope, images, placeholders };
}

function fileFor(files, name) {
  const matches = files.filter(f => f.name === name && !f.trashed);
  if (matches.length !== 1) fail('PNG de Drive faltante/duplicado: ' + name);
  if (matches[0].mimeType !== 'image/png') fail('No es PNG: ' + name);
  return matches[0];
}

function imageUrl(file) {
  const meta = file.imageMediaMetadata;
  if (!meta?.width || !meta?.height) fail('Faltan dimensiones de Drive: ' + file.name);
  if (meta.width * meta.height > 25_000_000) {
    const width = Math.max(1, Math.min(5000, Math.floor(Math.sqrt(24_000_000 * meta.width / meta.height))));
    return `https://drive.google.com/thumbnail?id=${encodeURIComponent(file.id)}&sz=w${width}`;
  }
  return `https://drive.google.com/uc?export=download&id=${encodeURIComponent(file.id)}`;
}

function sizeFor(doc, file) {
  for (const tab of tabsOf(doc)) {
    for (const block of tab.body?.content ?? []) {
      const section = block.sectionBreak?.sectionStyle;
      if (section && ((section.columnProperties?.length ?? 0) > 1 ||
          ['marginLeft', 'marginRight', 'marginTop', 'marginBottom'].some(key => section[key] &&
            (section[key].magnitude !== doc.documentStyle?.[key]?.magnitude || section[key].unit !== doc.documentStyle?.[key]?.unit)))) {
        fail('Geometria de seccion/columnas especial: resolver area util antes de publicar');
      }
    }
  }
  const s = doc.documentStyle;
  const width = s?.pageSize?.width?.magnitude;
  const height = s?.pageSize?.height?.magnitude;
  if (!width || !height || s.pageSize.width.unit !== 'PT' || s.pageSize.height.unit !== 'PT') {
    fail('Proveer documentStyle paginado en PT mediante lectura separada');
  }
  const w = width - (s.marginLeft?.magnitude ?? 0) - (s.marginRight?.magnitude ?? 0) - 4;
  const h = height - (s.marginTop?.magnitude ?? 0) - (s.marginBottom?.magnitude ?? 0) - 24;
  if (w <= 0 || h <= 0) fail('Margenes sin area util');
  const ratio = file.imageMediaMetadata.height / file.imageMediaMetadata.width;
  const finalWidth = Math.min(w, h / ratio);
  return { width: { magnitude: finalWidth, unit: 'PT' }, height: { magnitude: finalWidth * ratio, unit: 'PT' } };
}

export function plan(doc, files, targets) {
  if (!doc.documentId || !doc.revisionId) fail('Falta documentId/revisionId actual');
  const jobs = [], skipped = [], positions = new Set();
  for (const target of targets) {
    const found = inspect(doc, target);
    // Conservar por defecto; ninguna sustitucion de imagenes es implicita.
    if (found.images.length) { skipped.push({ image: target.image, reason: 'imagen existente', tabId: found.tab.id }); continue; }
    if (found.placeholders.length !== 1) fail('Placeholder faltante/duplicado: ' + target.image);
    const pos = found.placeholders[0], key = found.tab.id + ':' + pos.start;
    if (positions.has(key)) fail('Dos imagenes comparten destino: ' + key);
    positions.add(key);
    const file = fileFor(files, target.image);
    if (target.expectedMd5 && file.md5Checksum !== target.expectedMd5) fail('Contenido de Drive distinto del PNG preparado: ' + target.image);
    if (pos.length === 0) fail('Placeholder vacio');
    const row = found.rows.find(r => pos.start >= r.start && pos.start < r.end);
    if (row.inTable) fail('Destino en tabla: confirmar ancho de celda antes de publicar');
    jobs.push({ target, fileId: file.id, tabId: found.tab.id, start: pos.start, length: pos.length,
                uri: imageUrl(file), size: sizeFor(doc, file) });
  }
  jobs.sort((a, b) => a.tabId.localeCompare(b.tabId) || b.start - a.start);
  const requests = [];
  for (const j of jobs) {
    requests.push(
      { deleteContentRange: { range: { tabId: j.tabId, startIndex: j.start, endIndex: j.start + j.length } } },
      { insertInlineImage: { location: { tabId: j.tabId, index: j.start }, uri: j.uri, objectSize: j.size } },
      { updateParagraphStyle: { range: { tabId: j.tabId, startIndex: j.start, endIndex: j.start + 1 },
          paragraphStyle: { alignment: 'CENTER', indentStart: { magnitude: 0, unit: 'PT' },
            indentEnd: { magnitude: 0, unit: 'PT' }, indentFirstLine: { magnitude: 0, unit: 'PT' },
            lineSpacing: 100, spaceAbove: { magnitude: 6, unit: 'PT' }, spaceBelow: { magnitude: 6, unit: 'PT' }, keepWithNext: false },
          fields: 'alignment,indentStart,indentEnd,indentFirstLine,lineSpacing,spaceAbove,spaceBelow,keepWithNext' } });
  }
  return { documentId: doc.documentId, jobs, skipped,
           body: { requests, writeControl: { requiredRevisionId: doc.revisionId } } };
}

export function verify(doc, files, targets) {
  const results = targets.map(target => {
    const found = inspect(doc, target);
    const file = fileFor(files, target.image);
    const valid = found.images.filter(({ element, row }) => {
      const object = found.tab.inlineObjects?.[element.inlineObjectElement.inlineObjectId]?.inlineObjectProperties?.embeddedObject;
      const uri = object?.imageProperties?.sourceUri ?? '';
      let id;
      try { id = new URL(uri).searchParams.get('id'); } catch { return false; }
      const expected = sizeFor(doc, file);
      const size = object?.size;
      const dimensionsOk = ['width', 'height'].every(key => size?.[key]?.unit === 'PT' &&
        Number.isFinite(size[key].magnitude) && Math.abs(size[key].magnitude - expected[key].magnitude) <= 1);
      return id === file.id && Boolean(object.imageProperties.contentUri) && dimensionsOk && row.paragraph.paragraphStyle?.alignment === 'CENTER';
    });
    return { image: target.image, ok: valid.length === 1 && found.images.length === 1 && !found.placeholders.length,
             images: found.images.length, placeholders: found.placeholders.length };
  });
  return { ok: results.every(r => r.ok), results };
}

if (process.argv[1] && import.meta.url === pathToFileURL(process.argv[1]).href) {
  try {
    const [mode, docPath, filesPath, targetsPath, output] = process.argv.slice(2);
    if (!['plan', 'verify'].includes(mode) || !output) fail('Uso: node documento.mjs plan|verify doc.json files.json targets.json salida.json');
    const read = path => JSON.parse(readFileSync(path, 'utf8'));
    const doc = read(docPath), list = read(filesPath), targets = read(targetsPath);
    const result = mode === 'plan' ? plan(doc, list.files ?? list, targets) : verify(doc, list.files ?? list, targets);
    writeFileSync(output, JSON.stringify(result, null, 2) + '\n', { flag: 'wx' });
    console.log(mode === 'plan' ? `${result.jobs.length} inserciones; ${result.skipped.length} existentes conservadas` : `Verificacion: ${result.ok}`);
    if (mode === 'verify' && !result.ok) process.exitCode = 1;
  } catch (error) { console.error(error.message); process.exitCode = 1; }
}
