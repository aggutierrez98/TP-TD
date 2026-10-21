using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SERVICIOS;
using ABSTRACCIONES;

namespace DAL
{
    public class UsuarioPreferenciaDAL
    {
        public UsuarioPreferencia ObtenerPorUsuario(IUsuarioAutenticado usuario, AccesoDatosSqlServer accesoDatos = null)
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
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, usuario.Id)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                if (dataTable.Rows.Count == 0)
                {
                    return null;
                }

                DataRow row = dataTable.Rows[0];
                return new UsuarioPreferencia
                {
                    Usuario = usuario,
                    Idioma = new IdiomaDAL().ObtenerPorId((int)row["IdiomaId"], accesoDatos)
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
            return Guardar(preferencia?.Usuario, preferencia?.Idioma, accesoDatos);
        }

        public bool Guardar(IUsuarioAutenticado usuario, IIdioma idioma, AccesoDatosSqlServer accesoDatos = null)
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
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, usuario?.Id ?? 0),
                    accesoDatos.CreateParameter("@IdiomaId", SqlDbType.Int, idioma?.Id)
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
