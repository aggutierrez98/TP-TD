using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using ABSTRACCIONES;
using BE;
using BLL;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.WinForms;
using Krypton.Toolkit;
using SkiaSharp;
using SERVICIOS;

namespace TpIDS
{
    // Dashboard de analiticas: cada TarjetaDashboard del usuario se renderiza como un
    // panel con su grafico LiveCharts. Alta/baja de tarjetas siempre refresca la pantalla
    // completa (recargar todo el FlowLayoutPanel), no hay edicion incremental de una sola tarjeta.
    public partial class FormDashboardAnaliticas : FormTraducibleBase
    {
        // Disposicion responsiva: 2 tarjetas por fila si entran dos de ancho minimo, si no 1 (con scroll vertical).
        private const int AnchoMinimoTarjeta = 420;
        private const int AltoMinimoTarjeta = 380;
        private const int AltoMaximoTarjeta = 520;
        private const int MargenTarjeta = 10;
        private const int PaddingTarjeta = 12;
        private const int MesesComparativo = 6;
        private const int CantidadComercios = 8;
        private const int MaxLargoNombreComercio = 24;
        private const double ProporcionAltoTarjeta = 0.55;
        private const double CompartirMinimoEtiquetaTorta = 0.05;
        private const float TamanoTextoGrafico = 12F;

        // Los montos son siempre pesos: se formatean con una cultura fija para que no dependan del idioma de la UI.
        private static readonly CultureInfo CulturaMoneda = new CultureInfo("es-AR");

        // Colores de la paleta Finanzapp para las series (los acentos naranja/azul quedan fuera de alcance).
        private static readonly Color ColorIngresos = FinanzappPaleta.Verde;
        private static readonly Color ColorEgresos = FinanzappPaleta.GrisVioleta;
        private static readonly Color[] ColoresTorta =
        {
            FinanzappPaleta.Verde,
            FinanzappPaleta.GrisVioleta,
            FinanzappPaleta.VerdeOscuro,
            FinanzappPaleta.Deshabilitado,
            FinanzappPaleta.VerdeAcento,
            FinanzappPaleta.Texto,
            FinanzappPaleta.VerdeSuave,
            FinanzappPaleta.Borde
        };

        private readonly DashboardBLL dashboardBLL = CompositionRoot.Dashboard;
        private readonly MovimientoBLL movimientoBLL = CompositionRoot.Movimiento;

        public FormDashboardAnaliticas()
        {
            InitializeComponent();
            Load += FormDashboardAnaliticas_Load;
            flpTarjetasDashboard.SizeChanged += (sender, e) => AjustarTarjetas();
        }

        // Los textos de las tarjetas (periodo, totales, leyendas) se arman al construirlas: al cambiar el idioma se reconstruyen.
        public override void Actualizar(IObservado observado)
        {
            base.Actualizar(observado);
            if (IsHandleCreated)
            {
                CargarDashboard();
            }
        }

        private void FormDashboardAnaliticas_Load(object sender, EventArgs e)
        {
            // Abre ocupando el area de trabajo con su propia barra de titulo: el mismo
            // maximizado que el boton de la barra (ver FormTraducibleBase).
            MaximizarEnAreaMdi();

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

            AjustarTarjetas();
            flpTarjetasDashboard.ResumeLayout(true);
        }

        // 2 columnas si el ancho alcanza para dos tarjetas de ancho minimo (con margenes), si no 1.
        // Se reserva siempre el ancho de la barra vertical para que nunca aparezca scroll horizontal.
        internal static int CalcularColumnas(int anchoDisponible)
        {
            int utilizable = anchoDisponible - SystemInformation.VerticalScrollBarWidth;
            return utilizable >= 2 * (AnchoMinimoTarjeta + 2 * MargenTarjeta) ? 2 : 1;
        }

