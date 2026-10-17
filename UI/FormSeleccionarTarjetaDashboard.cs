using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using BE;
using BLL;

namespace TpIDS
{
    // Dialogo modal simple para elegir, entre los tipos de tarjeta todavia no agregados
    // al Dashboard del usuario actual, cual agregar. No persiste nada por si mismo: el
    // llamador (FormDashboardAnaliticas) hace el alta real via DashboardBLL.AgregarTarjeta.
    public partial class FormSeleccionarTarjetaDashboard : FormTraducibleBase
    {
        private readonly DashboardBLL dashboardBLL = CompositionRoot.Dashboard;

        public FormSeleccionarTarjetaDashboard()
        {
            InitializeComponent();
            Load += FormSeleccionarTarjetaDashboard_Load;
        }

        public TipoTarjetaDashboard? TipoSeleccionado { get; private set; }

        private void FormSeleccionarTarjetaDashboard_Load(object sender, EventArgs e)
        {
            CargarTiposDisponibles();
        }

        private void CargarTiposDisponibles()
        {
            try
            {
                IReadOnlyList<TipoTarjetaDashboard> tipos = dashboardBLL.ObtenerTiposDisponibles();

                lstbTiposTarjeta.Items.Clear();
                foreach (TipoTarjetaDashboard tipo in tipos)
                {
                    string clave = ClaveTipoTarjeta(tipo);
                    lstbTiposTarjeta.Items.Add(new OpcionTraducible(clave, tipo)
                    {
                        Texto = UiDialogHelper.Traducir(clave)
                    });
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Consulta de tipos de tarjeta disponibles");
            }
        }

        private void lstbTiposTarjeta_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnAceptarTarjeta.Enabled = lstbTiposTarjeta.SelectedItem is OpcionTraducible;
        }

        private void btnAceptarTarjeta_Click(object sender, EventArgs e)
        {
            if (!(lstbTiposTarjeta.SelectedItem is OpcionTraducible opcion) || !(opcion.Valor is TipoTarjetaDashboard tipo))
            {
                return;
            }

            TipoSeleccionado = tipo;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancelarTarjeta_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private static string ClaveTipoTarjeta(TipoTarjetaDashboard tipo)
        {
            return string.Format(CultureInfo.InvariantCulture, "FormDashboard.CardType.{0}", tipo);
        }
    }
}
