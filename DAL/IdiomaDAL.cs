using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using SERVICIOS;

namespace DAL
{
    public class IdiomaDAL
    {
        public List<Idioma> ObtenerIdiomas(AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = "SELECT Id, Nombre, [Default] FROM Idioma ORDER BY Nombre";

            return Ejecutar(accesoDatos, contexto =>
            {
                DataTable dataTable = contexto.Read(query);
                return dataTable.Rows.Cast<DataRow>()
                    .Select(MapIdiomaSimple)
                    .ToList();
            });
        }

        public Idioma ObtenerPorId(int id, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT i.Id, i.Nombre, i.[Default], p.Id AS PalabraId, p.[Key], t.Valor
                FROM Idioma i
                INNER JOIN Palabra p ON 1 = 1
                LEFT JOIN Traduccion t ON t.IdiomaId = i.Id AND t.PalabraId = p.Id
                WHERE i.Id = @IdiomaId
                ORDER BY [Key]";

            return Ejecutar(accesoDatos, contexto =>
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    contexto.CreateParameter("@IdiomaId", SqlDbType.Int, id)
                };

                DataTable dataTable = contexto.Read(query, parametros);
                if (dataTable.Rows.Count == 0)
                {
                    const string queryIdioma = "SELECT Id, Nombre, [Default] FROM Idioma WHERE Id = @IdiomaId";
                    DataTable dataTableIdioma = contexto.Read(queryIdioma, parametros);
                    if (dataTableIdioma.Rows.Count == 0)
                    {
                        return null;
                    }

                    DataRow rowIdioma = dataTableIdioma.Rows[0];
                    return new Idioma
                    {
                        Id = Convert.ToInt32(rowIdioma["Id"]),
                        Nombre = rowIdioma["Nombre"].ToString(),
                        Default = Convert.ToBoolean(rowIdioma["Default"])
                    };
                }

                DataRow primeraFila = dataTable.Rows[0];
                Idioma idioma = new Idioma
                {
                    Id = Convert.ToInt32(primeraFila["Id"]),
                    Nombre = primeraFila["Nombre"].ToString(),
                    Default = Convert.ToBoolean(primeraFila["Default"])
                };

                foreach (DataRow row in dataTable.Rows)
                {
                    if (row["PalabraId"] == DBNull.Value)
                    {
                        continue;
                    }

                    idioma.Traducciones.Add(new Traduccion
                    {
                        Idioma = idioma,
                        Valor = row["Valor"].ToString(),
                        Palabra = new Palabra
                        {
                            Id = Convert.ToInt32(row["PalabraId"]),
                            Key = row["Key"].ToString()
                        }
                    });
                }

                return idioma;
            });
        }

        public Idioma ObtenerIdiomaDefault(AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = "SELECT TOP 1 Id FROM Idioma WHERE [Default] = 1 ORDER BY Id";

            return Ejecutar(accesoDatos, contexto =>
            {
                DataTable dataTable = contexto.Read(query);
                if (dataTable.Rows.Count == 0)
                {
                    return null;
                }

                int idiomaId = Convert.ToInt32(dataTable.Rows[0]["Id"]);
                return ObtenerPorId(idiomaId, contexto);
            });
        }

        public int InsertarIdioma(Idioma idioma, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                INSERT INTO Idioma (Nombre, [Default])
                OUTPUT INSERTED.Id
                VALUES (@Nombre, @Default)";

            return Ejecutar(accesoDatos, contexto =>
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    contexto.CreateParameter("@Nombre", SqlDbType.VarChar, idioma.Nombre),
                    contexto.CreateParameter("@Default", SqlDbType.Bit, idioma.Default)
                };

                return contexto.WriteWithResult(query, parametros);
            });
        }

        public bool ActualizarIdioma(Idioma idioma, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = "UPDATE Idioma SET Nombre = @Nombre WHERE Id = @Id";

            return Ejecutar(accesoDatos, contexto =>
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    contexto.CreateParameter("@Id", SqlDbType.Int, idioma.Id),
                    contexto.CreateParameter("@Nombre", SqlDbType.VarChar, idioma.Nombre)
                };

                return contexto.Write(query, parametros) > 0;
            });
        }

        public bool EliminarIdioma(Idioma idioma, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                DELETE FROM UsuarioPreferencias WHERE IdiomaId = @IdiomaId;
                DELETE FROM Traduccion WHERE IdiomaId = @IdiomaId;
                DELETE FROM Idioma WHERE Id = @IdiomaId;";

            return Ejecutar(accesoDatos, contexto =>
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    contexto.CreateParameter("@IdiomaId", SqlDbType.Int, idioma.Id)
                };

                return contexto.Write(query, parametros) > 0;
            });
        }

        public bool ConsolidarIdioma(Idioma origen, Idioma destino, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                UPDATE UsuarioPreferencias
                SET IdiomaId = @IdiomaDestinoId
                WHERE IdiomaId = @IdiomaOrigenId;

                INSERT INTO Traduccion (IdiomaId, PalabraId, Valor)
                SELECT @IdiomaDestinoId, t.PalabraId, t.Valor
                FROM Traduccion t
                WHERE t.IdiomaId = @IdiomaOrigenId
                  AND NOT EXISTS (
                      SELECT 1
                      FROM Traduccion td
                      WHERE td.IdiomaId = @IdiomaDestinoId
                        AND td.PalabraId = t.PalabraId);

                DELETE FROM Traduccion
                WHERE IdiomaId = @IdiomaOrigenId;

                DELETE FROM Idioma
                WHERE Id = @IdiomaOrigenId;";

            return Ejecutar(accesoDatos, contexto =>
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    contexto.CreateParameter("@IdiomaOrigenId", SqlDbType.Int, origen.Id),
                    contexto.CreateParameter("@IdiomaDestinoId", SqlDbType.Int, destino.Id)
                };

                return contexto.Write(query, parametros) >= 0;
            });
        }

        public Palabra ObtenerPalabraPorKey(string key, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = "SELECT Id, [Key] FROM Palabra WHERE [Key] = @Key";

            return Ejecutar(accesoDatos, contexto =>
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    contexto.CreateParameter("@Key", SqlDbType.VarChar, key)
                };

                DataTable dataTable = contexto.Read(query, parametros);
                return dataTable.Rows.Count == 0 ? null : new Palabra { Id = Convert.ToInt32(dataTable.Rows[0]["Id"]), Key = dataTable.Rows[0]["Key"].ToString() };
            });
        }

        public bool GuardarTraduccion(Traduccion traduccion, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                IF EXISTS (
                    SELECT 1
                    FROM Traduccion
                    WHERE IdiomaId = @IdiomaId
                      AND PalabraId = @PalabraId)
                BEGIN
                    UPDATE Traduccion
                    SET Valor = @Valor
                    WHERE IdiomaId = @IdiomaId
                      AND PalabraId = @PalabraId;
                END
                ELSE
                BEGIN
                    INSERT INTO Traduccion (IdiomaId, PalabraId, Valor)
                    VALUES (@IdiomaId, @PalabraId, @Valor);
                END";

            return Ejecutar(accesoDatos, contexto =>
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    contexto.CreateParameter("@IdiomaId", SqlDbType.Int, traduccion.Idioma.Id),
                    contexto.CreateParameter("@PalabraId", SqlDbType.Int, traduccion.Palabra.Id),
                    contexto.CreateParameter("@Valor", SqlDbType.VarChar, traduccion.Valor)
                };

                return contexto.Write(query, parametros) > 0;
            });
        }

        public bool EliminarTraduccion(Traduccion traduccion, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                DELETE FROM Traduccion
                WHERE IdiomaId = @IdiomaId
                  AND PalabraId = @PalabraId";

            return Ejecutar(accesoDatos, contexto =>
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    contexto.CreateParameter("@IdiomaId", SqlDbType.Int, traduccion.Idioma.Id),
                    contexto.CreateParameter("@PalabraId", SqlDbType.Int, traduccion.Palabra.Id)
                };

                return contexto.Write(query, parametros) > 0;
            });
        }

        public bool SwitchearIdiomaDefault(Idioma idioma, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                UPDATE Idioma
                SET [Default] = CASE 
                    WHEN Id = @IdiomaId 
                        THEN 1 ELSE 0 
                END;
            ";

            return Ejecutar(accesoDatos, contexto =>
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    contexto.CreateParameter("@IdiomaId", SqlDbType.Int, idioma.Id)
                };

                return contexto.Write(query, parametros) > 0;
            });
        }

        private static T Ejecutar<T>(AccesoDatosSqlServer accesoDatos, Func<AccesoDatosSqlServer, T> operacion)
        {
            bool dispose = accesoDatos == null;
            AccesoDatosSqlServer contexto = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                return operacion(contexto);
            }
            finally
            {
                if (dispose)
                {
                    contexto.Dispose();
                }
            }
        }

        private static Idioma MapIdiomaSimple(DataRow row)
        {
            return new Idioma
            {
                Id = Convert.ToInt32(row["Id"]),
                Nombre = row["Nombre"].ToString(),
                Default = Convert.ToBoolean(row["Default"])
            };
        }
    }
}
