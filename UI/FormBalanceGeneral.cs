using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using ABSTRACCIONES;
using BE;
using BLL;
using SERVICIOS;

namespace TpIDS
{
    // Pantalla de analitica de solo lectura: totales de ingresos/egresos del periodo
    // seleccionado mas el detalle de movimientos que los componen, paginado en memoria
    // (no hay paginacion SQL en el proyecto). No hay alta/edicion/baja aca.
    public partial class FormBalanceGeneral : FormTraducibleBase
    {
        private const int TamanioPagina = 20;

        private readonly MovimientoBLL movimientoBLL = CompositionRoot.Movimiento;
        private readonly CategoriaBLL categoriaBLL = CompositionRoot.Categoria;
        private readonly ISesion sesion = CompositionRoot.Sesion;

        private List<Movimiento> movimientosFiltrados = new List<Movimiento>();
        private Dictionary<int, string> nombresCategoria = new Dictionary<int, string>();
        private int paginaActual;

        public FormBalanceGeneral()
        {
            InitializeComponent();
            Load += FormBalanceGeneral_Load;
        }

        private void FormBalanceGeneral_Load(object sender, EventArgs e)
        {
            dgvBalance.CellFormatting += dgvBalance_CellFormatting;

            // Abre ocupando el area de trabajo con su propia barra de titulo: el mismo
            // maximizado que el boton de la barra (ver FormTraducibleBase).
            MaximizarEnAreaMdi();

            CargarTipos();
            InicializarFechasPorDefecto();
            RefrescarDatos();
        }

        // View-model plano para la grilla. Mismo shape que FormMovimientos.FilaMovimiento.
        private sealed class FilaMovimiento
        {
            public int Id { get; set; }
            public DateTime Fecha { get; set; }
            public string Tipo { get; set; }
            public string Categoria { get; set; }
            public string Comercio { get; set; }
            public int? ComercioId { get; set; }
            public decimal Monto { get; set; }
            public string Descripcion { get; set; }
        }

        private void CargarTipos()
        {
            cmbTipoBalance.DisplayMember = "Texto";
            cmbTipoBalance.Items.Clear();
            cmbTipoBalance.Items.Add(new OpcionTraducible(string.Empty, string.Empty));
            cmbTipoBalance.Items.Add(new OpcionTraducible("FormMovements.Value.Type.Income", TipoMovimiento.Ingreso.ToString()));
            cmbTipoBalance.Items.Add(new OpcionTraducible("FormMovements.Value.Type.Expense", TipoMovimiento.Egreso.ToString()));
            cmbTipoBalance.SelectedIndex = 0;
        }

        // El rango del Balance General siempre esta acotado (sin checkbox de deshabilitar,
        // a diferencia de FormMovimientos): arranca en el mes actual completo.
        private void InicializarFechasPorDefecto()
        {
            DateTime hoy = DateTime.Today;
            dtpDesdeBalance.Value = new DateTime(hoy.Year, hoy.Month, 1);
            dtpHastaBalance.Value = dtpDesdeBalance.Value.AddMonths(1).AddDays(-1);
        }

        private void btnBuscarBalance_Click(object sender, EventArgs e)
        {
            RefrescarDatos();
        }

        private void RefrescarDatos()
        {
            try
            {
                int usuarioActualId = sesion.GetUsuarioActualId().Value;
                DateTime desde = dtpDesdeBalance.Value.Date;
                DateTime hasta = dtpHastaBalance.Value.Date;

                FiltroMovimiento filtro = new FiltroMovimiento
                {
                    FechaDesde = desde,
                    FechaHasta = hasta,
                    Tipo = ObtenerTipoSeleccionado(cmbTipoBalance),
                    Texto = string.IsNullOrWhiteSpace(txtbBuscarBalance.Text) ? null : txtbBuscarBalance.Text.Trim()
                };

                movimientosFiltrados = movimientoBLL.ObtenerPorUsuario(usuarioActualId, filtro);
                nombresCategoria = categoriaBLL
                    .ObtenerVisiblesParaUsuario(usuarioActualId)
                    .ToDictionary(item => item.Id, item => item.Nombre);

                paginaActual = 0;
                MostrarPagina();

                // El resumen (ingresos/egresos/balance) solo toma el rango de fechas: el metodo
                // del BLL no recibe tipo/texto/categoria, a diferencia del detalle de la grilla.
                ActualizarResumen(desde, hasta);
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Consulta de balance general");
            }
        }