        internal static Size CalcularTamanoTarjeta(int anchoDisponible)
        {
            int columnas = CalcularColumnas(anchoDisponible);
            int utilizable = anchoDisponible - SystemInformation.VerticalScrollBarWidth;
            int ancho = Math.Max(AnchoMinimoTarjeta, utilizable / columnas - 2 * MargenTarjeta);
            int alto = Math.Min(AltoMaximoTarjeta, Math.Max(AltoMinimoTarjeta, (int)(ancho * ProporcionAltoTarjeta)));
            return new Size(ancho, alto);
        }

        private void AjustarTarjetas()
        {
            if (flpTarjetasDashboard.Controls.Count == 0)
            {
                return;
            }

            Size tamano = CalcularTamanoTarjeta(flpTarjetasDashboard.ClientSize.Width);
            flpTarjetasDashboard.SuspendLayout();
            foreach (Control tarjeta in flpTarjetasDashboard.Controls)
            {
                tarjeta.Size = tamano;
            }

            flpTarjetasDashboard.ResumeLayout(true);
        }

        private Panel ConstruirPanelTarjeta(TarjetaDashboard tarjeta)
        {
            string claveTitulo = ClaveTipoTarjeta(tarjeta.Tipo);
            Size tamano = CalcularTamanoTarjeta(flpTarjetasDashboard.ClientSize.Width);
            ContenidoTarjeta contenido = ConstruirContenido(tarjeta.Tipo);

            Panel panel = new Panel
            {
                Name = "pnlTarjeta" + tarjeta.Tipo,
                Size = tamano,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = FinanzappPaleta.Fondo,
                Margin = new Padding(MargenTarjeta),
                Padding = new Padding(PaddingTarjeta)
            };

            // Tamaño fijo: con AutoSize el KryptonLabel se mide con la fuente por defecto y recorta el titulo en negrita.
            KryptonLabel titulo = new KryptonLabel
            {
                AutoSize = false,
                Size = new Size(tamano.Width - 2 * PaddingTarjeta - 90, 26),
                Location = new Point(PaddingTarjeta, PaddingTarjeta - 4),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Tag = claveTitulo,
                Text = UiDialogHelper.Traducir(claveTitulo)
            };
            titulo.StateCommon.ShortText.Font = new Font("Segoe UI", 11F, FontStyle.Bold);

            KryptonButton quitar = new KryptonButton
            {
                ButtonStyle = ButtonStyle.Custom1,
                Size = new Size(76, 26),
                Location = new Point(tamano.Width - PaddingTarjeta - 76 - 2, PaddingTarjeta - 4),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Tag = "FormDashboard.RemoveCardButton",
                Text = UiDialogHelper.Traducir("FormDashboard.RemoveCardButton")
            };
            int tarjetaId = tarjeta.Id;
            quitar.Click += (sender, e) => QuitarTarjeta(tarjetaId);

            // Periodo y totales: texto permanente bajo el titulo (se ve sin interactuar).
            KryptonLabel resumen = new KryptonLabel
            {
                AutoSize = false,
                Size = new Size(tamano.Width - 2 * PaddingTarjeta - 4, 44),
                Location = new Point(PaddingTarjeta, PaddingTarjeta + 24),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Text = contenido.Resumen
            };
            resumen.StateCommon.ShortText.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);

            int yGrafico = PaddingTarjeta + 74;
            Control cuerpo = contenido.Grafico ?? new KryptonLabel
            {
                AutoSize = false,
                Text = UiDialogHelper.Traducir("FormDashboard.NoData")
            };
            cuerpo.Location = new Point(PaddingTarjeta, yGrafico);
            cuerpo.Size = new Size(tamano.Width - 2 * PaddingTarjeta - 4, tamano.Height - yGrafico - PaddingTarjeta - 4);
            cuerpo.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            cuerpo.BackColor = FinanzappPaleta.Fondo;

            panel.Controls.Add(cuerpo);
            panel.Controls.Add(resumen);
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

        // Resumen (periodo y totales) y grafico de una tarjeta; Grafico es null cuando no hay datos.
        private sealed class ContenidoTarjeta
        {
            public string Resumen { get; set; }
            public Control Grafico { get; set; }
        }

