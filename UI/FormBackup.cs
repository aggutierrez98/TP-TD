using System;
using System.Windows.Forms;
using BLL;

namespace TpIDS
{
    public partial class FormBackup : FormTraducibleBase
    {
        private readonly BackupBLL _backupBLL;

        public FormBackup()
        {
            _backupBLL = CompositionRoot.Backup;
            InitializeComponent();
        }

        private void btnSeleccionarRutaCrear_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dialogo = new SaveFileDialog())
            {
                dialogo.Filter = "Archivos de backup (*.bak)|*.bak";
                dialogo.Title = UiDialogHelper.Traducir("FormBackup.SaveDialogTitle");
                dialogo.DefaultExt = "bak";
                dialogo.AddExtension = true;
                dialogo.OverwritePrompt = true;

                if (dialogo.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                txtbRutaCrearBackup.Text = dialogo.FileName;
                btnCrearBackup.Enabled = !string.IsNullOrWhiteSpace(txtbRutaCrearBackup.Text);
            }
        }

        private void btnCrearBackup_Click(object sender, EventArgs e)
        {
            try
            {
                _backupBLL.CrearBackup(txtbRutaCrearBackup.Text);
                UiDialogHelper.MostrarInfo("FormBackup.Success.BackupCreated");
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Creacion de backup");
            }
        }

        private void btnSeleccionarRutaRestaurar_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialogo = new OpenFileDialog())
            {
                dialogo.Filter = "Archivos de backup (*.bak)|*.bak";
                dialogo.Title = UiDialogHelper.Traducir("FormBackup.OpenDialogTitle");
                dialogo.CheckFileExists = true;

                if (dialogo.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                txtbRutaRestaurarBackup.Text = dialogo.FileName;
                btnRestaurarBackup.Enabled = !string.IsNullOrWhiteSpace(txtbRutaRestaurarBackup.Text);
            }
        }

        private void btnRestaurarBackup_Click(object sender, EventArgs e)
        {
            DialogResult confirmacion = UiDialogHelper.Confirmar(
                "FormBackup.Confirm.RestoreMessage",
                "FormBackup.Confirm.RestoreTitle",
                MessageBoxIcon.Warning);

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            try
            {
                _backupBLL.RestaurarBackup(txtbRutaRestaurarBackup.Text);
                UiDialogHelper.MostrarInfo("FormBackup.Success.BackupRestored");

                // Tras un restore exitoso, los singletons de sesion/autorizacion en memoria
                // pueden haber quedado desincronizados del estado restaurado. Se fuerza el
                // cierre de FormPrincipal: FormLogin ya engancha su FormClosed para cerrar la
                // sesion actual y volver a mostrarse (ver FormLogin.btnIngreso_Click), asi que
                // alcanza con cerrar el shell para reusar ese mismo mecanismo de logout/relogin.
                if (MdiParent != null)
                {
                    MdiParent.Close();
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Restauracion de backup");
            }
        }
    }
}
