namespace REPOSITORIO
{
    public interface IBackupRepositorio
    {
        void CrearBackup(string rutaArchivo);

        void RestaurarBackup(string rutaArchivo);
    }
}
