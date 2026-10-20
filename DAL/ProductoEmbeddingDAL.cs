using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    // Vector serializado como float32 little-endian (4 bytes por componente), independiente del
    // orden de bytes de la maquina.
    public class ProductoEmbeddingDAL
    {
        public List<ProductoEmbedding> ObtenerTodos(AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT ProductoId, Modelo, TextoFuente, Vector, FechaCalculo
                FROM ProductoEmbedding
                ORDER BY ProductoId";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                DataTable dataTable = accesoDatos.Read(query);
                List<ProductoEmbedding> embeddings = new List<ProductoEmbedding>();
                foreach (DataRow row in dataTable.Rows)
                {
                    embeddings.Add(new ProductoEmbedding
                    {
                        ProductoId = (int)row["ProductoId"],
                        Modelo = row["Modelo"].ToString(),
                        TextoFuente = row["TextoFuente"].ToString(),
                        Vector = ABytesAVector((byte[])row["Vector"]),
                        FechaCalculo = (DateTime)row["FechaCalculo"]
                    });
                }

                return embeddings;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public void Guardar(ProductoEmbedding embedding, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                UPDATE ProductoEmbedding
                SET Modelo = @Modelo, TextoFuente = @TextoFuente, Vector = @Vector, FechaCalculo = @FechaCalculo
                WHERE ProductoId = @ProductoId;

                IF @@ROWCOUNT = 0
                    INSERT INTO ProductoEmbedding (ProductoId, Modelo, TextoFuente, Vector, FechaCalculo)
                    VALUES (@ProductoId, @Modelo, @TextoFuente, @Vector, @FechaCalculo);";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@ProductoId", SqlDbType.Int, embedding.ProductoId),
                    accesoDatos.CreateParameter("@Modelo", SqlDbType.VarChar, embedding.Modelo),
                    accesoDatos.CreateParameter("@TextoFuente", SqlDbType.VarChar, embedding.TextoFuente),
                    accesoDatos.CreateParameter("@Vector", SqlDbType.VarBinary, AVectorABytes(embedding.Vector)),
                    accesoDatos.CreateParameter("@FechaCalculo", SqlDbType.DateTime, embedding.FechaCalculo)
                };
                parametros[3].Size = -1;

                accesoDatos.Write(query, parametros);
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        internal static byte[] AVectorABytes(float[] vector)
        {
            float[] valores = vector ?? new float[0];
            byte[] bytes = new byte[valores.Length * 4];
            for (int i = 0; i < valores.Length; i++)
            {
                byte[] componente = BitConverter.GetBytes(valores[i]);
                if (!BitConverter.IsLittleEndian)
                {
                    Array.Reverse(componente);
                }

                Buffer.BlockCopy(componente, 0, bytes, i * 4, 4);
            }

            return bytes;
        }

        internal static float[] ABytesAVector(byte[] bytes)
        {
            if (bytes == null || bytes.Length % 4 != 0)
            {
                throw new InvalidOperationException("El vector guardado no tiene un largo multiplo de 4 bytes.");
            }

            float[] vector = new float[bytes.Length / 4];
            byte[] componente = new byte[4];
            for (int i = 0; i < vector.Length; i++)
            {
                Buffer.BlockCopy(bytes, i * 4, componente, 0, 4);
                if (!BitConverter.IsLittleEndian)
                {
                    Array.Reverse(componente);
                }

                vector[i] = BitConverter.ToSingle(componente, 0);
            }

            return vector;
        }
    }
}
