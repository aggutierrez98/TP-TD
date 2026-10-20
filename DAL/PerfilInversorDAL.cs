using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.Json;
using BE;

namespace DAL
{
    public sealed class PerfilInversorDAL
    {
        public PerfilInversor ObtenerPorUsuario(int usuarioId, AccesoDatosSqlServer acceso = null)
        {
            bool propio = acceso == null;
            acceso = acceso ?? new AccesoDatosSqlServer();
            try
            {
                // El mismo lock de propietario que usa el escritor evita leer cabecera
                // y respuestas pertenecientes a evaluaciones distintas.
                var tabla = acceso.Read(@"DECLARE @Propietario INT;
                    SELECT @Propietario=Id FROM Usuarios WITH (HOLDLOCK) WHERE Id=@UsuarioId;
                    SELECT p.*, r.PreguntaId, r.OpcionId FROM PerfilInversor p
                    JOIN PerfilInversorRespuesta r ON r.PerfilInversorId=p.Id
                    WHERE p.UsuarioId=@UsuarioId ORDER BY r.PreguntaId;",
                    new List<SqlParameter> { acceso.CreateParameter("@UsuarioId", SqlDbType.Int, usuarioId) });
                if (tabla.Rows.Count == 0) return null;
                DataRow fila = tabla.Rows[0];
                return new PerfilInversor
                {
                    Id = (int)fila["Id"], UsuarioId = usuarioId, VersionCuestionario = (int)fila["VersionCuestionario"],
                    FechaEvaluacionUtc = DateTime.SpecifyKind((DateTime)fila["FechaEvaluacionUtc"], DateTimeKind.Utc),
                    Capacidad = (decimal)fila["Capacidad"], Tolerancia = (decimal)fila["Tolerancia"], Puntaje = (decimal)fila["Puntaje"],
                    NivelRiesgo = (NivelRiesgoInversor)(int)fila["NivelRiesgo"],
                    MotivosLimite = JsonSerializer.Deserialize<List<string>>(fila["MotivosLimite"].ToString()),
                    Respuestas = tabla.Rows.Cast<DataRow>().Select(r => new RespuestaPerfilInversor
                        { PreguntaId = (int)r["PreguntaId"], OpcionId = r["OpcionId"].ToString() }).ToList()
                };
            }
            finally { if (propio) acceso.Dispose(); }
        }

        public void Guardar(PerfilInversor perfil, AccesoDatosSqlServer acceso = null)
        {
            bool propio = acceso == null;
            acceso = acceso ?? new AccesoDatosSqlServer();
            try
            {
                var parametros = new List<SqlParameter>
                {
                    acceso.CreateParameter("@UsuarioId", SqlDbType.Int, perfil.UsuarioId),
                    acceso.CreateParameter("@Version", SqlDbType.Int, perfil.VersionCuestionario),
                    acceso.CreateParameter("@Fecha", SqlDbType.DateTime2, perfil.FechaEvaluacionUtc),
                    acceso.CreateParameter("@Capacidad", SqlDbType.Decimal, perfil.Capacidad),
                    acceso.CreateParameter("@Tolerancia", SqlDbType.Decimal, perfil.Tolerancia),
                    acceso.CreateParameter("@Puntaje", SqlDbType.Decimal, perfil.Puntaje),
                    acceso.CreateParameter("@Nivel", SqlDbType.Int, (int)perfil.NivelRiesgo),
                    acceso.CreateParameter("@Motivos", SqlDbType.NVarChar, JsonSerializer.Serialize(perfil.MotivosLimite))
                };
                var sql = new StringBuilder(@"DECLARE @Propietario INT, @PerfilId INT;
                    SELECT @Propietario=Id FROM Usuarios WITH (UPDLOCK,HOLDLOCK) WHERE Id=@UsuarioId;
                    SELECT @PerfilId=Id FROM PerfilInversor WHERE UsuarioId=@UsuarioId;
                    IF @PerfilId IS NULL BEGIN
                        INSERT INTO PerfilInversor (UsuarioId,VersionCuestionario,FechaEvaluacionUtc,Capacidad,Tolerancia,Puntaje,NivelRiesgo,MotivosLimite)
                        VALUES (@UsuarioId,@Version,@Fecha,@Capacidad,@Tolerancia,@Puntaje,@Nivel,@Motivos);
                        SET @PerfilId=CONVERT(INT,SCOPE_IDENTITY());
                    END ELSE BEGIN
                        UPDATE PerfilInversor SET VersionCuestionario=@Version,FechaEvaluacionUtc=@Fecha,
                            Capacidad=@Capacidad,Tolerancia=@Tolerancia,Puntaje=@Puntaje,NivelRiesgo=@Nivel,MotivosLimite=@Motivos
                        WHERE Id=@PerfilId;
                    END;
                    DELETE FROM PerfilInversorRespuesta WHERE PerfilInversorId=@PerfilId;");
                for (int i = 0; i < perfil.Respuestas.Count; i++)
                {
                    sql.Append("INSERT INTO PerfilInversorRespuesta (PerfilInversorId,PreguntaId,OpcionId) VALUES (@PerfilId,@Q").Append(i).Append(",@O").Append(i).Append(");");
                    parametros.Add(acceso.CreateParameter("@Q" + i, SqlDbType.Int, perfil.Respuestas[i].PreguntaId));
                    parametros.Add(acceso.CreateParameter("@O" + i, SqlDbType.Char, perfil.Respuestas[i].OpcionId));
                }
                acceso.Write(sql.ToString(), parametros);
            }
            finally { if (propio) acceso.Dispose(); }
        }
    }
}