        private void ActualizarResumen(DateTime desde, DateTime hasta)
        {
            ResumenMovimientos resumen = movimientoBLL.ObtenerResumenBalance(desde, hasta);
            decimal balance = resumen.TotalIngresos - resumen.TotalEgresos;

            lblIngresosMontoBalance.Text = resumen.TotalIngresos.ToString("N2", CultureInfo.InvariantCulture);
            lblEgresosMontoBalance.Text = resumen.TotalEgresos.ToString("N2", CultureInfo.InvariantCulture);
            lblSaldoMontoBalance.Text = balance.ToString("N2", CultureInfo.InvariantCulture);
        }

        private void MostrarPagina()
        {
            int totalPaginas = Math.Max(1, (int)Math.Ceiling(movimientosFiltrados.Count / (double)TamanioPagina));

            if (paginaActual >= totalPaginas)
            {
                paginaActual = totalPaginas - 1;
            }

            if (paginaActual < 0)
            {
                paginaActual = 0;
            }

            List<FilaMovimiento> filas = movimientosFiltrados
                .Skip(paginaActual * TamanioPagina)
                .Take(TamanioPagina)
                .Select(item => new FilaMovimiento
                {
                    Id = item.Id,
                    Fecha = item.Fecha,
                    Tipo = item.Tipo.ToString(),
                    Categoria = nombresCategoria.ContainsKey(item.Categoria.Id) ? nombresCategoria[item.Categoria.Id] : string.Empty,
                    Comercio = item.Comercio != null ? item.Comercio.Nombre : string.Empty,
                    ComercioId = item.Comercio != null ? (int?)item.Comercio.Id : null,
                    Monto = item.Monto,
                    Descripcion = item.Descripcion
                })
                .ToList();

            dgvBalance.DataSource = null;
            dgvBalance.DataSource = filas;

            lblPaginaBalance.Text = UiDialogHelper.Traducir("FormBalanceSheet.PageLabel", paginaActual + 1, totalPaginas);
            btnPaginaAnteriorBalance.Enabled = paginaActual > 0;
            btnPaginaSiguienteBalance.Enabled = paginaActual < totalPaginas - 1;
        }

        private void btnPaginaAnteriorBalance_Click(object sender, EventArgs e)
        {
            if (paginaActual > 0)
            {
                paginaActual--;
                MostrarPagina();
            }
        }

        private void btnPaginaSiguienteBalance_Click(object sender, EventArgs e)
        {
            int totalPaginas = Math.Max(1, (int)Math.Ceiling(movimientosFiltrados.Count / (double)TamanioPagina));
            if (paginaActual < totalPaginas - 1)
            {
                paginaActual++;
                MostrarPagina();
            }
        }

        private void dgvBalance_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null || e.RowIndex < 0)
            {
                return;
            }

            if (!string.Equals(dgvBalance.Columns[e.ColumnIndex].Name, "Tipo", StringComparison.Ordinal))
            {
                return;
            }

            IIdioma idioma = IdiomaService.GetInstance().IdiomaActual;
            if (idioma == null)
            {
                return;
            }

            string key = string.Equals(e.Value.ToString(), TipoMovimiento.Ingreso.ToString(), StringComparison.Ordinal)
                ? "FormMovements.Value.Type.Income"
                : "FormMovements.Value.Type.Expense";

            e.Value = idioma.ObtenerTraduccion(key);
            e.FormattingApplied = true;
        }

        private static TipoMovimiento? ObtenerTipoSeleccionado(Krypton.Toolkit.KryptonComboBox combo)
        {
            OpcionTraducible opcion = combo.SelectedItem as OpcionTraducible;
            string valor = opcion?.Valor as string;
            if (string.IsNullOrWhiteSpace(valor))
            {
                return null;
            }

            return (TipoMovimiento)Enum.Parse(typeof(TipoMovimiento), valor);
        }

        public override void Actualizar(IObservado observado)
        {
            base.Actualizar(observado);
            // El label de paginado no tiene Tag estatico (el texto lleva numeros
            // formateados con string.Format), asi que se recompone a mano ante un
            // cambio de idioma en caliente.
            MostrarPagina();
        }
    }
}
