using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ABSTRACCIONES;
using BE;
using BLL;
using Krypton.Toolkit;

namespace TpIDS
{
    public sealed class FormPerfilInversor : FormTraducibleBase
    {
        private readonly PerfilInversorBLL _perfilBLL = CompositionRoot.PerfilInversor;
        private readonly ContinuacionPerfilInversor _continuacion;
        private readonly string[] _opciones = new string[10];
        private readonly KryptonLabel lblPaso = new KryptonLabel();
        private readonly KryptonWrapLabel lblPregunta = new KryptonWrapLabel();
        private readonly KryptonWrapLabel lblExplicacion = new KryptonWrapLabel();
        private readonly KryptonRadioButton[] rdbOpciones = new KryptonRadioButton[4];
        private readonly KryptonButton btnAnterior = new KryptonButton();
        private readonly KryptonButton btnSiguiente = new KryptonButton();
        private readonly KryptonButton btnCancelar = new KryptonButton();
        private readonly KryptonLabel lblTitulo = new KryptonLabel();
        private readonly KryptonProgressBar prgbCuestionario = new KryptonProgressBar();
        private readonly KryptonPanel pnlContenido = new KryptonPanel();
        private readonly TableLayoutPanel tlpContenido = new TableLayoutPanel();
        private readonly KryptonGroup[] groupBoxOpciones = new KryptonGroup[4];
        private readonly Font fuenteTitulo = new Font("Segoe UI", 18F, FontStyle.Bold);
        private readonly Font fuentePregunta = new Font("Segoe UI", 15F, FontStyle.Bold);
        private readonly Font fuenteCuerpo = new Font("Segoe UI", 10.5F);
        private readonly Font fuenteBoton = new Font("Segoe UI", 10F, FontStyle.Bold);
        private readonly ErrorProvider epRespuestas;
        private int _paso = -1;
        private bool _guardando;
        private PerfilInversor _resultado;
        private bool _mostrandoPaso;

