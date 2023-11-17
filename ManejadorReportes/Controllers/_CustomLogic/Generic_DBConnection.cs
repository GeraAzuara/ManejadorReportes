using System;
using System.Data;
using System.Data.SqlClient;
using System.Xml;

namespace ManejadorReportes.Controllers._CustomLogic
{
    public class Generic_DBConnection: Generic_Cryptographer
    {
        private SqlConnection connection;
        public string _keyDbConector;

        public bool SuccessConnection { get; set; }
        public bool SuccessDisconnection { get; set; }

        protected const string FORMATO_STR_CON = "Server=@server; Database=@dbase; User ID=@user; Password=@password; Trusted_Connection=False; MultipleActiveResultSets=true";

        public Generic_DBConnection(string connectionString)
        {
            connection = new SqlConnection(connectionString);
        }

        public Generic_DBConnection(string source, string dataBase, string user, string password)
        {
            string connectionString = CreateStringConnection(source, dataBase, user, password);
            connection = new SqlConnection(connectionString);
        }

        public Generic_DBConnection(XmlNode conexionNodo)
        {
            _keyDbConector = conexionNodo.Attributes["key"].Value;
            var algo = conexionNodo.SelectSingleNode("server");

            string _codedServer = conexionNodo.SelectSingleNode("server").Attributes["coded"].Value;
            string _codedDatabase = conexionNodo.SelectSingleNode("database").Attributes["coded"].Value;
            string _codedUsername = conexionNodo.SelectSingleNode("username").Attributes["coded"].Value;
            string _codedPassword = conexionNodo.SelectSingleNode("password").Attributes["coded"].Value;

            string connectionString = CreateStringConnection(Decode(_codedServer), Decode(_codedDatabase), Decode(_codedUsername), Decode(_codedPassword));
            connection = new SqlConnection(connectionString);
        }

        public static string CreateStringConnection(string source, string dataBase, string user, string password)
        {
            SqlConnectionStringBuilder Cnb = new SqlConnectionStringBuilder(FORMATO_STR_CON)
            {
                DataSource = source,
                InitialCatalog = dataBase,
                Password = password,
                UserID = user
            };
            return Cnb.ToString();
        }
        public bool OpenConnection()
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                SuccessConnection = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al abrir la conexión: " + ex.Message);
                SuccessConnection = false;
            }
            return SuccessConnection;
        }

        public bool CloseConnection()
        {
            try
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
                SuccessDisconnection = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al cerrar la conexión: " + ex.Message);
                SuccessDisconnection = false;
            }
            return SuccessDisconnection;
        }

        public SqlConnection GetConnection()
        {
            return connection;
        }

        public DataTable ExecuteSelectQuery(string query, SqlParameter[] parameters = null)
        {
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
        }

        public int ExecuteUpdateQuery(string query, SqlParameter[] parameters = null)
        {
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                return cmd.ExecuteNonQuery();
            }
        }

        public int ExecuteDeleteQuery(string query, SqlParameter[] parameters = null)
        {
            return ExecuteUpdateQuery(query, parameters);
        }

        public int ExecuteStoredProcedure(string procedureName, SqlParameter[] parameters = null)
        {
            using (SqlCommand cmd = new SqlCommand(procedureName, connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                return cmd.ExecuteNonQuery();
            }
        }
    }
}