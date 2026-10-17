using System.Drawing;
using Krypton.Toolkit;

namespace TpIDS
{
    /// <summary>
    /// Paleta Krypton propia de FINAZAPP (solo modo claro). Los colores base salen de la
    /// presentacion del TP; los marcados como "derivado" son tonos calculados a partir de ellos
    /// (hover, borde, seleccion suave) y se pueden ajustar sin tocar el resto del codigo.
    /// Todo lo que no se sobrescribe aca se hereda de la paleta base (BasePalette).
    /// </summary>
    internal static class FinanzappPaleta
    {
        // Colores medidos de la presentacion.
        public static readonly Color Fondo = Color.FromArgb(0xFF, 0xFF, 0xFF);
        public static readonly Color Verde = Color.FromArgb(0x5D, 0xC4, 0x7D);
        public static readonly Color VerdeAcento = Color.FromArgb(0x4C, 0xDB, 0x81);
        public static readonly Color Texto = Color.FromArgb(0x3E, 0x3A, 0x4E);
        public static readonly Color GrisVioleta = Color.FromArgb(0x48, 0x46, 0x5F);

        // Derivados.
        public static readonly Color VerdeOscuro = Color.FromArgb(0x45, 0xA8, 0x62);
        public static readonly Color VerdeSuave = Color.FromArgb(0xE3, 0xF5, 0xE9);
        public static readonly Color Borde = Color.FromArgb(0xC9, 0xC7, 0xD3);
        public static readonly Color Deshabilitado = Color.FromArgb(0xB9, 0xB7, 0xC3);
        public static readonly Color FondoMdi = Color.FromArgb(0xF3, 0xF4, 0xF8);
        public static readonly Color FilaAlterna = Color.FromArgb(0xF8, 0xF8, 0xFB);
        // Alerta (texto) y AlertaSuave (marcos) son un rojo apagado: distinguen "excluir" de "incluir" sin
        // parecer un estado de error. TextoSecundario es el gris de las ayudas y leyendas.
        public static readonly Color Alerta = Color.FromArgb(0xB8, 0x6A, 0x6A);
        public static readonly Color AlertaSuave = Color.FromArgb(0xDE, 0x9A, 0x9A);
        public static readonly Color TextoSecundario = Color.FromArgb(0x7A, 0x78, 0x8A);

        public static KryptonCustomPaletteBase Crear()
        {
            KryptonCustomPaletteBase paleta = new KryptonCustomPaletteBase();
            paleta.BasePalette = KryptonManager.GetPaletteForMode(PaletteMode.Microsoft365White);

            AplicarBotones(paleta);
            AplicarCamposDeTexto(paleta);
            AplicarContenedores(paleta);
            AplicarEtiquetas(paleta);
            AplicarVentana(paleta);
            AplicarMenu(paleta);
            AplicarBotonDesplegable(paleta);
            AplicarGrillas(paleta);
            AplicarItemsDeLista(paleta);

            return paleta;
        }

        private static void AplicarBotones(KryptonCustomPaletteBase paleta)
        {
            // Estilo personalizado (ButtonStyle.Custom1) y no ButtonStandalone: otros controles (la flecha de
            // los combos y fechas) heredan de ButtonStandalone y, con el texto blanco, la flecha desaparecia.
            // Todo KryptonButton de los formularios usa ButtonStyle = Custom1.
            KryptonPaletteCheckButton boton = paleta.ButtonStyles.ButtonCustom1;

            boton.StateCommon.Back.ColorStyle = PaletteColorStyle.Solid;
            boton.StateCommon.Border.Rounding = 4;
            boton.StateCommon.Border.Width = 1;

            PintarEstado(boton.StateNormal, Verde, Verde);
            PintarEstado(boton.StateTracking, VerdeAcento, VerdeAcento);
            PintarEstado(boton.StatePressed, VerdeOscuro, VerdeOscuro);
            PintarEstado(boton.StateDisabled, Borde, Borde);
            boton.StateNormal.Content.ShortText.Color1 = Color.White;
            boton.StateTracking.Content.ShortText.Color1 = Color.White;
            boton.StatePressed.Content.ShortText.Color1 = Color.White;
            boton.StateDisabled.Content.ShortText.Color1 = Deshabilitado;

            // La base Microsoft 365 pinta en gris el boton con foco o por defecto (AcceptButton): se ve deshabilitado.
            // Se mantiene el verde y el foco se marca solo con el borde oscuro.
            PintarEstado(boton.OverrideFocus.Back, boton.OverrideFocus.Border, Verde, VerdeOscuro);
            PintarEstado(boton.OverrideDefault.Back, boton.OverrideDefault.Border, Verde, Verde);
            boton.OverrideFocus.Content.ShortText.Color1 = Color.White;
            boton.OverrideDefault.Content.ShortText.Color1 = Color.White;
        }

