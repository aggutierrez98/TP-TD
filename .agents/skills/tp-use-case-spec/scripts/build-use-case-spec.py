"""Generador de especificaciones de casos de uso para el TP.

Lee uno o mas archivos JSON con la definicion de casos de uso y produce:

- un documento Word (`.docx`) con una tabla ("cuadro") por caso de uso, y
- un espejo Markdown (`.md`) versionable en git para revisar diffs.

Modos:

- **Puntual** (un solo JSON): salidas por defecto en
  `diagramas/<requisito>/<requisito>-casos-de-uso.docx` (y `.md`).
- **Consolidado** (varios JSON o un directorio): todas las especificaciones
  quedan en UN solo `.docx`, un cuadro abajo de otro, separadas por el titulo
  del requisito (T0X). Salida por defecto:
  `diagramas/TP-especificaciones-casos-de-uso.docx` (y `.md`).

El formato de cada caso de uso sigue el estandar documentado en
`.agents/skills/tp-use-case-spec/references/use-case-spec-format.md`.

Uso tipico (desde la raiz del repo):

    # Puntual
    python .agents/skills/tp-use-case-spec/scripts/build-use-case-spec.py \
        --input diagramas/T05-multidioma/T05-multidioma-casos-de-uso.json

    # Consolidado: varios JSON explicitos, en orden
    python .agents/skills/tp-use-case-spec/scripts/build-use-case-spec.py \
        --input diagramas/T02-login-logout/T02-login-logout-CU01-login-caso-de-uso.json \
                diagramas/T02-login-logout/T02-login-logout-CU02-registro-caso-de-uso.json

    # Consolidado: todos los CU bajo diagramas/ (ordenados por ruta)
    python .agents/skills/tp-use-case-spec/scripts/build-use-case-spec.py --input diagramas

Requisitos: Python 3.8+ y `python-docx` (`pip install python-docx`).
"""

import argparse
import glob
import json
import os
import re
import sys


# Token de codigo de CU al principio de id_nombre: "CU01", "CU-T05-01", "CU 3", etc.
CU_CODE_RE = re.compile(r"^\s*CU[\w\-]*?\d+\s*")


def strip_cu_code(id_nombre):
    """Devuelve solo el nombre del CU, sin el codigo 'CUNN' inicial."""
    return CU_CODE_RE.sub("", id_nombre or "").strip()


def renumber_specs(specs, inicio=1):
    """Reasigna codigos de CU de forma global y correlativa (CU01, CU02, ...).

    Recorre los requisitos en el orden recibido y renumera cada caso de uso,
    conservando su nombre. Muta las copias en memoria; no toca los JSON fuente.
    """
    contador = inicio
    for data in specs:
        for caso in data.get("casos_de_uso", []):
            caso["id_nombre"] = "CU%02d %s" % (contador, strip_cu_code(caso.get("id_nombre", "")))
            contador += 1
    return specs


# Campos obligatorios de cada caso de uso. El orden define el render de la tabla.
SCALAR_FIELDS = [
    ("id_nombre", "ID y Nombre"),
    ("estado", "Estado"),
    ("descripcion", "Descripcion"),
    ("actor_principal", "Actor Principal"),
    ("actor_secundario", "Actor Secundario"),
    ("precondiciones", "Precondiciones"),
    ("puntos_extension", "Puntos de Extension"),
    ("condicion", "Condicion"),
]
LIST_FIELDS = [
    ("escenario_principal", "Escenario Principal"),
    ("flujos_alternativos", "Flujos Alternativos"),
]
POST_FIELD = ("postcondiciones", "Postcondiciones")

ESTADOS_CONOCIDOS = {"Implementado", "Definido", "Propuesto"}

FONT_NAME = "Calibri"

# Patrones de archivos de caso de uso al expandir un directorio.
CU_GLOBS = ("*-caso-de-uso.json", "*-casos-de-uso.json")

