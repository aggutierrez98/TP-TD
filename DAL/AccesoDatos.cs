using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
    {
        internal class Access
        {
            public const string SQLSERVER_DATA_SOURCE = ".\\SQLEXPRESS";
            //  Para correr en la UAI localmente
            //  public const string SQLSERVER_DATA_SOURCE = ".";
            public const string DATABASE_NAME = "TpCatedraIDS";

            private SqlConnection conection;
            private SqlTransaction transaction;

            public void Open()
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = SQLSERVER_DATA_SOURCE;
                builder.InitialCatalog = DATABASE_NAME;
                builder.IntegratedSecurity = true;
                conection = new SqlConnection(builder.ConnectionString);
                conection.Open();
            }

            public void Close()
            {
                conection.Close();
                conection = null;
                GC.Collect();
            }

            public void StartTransaction()
            {
                transaction = conection.BeginTransaction();
            }

            public void RollbackTransaction()
            {
                transaction.Rollback();
                transaction = null;
            }

            public void ConfirmTransaction()
            {
                transaction.Commit();
                transaction = null;
            }

            private SqlCommand CreateCommand(string sql, List<SqlParameter> parameters = null)
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conection;

                if (transaction != null)
                {
                    cmd.Transaction = transaction;
                }
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                }
                return cmd;
            }

            public SqlParameter CreateParameter(string name, string value)
            {

                SqlParameter p = new SqlParameter(name, value);
                p.DbType = DbType.String;
                return p;
            }


            public SqlParameter CreateParameter(string name, int value)
            {

                SqlParameter p = new SqlParameter(name, value);
                p.DbType = DbType.Int32;
                return p;
            }

            public int Write(string sql, List<SqlParameter> parameters = null)
            {
                int affectedRowsCount = 0;
                SqlCommand cmd = CreateCommand(sql, parameters);
                try
                {
                    affectedRowsCount = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    affectedRowsCount = -1;
                }
                cmd.Parameters.Clear();
                return affectedRowsCount;
            }

            public int WriteWithResult(string sql, List<SqlParameter> parameters = null)
            {
                //int idReturned = 0;
                SqlCommand cmd = CreateCommand(sql, parameters);
                int idInserted = 0;
                try
                {
                    //affectedRowsCount = cmd.ExecuteNonQuery();
                    idInserted = Convert.ToInt32(cmd.ExecuteScalar());
                    //if (result != null && result != DBNull.Value)
                    //{
                    //    idReturned = Convert.ToInt32(result);
                    //}
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    //idReturned = -1;
                }
                cmd.Parameters.Clear();
                //return idReturned;

                //if (idInserted == DBNull.Value)
                //{
                //    return null;
                //}

                return idInserted;
            }

            public DataTable Read(string sql, List<SqlParameter> parameters = null)
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = CreateCommand(sql, parameters);

                DataTable table = new DataTable();

                try
                {
                    adapter.Fill(table);
                    adapter.Dispose();
                    adapter = null;

                }
                catch (Exception error)
                {
                    Console.WriteLine(error.Message);
                    throw;
                }

                return table;
            }
        }
    }