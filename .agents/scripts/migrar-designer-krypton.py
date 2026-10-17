#!/usr/bin/env python3
"""
Migra un archivo *.Designer.cs de controles WinForms nativos a controles Krypton.

Herramienta de desarrollo del cambio OpenSpec `migrar-ui-krypton` (no forma parte de la
solucion ni se necesita para compilar). Aplica solo transformaciones mecanicas validadas
en el spike; todo lo demas queda para revision manual y lo avisa en el reporte.

Uso:
    python3 .agents/scripts/migrar-designer-krypton.py UI/FormLogin.Designer.cs            # dry-run (muestra el diff)
    python3 .agents/scripts/migrar-designer-krypton.py --escribir UI/FormLogin.Designer.cs  # escribe el archivo
    python3 .agents/scripts/migrar-designer-krypton.py --solo-etiquetas --escribir UI/X.Designer.cs  # solo ajusta etiquetas de un archivo ya migrado
    python3 .agents/scripts/migrar-designer-krypton.py --solo-estilo-boton --escribir UI/X.Designer.cs  # solo asigna el estilo de boton de un archivo ya migrado

Reglas:
  - Label, Button, TextBox, ComboBox, DataGridView, GroupBox, ListBox, RadioButton -> Krypton*
  - CheckBox -> TpIDS.CheckBoxAccesible, DateTimePicker -> TpIDS.DateTimePickerAccesible (UI/Tema)
  - Text de Label/Button/CheckBox/RadioButton -> Values.Text; Text de GroupBox -> Values.Heading
  - Font -> StateCommon.* segun el control (los controles Krypton ignoran Font)
  - GroupBox: los hijos pasan a Panel.Controls y su Y se corrige por el alto del encabezado
  - ForeColor -> StateCommon.*.Color1; se quitan propiedades que Krypton no tiene (UseVisualStyleBackColor, FlatStyle, BackColor, ...)
  - Los Name, Tag y handlers no se tocan (los AutomationId de los tests UI dependen de ellos)
  - FlowLayoutPanel, TableLayoutPanel, PictureBox y el resto quedan nativos
"""
import difflib
import re
import sys

TIPOS = {
    "Label": "Krypton.Toolkit.KryptonLabel",
    "Button": "Krypton.Toolkit.KryptonButton",
    "TextBox": "Krypton.Toolkit.KryptonTextBox",
    "ComboBox": "Krypton.Toolkit.KryptonComboBox",
    "DataGridView": "Krypton.Toolkit.KryptonDataGridView",
    "GroupBox": "Krypton.Toolkit.KryptonGroupBox",
    "ListBox": "Krypton.Toolkit.KryptonListBox",
    "RadioButton": "Krypton.Toolkit.KryptonRadioButton",
    "CheckBox": "TpIDS.CheckBoxAccesible",
    "DateTimePicker": "TpIDS.DateTimePickerAccesible",
}

# Propiedades que Krypton no tiene o que no hacen falta: se eliminan.
PROPIEDADES_A_QUITAR = {"UseVisualStyleBackColor", "FlatStyle", "UseCompatibleTextRendering", "BackColor", "IntegralHeight"}

# Control -> propiedad donde Krypton guarda el texto.
TEXTO = {"Label": "Values.Text", "Button": "Values.Text", "CheckBox": "Values.Text", "RadioButton": "Values.Text", "GroupBox": "Values.Heading"}

# Control -> donde Krypton guarda la fuente.
FUENTE = {
    "Label": "StateCommon.ShortText.Font",
    "Button": "StateCommon.Content.ShortText.Font",
    "CheckBox": "StateCommon.ShortText.Font",
    "RadioButton": "StateCommon.ShortText.Font",
    "TextBox": "StateCommon.Content.Font",
    "ComboBox": "StateCommon.ComboBox.Content.Font",
}

# Control -> donde Krypton guarda el color del texto (ForeColor tiene sentido semantico, p. ej. un rojo de error).
COLOR_TEXTO = {
    "Label": "StateCommon.ShortText.Color1",
    "Button": "StateCommon.Content.ShortText.Color1",
    "CheckBox": "StateCommon.ShortText.Color1",
    "RadioButton": "StateCommon.ShortText.Color1",
    "TextBox": "StateCommon.Content.ShortText.Color1",
    "ComboBox": "StateCommon.ComboBox.Content.ShortText.Color1",
}

# Propiedades sin equivalente directo: se conservan y se avisan para revision manual.
REVISAR = {"TextAlign", "AutoEllipsis"}

# KryptonLabel es mas alto que el Label nativo: sin este ajuste su fondo pisa el borde del campo de abajo.
DESPLAZAMIENTO_Y_ETIQUETA = 4

# Alto aproximado del encabezado del GroupBox nativo que Krypton ya reserva fuera del Panel.
DESPLAZAMIENTO_Y_GRUPO = 16


