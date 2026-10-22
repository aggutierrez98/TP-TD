#!/usr/bin/env python3
"""Prepara un lote local de PlantUML para carga manual a Drive."""
import argparse
import glob
import hashlib
import io
import json
import math
import os
from pathlib import Path
import re
import subprocess
import sys
import tempfile


def sin_leyendas(texto):
    salida, dentro = [], False
    for linea in texto.splitlines(keepends=True):
        if re.match(r"^\s*legend\b", linea, re.I):
            if dentro:
                raise ValueError("Bloque legend anidado")
            dentro = True
        elif re.match(r"^\s*end\s*legend\b", linea, re.I):
            if not dentro:
                raise ValueError("endlegend sin legend")
            dentro = False
        elif not dentro:
            salida.append(linea)
    if dentro:
        raise ValueError("legend sin cierre")
    return "".join(salida)


def main():
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument("sources", nargs="+", help="Archivos o glob .puml (entre comillas)")
    p.add_argument("--output", required=True, help="Carpeta NUEVA de PNG para subir")
    p.add_argument("--jar", default=os.environ.get("PLANTUML_JAR"))
    p.add_argument("--java", default="java")
    p.add_argument("--dpi", type=int, default=200)
    args = p.parse_args()
    try:
        from PIL import Image
    except ImportError:
        p.error("Falta Pillow en el Python de desarrollo: usar un venv con Pillow.")
    if not args.jar or not Path(args.jar).is_file():
        p.error("Indicar --jar o PLANTUML_JAR valido")
    if not 96 <= args.dpi <= 600:
        p.error("--dpi debe estar entre 96 y 600")
    sources = []
    for pattern in args.sources:
        matches = glob.glob(pattern, recursive=True)
        if not matches:
            p.error("Sin coincidencias: " + pattern)
        sources.extend(Path(x).resolve() for x in matches)
    sources = sorted(set(sources))
    if any(x.suffix.lower() != ".puml" or not x.is_file() for x in sources):
        p.error("Seleccionar exclusivamente archivos .puml")
    if len({x.stem.casefold() for x in sources}) != len(sources):
        p.error("Nombres PNG duplicados: resolver homonimos antes de exportar")
    output = Path(args.output).resolve()
    if output.exists():
        p.error("La carpeta de salida ya existe; usar un nombre de lote nuevo")
    output.parent.mkdir(parents=True, exist_ok=True)
    # Publicar la carpeta solo cuando TODO el lote es valido: no confundir un
    # render incompleto con archivos listos para subir.
    with tempfile.TemporaryDirectory(prefix=".png-lote-", dir=output.parent) as tmp:
        staging = Path(tmp)
        artifacts = []
        for source in sources:
            raw = source.read_bytes()
            text = raw.decode("utf-8-sig")
            if len(re.findall(r"^\s*@start\w+\b", text, re.M | re.I)) != 1:
                raise ValueError(f"Se requiere un diagrama por archivo: {source}")
            # Expandir primero: las leyendas tambien pueden vivir en includes
            # o ser emitidas por macros de PlantUML.
            expanded = subprocess.run([args.java, "-jar", str(Path(args.jar).resolve()),
                                       "-charset", "UTF-8", "-preproc", "-pipe"],
                                      input=raw, stdout=subprocess.PIPE, stderr=subprocess.PIPE,
                                      cwd=source.parent, timeout=180)
            if expanded.returncode:
                raise ValueError(f"Preprocesado PlantUML fallo: {source}\n" +
                                 expanded.stderr.decode("utf-8", errors="replace"))
            expanded_text = expanded.stdout.decode("utf-8-sig")
            if len(re.findall(r"^\s*@start\w+\b", expanded_text, re.M | re.I)) != 1:
                raise ValueError(f"Preprocesado no produjo un unico diagrama: {source}")
            cleaned = sin_leyendas(expanded_text)
            rendered = cleaned.encode("utf-8")
            command = [args.java, "-Xmx2g", "-DPLANTUML_LIMIT_SIZE=20000", "-jar",
                       str(Path(args.jar).resolve()), "-tpng", "-charset", "UTF-8",
                       f"-Sdpi={args.dpi}", "-pipe"]
            result = subprocess.run(command, input=rendered, stdout=subprocess.PIPE,
                                    stderr=subprocess.PIPE, cwd=source.parent, timeout=180)
            if result.returncode or not result.stdout.startswith(b"\x89PNG\r\n\x1a\n"):
                raise ValueError(f"PlantUML fallo: {source}\n" +
                                 result.stderr.decode("utf-8", errors="replace"))
            # PNG producido por el renderizador local, no por una descarga.
            Image.MAX_IMAGE_PIXELS = None
            with Image.open(io.BytesIO(result.stdout)) as image:
                image.load()
                original = image.size
                factor = min(1.0, math.sqrt(24_000_000 / (image.width * image.height)))
                if factor < 1:
                    image = image.resize((max(1, int(image.width * factor)),
                                          max(1, int(image.height * factor))), Image.Resampling.LANCZOS)
                target = staging / (source.stem + ".png")
                image.save(target, dpi=(args.dpi, args.dpi), optimize=True)
                if target.stat().st_size >= 45_000_000:
                    raise ValueError(f"PNG demasiado pesado para publicar: {source}")
                with Image.open(target) as check:
                    check.verify()
                artifacts.append({"source": str(source), "sourceSha256": hashlib.sha256(raw).hexdigest(),
                                  "image": target.name, "sha256": hashlib.sha256(target.read_bytes()).hexdigest(),
                                  "md5": hashlib.md5(target.read_bytes()).hexdigest(),
                                  "width": image.width, "height": image.height,
                                  "originalWidth": original[0], "originalHeight": original[1],
                                  "dpi": args.dpi, "bytes": target.stat().st_size,
                                  "legendOmitted": cleaned != expanded_text})
        manifest = {"version": 1, "artifacts": artifacts}
        (staging / "manifest.json").write_text(json.dumps(manifest, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
        staging.rename(output)
    print(f"Preparados {len(artifacts)} PNG en: {output}")
    print("Subir SOLO los PNG a una carpeta de Drive y compartir su enlace para continuar.")


if __name__ == "__main__":
    try:
        main()
    except (ValueError, OSError, subprocess.TimeoutExpired) as exc:
        print(str(exc), file=sys.stderr)
        sys.exit(1)