        private ContenidoTarjeta ConstruirContenido(TipoTarjetaDashboard tipo)
        {
            switch (tipo)
            {
                case TipoTarjetaDashboard.GastosPorCategoria:
                    return ConstruirContenidoPorCategoria(ABSTRACCIONES.TipoMovimiento.Egreso);
                case TipoTarjetaDashboard.IngresosPorCategoria:
                    return ConstruirContenidoPorCategoria(ABSTRACCIONES.TipoMovimiento.Ingreso);
                case TipoTarjetaDashboard.ComparativoMensual:
                    return ConstruirContenidoMensual(true);
                case TipoTarjetaDashboard.GastosPorComercio:
                    return ConstruirContenidoGastoPorComercio();
                case TipoTarjetaDashboard.GastoAcumuladoMensual:
                    return ConstruirContenidoGastoAcumulado();
                case TipoTarjetaDashboard.EvolucionBalance:
                default:
                    return ConstruirContenidoMensual(false);
            }
        }

        private ContenidoTarjeta ConstruirContenidoPorCategoria(ABSTRACCIONES.TipoMovimiento tipo)
        {
            DateTime hoy = DateTime.Today;
            DateTime desde = new DateTime(hoy.Year, hoy.Month, 1);
            DateTime hasta = desde.AddMonths(1).AddDays(-1);

            List<TotalPorCategoria> totales = movimientoBLL.ObtenerTotalesPorCategoria(tipo, desde, hasta)
                .Where(item => item.Total > 0)
                .OrderByDescending(item => item.Total)
                .ToList();
            decimal suma = totales.Sum(item => item.Total);
            string periodo = UiDialogHelper.Traducir("FormDashboard.PeriodLabel", FormatearMes(desde.Year, desde.Month));

            if (totales.Count == 0)
            {
                return new ContenidoTarjeta { Resumen = periodo };
            }

            PieSeries<double>[] series = totales
                .Select((item, indice) => new PieSeries<double>
                {
                    Values = new[] { (double)item.Total },
                    Name = string.Format(CulturaMoneda, "{0} - {1} ({2:P1})", item.CategoriaNombre, Moneda(item.Total), item.Total / suma),
                    Fill = Pintura(ColoresTorta[indice % ColoresTorta.Length]),
                    DataLabelsPaint = PinturaTexto(),
                    DataLabelsSize = TamanoTextoGrafico,
                    DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                    DataLabelsFormatter = punto => punto.StackedValue.Share >= CompartirMinimoEtiquetaTorta
                        ? punto.StackedValue.Share.ToString("P0", CulturaMoneda)
                        : string.Empty,
                    ToolTipLabelFormatter = punto => Moneda((decimal)punto.Coordinate.PrimaryValue)
                })
                .ToArray();

            PieChart grafico = new PieChart
            {
                Series = series,
                LegendPosition = LiveChartsCore.Measure.LegendPosition.Right,
                LegendTextPaint = PinturaTexto(),
                LegendTextSize = TamanoTextoGrafico
            };

            return new ContenidoTarjeta
            {
                Resumen = periodo + Environment.NewLine + UiDialogHelper.Traducir("FormDashboard.TotalLabel", Moneda(suma)),
                Grafico = grafico
            };
        }

