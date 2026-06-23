using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SERVICIOS;

namespace DAL
{
    public class UsuarioPreferenciaDAL
    {
        public UsuarioPreferencia ObtenerPorUsuarioId(int usuarioId, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT UsuarioId, IdiomaId
                FROM UsuarioPreferencias
                WHERE UsuarioId = @UsuarioId";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, usuarioId)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                if (dataTable.Rows.Count == 0)
                {
                    return null;
                }

                DataRow row = dataTable.Rows[0];
                return new UsuarioPreferencia
                {
                    UsuarioId = (int)row["UsuarioId"],
                    IdiomaId = (int)row["IdiomaId"]
                };
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public bool Guardar(UsuarioPreferencia preferencia, AccesoDatosSqlServer accesoDatos = null)
        {
            return Guardar(preferencia == null ? 0 : preferencia.UsuarioId, preferencia == null ? (int?)null : preferencia.IdiomaId, accesoDatos);
        }

        public bool Guardar(int usuarioId, int? idiomaId, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                IF @IdiomaId IS NULL
                BEGIN
                    DELETE FROM UsuarioPreferencias
                    WHERE UsuarioId = @UsuarioId;
                END
                ELSE IF EXISTS (
                    SELECT 1
                    FROM UsuarioPreferencias
                    WHERE UsuarioId = @UsuarioId)
                    BEGIN
                        UPDATE UsuarioPreferencias
                        SET IdiomaId = @IdiomaId
                        WHERE UsuarioId = @UsuarioId;
                    END
                ELSE
                BEGIN
                    INSERT INTO UsuarioPreferencias (UsuarioId, IdiomaId)
                    VALUES (@UsuarioId, @IdiomaId);
                END";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, usuarioId),
                    accesoDatos.CreateParameter("@IdiomaId", SqlDbType.Int, idiomaId)
                };

                return accesoDatos.Write(query, parametros) >= 0;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }
    }
}
