using System.Data;
using BE;

namespace DAL
{
    public class ChatbotFinancieroDAL
    {
        private ChatbotFinanciero Map(DataRow row)
        {
            return new ChatbotFinanciero
            {
                Id = (int)row["Id"],
                Nombre = row["Nombre"].ToString()
            };
        }

        // Fila unica sembrada: no hay alta/baja de chatbots desde la UI, solo lectura.
        public ChatbotFinanciero ObtenerUnico(AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT TOP (1) Id, Nombre
                FROM ChatbotFinanciero
                ORDER BY Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                DataTable dataTable = accesoDatos.Read(query);
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
    }
}
