using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace NT.Builder.DbHelper
{
    public class DbUtility
    {
        private string connectionString;

        public DbUtility(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public DbUtility(string host, string username, string password, string dbname)
        {
            connectionString = string.Format("Data Source={0};Initial Catalog={3};User Id={1};Password={2};MultipleActiveResultSets=True;", host, username, password, dbname);
        }

        public bool IsConnection
        {
            get
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }

            }
        }


        public DataTable Tables()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                //connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter("select * from sysobjects where xtype='V' or xtype='U' order by xtype", (SqlConnection)connection);

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
        }

        public DataTable TableInfo(string tableName)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter("select top 1 * from " + tableName, (SqlConnection)connection);

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
        }

    }
}
