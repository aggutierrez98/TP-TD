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
                Comercio = row.Table.Columns.Contains("ComercioId") && !row.IsNull("ComercioId")
                    ? new Comercio { Id = (int)row["ComercioId"] }
                    : null,
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
                SELECT Id, UsuarioId, CategoriaId, ComercioId, Tipo, Monto, Fecha, Descripcion, FechaCreacion
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
                SELECT Id, UsuarioId, CategoriaId, ComercioId, Tipo, Monto, Fecha, Descripcion, FechaCreacion
                FROM Movimiento
                WHERE UsuarioId = @UsuarioId
                  AND (@FechaDesde IS NULL OR Fecha >= @FechaDesde)
                  AND (@FechaHasta IS NULL OR Fecha <= @FechaHasta)
                  AND (@CategoriaId IS NULL OR CategoriaId = @CategoriaId)
                  AND (@Tipo IS NULL OR Tipo = @Tipo)
                  AND (@Texto IS NULL OR Descripcion LIKE '%' + @Texto + '%')
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
                    accesoDatos.CreateParameter("@Tipo", SqlDbType.VarChar, filtro.Tipo.HasValue ? filtro.Tipo.Value.ToString() : null),
                    accesoDatos.CreateParameter("@Texto", SqlDbType.VarChar, filtro.Texto)
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

        public List<TotalPorCategoria> ObtenerTotalesPorCategoria(int usuarioId, TipoMovimiento tipo, DateTime desde, DateTime hasta, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT c.Nombre AS CategoriaNombre, SUM(m.Monto) AS Total
                FROM Movimiento m JOIN Categoria c ON c.Id = m.CategoriaId
                WHERE m.UsuarioId=@UsuarioId AND m.Tipo=@Tipo AND m.Fecha BETWEEN @Desde AND @Hasta
                GROUP BY c.Nombre";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, usuarioId),
                    accesoDatos.CreateParameter("@Tipo", SqlDbType.VarChar, tipo.ToString()),
                    accesoDatos.CreateParameter("@Desde", SqlDbType.Date, desde),
                    accesoDatos.CreateParameter("@Hasta", SqlDbType.Date, hasta)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                List<TotalPorCategoria> totales = new List<TotalPorCategoria>();
                foreach (DataRow row in dataTable.Rows)
                {
                    totales.Add(new TotalPorCategoria
                    {
                        CategoriaNombre = row["CategoriaNombre"].ToString(),
                        Total = (decimal)row["Total"]
                    });
                }

                return totales;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public List<TotalPorMes> ObtenerTotalesPorMes(int usuarioId, DateTime desde, DateTime hasta, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT YEAR(Fecha) AS Anio, MONTH(Fecha) AS Mes,
                       SUM(CASE WHEN Tipo='Ingreso' THEN Monto ELSE 0 END) AS TotalIngresos,
                       SUM(CASE WHEN Tipo='Egreso' THEN Monto ELSE 0 END) AS TotalEgresos
                FROM Movimiento WHERE UsuarioId=@UsuarioId AND Fecha BETWEEN @Desde AND @Hasta
                GROUP BY YEAR(Fecha), MONTH(Fecha) ORDER BY YEAR(Fecha), MONTH(Fecha)";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, usuarioId),
                    accesoDatos.CreateParameter("@Desde", SqlDbType.Date, desde),
                    accesoDatos.CreateParameter("@Hasta", SqlDbType.Date, hasta)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                List<TotalPorMes> totales = new List<TotalPorMes>();
                foreach (DataRow row in dataTable.Rows)
                {
                    totales.Add(new TotalPorMes
                    {
                        Anio = (int)row["Anio"],
                        Mes = (int)row["Mes"],
                        TotalIngresos = (decimal)row["TotalIngresos"],
                        TotalEgresos = (decimal)row["TotalEgresos"]
                    });
                }

                return totales;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public ResumenMovimientos ObtenerResumen(int usuarioId, DateTime desde, DateTime hasta, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT
                    SUM(CASE WHEN Tipo='Ingreso' THEN Monto ELSE 0 END) AS TotalIngresos,
                    SUM(CASE WHEN Tipo='Egreso' THEN Monto ELSE 0 END) AS TotalEgresos
                FROM Movimiento WHERE UsuarioId=@UsuarioId AND Fecha BETWEEN @Desde AND @Hasta";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, usuarioId),
                    accesoDatos.CreateParameter("@Desde", SqlDbType.Date, desde),
                    accesoDatos.CreateParameter("@Hasta", SqlDbType.Date, hasta)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                if (dataTable.Rows.Count == 0 || dataTable.Rows[0].IsNull("TotalIngresos"))
                {
                    return new ResumenMovimientos();
                }

                DataRow row = dataTable.Rows[0];
                return new ResumenMovimientos
                {
                    TotalIngresos = (decimal)row["TotalIngresos"],
                    TotalEgresos = (decimal)row["TotalEgresos"]
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

        public int InsertarYObtenerId(Movimiento movimiento, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                INSERT INTO Movimiento (UsuarioId, CategoriaId, ComercioId, Tipo, Monto, Fecha, Descripcion)
                OUTPUT INSERTED.Id
                VALUES (@UsuarioId, @CategoriaId, @ComercioId, @Tipo, @Monto, @Fecha, @Descripcion)";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, movimiento.Usuario.Id),
                    accesoDatos.CreateParameter("@CategoriaId", SqlDbType.Int, movimiento.Categoria.Id),
                    accesoDatos.CreateParameter("@ComercioId", SqlDbType.Int, movimiento.Comercio != null && movimiento.Comercio.Id > 0 ? (object)movimiento.Comercio.Id : null),
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
                    ComercioId = @ComercioId,
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
                    accesoDatos.CreateParameter("@ComercioId", SqlDbType.Int, movimiento.Comercio != null && movimiento.Comercio.Id > 0 ? (object)movimiento.Comercio.Id : null),
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
