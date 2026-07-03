using System.Data.SqlClient;

namespace TpIDS.UiTests.Infrastructure
{
    /// <summary>
    /// Acceso de solo lectura a la BD de test para asserts.
    /// Las escrituras de negocio siempre van por la UI para no romper DVH/DVV;
    /// la unica excepcion documentada es la corrupcion intencional de T07.
    /// </summary>
    internal static class TestDatabase
    {
        public static object EscalarOrNull(string sql, params SqlParameter[] parametros)
        {
            using (SqlConnection connection = new SqlConnection(TestConfig.ConnectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                foreach (SqlParameter parametro in parametros)
                {
                    command.Parameters.Add(parametro);
                }

                connection.Open();
                return command.ExecuteScalar();
            }
        }

        public static int Contar(string sql, params SqlParameter[] parametros)
        {
            return (int)EscalarOrNull(sql, parametros);
        }

        public static int ContarBitacora(string actividad, string usernameSnapshot)
        {
            return Contar(
                "SELECT COUNT(*) FROM dbo.Bitacora WHERE Actividad = @actividad " +
                "AND (@username IS NULL OR UsernameSnapshot = @username)",
                new SqlParameter("@actividad", actividad),
                new SqlParameter("@username", (object)usernameSnapshot ?? System.DBNull.Value));
        }

        public static int ContarBitacoraPorInformacion(string actividad, string informacion)
        {
            return Contar(
                "SELECT COUNT(*) FROM dbo.Bitacora WHERE Actividad = @actividad AND Informacion = @informacion",
                new SqlParameter("@actividad", actividad),
                new SqlParameter("@informacion", informacion));
        }

        public static string ObtenerDvv(string nombreTabla)
        {
            return EscalarOrNull(
                "SELECT Valor FROM dbo.DigitoVerificadorVertical WHERE NombreTabla = @tabla",
                new SqlParameter("@tabla", nombreTabla)) as string;
        }

        /// <summary>
        /// Escritura directa a la BD de test. Uso restringido: SOLO para la corrupcion
        /// intencional de firmas de T07 (y su restauracion). El resto de las escrituras
        /// de negocio deben ir por la UI para no romper DVH/DVV.
        /// </summary>
        public static int EjecutarSql(string sql, params SqlParameter[] parametros)
        {
            using (SqlConnection connection = new SqlConnection(TestConfig.ConnectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                foreach (SqlParameter parametro in parametros)
                {
                    command.Parameters.Add(parametro);
                }

                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Traduccion de una clave en el idioma default (Español, seed de script_bd.sql).
        /// Si la clave no esta traducida devuelve la clave, igual que UiDialogHelper.
        /// </summary>
        public static string ObtenerTraduccion(string key)
        {
            string valor = EscalarOrNull(
                "SELECT t.Valor FROM dbo.Traduccion t " +
                "JOIN dbo.Palabra p ON p.Id = t.PalabraId " +
                "JOIN dbo.Idioma i ON i.Id = t.IdiomaId " +
                "WHERE p.[Key] = @key AND i.[Default] = 1",
                new SqlParameter("@key", key)) as string;

            return string.IsNullOrWhiteSpace(valor) ? key : valor;
        }
    }
}
