import { test } from 'node:test';
import assert from 'node:assert/strict';
import { plan, verify } from './documento.mjs';

const marker = '[Pegar aquí el diagrama de secuencia]';
const classesMarker = '[Pegar aquí el diagrama de clases]';
const file = (name = 'CUN20-secuencia.png', id = 'file20') => ({
  id, name, mimeType: 'image/png', md5Checksum: 'abcd', imageMediaMetadata: { width: 6000, height: 3000 }
});
const target = (extra = {}) => ({ image: 'CUN20-secuencia.png', headingPath: ['CUN20 Alta'],
  slotLabel: 'Diagrama de secuencia:', placeholder: marker, expectedMd5: 'abcd', ...extra });

function document(rows, tabId = 't.0') {
  let index = 1;
  const content = rows.map(row => {
    const text = row.text ?? '\n';
    const elements = row.image ? [{ startIndex: index, endIndex: index + 1, inlineObjectElement: { inlineObjectId: row.image } },
      { startIndex: index + 1, endIndex: index + 2, textRun: { content: '\n' } }] :
      [{ startIndex: index, endIndex: index + text.length, textRun: { content: text } }];
    const result = { startIndex: index, endIndex: index + (row.image ? 2 : text.length),
      paragraph: { elements, paragraphStyle: { namedStyleType: row.style ?? 'NORMAL_TEXT', alignment: row.align ?? 'START' } } };
    index = result.endIndex;
    return result;
  });
  return { documentId: 'test-doc', revisionId: 'revision-1',
    documentStyle: { pageSize: { width: { magnitude: 595, unit: 'PT' }, height: { magnitude: 842, unit: 'PT' } },
      marginLeft: { magnitude: 85 }, marginRight: { magnitude: 57 }, marginTop: { magnitude: 113 }, marginBottom: { magnitude: 43 } },
    tabs: [{ tabProperties: { tabId }, documentTab: { body: { content }, inlineObjects: {} } }] };
}
const businessRows = () => [
  { text: 'CUN19 Consulta\n', style: 'HEADING_4' },
  { text: 'Alternativa: continuar con CUN20 Alta 😀\n' },
  { text: 'CUN20 Alta\n', style: 'HEADING_4' },
  { text: 'Diagrama de clases:\n' }, { image: 'existing-classes' },
  { text: 'Diagrama de secuencia:\n' }, { text: marker + '\n' }
];

test('Ubica CUN por encabezado, ignora indice/menciones y respeta UTF-16', () => {
  const doc = document(businessRows());
  doc.tabs[0].documentTab.body.content.unshift({ tableOfContents: { content: document([
    { text: 'CUN20 Alta\n', style: 'HEADING_4' }, { text: marker + '\n' }]).tabs[0].documentTab.body.content } });
  const result = plan(doc, [file()], [target()]);
  const p = doc.tabs[0].documentTab.body.content.at(-1);
  assert.equal(result.jobs[0].start, p.startIndex);
  assert.equal(result.body.requests[0].deleteContentRange.range.endIndex, p.endIndex - 1);
  assert.equal(result.jobs.length, 1);
  assert.equal(result.skipped.length, 0); // La imagen de clases no ocupa el slot de secuencia.
});

test('Admite diagramas tecnicos de clases con subencabezados y cualquier tabId', () => {
  const doc = document([{ text: 'T02. Login\n', style: 'HEADING_2' },
    { text: 'Implementacion\n', style: 'HEADING_3' }, { text: 'Diagrama de clases:\n' },
    { text: classesMarker + '\n' }], 'technical-tab');
  const result = plan(doc, [file('T02-login-clases.png')], [target({ image: 'T02-login-clases.png',
    headingPath: ['T02. Login'], slotLabel: 'Diagrama de clases:', placeholder: classesMarker })]);
  assert.equal(result.jobs[0].tabId, 'technical-tab');
  assert.equal(result.jobs[0].size.width.magnitude, 449);
});

test('Orden descendente y revision obligatoria; una ruta ambigua no produce lote', () => {
  const doc = document([...businessRows(), { text: 'CUN21 Baja\n', style: 'HEADING_4' },
    { text: 'Diagrama de secuencia:\n' }, { text: marker + '\n' }]);
  const other = target({ image: 'CUN21-secuencia.png', headingPath: ['CUN21 Baja'] });
  const result = plan(doc, [file(), file('CUN21-secuencia.png', 'file21')], [target(), other]);
  assert.ok(result.jobs[0].start > result.jobs[1].start);
  assert.equal(result.body.writeControl.requiredRevisionId, 'revision-1');
  const dup = document([...businessRows(), ...businessRows()]);
  assert.throws(() => plan(dup, [file()], [target()]), /ambiguo/);
  delete doc.revisionId;
  assert.throws(() => plan(doc, [file()], [target()]), /revisionId/);
});

