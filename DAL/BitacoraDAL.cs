using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using SERVICIOS;

namespace DAL
{
    public class BitacoraDAL
    {
        public int Guardar(EventoBitacora evento, AccesoDatos accesoDatos = null)
        {
            const string query = @"
                INSERT INTO Bitacora
                (
                    FechaHora,
                    UsuarioId,
                    UsernameSnapshot,
                    Actividad,
                    Informacion,
                    Nivel,
                    Origen,
                    EsTecnica
                )
                OUTPUT INSERTED.Id
                VALUES
                (
                    @FechaHora,
                    @UsuarioId,
                    @UsernameSnapshot,
                    @Actividad,
                    @Informacion,
                    @Nivel,
                    @Origen,
                    @EsTecnica
                )";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatos();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@FechaHora", SqlDbType.DateTime, evento.FechaHora),
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, evento.UsuarioId),
                    accesoDatos.CreateParameter("@UsernameSnapshot", SqlDbType.VarChar, evento.UsernameSnapshot),
                    accesoDatos.CreateParameter("@Actividad", SqlDbType.VarChar, evento.Actividad),
                    accesoDatos.CreateParameter("@Informacion", SqlDbType.VarChar, evento.Informacion),
                    accesoDatos.CreateParameter("@Nivel", SqlDbType.VarChar, evento.Nivel),
                    accesoDatos.CreateParameter("@Origen", SqlDbType.VarChar, evento.Origen),
                    accesoDatos.CreateParameter("@EsTecnica", SqlDbType.Bit, evento.EsTecnica)
                };

                return accesoDatos.WriteWithResult(query, parametros);
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public List<EventoBitacora> Buscar(FiltroBitacora filtro)
        {
            filtro = filtro ?? new FiltroBitacora();
            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();
                string query = @"
                    SELECT Id, FechaHora, UsuarioId, UsernameSnapshot, Actividad, Informacion, Nivel, Origen, EsTecnica
                    FROM Bitacora
                    WHERE 1 = 1";

                if (filtro.FechaDesde.HasValue)
                {
                    query += " AND FechaHora >= @FechaDesde";
                    parametros.Add(accesoDatos.CreateParameter("@FechaDesde", SqlDbType.DateTime, filtro.FechaDesde.Value));
                }

                if (filtro.FechaHasta.HasValue)
                {
                    query += " AND FechaHora <= @FechaHasta";
                    parametros.Add(accesoDatos.CreateParameter("@FechaHasta", SqlDbType.DateTime, filtro.FechaHasta.Value));
                }

                if (!string.IsNullOrWhiteSpace(filtro.Usuario))
                {
                    query += " AND UsernameSnapshot LIKE @Usuario";
                    parametros.Add(accesoDatos.CreateParameter("@Usuario", SqlDbType.VarChar, "%" + filtro.Usuario.Trim() + "%"));
                }

                if (!string.IsNullOrWhiteSpace(filtro.Actividad))
                {
                    query += " AND Actividad LIKE @Actividad";
                    parametros.Add(accesoDatos.CreateParameter("@Actividad", SqlDbType.VarChar, "%" + filtro.Actividad.Trim() + "%"));
                }

                if (!string.IsNullOrWhiteSpace(filtro.Nivel))
                {
                    query += " AND Nivel = @Nivel";
                    parametros.Add(accesoDatos.CreateParameter("@Nivel", SqlDbType.VarChar, filtro.Nivel.Trim()));
                }

                if (filtro.EsTecnica.HasValue)
                {
                    query += " AND EsTecnica = @EsTecnica";
                    parametros.Add(accesoDatos.CreateParameter("@EsTecnica", SqlDbType.Bit, filtro.EsTecnica.Value));
                }

                query += " ORDER BY FechaHora DESC, Id DESC";

                DataTable dataTable = accesoDatos.Read(query, parametros);
                return dataTable.Rows.Cast<DataRow>()
                    .Select(Map)
                    .ToList();
            }
        }

        private static EventoBitacora Map(DataRow row)
        {
            return new EventoBitacora
            {
                Id = (int)row["Id"],
                FechaHora = (DateTime)row["FechaHora"],
                UsuarioId = row["UsuarioId"] == DBNull.Value ? (int?)null : (int)row["UsuarioId"],
                UsernameSnapshot = row["UsernameSnapshot"].ToString(),
                Actividad = row["Actividad"].ToString(),
                Informacion = row["Informacion"].ToString(),
                Nivel = row["Nivel"].ToString(),
                Origen = row["Origen"].ToString(),
                EsTecnica = (bool)row["EsTecnica"]
            };
        }
    }
}
