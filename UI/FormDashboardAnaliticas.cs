using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using BE;
using BLL;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WinForms;
using SERVICIOS;

namespace TpIDS
{
    // Dashboard de analiticas: cada TarjetaDashboard del usuario se renderiza como un
    // panel con su grafico LiveCharts. Alta/baja de tarjetas siempre refresca la pantalla
    // completa (recargar todo el FlowLayoutPanel), no hay edicion incremental de una sola tarjeta.
    public partial class FormDashboardAnaliticas : FormTraducibleBase
    {
        private const int AnchoTarjeta = 380;
        private const int AltoTarjeta = 300;
        private const int MesesComparativo = 6;

        private readonly DashboardBLL dashboardBLL = CompositionRoot.Dashboard;
        private readonly MovimientoBLL movimientoBLL = CompositionRoot.Movimiento;

        public FormDashboardAnaliticas()
        {
            InitializeComponent();
            Load += FormDashboardAnaliticas_Load;
        }

        private void FormDashboardAnaliticas_Load(object sender, EventArgs e)
        {
            // Ventana normal (misma barra de titulo/botones que el resto de pantallas MDI
            // del proyecto), solo que abre ya ocupando el area util del contenedor MDI en
            // vez del tamaño de diseño fijo. WindowState=Maximized fusiona la barra de
            // titulo/botones con la ventana principal (no es lo que se pidio), por eso se
            // toma el tamaño real del MdiClient en su lugar.
            MdiClient mdiClient = MdiParent?.Controls.OfType<MdiClient>().FirstOrDefault();
            if (mdiClient != null)
            {
                Location = Point.Empty;
                Size = mdiClient.ClientSize;
            }

            CargarDashboard();
        }

        private void CargarDashboard()
        {
            try
            {
                Dashboard dashboard = dashboardBLL.ObtenerOCrearDashboardActual();
                RenderizarTarjetas(dashboard.Tarjetas ?? new List<TarjetaDashboard>());
                ActualizarBotonAgregar();
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Consulta de dashboard de analiticas");
            }
        }

        private void ActualizarBotonAgregar()
        {
            bool hayDisponibles = dashboardBLL.ObtenerTiposDisponibles().Count > 0;
            btnAgregarTarjeta.Enabled = hayDisponibles;
            lblSinTarjetasDisponibles.Visible = !hayDisponibles;
        }

        private void RenderizarTarjetas(List<TarjetaDashboard> tarjetas)
        {
            flpTarjetasDashboard.SuspendLayout();

            foreach (Control control in flpTarjetasDashboard.Controls.Cast<Control>().ToList())
            {
                flpTarjetasDashboard.Controls.Remove(control);
                control.Dispose();
            }

            foreach (TarjetaDashboard tarjeta in tarjetas.OrderBy(item => item.Orden))
            {
                flpTarjetasDashboard.Controls.Add(ConstruirPanelTarjeta(tarjeta));
            }

            flpTarjetasDashboard.ResumeLayout();
        }

        private Panel ConstruirPanelTarjeta(TarjetaDashboard tarjeta)
        {
            string claveTitulo = ClaveTipoTarjeta(tarjeta.Tipo);

            Panel panel = new Panel
            {
                Size = new Size(AnchoTarjeta, AltoTarjeta),
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(10)
            };

            Label titulo = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(10, 8),
                Tag = claveTitulo,
                Text = UiDialogHelper.Traducir(claveTitulo)
            };

            Button quitar = new Button
            {
                Size = new Size(70, 24),
                Location = new Point(AnchoTarjeta - 82, 6),
                Tag = "FormDashboard.RemoveCardButton",
                Text = UiDialogHelper.Traducir("FormDashboard.RemoveCardButton")
            };
            int tarjetaId = tarjeta.Id;
            quitar.Click += (sender, e) => QuitarTarjeta(tarjetaId);

            Control grafico = ConstruirGrafico(tarjeta.Tipo);
            grafico.Location = new Point(8, 36);
            grafico.Size = new Size(AnchoTarjeta - 16, AltoTarjeta - 44);

            panel.Controls.Add(grafico);
            panel.Controls.Add(titulo);
            panel.Controls.Add(quitar);

            return panel;
        }

        private void QuitarTarjeta(int tarjetaId)
        {
            try
            {
                dashboardBLL.QuitarTarjeta(tarjetaId);
                CargarDashboard();
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Baja de tarjeta de dashboard");
            }
        }