def asignar_estilo_boton(texto):
    """Agrega ButtonStyle = Custom1 (el estilo verde de la paleta) a cada KryptonButton que no lo tenga."""
    botones = re.findall(r"this\.(\w+) = new Krypton\.Toolkit\.KryptonButton\(\);", texto)
    cantidad = 0
    for nombre in botones:
        if re.search(r"this\.%s\.ButtonStyle =" % re.escape(nombre), texto):
            continue
        patron = r"(^\s*)(this\.%s\.Location = new System\.Drawing\.Point\([^;]*\);)" % re.escape(nombre)
        texto, n = re.subn(patron, r"\1this.%s.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;\n\1\2" % nombre, texto, count=1, flags=re.M)
        cantidad += n
    return texto, cantidad


def ajustar_etiquetas(texto, dy=DESPLAZAMIENTO_Y_ETIQUETA):
    """Sube dy px las KryptonLabel (salvo las de titulo, que tienen Font propia). Devuelve (texto, cantidad)."""
    etiquetas = re.findall(r"this\.(\w+) = new Krypton\.Toolkit\.KryptonLabel\(\);", texto)
    con_fuente = set(re.findall(r"this\.(\w+)\.StateCommon\.ShortText\.Font =", texto))
    cantidad = 0

    def corregir(m):
        nonlocal cantidad
        nombre = m.group(2)
        if nombre not in etiquetas or nombre in con_fuente:
            return m.group(0)
        x, y = int(m.group(3)), int(m.group(4))
        cantidad += 1
        return "%sthis.%s.Location = new System.Drawing.Point(%d, %d);" % (m.group(1), nombre, x, max(0, y - dy))

    texto = re.sub(r"^(\s*)this\.(\w+)\.Location = new System\.Drawing\.Point\((-?\d+), (-?\d+)\);", corregir, texto, flags=re.M)
    return texto, cantidad


def migrar(origen):
    reporte = []
    texto = origen

    # 1) Controles a migrar: nombre -> tipo corto.
    controles = {}
    for nombre, tipo in re.findall(r"this\.(\w+) = new System\.Windows\.Forms\.(\w+)\(\);", texto):
        if tipo in TIPOS:
            controles[nombre] = tipo

    # 2) Tipos en las declaraciones.
    for nombre, tipo in controles.items():
        texto = texto.replace(
            "this.%s = new System.Windows.Forms.%s();" % (nombre, tipo),
            "this.%s = new %s();" % (nombre, TIPOS[tipo]),
        )
        texto = re.sub(
            r"private System\.Windows\.Forms\.%s %s;" % (tipo, re.escape(nombre)),
            "private %s %s;" % (TIPOS[tipo], nombre),
            texto,
        )

    # 3) Hijos de cada GroupBox (para moverlos al Panel y corregir su Y).
    grupos = [n for n, t in controles.items() if t == "GroupBox"]
    hijos = {}
    for grupo in grupos:
        hijos[grupo] = re.findall(r"this\.%s\.Controls\.Add\(this\.(\w+)\);" % re.escape(grupo), texto)
    hijo_de = {h: g for g, hs in hijos.items() for h in hs}

    # 4) Propiedades, linea por linea.
    nuevas = []
    for linea in texto.split("\n"):
        m = re.match(r"^(\s*)this\.(\w+)\.(\w+)(\s*)(=|\+=)(.*)$", linea)
        # Los hijos de un grupo que quedan nativos (TreeView, CheckedListBox, paneles) tambien
        # pasan al Panel, asi que su Y se corrige igual que la de los migrados.
        if m and m.group(2) not in controles and m.group(2) in hijo_de and m.group(3) == "Location":
            mp = re.search(r"Point\((-?\d+),\s*(-?\d+)\)", m.group(6))
            if mp:
                x, y = int(mp.group(1)), max(0, int(mp.group(2)) - DESPLAZAMIENTO_Y_GRUPO)
                linea = "%sthis.%s.Location = new System.Drawing.Point(%d, %d);" % (m.group(1), m.group(2), x, y)
            nuevas.append(linea)
            continue
        if not m or m.group(2) not in controles:
            nuevas.append(linea)
            continue

        sangria, nombre, prop, _, operador, resto = m.groups()
        tipo = controles[nombre]

        if operador == "=" and prop in PROPIEDADES_A_QUITAR:
            reporte.append("quitada  %s.%s" % (nombre, prop))
            continue

        if operador == "=" and prop == "Text" and tipo in TEXTO:
            linea = "%sthis.%s.%s =%s" % (sangria, nombre, TEXTO[tipo], resto)
        elif operador == "=" and prop == "Font":
            if tipo in FUENTE:
                linea = "%sthis.%s.%s =%s" % (sangria, nombre, FUENTE[tipo], resto)
            else:
                reporte.append("REVISAR  %s.Font sin equivalente (quitada)" % nombre)
                continue
        elif operador == "=" and prop == "ForeColor":
            if tipo in COLOR_TEXTO:
                linea = "%sthis.%s.%s =%s" % (sangria, nombre, COLOR_TEXTO[tipo], resto)
                reporte.append("mapeada  %s.ForeColor -> %s" % (nombre, COLOR_TEXTO[tipo]))
            else:
                reporte.append("REVISAR  %s.ForeColor sin equivalente (quitada)" % nombre)
                continue
        elif prop in REVISAR:
            reporte.append("REVISAR  %s.%s (propiedad sin equivalente directo en Krypton)" % (nombre, prop))
        elif operador == "=" and prop == "Location" and nombre in hijo_de:
            mp = re.search(r"Point\((-?\d+),\s*(-?\d+)\)", resto)
            if mp:
                x, y = int(mp.group(1)), int(mp.group(2))
                y = max(0, y - DESPLAZAMIENTO_Y_GRUPO)
                linea = "%sthis.%s.Location = new System.Drawing.Point(%d, %d);" % (sangria, nombre, x, y)

        nuevas.append(linea)
    texto = "\n".join(nuevas)
    texto, estilos = asignar_estilo_boton(texto)
    if estilos:
        reporte.append("botones  %d KryptonButton con ButtonStyle.Custom1" % estilos)
    texto, movidas = ajustar_etiquetas(texto)
    if movidas:
        reporte.append("etiquetas %d KryptonLabel subidas %d px" % (movidas, DESPLAZAMIENTO_Y_ETIQUETA))

    # 5) GroupBox: hijos al Panel e inicializacion de Krypton.
    for grupo in grupos:
        texto = texto.replace("this.%s.Controls.Add(" % grupo, "this.%s.Panel.Controls.Add(" % grupo)

        inicio = (
            "((System.ComponentModel.ISupportInitialize)(this.%(g)s)).BeginInit();\n"
            "            ((System.ComponentModel.ISupportInitialize)(this.%(g)s.Panel)).BeginInit();\n"
            "            this.%(g)s.Panel.SuspendLayout();\n"
            "            this.%(g)s.SuspendLayout();"
        ) % {"g": grupo}
        texto = texto.replace("this.%s.SuspendLayout();" % grupo, inicio, 1)

        fin = (
            "((System.ComponentModel.ISupportInitialize)(this.%(g)s.Panel)).EndInit();\n"
            "            this.%(g)s.Panel.ResumeLayout(false);\n"
            "            this.%(g)s.Panel.PerformLayout();\n"
            "            ((System.ComponentModel.ISupportInitialize)(this.%(g)s)).EndInit();\n"
            "            this.%(g)s.ResumeLayout(false);\n"
            "            this.%(g)s.PerformLayout();"
        ) % {"g": grupo}
        cierre = "this.%(g)s.ResumeLayout(false);\n            this.%(g)s.PerformLayout();" % {"g": grupo}
        if cierre not in texto:
            # El designer omite PerformLayout en grupos sin hijos AutoSize; sin EndInit el grupo nunca queda
            # inicializado y su Panel no acompaña al grupo cuando este cambia de tamaño.
            cierre = "this.%(g)s.ResumeLayout(false);" % {"g": grupo}
        texto = texto.replace(cierre, fin, 1)
        reporte.append("grupo    %s: %d hijo(s) movidos a Panel (Y -%d)" % (grupo, len(hijos[grupo]), DESPLAZAMIENTO_Y_GRUPO))

    resumen = {}
    for tipo in controles.values():
        resumen[tipo] = resumen.get(tipo, 0) + 1
    reporte.insert(0, "migrados %s" % ", ".join("%d %s" % (c, t) for t, c in sorted(resumen.items())))
    return texto, reporte