        public FormPerfilInversor(ContinuacionPerfilInversor continuacion = null)
        {
            _continuacion = continuacion;
            Name = nameof(FormPerfilInversor);
            ClientSize = new Size(780, 600);
            StartPosition = FormStartPosition.CenterParent;
            AutoScaleMode = AutoScaleMode.Dpi;
            epRespuestas = new ErrorProvider { ContainerControl = this };
            lblTitulo.Name = nameof(lblTitulo);
            lblTitulo.SetBounds(28, 22, 420, 38);
            lblTitulo.StateCommon.ShortText.Font = fuenteTitulo;
            lblTitulo.StateCommon.ShortText.Color1 = FinanzappPaleta.Texto;
            lblPaso.Name = nameof(lblPaso);
            lblPaso.AutoSize = false;
            lblPaso.SetBounds(452, 29, 300, 28);
            lblPaso.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblPaso.StateCommon.ShortText.Font = fuenteCuerpo;
            lblPaso.StateCommon.ShortText.Color1 = FinanzappPaleta.TextoSecundario;
            lblPaso.StateCommon.ShortText.TextH = PaletteRelativeAlign.Far;
            prgbCuestionario.Name = nameof(prgbCuestionario);
            prgbCuestionario.SetBounds(28, 77, 724, 7);
            prgbCuestionario.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            prgbCuestionario.Maximum = 10;
            prgbCuestionario.Style = ProgressBarStyle.Continuous;
            TemaFinanzapp.TematizarBarraProgreso(prgbCuestionario);
            pnlContenido.Name = nameof(pnlContenido);
            pnlContenido.SetBounds(28, 108, 724, 394);
            pnlContenido.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlContenido.AutoScroll = true;
            tlpContenido.Name = nameof(tlpContenido);
            tlpContenido.ColumnCount = 1;
            tlpContenido.RowCount = 6;
            tlpContenido.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            for (int i = 0; i < 6; i++) tlpContenido.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlpContenido.AutoSize = true;
            tlpContenido.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tlpContenido.Dock = DockStyle.Top;
            tlpContenido.Padding = new Padding(0, 0, 18, 4);
            pnlContenido.Controls.Add(tlpContenido);
            lblPregunta.Name = nameof(lblPregunta);
            lblPregunta.AutoSize = true;
            lblPregunta.Dock = DockStyle.Top;
            lblPregunta.Margin = new Padding(0, 0, 0, 14);
            lblPregunta.StateCommon.Font = fuentePregunta;
            lblPregunta.StateCommon.TextColor = FinanzappPaleta.Texto;
            lblExplicacion.Name = nameof(lblExplicacion);
            lblExplicacion.AutoSize = true;
            lblExplicacion.Dock = DockStyle.Top;
            lblExplicacion.Margin = new Padding(0, 0, 0, 20);
            lblExplicacion.StateCommon.Font = fuenteCuerpo;
            lblExplicacion.StateCommon.TextColor = FinanzappPaleta.TextoSecundario;
            tlpContenido.Controls.Add(lblPregunta, 0, 0);
            tlpContenido.Controls.Add(lblExplicacion, 0, 1);
            Controls.AddRange(new Control[] { lblTitulo, lblPaso, prgbCuestionario, pnlContenido });
            for (int i = 0; i < rdbOpciones.Length; i++)
            {
                var tarjeta = new KryptonGroup { Name = "groupBoxOpcion" + (char)('A' + i), Height = 64,
                    Dock = DockStyle.Top, Margin = new Padding(0, 0, 0, 10) };
                tarjeta.StateCommon.Border.Rounding = 7;
                tarjeta.StateCommon.Border.Width = 1;
                tarjeta.StateCommon.Back.Color1 = FinanzappPaleta.FilaAlterna;
                tarjeta.StateCommon.Back.Color2 = FinanzappPaleta.FilaAlterna;
                tarjeta.StateCommon.Border.Color1 = FinanzappPaleta.Borde;
                tarjeta.Panel.Padding = new Padding(14, 8, 14, 8);
                var opcion = new RadioButtonAccesible { Name = "rdbOpcion" + (char)('A' + i), AutoSize = false };
                opcion.Dock = DockStyle.Fill;
                opcion.StateCommon.ShortText.MultiLine = InheritBool.True;
                opcion.StateCommon.ShortText.Font = fuenteCuerpo;
                opcion.StateCommon.ShortText.Color1 = FinanzappPaleta.Texto;
                opcion.CheckedChanged += rdbOpciones_CheckedChanged;
                opcion.KeyDown += rdbOpciones_KeyDown;
                rdbOpciones[i] = opcion;
                groupBoxOpciones[i] = tarjeta;
                tarjeta.Panel.Controls.Add(opcion);
                tlpContenido.Controls.Add(tarjeta, 0, i + 2);
            }
            ConfigurarBoton(btnAnterior, nameof(btnAnterior), 28, false);
            ConfigurarBoton(btnCancelar, nameof(btnCancelar), 268, false);
            ConfigurarBoton(btnSiguiente, nameof(btnSiguiente), 508, true);
            btnAnterior.Click += btnAnterior_Click;
            btnSiguiente.Click += btnSiguiente_Click;
            btnCancelar.Click += btnCancelar_Click;
            AcceptButton = btnSiguiente;
            CancelButton = btnCancelar;
            pnlContenido.SizeChanged += pnlContenido_SizeChanged;
            MostrarPaso();
        }

        private void ConfigurarBoton(KryptonButton boton, string nombre, int izquierda, bool principal)
        {
            boton.Name = nombre;
            boton.ButtonStyle = ButtonStyle.Custom1;
            boton.SetBounds(izquierda, 530, 224, 42);
            boton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            boton.StateCommon.Content.ShortText.Font = fuenteBoton;
            boton.StateCommon.Border.Rounding = 6;
            if (!principal)
            {
                foreach (var estado in new[] { boton.StateNormal, boton.StateTracking, boton.StatePressed })
                {
                    estado.Back.Color1 = estado.Back.Color2 = estado == boton.StateNormal ? FinanzappPaleta.Fondo : FinanzappPaleta.VerdeSuave;
                    estado.Border.Color1 = FinanzappPaleta.Borde;
                    estado.Content.ShortText.Color1 = FinanzappPaleta.Texto;
                }
                boton.OverrideFocus.Back.Color1 = boton.OverrideFocus.Back.Color2 = FinanzappPaleta.VerdeSuave;
                boton.OverrideFocus.Content.ShortText.Color1 = FinanzappPaleta.Texto;
            }
            Controls.Add(boton);
        }

        private void pnlContenido_SizeChanged(object sender, EventArgs e) => AjustarContenido();