        private static void AplicarItemsDeLista(KryptonCustomPaletteBase paleta)
        {
            // Items de KryptonListBox/KryptonCheckedListBox: la base Microsoft 365 los selecciona en amarillo.
            KryptonPaletteCheckButton item = paleta.ButtonStyles.ButtonListItem;

            item.StateCommon.Back.ColorStyle = PaletteColorStyle.Solid;
            PintarEstado(item.StateTracking, FilaAlterna, FilaAlterna);
            PintarEstado(item.StatePressed, VerdeSuave, VerdeSuave);
            PintarEstado(item.StateCheckedNormal, VerdeSuave, Verde);
            PintarEstado(item.StateCheckedTracking, VerdeSuave, Verde);
            PintarEstado(item.StateCheckedPressed, VerdeSuave, Verde);
            item.StateCommon.Content.ShortText.Color1 = Texto;
            item.StateCheckedNormal.Content.ShortText.Color1 = Texto;
            item.StateCheckedTracking.Content.ShortText.Color1 = Texto;
            item.StateCheckedPressed.Content.ShortText.Color1 = Texto;
        }

        private static void PintarEstado(PaletteTriple estado, Color fondo, Color borde)
        {
            PintarEstado(estado.Back, estado.Border, fondo, borde);
        }

        private static void PintarEstado(PaletteBack back, PaletteBorder border, Color fondo, Color borde)
        {
            back.Color1 = fondo;
            back.Color2 = fondo;
            border.Color1 = borde;
            border.Color2 = borde;
        }

        private static void AplicarCamposDeTexto(KryptonCustomPaletteBase paleta)
        {
            KryptonPaletteInputControl campo = paleta.InputControlStyles.InputControlStandalone;

            campo.StateCommon.Back.ColorStyle = PaletteColorStyle.Solid;
            campo.StateCommon.Back.Color1 = Fondo;
            campo.StateCommon.Back.Color2 = Fondo;
            campo.StateCommon.Border.Rounding = 3;
            campo.StateCommon.Content.ShortText.Color1 = Texto;

            campo.StateNormal.Border.Color1 = Borde;
            campo.StateNormal.Border.Color2 = Borde;
            campo.StateActive.Border.Color1 = Verde;
            campo.StateActive.Border.Color2 = Verde;
        }

        private static void AplicarContenedores(KryptonCustomPaletteBase paleta)
        {
            // Fondo de los paneles (el cuerpo de los formularios).
            KryptonPalettePanel panel = paleta.PanelStyles.PanelClient;
            panel.StateCommon.ColorStyle = PaletteColorStyle.Solid;
            panel.StateCommon.Color1 = Fondo;
            panel.StateCommon.Color2 = Fondo;

            // KryptonGroupBox.
            KryptonPaletteControl grupo = paleta.ControlStyles.ControlGroupBox;
            grupo.StateCommon.Back.ColorStyle = PaletteColorStyle.Solid;
            grupo.StateCommon.Back.Color1 = Fondo;
            grupo.StateCommon.Back.Color2 = Fondo;
            grupo.StateCommon.Border.Color1 = Borde;
            grupo.StateCommon.Border.Color2 = Borde;
            grupo.StateCommon.Border.Rounding = 4;
        }

        private static void AplicarEtiquetas(KryptonCustomPaletteBase paleta)
        {
            paleta.LabelStyles.LabelNormalControl.StateCommon.ShortText.Color1 = Texto;
            paleta.LabelStyles.LabelBoldControl.StateCommon.ShortText.Color1 = Texto;
            paleta.LabelStyles.LabelTitleControl.StateCommon.ShortText.Color1 = Texto;
            paleta.LabelStyles.LabelNormalPanel.StateCommon.ShortText.Color1 = Texto;
        }