        // comparativo = true: columnas de ingresos y egresos; false: evolucion del balance (areas apiladas).
        private ContenidoTarjeta ConstruirContenidoMensual(bool comparativo)
        {
            List<TotalPorMes> totales = ObtenerTotalesUltimosMeses();
            if (totales.Count == 0)
            {
                return new ContenidoTarjeta
                {
                    Resumen = UiDialogHelper.Traducir("FormDashboard.PeriodLabel", FormatearMes(DateTime.Today.Year, DateTime.Today.Month))
                };
            }

            string[] etiquetas = ObtenerEtiquetasMeses(totales);
            decimal ingresos = totales.Sum(item => item.TotalIngresos);
            decimal egresos = totales.Sum(item => item.TotalEgresos);
            string periodo = UiDialogHelper.Traducir("FormDashboard.PeriodLabel", etiquetas.First() + " - " + etiquetas.Last());
            string nombreIngresos = UiDialogHelper.Traducir("FormMovements.Value.Type.Income");
            string nombreEgresos = UiDialogHelper.Traducir("FormMovements.Value.Type.Expense");

            ISeries[] series = comparativo
                ? new ISeries[]
                {
                    CrearColumnas(nombreIngresos, totales.Select(item => (double)item.TotalIngresos), ColorIngresos),
                    CrearColumnas(nombreEgresos, totales.Select(item => (double)item.TotalEgresos), ColorEgresos)
                }
                : new ISeries[]
                {
                    CrearArea(nombreIngresos, totales.Select(item => (double)item.TotalIngresos), ColorIngresos),
                    CrearArea(nombreEgresos, totales.Select(item => (double)item.TotalEgresos), ColorEgresos)
                };

            CartesianChart grafico = new CartesianChart
            {
                Series = series,
                XAxes = new[] { new Axis { Labels = etiquetas, TextSize = TamanoTextoGrafico, LabelsPaint = PinturaTexto() } },
                YAxes = new[]
                {
                    new Axis
                    {
                        MinLimit = 0,
                        TextSize = TamanoTextoGrafico,
                        LabelsPaint = PinturaTexto(),
                        Labeler = valor => MonedaAbreviada(valor)
                    }
                },
                LegendPosition = LiveChartsCore.Measure.LegendPosition.Bottom,
                LegendTextPaint = PinturaTexto(),
                LegendTextSize = TamanoTextoGrafico
            };

            string totalesTexto = comparativo
                ? UiDialogHelper.Traducir("FormDashboard.IncomeExpenseTotals", Moneda(ingresos), Moneda(egresos))
                : UiDialogHelper.Traducir("FormDashboard.BalanceLabel", Moneda(ingresos - egresos));

            return new ContenidoTarjeta { Resumen = periodo + Environment.NewLine + totalesTexto, Grafico = grafico };
        }

        private ContenidoTarjeta ConstruirContenidoGastoPorComercio()
        {
            DateTime hoy = DateTime.Today;
            DateTime desde = new DateTime(hoy.Year, hoy.Month, 1);
            DateTime hasta = desde.AddMonths(1).AddDays(-1);
            string periodo = UiDialogHelper.Traducir("FormDashboard.PeriodLabel", FormatearMes(desde.Year, desde.Month));

            GastoPorComercio gasto = movimientoBLL.ObtenerGastoPorComercio(desde, hasta, CantidadComercios);
            string sinComercio = gasto.TotalSinComercio > 0
                ? UiDialogHelper.Traducir("FormDashboard.WithoutCommerceLabel", Moneda(gasto.TotalSinComercio))
                : string.Empty;

            if (gasto.Comercios.Count == 0)
            {
                return new ContenidoTarjeta { Resumen = Unir(periodo, sinComercio) };
            }

            // La primera barra se dibuja abajo: se invierte el orden para que el mayor quede arriba.
            List<TotalPorComercio> barras = Enumerable.Reverse(gasto.Comercios).ToList();
            string[] nombres = barras.Select(item => item.ComercioNombre).ToArray();
            decimal total = gasto.Comercios.Sum(item => item.Total);

            RowSeries<double> serie = new RowSeries<double>
            {
                Values = barras.Select(item => (double)item.Total).ToArray(),
                Name = UiDialogHelper.Traducir(ClaveTipoTarjeta(TipoTarjetaDashboard.GastosPorComercio)),
                Fill = Pintura(ColorEgresos),
                DataLabelsPaint = PinturaTexto(),
                DataLabelsSize = TamanoTextoGrafico,
                DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.End,
                DataLabelsFormatter = punto => Moneda((decimal)punto.Coordinate.PrimaryValue),
                XToolTipLabelFormatter = punto => nombres[(int)punto.Coordinate.SecondaryValue] + ": " + Moneda((decimal)punto.Coordinate.PrimaryValue)
            };

            CartesianChart grafico = new CartesianChart
            {
                Series = new ISeries[] { serie },
                XAxes = new[]
                {
                    new Axis
                    {
                        MinLimit = 0,
                        TextSize = TamanoTextoGrafico,
                        LabelsPaint = PinturaTexto(),
                        Labeler = valor => MonedaAbreviada(valor)
                    }
                },
                YAxes = new[]
                {
                    new Axis
                    {
                        Labels = nombres.Select(Truncar).ToArray(),
                        TextSize = TamanoTextoGrafico,
                        LabelsPaint = PinturaTexto()
                    }
                }
            };

            string totalTexto = UiDialogHelper.Traducir("FormDashboard.TotalLabel", Moneda(total));
            return new ContenidoTarjeta { Resumen = Unir(periodo, Unir(totalTexto, sinComercio, "   ")), Grafico = grafico };
        }