        private void AjustarContenido()
        {
            int ancho = Math.Max(200, pnlContenido.ClientSize.Width - 36);
            lblPregunta.MaximumSize = new Size(ancho, 0);
            lblExplicacion.MaximumSize = new Size(ancho, 0);
            for (int i = 0; i < rdbOpciones.Length; i++)
            {
                int altoTexto = TextRenderer.MeasureText(rdbOpciones[i].Text, fuenteCuerpo,
                    new Size(Math.Max(150, ancho - 64), int.MaxValue), TextFormatFlags.WordBreak).Height;
                groupBoxOpciones[i].Height = Math.Max(64, altoTexto + 28);
            }
        }

        public override void Actualizar(IObservado observado)
        {
            base.Actualizar(observado);
            MostrarPaso();
        }

        private void MostrarPaso()
        {
            Text = UiDialogHelper.Traducir("FormInvestorProfile.Title");
            bool pregunta = _paso >= 0 && _paso < 10;
            bool resultado = _paso == 10;
            _mostrandoPaso = true;
            tlpContenido.SuspendLayout();
            lblTitulo.Text = UiDialogHelper.Traducir("FormInvestorProfile.Title");
            lblPaso.Text = pregunta ? UiDialogHelper.Traducir("FormInvestorProfile.Value.Progress", _paso + 1)
                : UiDialogHelper.Traducir(resultado ? "FormInvestorProfile.Value.Completed" : "FormInvestorProfile.Value.Duration");
            prgbCuestionario.Value = _paso < 0 ? 0 : Math.Min(10, _paso + 1);
            btnAnterior.Visible = _paso >= 0;
            btnAnterior.Text = UiDialogHelper.Traducir(resultado ? "FormInvestorProfile.Button.Review" : "FormInvestorProfile.Button.Previous");
            btnCancelar.Text = UiDialogHelper.Traducir(_paso < 0 && _continuacion != null ? "FormInvestorProfile.Button.Skip" : "FormInvestorProfile.Button.Cancel");
            btnSiguiente.Text = UiDialogHelper.Traducir(_paso < 0 ? "FormInvestorProfile.Button.Start" : resultado ? "FormInvestorProfile.Button.Save"
                : _paso == 9 ? "FormInvestorProfile.Button.Result" : "FormInvestorProfile.Button.Next");
            if (_paso < 0)
            {
                lblPregunta.Text = UiDialogHelper.Traducir("FormInvestorProfile.Intro.Title");
                lblExplicacion.Text = UiDialogHelper.Traducir("FormInvestorProfile.Intro.Description") + "\n\n"
                    + UiDialogHelper.Traducir("FormInvestorProfile.Intro.Optional");
            }
            else if (pregunta)
            {
                lblPregunta.Text = UiDialogHelper.Traducir("FormInvestorProfile.Question." + (_paso + 1));
                lblExplicacion.Text = UiDialogHelper.Traducir("FormInvestorProfile.Value.SelectOne");
            }
            else
            {
                lblPregunta.Text = UiDialogHelper.Traducir("FormInvestorProfile.Value.Result", UiDialogHelper.Traducir("FormInvestorProfile.Level." + _resultado.NivelRiesgo));
                string motivos = _resultado.MotivosLimite.Count == 0 ? UiDialogHelper.Traducir("FormInvestorProfile.Value.NoLimit")
                    : string.Join("\n", _resultado.MotivosLimite.Select(m => "• " + UiDialogHelper.Traducir("FormInvestorProfile.Limit." + m)));
                lblExplicacion.Text = UiDialogHelper.Traducir("FormInvestorProfile.Description." + _resultado.NivelRiesgo)
                    + "\n\n" + motivos + "\n\n" + UiDialogHelper.Traducir("FormInvestorProfile.Value.Disclaimer");
            }
            for (int i = 0; i < rdbOpciones.Length; i++)
            {
                var opcion = rdbOpciones[i];
                bool visible = pregunta && i < _perfilBLL.ObtenerCantidadOpciones(_paso + 1);
                groupBoxOpciones[i].Visible = visible;
                opcion.Visible = visible;
                opcion.Checked = pregunta && _opciones[_paso] == ((char)('A' + i)).ToString();
                if (opcion.Visible) opcion.Text = UiDialogHelper.Traducir("FormInvestorProfile.Question." + (_paso + 1) + "." + (char)('A' + i));
                PintarOpcion(i);
            }
            _mostrandoPaso = false;
            AjustarContenido();
            tlpContenido.ResumeLayout(true);
            pnlContenido.AutoScrollPosition = Point.Empty;
        }