test('Un PNG homonimo, MD5 diferente o doble destino impide publicar', () => {
  const doc = document(businessRows());
  assert.throws(() => plan(doc, [file(), file()], [target()]), /duplicado/);
  assert.throws(() => plan(doc, [file()], [target({ expectedMd5: 'changed' })]), /Contenido/);
  assert.throws(() => plan(doc, [file()], [target(), target()]), /comparten/);
});

test('Placeholder dividido entre estilos conserva offsets UTF-16', () => {
  const doc = document(businessRows());
  const p = doc.tabs[0].documentTab.body.content.at(-1);
  const split = 12;
  p.paragraph.elements = [
    { startIndex: p.startIndex, textRun: { content: marker.slice(0, split) } },
    { startIndex: p.startIndex + split, textRun: { content: marker.slice(split) + '\n' } }
  ];
  assert.equal(plan(doc, [file()], [target()]).jobs[0].start, p.startIndex);
});

test('Tras timeout, la imagen aplicada se conserva y la verificacion comprueba fuente', () => {
  const rows = businessRows();
  rows[rows.length - 1] = { image: 'new-image', align: 'CENTER' };
  const doc = document(rows);
  doc.tabs[0].documentTab.inlineObjects['new-image'] = { inlineObjectProperties: { embeddedObject: {
    size: { width: { magnitude: 449, unit: 'PT' }, height: { magnitude: 224.5, unit: 'PT' } },
    imageProperties: { sourceUri: 'https://drive.google.com/uc?export=download&id=file20', contentUri: 'https://image' }
  } } };
  const result = plan(doc, [file()], [target()]);
  assert.equal(result.body.requests.length, 0);
  assert.equal(result.skipped.length, 1);
  assert.equal(verify(doc, [file()], [target()]).ok, true);
  doc.tabs[0].documentTab.inlineObjects['new-image'].inlineObjectProperties.embeddedObject.imageProperties.sourceUri = 'https://drive.google.com/uc?id=wrong';
  assert.equal(verify(doc, [file()], [target()]).ok, false);
});

test('Los rotulos genericos delimitan slots de componentes y DER', () => {
  const doc = document([{ text: 'G08\n', style: 'HEADING_2' },
    { text: 'Componentes:\n' }, { text: marker + '\n' }, { text: 'DER:\n' }, { image: 'der' }]);
  const result = plan(doc, [file()], [target({ headingPath: ['G08'], slotLabel: 'Componentes:' })]);
  assert.equal(result.jobs.length, 1);
});

test('La verificacion rechaza una imagen deformada y el plan no supone geometria de seccion', () => {
  const rows = businessRows();
  rows[rows.length - 1] = { image: 'new-image', align: 'CENTER' };
  const doc = document(rows);
  doc.tabs[0].documentTab.inlineObjects['new-image'] = { inlineObjectProperties: { embeddedObject: {
    size: { width: { magnitude: 9999, unit: 'PT' }, height: { magnitude: 1, unit: 'PT' } },
    imageProperties: { sourceUri: 'https://drive.google.com/uc?id=file20', contentUri: 'https://image' }
  } } };
  assert.equal(verify(doc, [file()], [target()]).ok, false);
  const columnsDoc = document(businessRows());
  columnsDoc.tabs[0].documentTab.body.content.unshift({ sectionBreak: { sectionStyle: { columnProperties: [{}, {}] } } });
  assert.throws(() => plan(columnsDoc, [file()], [target()]), /seccion/);
});

test('Dimensiona imagenes muy altas y thumbnail bajo 24 MP', () => {
  const image = file();
  image.imageMediaMetadata = { width: 2000, height: 20000 };
  const j = plan(document(businessRows()), [image], [target()]).jobs[0];
  assert.ok(j.size.height.magnitude <= 662);
  const w = Number(j.uri.match(/sz=w(\d+)/)[1]);
  assert.ok(w * (w * 10) < 25_000_000);
});

test('No edita tablas ni contenido de sugerencias sin resolver', () => {
  const doc = document([{ text: 'CUN20 Alta\n', style: 'HEADING_4' }]);
  const content = document([{ text: 'Diagrama de secuencia:\n' }, { text: marker + '\n' }]).tabs[0].documentTab.body.content;
  doc.tabs[0].documentTab.body.content.push({ table: { tableRows: [{ tableCells: [{ content }] }] } });
  assert.throws(() => plan(doc, [file()], [target()]), /tabla/);
  const suggestedDoc = document(businessRows());
  suggestedDoc.tabs[0].documentTab.body.content.at(-1).paragraph.elements[0].suggestedInsertionIds = ['pending'];
  assert.throws(() => plan(suggestedDoc, [file()], [target()]), /Sugerencias/);
});
