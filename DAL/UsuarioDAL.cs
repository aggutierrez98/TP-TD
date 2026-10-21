using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class UsuarioDAL
    {
        private Usuario Map(DataRow row)
        {
            return new Usuario
            {
                Id = (int)row["Id"],
                Username = row["Username"].ToString(),
                PasswordHash = row["PasswordHash"].ToString(),
                Activo = (bool)row["Activo"],
                IntentosFallidos = (int)row["IntentosFallidos"],
                Bloqueado = (bool)row["Bloqueado"],
                BloqueadoHasta = row.IsNull("BloqueadoHasta") ? (DateTime?)null : (DateTime)row["BloqueadoHasta"],
                DVH = row.IsNull("DVH") ? null : row["DVH"].ToString()
            };
        }

        public Usuario ObtenerPorId(int id, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, Username, PasswordHash, Activo, IntentosFallidos, Bloqueado, BloqueadoHasta, DVH
                FROM Usuarios
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

        public Usuario ObtenerPorUsername(string username, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, Username, PasswordHash, Activo, IntentosFallidos, Bloqueado, BloqueadoHasta, DVH
                FROM Usuarios
                WHERE Username = @Username";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Username", SqlDbType.VarChar, username)
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

        public int InsertarYObtenerId(Usuario usuario, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                INSERT INTO Usuarios (Username, PasswordHash, Activo, IntentosFallidos, Bloqueado, BloqueadoHasta, DVH)
                OUTPUT INSERTED.Id
                VALUES (@Username, @PasswordHash, @Activo, @IntentosFallidos, @Bloqueado, @BloqueadoHasta, @DVH)";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Username", SqlDbType.VarChar, usuario.Username),
                    accesoDatos.CreateParameter("@PasswordHash", SqlDbType.VarChar, usuario.PasswordHash),
                    accesoDatos.CreateParameter("@Activo", SqlDbType.Bit, usuario.Activo),
                    accesoDatos.CreateParameter("@IntentosFallidos", SqlDbType.Int, usuario.IntentosFallidos),
                    accesoDatos.CreateParameter("@Bloqueado", SqlDbType.Bit, usuario.Bloqueado),
                    accesoDatos.CreateParameter("@BloqueadoHasta", SqlDbType.DateTime, usuario.BloqueadoHasta),
                    accesoDatos.CreateParameter("@DVH", SqlDbType.VarChar, usuario.DVH)
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

        public bool Insertar(Usuario usuario, AccesoDatosSqlServer accesoDatos = null)
        {
            return InsertarYObtenerId(usuario, accesoDatos) > 0;
        }

        // Pagina por clave (Id ascendente) de cuentas activas: no filtra por rol ni por bloqueo temporal.
        public List<Usuario> ObtenerActivosDespuesDe(Usuario ultimo, int cantidad, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT TOP (@Cantidad) Id, Username, PasswordHash, Activo, IntentosFallidos, Bloqueado, BloqueadoHasta, DVH
                FROM Usuarios
                WHERE Activo = 1 AND Id > @UltimoId
                ORDER BY Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();
            try
            {
                DataTable tabla = accesoDatos.Read(query, new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Cantidad", SqlDbType.Int, cantidad),
                    accesoDatos.CreateParameter("@UltimoId", SqlDbType.Int, ultimo?.Id ?? 0)
                });
                List<Usuario> usuarios = new List<Usuario>();
                foreach (DataRow row in tabla.Rows)
                {
                    usuarios.Add(Map(row));
                }

                return usuarios;
            }
            finally { if (dispose) accesoDatos.Dispose(); }
        }

        public List<Usuario> ObtenerTodos()
        {
            const string query = @"
                SELECT Id, Username, PasswordHash, Activo, IntentosFallidos, Bloqueado, BloqueadoHasta, DVH
                FROM Usuarios
                ORDER BY Activo DESC, Username";

            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                DataTable dataTable = accesoDatos.Read(query);
                List<Usuario> usuarios = new List<Usuario>();

                foreach (DataRow row in dataTable.Rows)
                {
                    usuarios.Add(Map(row));
                }

                return usuarios;
            }
        }

        // Sobrecarga ordenada por Id para el control de integridad: el DVV depende del
        // orden de los DVH, por lo que debe ser deterministico y reutilizable dentro
        // de la transaccion de escritura en curso.
        public List<Usuario> ObtenerTodos(AccesoDatosSqlServer accesoDatos)
        {
            const string query = @"
                SELECT Id, Username, PasswordHash, Activo, IntentosFallidos, Bloqueado, BloqueadoHasta, DVH
                FROM Usuarios
                ORDER BY Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                DataTable dataTable = accesoDatos.Read(query);
                List<Usuario> usuarios = new List<Usuario>();

                foreach (DataRow row in dataTable.Rows)
                {
                    usuarios.Add(Map(row));
                }

                return usuarios;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public bool Actualizar(Usuario usuario, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                UPDATE Usuarios
                SET Username = @Username,
                    PasswordHash = @PasswordHash,
                    Activo = @Activo,
                    IntentosFallidos = @IntentosFallidos,
                    Bloqueado = @Bloqueado,
                    BloqueadoHasta = @BloqueadoHasta,
                    DVH = @DVH
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Username", SqlDbType.VarChar, usuario.Username),
                    accesoDatos.CreateParameter("@PasswordHash", SqlDbType.VarChar, usuario.PasswordHash),
                    accesoDatos.CreateParameter("@Activo", SqlDbType.Bit, usuario.Activo),
                    accesoDatos.CreateParameter("@IntentosFallidos", SqlDbType.Int, usuario.IntentosFallidos),
                    accesoDatos.CreateParameter("@Bloqueado", SqlDbType.Bit, usuario.Bloqueado),
                    accesoDatos.CreateParameter("@BloqueadoHasta", SqlDbType.DateTime, usuario.BloqueadoHasta),
                    accesoDatos.CreateParameter("@DVH", SqlDbType.VarChar, usuario.DVH),
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, usuario.Id)
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

        // Actualiza el contador de intentos fallidos, el estado de bloqueo (temporal o
        // permanente) y su DVH en una unica sentencia: Login los modifica siempre juntos.
        public bool ActualizarEstadoBloqueo(Usuario usuario, int intentosFallidos, bool bloqueado, DateTime? bloqueadoHasta, string dvh, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                UPDATE Usuarios
                SET IntentosFallidos = @IntentosFallidos,
                    Bloqueado = @Bloqueado,
                    BloqueadoHasta = @BloqueadoHasta,
                    DVH = @DVH
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@IntentosFallidos", SqlDbType.Int, intentosFallidos),
                    accesoDatos.CreateParameter("@Bloqueado", SqlDbType.Bit, bloqueado),
                    accesoDatos.CreateParameter("@BloqueadoHasta", SqlDbType.DateTime, bloqueadoHasta),
                    accesoDatos.CreateParameter("@DVH", SqlDbType.VarChar, dvh),
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, usuario.Id)
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

        // Actualiza unicamente el DVH de una fila. Se usa al inicializar las firmas o
        // tras restaurar un estado historico, sin reescribir el resto de las columnas.
        public bool ActualizarDVH(Usuario usuario, string dvh, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                UPDATE Usuarios
                SET DVH = @DVH
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@DVH", SqlDbType.VarChar, dvh),
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, usuario.Id)
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

        public bool GuardarEstadoCompleto(Usuario usuario, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                UPDATE Usuarios
                SET Username = @Username,
                    PasswordHash = @PasswordHash,
                    Activo = @Activo
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, usuario.Id),
                    accesoDatos.CreateParameter("@Username", SqlDbType.VarChar, usuario.Username),
                    accesoDatos.CreateParameter("@PasswordHash", SqlDbType.VarChar, usuario.PasswordHash),
                    accesoDatos.CreateParameter("@Activo", SqlDbType.Bit, usuario.Activo)
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

        public bool Desactivar(Usuario usuario, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                UPDATE Usuarios
                SET Activo = 0
                WHERE Id = @Id
                  AND Activo = 1";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, usuario.Id)
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

        public bool EliminarFisicamente(Usuario usuario, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                UPDATE UsuarioHistorial
                SET ActorUsuarioId = NULL
                WHERE ActorUsuarioId = @Id;

                DELETE FROM UsuarioHistorialPermiso
                WHERE HistorialId IN (
                    SELECT HistorialId
                    FROM UsuarioHistorial
                    WHERE UsuarioId = @Id
                );

                DELETE FROM UsuarioHistorial
                WHERE UsuarioId = @Id;

                DELETE FROM UsuarioPermiso
                WHERE UsuarioId = @Id;

                DELETE FROM UsuarioPreferencias
                WHERE UsuarioId = @Id;

                DELETE FROM Bitacora
                WHERE UsuarioId = @Id;

                DELETE FROM Usuarios
                WHERE Id = @Id;";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, usuario.Id)
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
