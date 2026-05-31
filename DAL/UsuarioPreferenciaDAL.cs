using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ABSTRACCIONES;
using SERVICIOS;

namespace DAL
{
    public class UsuarioPreferenciaDAL : IUsuarioPreferenciaRepository
    {
        public IUsuarioPreferencia ObtenerPorUsuarioId(int usuarioId)
        {
            const string query = @"
SELECT UsuarioId, IdiomaId
FROM UsuarioPreferencias
WHERE UsuarioId = @UsuarioId";

            using (AccesoDatos accesoDatos = new AccesoDatos())
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
        }

        public bool Guardar(IUsuarioPreferencia preferencia)
        {
            const string query = @"
IF EXISTS (
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

            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, preferencia.UsuarioId),
                    accesoDatos.CreateParameter("@IdiomaId", SqlDbType.Int, preferencia.IdiomaId)
                };

                return accesoDatos.Write(query, parametros) > 0;
            }
        }
    }
}
