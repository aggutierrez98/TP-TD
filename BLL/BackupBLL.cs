using System;
using System.IO;
using REPOSITORIO;
using SERVICIOS;

namespace BLL
{
    // T08: backup y restauracion de la base de datos.
    //
    // A diferencia de todas las demas BLL del proyecto, esta NO recibe un
    // IUnidadDeTrabajoFactory: SQL Server prohibe ejecutar BACKUP DATABASE / RESTORE DATABASE
    // dentro de una transaccion explicita, y AccesoDatosSqlServer (el mecanismo detras de
    // IUnidadDeTrabajo) siempre abre una si no hay ninguna activa. BackupRepositorio ejecuta
    // por fuera de ese mecanismo, con su propia conexion en autocommit (ver DAL.BackupDAL).
    // Por eso tambien se loguea con RegistrarOperacion/RegistrarFalloNegocio (unidad propia
    // de BitacoraBLL) en lugar de las variantes ...Interno que comparten unidad de trabajo.
    public class BackupBLL
    {
        private const string ExtensionBackup = ".bak";

        private readonly IBackupRepositorio _backupRepositorio;
        private readonly IBitacoraBLL _bitacoraBLL;

        internal BackupBLL(IBackupRepositorio backupRepositorio, IBitacoraBLL bitacoraBLL)
        {
            _backupRepositorio = backupRepositorio;
            _bitacoraBLL = bitacoraBLL;
        }

        public void CrearBackup(string rutaArchivo)
        {
            string ruta = (rutaArchivo ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(ruta))
            {
                _bitacoraBLL.RegistrarFalloNegocio("FormAuditLog.Value.Activity.RejectedBackupCreation", "FormAuditLog.Value.Info.BackupPathRequired", "BackupBLL");
                throw new ExcepcionNegocio("Error.Backup.PathRequired");
            }

            if (!TieneExtensionBackup(ruta))
            {
                _bitacoraBLL.RegistrarFalloNegocio("FormAuditLog.Value.Activity.RejectedBackupCreation", "FormAuditLog.Value.Info.BackupInvalidExtension", "BackupBLL");
                throw new ExcepcionNegocio("Error.Backup.InvalidExtension");
            }

            string directorio = Path.GetDirectoryName(ruta);
            if (string.IsNullOrWhiteSpace(directorio) || !Directory.Exists(directorio))
            {
                _bitacoraBLL.RegistrarFalloNegocio("FormAuditLog.Value.Activity.RejectedBackupCreation", "FormAuditLog.Value.Info.BackupDirectoryNotFound", "BackupBLL");
                throw new ExcepcionNegocio("Error.Backup.DirectoryNotFound");
            }

            _backupRepositorio.CrearBackup(ruta);
            _bitacoraBLL.RegistrarOperacion("FormAuditLog.Value.Activity.BackupCreated", "FormAuditLog.Value.Info.BackupCreated", "BackupBLL");
        }

        public void RestaurarBackup(string rutaArchivo)
        {
            string ruta = (rutaArchivo ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(ruta))
            {
                _bitacoraBLL.RegistrarFalloNegocio("FormAuditLog.Value.Activity.RejectedBackupRestoration", "FormAuditLog.Value.Info.BackupPathRequired", "BackupBLL");
                throw new ExcepcionNegocio("Error.Backup.PathRequired");
            }

            if (!TieneExtensionBackup(ruta))
            {
                _bitacoraBLL.RegistrarFalloNegocio("FormAuditLog.Value.Activity.RejectedBackupRestoration", "FormAuditLog.Value.Info.BackupInvalidExtension", "BackupBLL");
                throw new ExcepcionNegocio("Error.Backup.InvalidExtension");
            }

            if (!File.Exists(ruta))
            {
                _bitacoraBLL.RegistrarFalloNegocio("FormAuditLog.Value.Activity.RejectedBackupRestoration", "FormAuditLog.Value.Info.BackupFileNotFound", "BackupBLL");
                throw new ExcepcionNegocio("Error.Backup.FileNotFound");
            }

            _backupRepositorio.RestaurarBackup(ruta);
            _bitacoraBLL.RegistrarOperacion("FormAuditLog.Value.Activity.BackupRestored", "FormAuditLog.Value.Info.BackupRestored", "BackupBLL");
        }

        private static bool TieneExtensionBackup(string ruta)
        {
            return string.Equals(Path.GetExtension(ruta), ExtensionBackup, StringComparison.OrdinalIgnoreCase);
        }
    }
}