DEFAULT_DOC_TITLE = "Especificaciones de Casos de Uso"
DEFAULT_CONSOLIDATED_BASENAME = "TP-especificaciones-casos-de-uso"


def find_repo_root(start):
    """Sube directorios hasta encontrar `TpIDS.sln` (raiz del repo)."""
    current = os.path.abspath(start)
    while True:
        if os.path.exists(os.path.join(current, "TpIDS.sln")):
            return current
        parent = os.path.dirname(current)
        if parent == current:
            # Fallback: 4 niveles arriba de scripts/ -> raiz del repo.
            return os.path.abspath(os.path.join(start, "..", "..", "..", ".."))
        current = parent


def gather_input_files(inputs):
    """Expande la lista de --input: archivos tal cual y directorios via glob.

    Los directorios se expanden a los JSON de caso de uso hallados recursivamente,
    ordenados por ruta (asi T02 < T03 < ... < T07 quedan en orden). Se preserva el
    orden de aparicion y se descartan duplicados.
    """
    files = []
    for entry in inputs:
        if os.path.isdir(entry):
            matches = []
            for pattern in CU_GLOBS:
                matches.extend(glob.glob(os.path.join(entry, "**", pattern), recursive=True))
            files.extend(sorted(matches))
        else:
            files.append(entry)

    seen = set()
    ordered = []
    for path in files:
        key = os.path.abspath(path)
        if key not in seen:
            seen.add(key)
            ordered.append(path)
    return ordered


def load_spec(input_path):
    with open(input_path, "r", encoding="utf-8") as handle:
        data = json.load(handle)

    if "casos_de_uso" not in data or not isinstance(data["casos_de_uso"], list):
        raise ValueError("El JSON %s debe tener una lista 'casos_de_uso'." % input_path)
    if not data["casos_de_uso"]:
        raise ValueError("La lista 'casos_de_uso' de %s esta vacia." % input_path)

    errors = []
    for index, caso in enumerate(data["casos_de_uso"], start=1):
        prefijo = "caso #%d" % index
        if isinstance(caso, dict) and caso.get("id_nombre"):
            prefijo = caso["id_nombre"]
        for key, label in SCALAR_FIELDS + [POST_FIELD]:
            if not isinstance(caso, dict) or not str(caso.get(key, "")).strip():
                errors.append("%s: falta el campo obligatorio '%s' (%s)." % (prefijo, key, label))
        for key, label in LIST_FIELDS:
            value = caso.get(key) if isinstance(caso, dict) else None
            if key == "escenario_principal":
                if not isinstance(value, list) or not value:
                    errors.append("%s: '%s' (%s) debe ser una lista con al menos un paso." % (prefijo, key, label))
            else:
                if value is not None and not isinstance(value, list):
                    errors.append("%s: '%s' (%s) debe ser una lista (o omitirse)." % (prefijo, key, label))
        estado = caso.get("estado") if isinstance(caso, dict) else None
        if estado and estado not in ESTADOS_CONOCIDOS:
            sys.stderr.write(
                "Aviso: %s tiene estado '%s' fuera de los valores sugeridos %s.\n"
                % (prefijo, estado, sorted(ESTADOS_CONOCIDOS))
            )

    if errors:
        raise ValueError("Errores de validacion en %s:\n - %s" % (input_path, "\n - ".join(errors)))

    return data


