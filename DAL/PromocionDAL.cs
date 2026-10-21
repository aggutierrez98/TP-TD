using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class PromocionDAL
    {
        private Promocion Map(DataRow row)
        {
            return new Promocion
            {
                Id = (int)row["Id"],
                Descripcion = row["Descripcion"].ToString(),
                Descuento = (decimal)row["Descuento"],
                TopeDescuento = row.IsNull("TopeDescuento") ? (decimal?)null : (decimal)row["TopeDescuento"],
                VigenciaDesde = (System.DateTime)row["VigenciaDesde"],
                VigenciaHasta = (System.DateTime)row["VigenciaHasta"]
            };
        }

        public Promocion ObtenerPorId(int id, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, Descripcion, Descuento, TopeDescuento, VigenciaDesde, VigenciaHasta
                FROM Promocion
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, id)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                return dataTable.Rows.Count == 0 ? null : Map(dataTable.Rows[0]);
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public List<Promocion> ObtenerTodas(AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, Descripcion, Descuento, TopeDescuento, VigenciaDesde, VigenciaHasta
                FROM Promocion
                ORDER BY VigenciaDesde DESC";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                DataTable dataTable = accesoDatos.Read(query);
                List<Promocion> promociones = new List<Promocion>();
                foreach (DataRow row in dataTable.Rows)
                {
                    promociones.Add(Map(row));
                }

                return promociones;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public int InsertarYObtenerId(Promocion promocion, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                INSERT INTO Promocion (Descripcion, Descuento, TopeDescuento, VigenciaDesde, VigenciaHasta)
                OUTPUT INSERTED.Id
                VALUES (@Descripcion, @Descuento, @TopeDescuento, @VigenciaDesde, @VigenciaHasta)";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Descripcion", SqlDbType.VarChar, promocion.Descripcion),
                    accesoDatos.CreateParameter("@Descuento", SqlDbType.Decimal, promocion.Descuento),
                    accesoDatos.CreateParameter("@TopeDescuento", SqlDbType.Decimal, promocion.TopeDescuento),
                    accesoDatos.CreateParameter("@VigenciaDesde", SqlDbType.Date, promocion.VigenciaDesde),
                    accesoDatos.CreateParameter("@VigenciaHasta", SqlDbType.Date, promocion.VigenciaHasta)
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

        public bool Actualizar(Promocion promocion, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                UPDATE Promocion
                SET Descripcion = @Descripcion,
                    Descuento = @Descuento,
                    TopeDescuento = @TopeDescuento,
                    VigenciaDesde = @VigenciaDesde,
                    VigenciaHasta = @VigenciaHasta
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Descripcion", SqlDbType.VarChar, promocion.Descripcion),
                    accesoDatos.CreateParameter("@Descuento", SqlDbType.Decimal, promocion.Descuento),
                    accesoDatos.CreateParameter("@TopeDescuento", SqlDbType.Decimal, promocion.TopeDescuento),
                    accesoDatos.CreateParameter("@VigenciaDesde", SqlDbType.Date, promocion.VigenciaDesde),
                    accesoDatos.CreateParameter("@VigenciaHasta", SqlDbType.Date, promocion.VigenciaHasta),
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, promocion.Id)
                };

                return accesoDatos.Write(query, parametros) > 0;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public bool Eliminar(Promocion promocion, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                DELETE FROM Promocion
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, promocion.Id)
                };

                return accesoDatos.Write(query, parametros) > 0;
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