        private void PintarOpcion(int indice)
        {
            var tarjeta = groupBoxOpciones[indice];
            tarjeta.StateCommon.Back.Color1 = tarjeta.StateCommon.Back.Color2 = rdbOpciones[indice].Checked
                ? FinanzappPaleta.VerdeSuave : FinanzappPaleta.FilaAlterna;
            tarjeta.StateCommon.Border.Color1 = rdbOpciones[indice].Checked ? FinanzappPaleta.Verde : FinanzappPaleta.Borde;
        }

        private void rdbOpciones_CheckedChanged(object sender, EventArgs e)
        {
            if (!_mostrandoPaso && sender is KryptonRadioButton opcion && opcion.Checked && _paso >= 0 && _paso < 10)
            {
                for (int i = 0; i < rdbOpciones.Length; i++)
                {
                    if (rdbOpciones[i] != opcion) rdbOpciones[i].Checked = false;
                    PintarOpcion(i);
                }
                _opciones[_paso] = ((char)('A' + Array.IndexOf(rdbOpciones, opcion))).ToString();
                epRespuestas.Clear();
            }
        }

        private void rdbOpciones_KeyDown(object sender, KeyEventArgs e)
        {
            int direccion = e.KeyCode == Keys.Down || e.KeyCode == Keys.Right ? 1
                : e.KeyCode == Keys.Up || e.KeyCode == Keys.Left ? -1 : 0;
            if (direccion == 0 || _paso < 0 || _paso >= 10) return;
            int cantidad = _perfilBLL.ObtenerCantidadOpciones(_paso + 1);
            int indice = (Array.IndexOf(rdbOpciones, sender) + direccion + cantidad) % cantidad;
            rdbOpciones[indice].Checked = true;
            rdbOpciones[indice].Focus();
            e.Handled = e.SuppressKeyPress = true;
        }

        private RespuestaPerfilInversor[] ObtenerRespuestas() => _opciones.Select((o, i) =>
            new RespuestaPerfilInversor { PreguntaId = i + 1, OpcionId = o }).ToArray();

        private async void btnSiguiente_Click(object sender, EventArgs e)
        {
            if (_guardando) return;
            if (_paso < 0) { _paso = 0; MostrarPaso(); btnSiguiente.Focus(); return; }
            if (_paso < 10)
            {
                if (_opciones[_paso] == null)
                {
                    epRespuestas.SetError(rdbOpciones[0], UiDialogHelper.Traducir("Error.InvestorProfile.InvalidAnswers"));
                    rdbOpciones[0].Focus();
                    return;
                }
                if (_paso == 9)
                {
                    try { _resultado = _perfilBLL.EvaluarCuestionario(PerfilInversorBLL.VersionCuestionario, ObtenerRespuestas()); }
                    catch (Exception ex) { UiExceptionHandler.Mostrar(ex, nameof(FormPerfilInversor)); return; }
                }
                _paso++;
                MostrarPaso();
                btnSiguiente.Focus();
                return;
            }
            var respuestas = ObtenerRespuestas();
            _guardando = true;
            btnSiguiente.Enabled = btnAnterior.Enabled = btnCancelar.Enabled = false;
            try
            {
                await Task.Run(() => _continuacion == null
                    ? _perfilBLL.GuardarMiPerfil(PerfilInversorBLL.VersionCuestionario, respuestas)
                    : _perfilBLL.GuardarPostRegistro(_continuacion, PerfilInversorBLL.VersionCuestionario, respuestas));
                DialogResult = DialogResult.OK;
                _guardando = false;
                Close();
            }
            catch (Exception ex) { UiExceptionHandler.Mostrar(ex, nameof(FormPerfilInversor)); }
            finally
            {
                _guardando = false;
                if (!IsDisposed) btnSiguiente.Enabled = btnAnterior.Enabled = btnCancelar.Enabled = true;
            }
        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {
            if (_guardando) return;
            epRespuestas.Clear();
            _paso = _paso == 10 ? 0 : _paso - 1;
            MostrarPaso();
        }

        private void btnCancelar_Click(object sender, EventArgs e) { if (!_guardando) Close(); }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_guardando) e.Cancel = true;
            base.OnFormClosing(e);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _perfilBLL.CancelarPostRegistro(_continuacion);
            base.OnFormClosed(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                epRespuestas.Dispose();
                fuenteTitulo.Dispose();
                fuentePregunta.Dispose();
                fuenteCuerpo.Dispose();
                fuenteBoton.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