        private ContenidoTarjeta ConstruirContenidoGastoAcumulado()
        {
            DateTime hoy = DateTime.Today;
            DateTime anterior = hoy.AddMonths(-1);
            string periodo = UiDialogHelper.Traducir("FormDashboard.PeriodLabel",
                FormatearMes(hoy.Year, hoy.Month) + " / " + FormatearMes(anterior.Year, anterior.Month));

            GastoAcumuladoMensual gasto = movimientoBLL.ObtenerGastoAcumuladoMensual(hoy);
            if (gasto.AcumuladoActual == 0 && gasto.MesAnterior.Last() == 0)
            {
                return new ContenidoTarjeta { Resumen = periodo };
            }

            string nombreActual = UiDialogHelper.Traducir("FormDashboard.CurrentMonthName");
            string nombreAnterior = UiDialogHelper.Traducir("FormDashboard.PreviousMonthName");
            int ultimoActual = gasto.MesActual.Count - 1;
            int ultimoAnterior = gasto.MesAnterior.Count - 1;

            ISeries[] series =
            {
                CrearLinea(nombreActual, gasto.MesActual, ColorIngresos, ultimoActual),
                CrearLinea(nombreAnterior, gasto.MesAnterior, ColorEgresos, ultimoAnterior)
            };

            CartesianChart grafico = new CartesianChart
            {
                Series = series,
                XAxes = new[]
                {
                    new Axis
                    {
                        Labels = Enumerable.Range(1, gasto.MesAnterior.Count)
                            .Select(dia => dia.ToString(CultureInfo.InvariantCulture)).ToArray(),
                        TextSize = TamanoTextoGrafico,
                        LabelsPaint = PinturaTexto()
                    }
                },
                YAxes = new[]
                {
                    new Axis
                    {
                        MinLimit = 0,
                        TextSize = TamanoTextoGrafico,
                        LabelsPaint = PinturaTexto(),
                        Labeler = valor => MonedaAbreviada(valor)
                    }
                },
                // Margen derecho fijo: el valor del ultimo punto se escribe a su derecha y se cortaba contra el borde.
                DrawMargin = new LiveChartsCore.Measure.Margin(float.NaN, float.NaN, 70F, float.NaN),
                LegendPosition = LiveChartsCore.Measure.LegendPosition.Bottom,
                LegendTextPaint = PinturaTexto(),
                LegendTextSize = TamanoTextoGrafico
            };

            string resumen = nombreActual + ": " + Moneda(gasto.AcumuladoActual)
                + "   " + nombreAnterior + ": " + Moneda(gasto.AcumuladoAnteriorAlMismoDia);
            if (gasto.VariacionPorcentual.HasValue)
            {
                decimal variacion = gasto.VariacionPorcentual.Value;
                resumen += "   " + UiDialogHelper.Traducir("FormDashboard.VariationLabel",
                    (variacion > 0 ? "+" : string.Empty) + variacion.ToString("0.#", CulturaMoneda) + " %");
            }

            return new ContenidoTarjeta { Resumen = Unir(periodo, resumen), Grafico = grafico };
        }