# --------------------------------------------------------------------------- #
# Render Word (.docx)
# --------------------------------------------------------------------------- #
def build_docx(specs, output_docx, doc_title=None, page_break_por_requisito=False):
    """Renderiza una o varias especificaciones en un unico .docx.

    - `specs`: lista de dicts (uno por JSON/requisito).
    - `doc_title`: si se pasa, agrega un titulo general del documento arriba de todo
      (modo consolidado). En modo puntual (un solo spec) suele ir en None.
    - `page_break_por_requisito`: si True, cada requisito arranca en pagina nueva.
      Por defecto False -> las especificaciones quedan una abajo de otra.
    """
    from docx import Document
    from docx.enum.text import WD_ALIGN_PARAGRAPH
    from docx.oxml import OxmlElement
    from docx.oxml.ns import qn
    from docx.shared import Inches, Pt, RGBColor

    def set_font(run, size, bold=False, italic=False, color=None):
        run.bold = bold
        run.italic = italic
        run.font.name = FONT_NAME
        run._element.rPr.rFonts.set(qn("w:ascii"), FONT_NAME)
        run._element.rPr.rFonts.set(qn("w:hAnsi"), FONT_NAME)
        run.font.size = Pt(size)
        if color is not None:
            run.font.color.rgb = color

    def set_single_cell_borders(cell):
        tc_pr = cell._tc.get_or_add_tcPr()
        borders = tc_pr.first_child_found_in("w:tcBorders")
        if borders is None:
            borders = OxmlElement("w:tcBorders")
            tc_pr.append(borders)
        for edge in ("top", "left", "bottom", "right"):
            elem = borders.find(qn("w:" + edge))
            if elem is None:
                elem = OxmlElement("w:" + edge)
                borders.append(elem)
            elem.set(qn("w:val"), "single")
            elem.set(qn("w:sz"), "6")
            elem.set(qn("w:space"), "0")
            elem.set(qn("w:color"), "000000")

    def set_cell_width(cell, dxa=9360):
        # Ancho explicito por celda (w:tcW). Word respeta el ancho de tabla aunque
        # falte, pero el importador de Google Docs dimensiona las columnas segun el
        # w:tcW de cada celda: sin esto, al subir el .docx a Drive la columna colapsa
        # al ancho del contenido y el cuadro queda angosto.
        tc_pr = cell._tc.get_or_add_tcPr()
        tc_w = tc_pr.first_child_found_in("w:tcW")
        if tc_w is None:
            tc_w = OxmlElement("w:tcW")
            tc_pr.append(tc_w)
        tc_w.set(qn("w:w"), str(dxa))
        tc_w.set(qn("w:type"), "dxa")

    def set_cell_margins(cell, left=40, right=40, top=0, bottom=0):
        tc_pr = cell._tc.get_or_add_tcPr()
        tc_mar = tc_pr.first_child_found_in("w:tcMar")
        if tc_mar is None:
            tc_mar = OxmlElement("w:tcMar")
            tc_pr.append(tc_mar)
        for edge, value in (("top", top), ("left", left), ("bottom", bottom), ("right", right)):
            elem = tc_mar.find(qn("w:" + edge))
            if elem is None:
                elem = OxmlElement("w:" + edge)
                tc_mar.append(elem)
            elem.set(qn("w:w"), str(value))
            elem.set(qn("w:type"), "dxa")

    def add_cu_line(cell, label, value=""):
        first = len(cell.paragraphs) == 1 and not cell.paragraphs[0].text
        p = cell.paragraphs[0] if first else cell.add_paragraph()
        p.paragraph_format.space_after = Pt(0)
        p.paragraph_format.space_before = Pt(0)
        set_font(p.add_run(label), 10, bold=True)
        if value:
            set_font(p.add_run(value), 10)
        return p

    def add_steps(cell, steps, empty_text):
        lines = steps or [empty_text]
        for step in lines:
            p = cell.add_paragraph()
            p.paragraph_format.space_after = Pt(0)
            p.paragraph_format.space_before = Pt(0)
            set_font(p.add_run(step), 10)

    def add_cu_table(document, caso):
        total_rows = len(SCALAR_FIELDS) + len(LIST_FIELDS) + 1  # +1 postcondiciones
        table = document.add_table(rows=total_rows, cols=1)
        table.autofit = False
        table.style = "Table Grid"
        tbl_pr = table._tbl.tblPr

        tbl_w = tbl_pr.first_child_found_in("w:tblW")
        if tbl_w is not None:
            tbl_w.set(qn("w:w"), "9360")
            tbl_w.set(qn("w:type"), "dxa")

        tbl_layout = tbl_pr.first_child_found_in("w:tblLayout")
        if tbl_layout is None:
            tbl_layout = OxmlElement("w:tblLayout")
            tbl_pr.append(tbl_layout)
        tbl_layout.set(qn("w:type"), "fixed")

        tbl_jc = tbl_pr.first_child_found_in("w:jc")
        if tbl_jc is None:
            tbl_jc = OxmlElement("w:jc")
            tbl_pr.append(tbl_jc)
        tbl_jc.set(qn("w:val"), "left")

        borders = tbl_pr.first_child_found_in("w:tblBorders")
        if borders is not None:
            tbl_pr.remove(borders)

        table._tbl.tblGrid.gridCol_lst[0].w = 9360

        cells = [row.cells[0] for row in table.rows]
        for cell in cells:
            cell.text = ""
            set_single_cell_borders(cell)
            set_cell_width(cell, 9360)
            set_cell_margins(cell)

        idx = 0
        for key, label in SCALAR_FIELDS:
            add_cu_line(cells[idx], label + ": ", str(caso.get(key, "")).strip())
            idx += 1

        add_cu_line(cells[idx], "Escenario Principal:", "")
        add_steps(cells[idx], caso.get("escenario_principal"), "Sin pasos definidos.")
        idx += 1

        add_cu_line(cells[idx], "Flujos Alternativos:", "")
        add_steps(cells[idx], caso.get("flujos_alternativos"), "Sin flujos alternativos relevantes.")
        idx += 1

        add_cu_line(cells[idx], POST_FIELD[1] + ": ", str(caso.get(POST_FIELD[0], "")).strip())
        return table

    def add_requisito_block(document, data, es_primero):
        # Titulo del requisito (T0X): mismo bloque que usa el docx puntual.
        # Actua como separador entre requisitos en el modo consolidado.
        if page_break_por_requisito and not es_primero:
            document.add_page_break()

        titulo = data.get("titulo", "Especificaciones de casos de uso")
        title = document.add_paragraph()
        title.alignment = WD_ALIGN_PARAGRAPH.CENTER
        title.paragraph_format.space_before = Pt(0 if es_primero else 14)
        title.paragraph_format.space_after = Pt(6)
        set_font(title.add_run(titulo), 20, bold=True, color=RGBColor(0x2E, 0x74, 0xB5))

        subtitulo = data.get("subtitulo")
        if subtitulo:
            subtitle = document.add_paragraph()
            subtitle.alignment = WD_ALIGN_PARAGRAPH.CENTER
            subtitle.paragraph_format.space_after = Pt(10)
            set_font(subtitle.add_run(subtitulo), 11, italic=True)

        introduccion = data.get("introduccion")
        if introduccion:
            p = document.add_paragraph()
            p.alignment = WD_ALIGN_PARAGRAPH.JUSTIFY
            p.paragraph_format.space_after = Pt(6)
            p.paragraph_format.line_spacing = 1.1
            set_font(p.add_run(introduccion), 11)

        # Un cuadro abajo de otro (sin salto de pagina entre CU).
        for caso in data["casos_de_uso"]:
            add_cu_table(document, caso)
            document.add_paragraph()

    document = Document()
    section = document.sections[0]
    section.top_margin = Inches(1)
    section.bottom_margin = Inches(1)
    section.left_margin = Inches(1)
    section.right_margin = Inches(1)

    styles = document.styles
    normal = styles["Normal"]
    normal.font.name = FONT_NAME
    normal._element.rPr.rFonts.set(qn("w:ascii"), FONT_NAME)
    normal._element.rPr.rFonts.set(qn("w:hAnsi"), FONT_NAME)
    normal.font.size = Pt(11)

    for name, size, before, after in (
        ("Heading 1", 16, 16, 8),
        ("Heading 2", 13, 12, 6),
    ):
        style = styles[name]
        style.font.name = FONT_NAME
        style._element.rPr.rFonts.set(qn("w:ascii"), FONT_NAME)
        style._element.rPr.rFonts.set(qn("w:hAnsi"), FONT_NAME)
        style.font.size = Pt(size)
        style.font.color.rgb = RGBColor(0x2E, 0x74, 0xB5)
        style.paragraph_format.space_before = Pt(before)
        style.paragraph_format.space_after = Pt(after)

    # Titulo general del documento (solo modo consolidado).
    if doc_title:
        doc_title_p = document.add_paragraph()
        doc_title_p.alignment = WD_ALIGN_PARAGRAPH.CENTER
        doc_title_p.paragraph_format.space_after = Pt(14)
        set_font(doc_title_p.add_run(doc_title), 24, bold=True, color=RGBColor(0x1F, 0x4E, 0x79))

    for index, data in enumerate(specs):
        add_requisito_block(document, data, es_primero=(index == 0))

    os.makedirs(os.path.dirname(os.path.abspath(output_docx)), exist_ok=True)
    document.save(output_docx)