        private void btnAgregarTarjeta_Click(object sender, EventArgs e)
        {
            using (FormSeleccionarTarjetaDashboard dialogo = new FormSeleccionarTarjetaDashboard())
            {
                if (dialogo.ShowDialog(this) != DialogResult.OK || !dialogo.TipoSeleccionado.HasValue)
                {
                    return;
                }

                try
                {
                    dashboardBLL.AgregarTarjeta(dialogo.TipoSeleccionado.Value);
                    CargarDashboard();
                }
                catch (Exception ex)
                {
                    UiExceptionHandler.Mostrar(ex, "Alta de tarjeta de dashboard");
                }
            }
        }

        private Control ConstruirGrafico(TipoTarjetaDashboard tipo)
        {
            switch (tipo)
            {
                case TipoTarjetaDashboard.GastosPorCategoria:
                    return ConstruirGraficoPorCategoria(ABSTRACCIONES.TipoMovimiento.Egreso);
                case TipoTarjetaDashboard.IngresosPorCategoria:
                    return ConstruirGraficoPorCategoria(ABSTRACCIONES.TipoMovimiento.Ingreso);
                case TipoTarjetaDashboard.ComparativoMensual:
                    return ConstruirGraficoComparativoMensual();
                case TipoTarjetaDashboard.EvolucionBalance:
                default:
                    return ConstruirGraficoEvolucionBalance();
            }
        }

        private PieChart ConstruirGraficoPorCategoria(ABSTRACCIONES.TipoMovimiento tipo)
        {
            DateTime hoy = DateTime.Today;
            DateTime desde = new DateTime(hoy.Year, hoy.Month, 1);
            DateTime hasta = desde.AddMonths(1).AddDays(-1);

            List<TotalPorCategoria> totales = movimientoBLL.ObtenerTotalesPorCategoria(tipo, desde, hasta);

            return new PieChart
            {
                Series = totales
                    .Select(item => new PieSeries<double> { Values = new[] { (double)item.Total }, Name = item.CategoriaNombre })
                    .ToArray()
            };
        }

        private CartesianChart ConstruirGraficoComparativoMensual()
        {
            List<TotalPorMes> totales = ObtenerTotalesUltimosMeses();
            string[] etiquetas = ObtenerEtiquetasMeses(totales);

            return new CartesianChart
            {
                Series = new ISeries[]
                {
                    new ColumnSeries<double>
                    {
                        Values = totales.Select(item => (double)item.TotalIngresos).ToArray(),
                        Name = UiDialogHelper.Traducir("FormMovements.Value.Type.Income")
                    },
                    new ColumnSeries<double>
                    {
                        Values = totales.Select(item => (double)item.TotalEgresos).ToArray(),
                        Name = UiDialogHelper.Traducir("FormMovements.Value.Type.Expense")
                    }
                },
                XAxes = new[] { new Axis { Labels = etiquetas } }
            };
        }

        private CartesianChart ConstruirGraficoEvolucionBalance()
        {
            List<TotalPorMes> totales = ObtenerTotalesUltimosMeses();
            string[] etiquetas = ObtenerEtiquetasMeses(totales);

            return new CartesianChart
            {
                Series = new ISeries[]
                {
                    new StackedAreaSeries<double>
                    {
                        Values = totales.Select(item => (double)item.TotalIngresos).ToArray(),
                        Name = UiDialogHelper.Traducir("FormMovements.Value.Type.Income")
                    },
                    new StackedAreaSeries<double>
                    {
                        Values = totales.Select(item => (double)item.TotalEgresos).ToArray(),
                        Name = UiDialogHelper.Traducir("FormMovements.Value.Type.Expense")
                    }
                },
                XAxes = new[] { new Axis { Labels = etiquetas } }
            };
        }

        private List<TotalPorMes> ObtenerTotalesUltimosMeses()
        {
            DateTime hoy = DateTime.Today;
            DateTime desde = new DateTime(hoy.Year, hoy.Month, 1).AddMonths(-(MesesComparativo - 1));
            return movimientoBLL.ObtenerTotalesPorMes(desde, hoy);
        }

        private static string[] ObtenerEtiquetasMeses(List<TotalPorMes> totales)
        {
            return totales
                .Select(item => new DateTime(item.Anio, item.Mes, 1).ToString("MM/yyyy", CultureInfo.InvariantCulture))
                .ToArray();
        }

        private static string ClaveTipoTarjeta(TipoTarjetaDashboard tipo)
        {
            return string.Format(CultureInfo.InvariantCulture, "FormDashboard.CardType.{0}", tipo);
        }
    }
}