        // El valor completo solo se escribe en el ultimo punto de la serie para no saturar el grafico.
        private LineSeries<double> CrearLinea(string nombre, IEnumerable<decimal> acumulados, Color color, int indiceUltimo)
        {
            SolidColorPaint trazo = Pintura(color);
            trazo.StrokeThickness = 3;

            return new LineSeries<double>
            {
                Values = acumulados.Select(valor => (double)valor).ToArray(),
                Name = nombre,
                Fill = null,
                Stroke = trazo,
                GeometrySize = 6,
                GeometryFill = Pintura(color),
                GeometryStroke = Pintura(color),
                LineSmoothness = 0,
                DataLabelsPaint = PinturaTexto(),
                DataLabelsSize = TamanoTextoGrafico,
                DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top,
                DataLabelsFormatter = punto => (int)punto.Coordinate.SecondaryValue == indiceUltimo
                    ? Moneda((decimal)punto.Coordinate.PrimaryValue)
                    : string.Empty,
                YToolTipLabelFormatter = punto => Moneda((decimal)punto.Coordinate.PrimaryValue)
            };
        }

        private static string Truncar(string texto)
        {
            return texto.Length <= MaxLargoNombreComercio ? texto : texto.Substring(0, MaxLargoNombreComercio - 1) + "…";
        }

        private static string Unir(string primero, string segundo, string separador = null)
        {
            if (string.IsNullOrEmpty(segundo))
            {
                return primero;
            }

            return primero + (separador ?? Environment.NewLine) + segundo;
        }

        private ColumnSeries<double> CrearColumnas(string nombre, IEnumerable<double> valores, Color color)
        {
            return new ColumnSeries<double>
            {
                Values = valores.ToArray(),
                Name = nombre,
                Fill = Pintura(color),
                DataLabelsPaint = PinturaTexto(),
                DataLabelsSize = 10F,
                DataLabelsRotation = -90,
                DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.End,
                DataLabelsFormatter = punto => Moneda((decimal)punto.Coordinate.PrimaryValue),
                YToolTipLabelFormatter = punto => Moneda((decimal)punto.Coordinate.PrimaryValue)
            };
        }

        private StackedAreaSeries<double> CrearArea(string nombre, IEnumerable<double> valores, Color color)
        {
            return new StackedAreaSeries<double>
            {
                Values = valores.ToArray(),
                Name = nombre,
                Fill = Pintura(color, 0x99),
                Stroke = Pintura(color),
                GeometryFill = Pintura(color),
                GeometryStroke = Pintura(color),
                DataLabelsPaint = PinturaTexto(),
                DataLabelsSize = 10F,
                DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Middle,
                DataLabelsFormatter = punto => Moneda((decimal)punto.Coordinate.PrimaryValue),
                YToolTipLabelFormatter = punto => Moneda((decimal)punto.Coordinate.PrimaryValue)
            };
        }

        private static string Moneda(decimal valor)
        {
            return "$ " + valor.ToString("N0", CulturaMoneda);
        }

        // Solo para las marcas del eje vertical: 3,1 M / 500 mil.
        private static string MonedaAbreviada(double valor)
        {
            if (valor >= 1000000)
            {
                return "$ " + (valor / 1000000).ToString("0.#", CulturaMoneda) + " M";
            }

            if (valor >= 1000)
            {
                return "$ " + (valor / 1000).ToString("0.#", CulturaMoneda) + " mil";
            }

            return "$ " + valor.ToString("0", CulturaMoneda);
        }

        private static string FormatearMes(int anio, int mes)
        {
            return new DateTime(anio, mes, 1).ToString("MM/yyyy", CultureInfo.InvariantCulture);
        }

        private static SolidColorPaint PinturaTexto()
        {
            return Pintura(FinanzappPaleta.Texto);
        }

        private static SolidColorPaint Pintura(Color color, byte alfa = 0xFF)
        {
            return new SolidColorPaint(new SKColor(color.R, color.G, color.B, alfa));
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
