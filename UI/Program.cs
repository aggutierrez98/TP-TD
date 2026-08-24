using System;
using System.Windows.Forms;
using BLL;
using SERVICIOS;

namespace TpIDS
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicacion.
        /// </summary>
        [STAThread]
        static void Main()  
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            CompositionRoot.Inicializar();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // Arranque funcional (carga de idioma T05 + verificacion de integridad T07).
                InicializadorAplicacionBLL.Inicializar();
            }
            catch (ExcepcionIntegridad ex)
            {
                ManejadorExcepcionesBLL.RegistrarExcepcionTecnica(ex, "Verificacion de integridad", "Program");
                Application.Run(new FormContingenciaIntegridad(ex));
                return;
            }
            catch (Exception ex)
            {
                ManejadorExcepcionesBLL.RegistrarExcepcionTecnica(ex, "Arranque de la aplicacion", "Program");
                UiDialogHelper.MostrarError("Error.Technical.Unexpected");
                return;
            }

            Application.Run(new FormLogin());
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            ManejadorExcepcionesBLL.RegistrarExcepcionTecnica(e.Exception, "Excepcion UI no controlada", "Program");
            UiDialogHelper.MostrarError("Error.Technical.Unexpected");
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = e.ExceptionObject as Exception ?? new Exception("Excepcion no controlada.");
            ManejadorExcepcionesBLL.RegistrarExcepcionTecnica(exception, "Excepcion de dominio no controlada", "Program");
        }
    }
}
