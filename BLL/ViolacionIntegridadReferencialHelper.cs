using System;
using System.Data.SqlClient;

namespace BLL
{
    // Deteccion de violaciones de FK que llegan envueltas como ExcepcionTecnica desde
    // AccesoDatosSqlServer.ExecuteInTransaction (que ya captura el SqlException original
    // y lo relanza generico como "Error.Technical.SqlFailure"). Se usa en los Eliminar()
    // que no pueden validar la integridad referencial de antemano (p. ej. Comercio,
    // que depende de Producto, fuera de esta tanda).
    internal static class ViolacionIntegridadReferencialHelper
    {
        private const int SqlErrorForeignKeyConflict = 547;

        public static bool EsViolacionRestriccionForanea(Exception exception)
        {
            SqlException sqlException = ExtraerSqlException(exception);
            return sqlException != null && sqlException.Number == SqlErrorForeignKeyConflict;
        }

        private static SqlException ExtraerSqlException(Exception exception)
        {
            for (Exception actual = exception; actual != null; actual = actual.InnerException)
            {
                SqlException sqlException = actual as SqlException;
                if (sqlException != null)
                {
                    return sqlException;
                }
            }

            return null;
        }
    }
}