def main(argumentos):
    escribir = "--escribir" in argumentos
    solo_etiquetas = "--solo-etiquetas" in argumentos
    solo_estilo_boton = "--solo-estilo-boton" in argumentos
    archivos = [a for a in argumentos if not a.startswith("--")]
    if not archivos:
        print(__doc__)
        return 1

    for ruta in archivos:
        with open(ruta, encoding="utf-8", newline="") as f:
            origen = f.read()
        fin_linea = "\r\n" if "\r\n" in origen else "\n"
        if solo_estilo_boton:
            # Para archivos ya migrados: solo asigna ButtonStyle = Custom1 a los KryptonButton.
            nuevo, estilos = asignar_estilo_boton(origen.replace("\r\n", "\n"))
            reporte = ["botones  %d KryptonButton con ButtonStyle.Custom1" % estilos]
        elif solo_etiquetas:
            # Para archivos ya migrados: solo ajusta la posicion de las KryptonLabel.
            nuevo, movidas = ajustar_etiquetas(origen.replace("\r\n", "\n"))
            reporte = ["etiquetas %d KryptonLabel subidas %d px" % (movidas, DESPLAZAMIENTO_Y_ETIQUETA)]
        else:
            nuevo, reporte = migrar(origen.replace("\r\n", "\n"))
        nuevo = nuevo.replace("\n", fin_linea)

        print("== %s" % ruta)
        for r in reporte:
            print("   " + r)

        if nuevo == origen:
            print("   (sin cambios)")
            continue
        if escribir:
            with open(ruta, "w", encoding="utf-8", newline="") as f:
                f.write(nuevo)
            print("   ESCRITO")
        else:
            diff = difflib.unified_diff(origen.split("\n"), nuevo.split("\n"), "antes", "despues", lineterm="", n=0)
            print("\n".join("   " + d for d in list(diff)[:80]))
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv[1:]))
