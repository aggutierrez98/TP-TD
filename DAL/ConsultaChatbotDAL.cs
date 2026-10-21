using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class ConsultaChatbotDAL
    {
        private ConsultaChatbot Map(DataRow row, Usuario usuario)
        {
            return new ConsultaChatbot
            {
                Id = (int)row["Id"],
                ChatbotFinanciero = new ChatbotFinanciero { Id = (int)row["ChatbotFinancieroId"] },
                Usuario = usuario,
                Pregunta = row["Pregunta"].ToString(),
                Respuesta = row["Respuesta"].ToString(),
                Fecha = (DateTime)row["Fecha"],
                EsPregunta = (bool)row["EsPregunta"]
            };
        }

        public List<ConsultaChatbot> ObtenerPorUsuario(Usuario usuario, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, ChatbotFinancieroId, UsuarioId, Pregunta, Respuesta, Fecha, EsPregunta
                FROM ConsultaChatbot
                WHERE UsuarioId = @UsuarioId
                ORDER BY Fecha";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, usuario.Id)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                List<ConsultaChatbot> consultas = new List<ConsultaChatbot>();
                foreach (DataRow row in dataTable.Rows)
                {
                    consultas.Add(Map(row, usuario));
                }

                return consultas;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public int InsertarYObtenerId(ConsultaChatbot consultaChatbot, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                INSERT INTO ConsultaChatbot (ChatbotFinancieroId, UsuarioId, Pregunta, Respuesta, Fecha, EsPregunta)
                OUTPUT INSERTED.Id
                VALUES (@ChatbotFinancieroId, @UsuarioId, @Pregunta, @Respuesta, @Fecha, @EsPregunta)";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@ChatbotFinancieroId", SqlDbType.Int, consultaChatbot.ChatbotFinanciero.Id),
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, consultaChatbot.Usuario.Id),
                    accesoDatos.CreateParameter("@Pregunta", SqlDbType.VarChar, consultaChatbot.Pregunta),
                    accesoDatos.CreateParameter("@Respuesta", SqlDbType.VarChar, consultaChatbot.Respuesta),
                    accesoDatos.CreateParameter("@Fecha", SqlDbType.DateTime, consultaChatbot.Fecha),
                    accesoDatos.CreateParameter("@EsPregunta", SqlDbType.Bit, consultaChatbot.EsPregunta)
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
    }
}