# --------------------------------------------------------------------------- #
# Render Markdown espejo (.md)
# --------------------------------------------------------------------------- #
def build_markdown(specs, output_md, doc_title=None):
    lines = []
    consolidado = doc_title is not None
    if consolidado:
        lines.append("# " + doc_title)
        lines.append("")

    requisito_heading = "## " if consolidado else "# "

    for data in specs:
        titulo = data.get("titulo", "Especificaciones de casos de uso")
        lines.append(requisito_heading + titulo)
        lines.append("")
        subtitulo = data.get("subtitulo")
        if subtitulo:
            lines.append("_" + subtitulo + "_")
            lines.append("")
        introduccion = data.get("introduccion")
        if introduccion:
            lines.append(introduccion)
            lines.append("")

        for caso in data["casos_de_uso"]:
            lines.append("---")
            lines.append("")
            for key, label in SCALAR_FIELDS:
                lines.append("**%s:** %s  " % (label, str(caso.get(key, "")).strip()))
            lines.append("")
            lines.append("**Escenario Principal:**")
            for step in caso.get("escenario_principal") or ["Sin pasos definidos."]:
                lines.append(step)
            lines.append("")
            lines.append("**Flujos Alternativos:**")
            for step in caso.get("flujos_alternativos") or ["Sin flujos alternativos relevantes."]:
                lines.append(step)
            lines.append("")
            lines.append("**%s:** %s" % (POST_FIELD[1], str(caso.get(POST_FIELD[0], "")).strip()))
            lines.append("")

    os.makedirs(os.path.dirname(os.path.abspath(output_md)), exist_ok=True)
    with open(output_md, "w", encoding="utf-8") as handle:
        handle.write("\n".join(lines).rstrip() + "\n")


