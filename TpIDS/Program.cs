using System;
using System.Windows.Forms;
using BLL;

namespace TpIDS
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            BootstrapAplicacionBLL.Inicializar();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormLogin());
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            ManejadorExcepcionesBLL.RegistrarExcepcionTecnica(e.Exception, "Excepcion UI no controlada", "Program");
            MessageBox.Show(
                "Ocurrio un error tecnico inesperado. Revise la bitacora para mas informacion.",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = e.ExceptionObject as Exception ?? new Exception("Excepcion no controlada.");
            ManejadorExcepcionesBLL.RegistrarExcepcionTecnica(exception, "Excepcion de dominio no controlada", "Program");
        }
    }
}
