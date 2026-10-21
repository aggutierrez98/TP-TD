using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using BE;

namespace DAL
{
    // Tabla puente Promocion <-> DiaSemana. DiaSemana es un enum con CHECK en SQL (no una
    // tabla propia). Sin filas para una promocion = aplica todos los dias.
    public class PromocionDiaSemanaDAL
    {
        public List<DiaSemana> ObtenerDiasPorPromocion(Promocion promocion, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT DiaSemana
                FROM PromocionDiaSemana
                WHERE PromocionId = @PromocionId";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@PromocionId", SqlDbType.Int, promocion.Id)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                return dataTable.Rows.Cast<DataRow>()
                    .Select(row => (DiaSemana)Enum.Parse(typeof(DiaSemana), row["DiaSemana"].ToString()))
                    .OrderBy(dia => dia)
                    .ToList();
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public bool ReemplazarDiasDePromocion(Promocion promocion, List<DiaSemana> dias, AccesoDatosSqlServer accesoDatos = null)
        {
            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@PromocionId", SqlDbType.Int, promocion.Id)
                };

                string query = @"
                    DELETE FROM PromocionDiaSemana
                    WHERE PromocionId = @PromocionId;";

                List<DiaSemana> diasDistintos = dias.Distinct().ToList();
                for (int i = 0; i < diasDistintos.Count; i++)
                {
                    query += string.Format(@"
                        INSERT INTO PromocionDiaSemana (PromocionId, DiaSemana)
                        VALUES (@PromocionId, @DiaSemana{0});", i);

                    parametros.Add(accesoDatos.CreateParameter("@DiaSemana" + i, SqlDbType.VarChar, diasDistintos[i].ToString()));
                }

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
