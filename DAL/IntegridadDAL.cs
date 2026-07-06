using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    // Acceso a la tabla de control de digitos verificadores verticales (DVV).
    public class IntegridadDAL
    {
        // Obtiene el DVV almacenado para una tabla, o null si nunca fue inicializado.
        public string ObtenerDVV(string nombreTabla, AccesoDatos accesoDatos = null)
        {
            const string query = @"
                SELECT Valor
                FROM DigitoVerificadorVertical
                WHERE NombreTabla = @NombreTabla";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatos();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@NombreTabla", SqlDbType.VarChar, nombreTabla)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                return dataTable.Rows.Count == 0 ? null : dataTable.Rows[0]["Valor"].ToString();
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        // Inserta o actualiza (UPSERT) el DVV de una tabla, registrando la fecha.
        public void GuardarDVV(string nombreTabla, string valor, AccesoDatos accesoDatos = null)
        {
            const string query = @"
                UPDATE DigitoVerificadorVertical
                SET Valor = @Valor,
                    FechaActualizacion = GETDATE()
                WHERE NombreTabla = @NombreTabla;

                IF @@ROWCOUNT = 0
                    INSERT INTO DigitoVerificadorVertical (NombreTabla, Valor, FechaActualizacion)
                    VALUES (@NombreTabla, @Valor, GETDATE());";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatos();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@NombreTabla", SqlDbType.VarChar, nombreTabla),
                    accesoDatos.CreateParameter("@Valor", SqlDbType.VarChar, valor)
                };

                accesoDatos.Write(query, parametros);
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
