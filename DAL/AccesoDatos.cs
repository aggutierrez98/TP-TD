using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using SERVICIOS;

namespace DAL
{
    public sealed class AccesoDatos : IDisposable
    {
        private static readonly string[] ConnectionStringNames = { "TpIDS", "TpIDS-UAI" };

        private SqlConnection connection;
        private SqlTransaction transaction;

        public AccesoDatos()
        {
        }

        public SqlParameter CreateParameter(string name, SqlDbType dbType, object value)
        {
            return new SqlParameter(name, dbType)
            {
                Value = value ?? DBNull.Value
            };
        }

        public int Write(string sql, List<SqlParameter> parameters = null)
        {
            return ExecuteInTransaction(
                sql,
                parameters,
                CommandType.Text,
                command => command.ExecuteNonQuery());
        }

        public int WriteWithResult(string sql, List<SqlParameter> parameters = null)
        {
            return ExecuteInTransaction(
                sql,
                parameters,
                CommandType.Text,
                command =>
                {
                    object result = command.ExecuteScalar();
                    return result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
                });
        }

        public DataTable Read(string sql, List<SqlParameter> parameters = null)
        {
            return ExecuteInTransaction(
                sql,
                parameters,
                CommandType.Text,
                command =>
                {
                    DataTable table = new DataTable();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(table);
                    }

                    return table;
                });
        }

        public void BeginTransactionScope()
        {
            EnsureConnectionOpen();
            StartTransaction();
        }

        public void CommitTransactionScope()
        {
            ConfirmTransaction();
        }

        public void RollbackTransactionScope()
        {
            RollbackTransaction();
        }

        private T ExecuteInTransaction<T>(
            string sql,
            List<SqlParameter> parameters,
            CommandType commandType,
            Func<SqlCommand, T> operation)
        {
            EnsureConnectionOpen();
            bool transaccionCreadaEnLlamada = false;
            if (transaction == null)
            {
                StartTransaction();
                transaccionCreadaEnLlamada = true;
            }

            using (SqlCommand command = CreateCommand(sql, commandType, parameters))
            {
                try
                {
                    T result = operation(command);
                    if (transaccionCreadaEnLlamada)
                    {
                        ConfirmTransaction();
                    }

                    return result;
                }
                catch (SqlException ex)
                {
                    RollbackTransaction();
                    throw new ExcepcionTecnica("Error de SQL.", ex);
                }
                catch
                {
                    RollbackTransaction();
                    throw;
                }
            }
        }

        private void EnsureConnectionOpen()
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                return;
            }

            Exception lastException = null;

            foreach (string connectionStringName in ConnectionStringNames)
            {
                ConnectionStringSettings connectionSettings = ConfigurationManager.ConnectionStrings[connectionStringName];
                if (connectionSettings == null || string.IsNullOrWhiteSpace(connectionSettings.ConnectionString))
                {
                    continue;
                }

                try
                {
                    connection = new SqlConnection(connectionSettings.ConnectionString);
                    connection.Open();
                    return;
                }
                catch (SqlException ex)
                {
                    lastException = ex;

                    if (connection != null)
                    {
                        connection.Dispose();
                        connection = null;
                    }
                }
            }

            if (lastException != null)
            {
                throw new ExcepcionTecnica("No se pudo conectar a ninguna de las conexiones configuradas.", lastException);
            }

            throw new InvalidOperationException(
                "No se encontro ninguna connection string valida en la configuracion para TpIDS.");
        }

        private void StartTransaction()
        {
            if (transaction == null)
            {
                transaction = connection.BeginTransaction();
            }
        }

        private void ConfirmTransaction()
        {
            if (transaction != null)
            {
                transaction.Commit();
                transaction.Dispose();
                transaction = null;
            }
        }

        private void RollbackTransaction()
        {
            if (transaction != null)
            {
                transaction.Rollback();
                transaction.Dispose();
                transaction = null;
            }
        }

        private SqlCommand CreateCommand(string sql, CommandType commandType, List<SqlParameter> parameters = null)
        {
            SqlCommand command = new SqlCommand(sql, connection, transaction);
            command.CommandType = commandType;

            if (parameters != null && parameters.Count > 0)
            {
                command.Parameters.AddRange(parameters.ToArray());
            }

            return command;
        }

        public void Dispose()
        {
            if (transaction != null)
            {
                transaction.Dispose();
                transaction = null;
            }

            if (connection != null && connection.State != ConnectionState.Closed)
            {
                connection.Close();
            }

            if (connection != null)
            {
                connection.Dispose();
                connection = null;
            }
        }
    }
}
