using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ABSTRACCIONES;
using BE;
using SERVICIOS;

namespace DAL
{
    public class MovimientoDAL
    {
        private Movimiento Map(DataRow row)
        {
            return new Movimiento
            {
                Id = (int)row["Id"],
                Usuario = new Usuario { Id = (int)row["UsuarioId"] },
                Categoria = new Categoria { Id = (int)row["CategoriaId"] },
                Tipo = (TipoMovimiento)Enum.Parse(typeof(TipoMovimiento), row["Tipo"].ToString()),
                Monto = (decimal)row["Monto"],
                Fecha = (DateTime)row["Fecha"],
                Descripcion = row.IsNull("Descripcion") ? null : row["Descripcion"].ToString(),
                FechaCreacion = (DateTime)row["FechaCreacion"]
            };
        }

        public Movimiento ObtenerPorId(int id, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, UsuarioId, CategoriaId, Tipo, Monto, Fecha, Descripcion, FechaCreacion
                FROM Movimiento
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

        public List<Movimiento> ObtenerPorUsuario(int usuarioId, FiltroMovimiento filtro, AccesoDatosSqlServer accesoDatos = null)
        {
            filtro = filtro ?? new FiltroMovimiento();

            string query = @"
                SELECT Id, UsuarioId, CategoriaId, Tipo, Monto, Fecha, Descripcion, FechaCreacion
                FROM Movimiento
                WHERE UsuarioId = @UsuarioId
                  AND (@FechaDesde IS NULL OR Fecha >= @FechaDesde)
                  AND (@FechaHasta IS NULL OR Fecha <= @FechaHasta)
                  AND (@CategoriaId IS NULL OR CategoriaId = @CategoriaId)
                  AND (@Tipo IS NULL OR Tipo = @Tipo)
                ORDER BY Fecha DESC, Id DESC";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, usuarioId),
                    accesoDatos.CreateParameter("@FechaDesde", SqlDbType.Date, filtro.FechaDesde),
                    accesoDatos.CreateParameter("@FechaHasta", SqlDbType.Date, filtro.FechaHasta),
                    accesoDatos.CreateParameter("@CategoriaId", SqlDbType.Int, filtro.CategoriaId),
                    accesoDatos.CreateParameter("@Tipo", SqlDbType.VarChar, filtro.Tipo.HasValue ? filtro.Tipo.Value.ToString() : null)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                List<Movimiento> movimientos = new List<Movimiento>();
                foreach (DataRow row in dataTable.Rows)
                {
                    movimientos.Add(Map(row));
                }

                return movimientos;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public int InsertarYObtenerId(Movimiento movimiento, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                INSERT INTO Movimiento (UsuarioId, CategoriaId, Tipo, Monto, Fecha, Descripcion)
                OUTPUT INSERTED.Id
                VALUES (@UsuarioId, @CategoriaId, @Tipo, @Monto, @Fecha, @Descripcion)";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, movimiento.Usuario.Id),
                    accesoDatos.CreateParameter("@CategoriaId", SqlDbType.Int, movimiento.Categoria.Id),
                    accesoDatos.CreateParameter("@Tipo", SqlDbType.VarChar, movimiento.Tipo.ToString()),
                    accesoDatos.CreateParameter("@Monto", SqlDbType.Decimal, movimiento.Monto),
                    accesoDatos.CreateParameter("@Fecha", SqlDbType.Date, movimiento.Fecha),
                    accesoDatos.CreateParameter("@Descripcion", SqlDbType.VarChar, movimiento.Descripcion)
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

        public bool Actualizar(Movimiento movimiento, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                UPDATE Movimiento
                SET CategoriaId = @CategoriaId,
                    Tipo = @Tipo,
                    Monto = @Monto,
                    Fecha = @Fecha,
                    Descripcion = @Descripcion
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@CategoriaId", SqlDbType.Int, movimiento.Categoria.Id),
                    accesoDatos.CreateParameter("@Tipo", SqlDbType.VarChar, movimiento.Tipo.ToString()),
                    accesoDatos.CreateParameter("@Monto", SqlDbType.Decimal, movimiento.Monto),
                    accesoDatos.CreateParameter("@Fecha", SqlDbType.Date, movimiento.Fecha),
                    accesoDatos.CreateParameter("@Descripcion", SqlDbType.VarChar, movimiento.Descripcion),
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, movimiento.Id)
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

        public bool Eliminar(int id, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                DELETE FROM Movimiento
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, id)
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
