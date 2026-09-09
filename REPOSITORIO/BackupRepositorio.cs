using DAL;

namespace REPOSITORIO
{
    // A diferencia del resto de los repositorios, no expone un constructor interno que
    // reciba una AccesoDatosSqlServer compartida: BackupDAL maneja su propia conexion fuera
    // de IUnidadDeTrabajo (ver comentario en DAL.BackupDAL), asi que no hay nada que compartir.
    public class BackupRepositorio : IBackupRepositorio
    {
        private readonly BackupDAL _backupDAL;

        public BackupRepositorio()
        {
            _backupDAL = new BackupDAL();
        }

        public void CrearBackup(string rutaArchivo)
        {
            _backupDAL.CrearBackup(rutaArchivo);
        }

        public void RestaurarBackup(string rutaArchivo)
        {
            _backupDAL.RestaurarBackup(rutaArchivo);
        }
    }
}
