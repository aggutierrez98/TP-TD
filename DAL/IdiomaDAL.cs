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
        public List<Idioma> ObtenerIdiomas()
        {
            const string query = "SELECT Id, Nombre, [Default] FROM Idioma ORDER BY Nombre";

            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                DataTable dataTable = accesoDatos.Read(query);
                return dataTable.Rows.Cast<DataRow>()
                    .Select(MapIdiomaSimple)
                    .ToList();
            }
        }

        public List<Palabra> ObtenerPalabras()
        {
            const string query = "SELECT Id, [Key], EsBase FROM Palabra ORDER BY [Key]";

            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                DataTable dataTable = accesoDatos.Read(query);
                return dataTable.Rows.Cast<DataRow>()
                    .Select(MapPalabra)
                    .ToList();
            }
        }

        public Idioma ObtenerIdiomaCompleto(int idiomaId)
        {
            const string query = @"
SELECT i.Id, i.Nombre, i.[Default], p.Id AS PalabraId, p.[Key], p.EsBase, t.Valor
FROM Idioma i
INNER JOIN Palabra p ON p.EsBase = 1
LEFT JOIN Traduccion t ON t.IdiomaId = i.Id AND t.PalabraId = p.Id
WHERE i.Id = @IdiomaId

UNION ALL

SELECT i.Id, i.Nombre, i.[Default], p.Id AS PalabraId, p.[Key], p.EsBase, t.Valor
FROM Idioma i
INNER JOIN Traduccion t ON t.IdiomaId = i.Id
INNER JOIN Palabra p ON p.Id = t.PalabraId
WHERE i.Id = @IdiomaId
  AND p.EsBase = 0
ORDER BY [Key]";

            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@IdiomaId", SqlDbType.Int, idiomaId)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                if (dataTable.Rows.Count == 0)
                {
                    const string queryIdioma = "SELECT Id, Nombre, [Default] FROM Idioma WHERE Id = @IdiomaId";
                    DataTable dataTableIdioma = accesoDatos.Read(queryIdioma, parametros);
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
                        IdiomaId = idioma.Id,
                        PalabraId = Convert.ToInt32(row["PalabraId"]),
                        Valor = row["Valor"].ToString(),
                        Palabra = new Palabra
                        {
                            Id = Convert.ToInt32(row["PalabraId"]),
                            Key = row["Key"].ToString(),
                            EsBase = Convert.ToBoolean(row["EsBase"])
                        }
                    });
                }

                return idioma;
            }
        }

        public Idioma ObtenerIdiomaDefault()
        {
            const string query = "SELECT TOP 1 Id FROM Idioma WHERE [Default] = 1 ORDER BY Id";

            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                DataTable dataTable = accesoDatos.Read(query);
                if (dataTable.Rows.Count == 0)
                {
                    return null;
                }

                int idiomaId = Convert.ToInt32(dataTable.Rows[0]["Id"]);
                return ObtenerIdiomaCompleto(idiomaId);
            }
        }

        public int InsertarIdioma(Idioma idioma)
        {
            const string query = @"
INSERT INTO Idioma (Nombre, [Default])
OUTPUT INSERTED.Id
VALUES (@Nombre, @Default)";

            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Nombre", SqlDbType.VarChar, idioma.Nombre),
                    accesoDatos.CreateParameter("@Default", SqlDbType.Bit, idioma.Default)
                };

                return accesoDatos.WriteWithResult(query, parametros);
            }
        }

        public bool ActualizarIdioma(Idioma idioma)
        {
            const string query = "UPDATE Idioma SET Nombre = @Nombre WHERE Id = @Id";

            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, idioma.Id),
                    accesoDatos.CreateParameter("@Nombre", SqlDbType.VarChar, idioma.Nombre)
                };

                return accesoDatos.Write(query, parametros) > 0;
            }
        }

        public bool EliminarIdioma(int idiomaId)
        {
            const string query = @"
DELETE FROM UsuarioPreferencias WHERE IdiomaId = @IdiomaId;
DELETE FROM Traduccion WHERE IdiomaId = @IdiomaId;
DELETE FROM Idioma WHERE Id = @IdiomaId;";

            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@IdiomaId", SqlDbType.Int, idiomaId)
                };

                return accesoDatos.Write(query, parametros) > 0;
            }
        }

        public bool ConsolidarIdioma(int idiomaOrigenId, int idiomaDestinoId)
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

            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@IdiomaOrigenId", SqlDbType.Int, idiomaOrigenId),
                    accesoDatos.CreateParameter("@IdiomaDestinoId", SqlDbType.Int, idiomaDestinoId)
                };

                return accesoDatos.Write(query, parametros) >= 0;
            }
        }

        public int ObtenerOCrearPalabra(string key, bool esBase)
        {
            const string query = @"
DECLARE @PalabraId INT;

SELECT @PalabraId = Id
FROM Palabra
WHERE [Key] = @Key;

IF @PalabraId IS NULL
BEGIN
    INSERT INTO Palabra ([Key], EsBase)
    VALUES (@Key, @EsBase);

    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE IF @EsBase = 1
BEGIN
    UPDATE Palabra
    SET EsBase = 1
    WHERE Id = @PalabraId
      AND EsBase = 0;
END

SELECT @PalabraId;";

            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Key", SqlDbType.VarChar, key),
                    accesoDatos.CreateParameter("@EsBase", SqlDbType.Bit, esBase)
                };

                return accesoDatos.WriteWithResult(query, parametros);
            }
        }

        public bool GuardarTraduccion(int idiomaId, int palabraId, string valor)
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

            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@IdiomaId", SqlDbType.Int, idiomaId),
                    accesoDatos.CreateParameter("@PalabraId", SqlDbType.Int, palabraId),
                    accesoDatos.CreateParameter("@Valor", SqlDbType.VarChar, valor)
                };

                return accesoDatos.Write(query, parametros) > 0;
            }
        }

        public bool EliminarTraduccion(int idiomaId, int palabraId)
        {
            const string query = @"
DELETE FROM Traduccion
WHERE IdiomaId = @IdiomaId
  AND PalabraId = @PalabraId";

            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@IdiomaId", SqlDbType.Int, idiomaId),
                    accesoDatos.CreateParameter("@PalabraId", SqlDbType.Int, palabraId)
                };

                return accesoDatos.Write(query, parametros) > 0;
            }
        }

        public bool EstablecerIdiomaDefault(int idiomaId)
        {
            const string query = @"
UPDATE Idioma
SET [Default] = CASE WHEN Id = @IdiomaId THEN 1 ELSE 0 END;";

            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@IdiomaId", SqlDbType.Int, idiomaId)
                };

                return accesoDatos.Write(query, parametros) > 0;
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

        private static Palabra MapPalabra(DataRow row)
        {
            return new Palabra
            {
                Id = Convert.ToInt32(row["Id"]),
                Key = row["Key"].ToString(),
                EsBase = Convert.ToBoolean(row["EsBase"])
            };
        }
    }
}