        // Boton de la flecha de los combos y fechas: sin esto hereda el texto blanco de los botones y la flecha desaparece.
        private static void AplicarBotonDesplegable(KryptonCustomPaletteBase paleta)
        {
            KryptonPaletteCheckButton flecha = paleta.ButtonStyles.ButtonInputControl;

            flecha.StateCommon.Content.ShortText.Color1 = Texto;
            flecha.StateCommon.Border.Rounding = 3;

            PintarEstado(flecha.StateNormal, Fondo, Borde);
            PintarEstado(flecha.StateTracking, VerdeSuave, Verde);
            PintarEstado(flecha.StatePressed, VerdeSuave, Verde);
            PintarEstado(flecha.StateDisabled, Fondo, Borde);
        }

        // KryptonDataGridView: mismo aspecto que la grilla nativa tematizada (encabezado verde, seleccion verde suave).
        private static void AplicarGrillas(KryptonCustomPaletteBase paleta)
        {
            KryptonPaletteGrid grilla = paleta.GridStyles.GridList;

            grilla.StateCommon.Background.Color1 = Fondo;
            grilla.StateCommon.Background.Color2 = Fondo;

            grilla.StateCommon.HeaderColumn.Back.ColorStyle = PaletteColorStyle.Solid;
            grilla.StateCommon.HeaderColumn.Back.Color1 = Verde;
            grilla.StateCommon.HeaderColumn.Back.Color2 = Verde;
            grilla.StateCommon.HeaderColumn.Border.Color1 = Verde;
            grilla.StateCommon.HeaderColumn.Border.Color2 = Verde;
            grilla.StateCommon.HeaderColumn.Content.Color1 = Color.White;

            grilla.StateCommon.DataCell.Back.ColorStyle = PaletteColorStyle.Solid;
            grilla.StateCommon.DataCell.Back.Color1 = Fondo;
            grilla.StateCommon.DataCell.Back.Color2 = Fondo;
            grilla.StateCommon.DataCell.Border.Color1 = Borde;
            grilla.StateCommon.DataCell.Border.Color2 = Borde;
            grilla.StateCommon.DataCell.Content.Color1 = Texto;

            grilla.StateSelected.DataCell.Back.Color1 = VerdeSuave;
            grilla.StateSelected.DataCell.Back.Color2 = VerdeSuave;
            grilla.StateSelected.DataCell.Content.Color1 = Texto;
        }

        private static void AplicarMenu(KryptonCustomPaletteBase paleta)
        {
            KryptonPaletteTMS menu = paleta.ToolMenuStatus;

            menu.MenuStrip.MenuStripGradientBegin = Fondo;
            menu.MenuStrip.MenuStripGradientEnd = Fondo;
            menu.MenuStrip.MenuStripText = Texto;

            menu.Menu.MenuItemText = Texto;
            menu.Menu.MenuBorder = Borde;
            menu.Menu.MenuItemBorder = Verde;
            menu.Menu.MenuItemSelected = VerdeSuave;
            menu.Menu.MenuItemSelectedGradientBegin = VerdeSuave;
            menu.Menu.MenuItemSelectedGradientEnd = VerdeSuave;
            menu.Menu.MenuItemPressedGradientBegin = VerdeSuave;
            menu.Menu.MenuItemPressedGradientMiddle = VerdeSuave;
            menu.Menu.MenuItemPressedGradientEnd = VerdeSuave;
        }

        private static void AplicarVentana(KryptonCustomPaletteBase paleta)
        {
            // Borde fino del tema de Windows en vez del marco grueso de Krypton.
            paleta.UseThemeFormChromeBorderWidth = InheritBool.True;

            KryptonPaletteForm ventana = paleta.FormStyles.FormMain;

            ventana.StateActive.Back.Color1 = Fondo;
            ventana.StateActive.Back.Color2 = Fondo;
            ventana.StateActive.Border.Color1 = Verde;
            ventana.StateActive.Border.Color2 = Verde;

            ventana.StateInactive.Back.Color1 = FondoMdi;
            ventana.StateInactive.Back.Color2 = FondoMdi;
            ventana.StateInactive.Border.Color1 = Borde;
            ventana.StateInactive.Border.Color2 = Borde;
        }
    }
}