def resolve_single_outputs(data, args, repo_root):
    """Salidas del modo puntual (un solo JSON), compatible con el comportamiento previo."""
    output_docx = args.output_docx
    output_md = args.output_md
    requisito = data.get("requisito")

    if output_docx is None:
        if not requisito:
            raise ValueError(
                "El JSON no declara 'requisito'; especifica la salida con --output-docx."
            )
        output_docx = os.path.join(
            repo_root, "diagramas", requisito, requisito + "-casos-de-uso.docx"
        )
    if output_md is None:
        base, _ = os.path.splitext(output_docx)
        output_md = base + ".md"
    return output_docx, output_md


def resolve_consolidated_outputs(args, repo_root):
    output_docx = args.output_docx or os.path.join(
        repo_root, "diagramas", DEFAULT_CONSOLIDATED_BASENAME + ".docx"
    )
    output_md = args.output_md
    if output_md is None:
        base, _ = os.path.splitext(output_docx)
        output_md = base + ".md"
    return output_docx, output_md


def main(argv=None):
    parser = argparse.ArgumentParser(description="Genera especificaciones de casos de uso (.docx + .md).")
    parser.add_argument(
        "--input", "-i", required=True, nargs="+",
        help="Uno o mas JSON de casos de uso, o directorios (se expanden a *-caso-de-uso.json).",
    )
    parser.add_argument("--output-docx", help="Ruta de salida del .docx (override).")
    parser.add_argument("--output-md", help="Ruta de salida del .md espejo (override).")
    parser.add_argument("--no-md", action="store_true", help="No generar el espejo Markdown.")
    parser.add_argument("--no-docx", action="store_true", help="No generar el documento Word.")
    parser.add_argument(
        "--consolidar", action="store_true",
        help="Fuerza el modo consolidado (un solo docx) aunque haya un solo JSON.",
    )
    parser.add_argument(
        "--doc-title", default=DEFAULT_DOC_TITLE,
        help="Titulo general del documento en modo consolidado.",
    )
    parser.add_argument(
        "--page-break-por-requisito", action="store_true",
        help="En consolidado, arranca cada requisito en pagina nueva (por defecto van seguidos).",
    )
    parser.add_argument(
        "--renumerar-global", action="store_true",
        help="Fuerza el renumerado global de CU (CU01, CU02, ...) tambien en modo puntual.",
    )
    parser.add_argument(
        "--sin-renumerar", action="store_true",
        help="Desactiva el renumerado global en modo consolidado (respeta el codigo de cada JSON).",
    )
    parser.add_argument(
        "--inicio", type=int, default=1,
        help="Numero inicial para el renumerado global (por defecto 1).",
    )
    args = parser.parse_args(argv)

    repo_root = find_repo_root(os.path.dirname(__file__))

    input_files = gather_input_files(args.input)
    if not input_files:
        sys.stderr.write("Error: no se encontraron JSON de casos de uso en --input.\n")
        return 1

    try:
        specs = [load_spec(path) for path in input_files]
    except (OSError, ValueError, json.JSONDecodeError) as exc:
        sys.stderr.write("Error: %s\n" % exc)
        return 1

    consolidado = args.consolidar or len(specs) > 1

    # Renumerado global de CU (CU01, CU02, ... de corrido). Por defecto activo en
    # modo consolidado (para que no se repitan los codigos entre requisitos); en
    # modo puntual se mantiene la numeracion local salvo --renumerar-global.
    renumerar = args.renumerar_global or (consolidado and not args.sin_renumerar)
    if renumerar:
        renumber_specs(specs, inicio=args.inicio)

    if consolidado:
        output_docx, output_md = resolve_consolidated_outputs(args, repo_root)
        doc_title = args.doc_title
    else:
        try:
            output_docx, output_md = resolve_single_outputs(specs[0], args, repo_root)
        except ValueError as exc:
            sys.stderr.write("Error: %s\n" % exc)
            return 1
        doc_title = None

    if not args.no_docx:
        try:
            build_docx(specs, output_docx, doc_title=doc_title,
                       page_break_por_requisito=args.page_break_por_requisito)
        except ImportError:
            sys.stderr.write(
                "Error: falta 'python-docx'. Instalalo con: pip install python-docx\n"
            )
            return 1
        print("Word generado: %s" % output_docx)

    if not args.no_md:
        build_markdown(specs, output_md, doc_title=doc_title)
        print("Markdown generado: %s" % output_md)

    total_cu = sum(len(data["casos_de_uso"]) for data in specs)
    print("Requisitos: %d | Total de casos de uso: %d" % (len(specs), total_cu))
    if consolidado:
        print("Modo: consolidado (%d archivos)" % len(input_files))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
