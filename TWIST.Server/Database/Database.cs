using System.Data;
using System.Data.SqlClient;

namespace TWISTServer.Database
{
    /// <summary>
    /// Acts as an abstracted version of the database. Makes it easier so that we don't have to make direct SQLConnection calls.
    /// </summary>
    public class Database
    {
        private readonly IConfiguration config = new ConfigurationManager(); // Might need to change/might be empty and without secrets. Not sure yet.
        private readonly string databaseUserIdKey = "twist-db-user-id";
        private readonly string databasePasswordKey = "twist-db-password";

        public Database() { }

        public Database(IConfiguration config)
        {
            this.config = config;
        }

        public Database(string server, string initialCatalog, bool persistSecurityInfo, bool multipleActiveResultSets, bool encrypt, bool trustServerCertificate, int connectionTimeout)
        {
            Server = server;
            InitialCatalog = initialCatalog;
            PersistSecurityInfo = persistSecurityInfo;
            MultipleActiveResultSets = multipleActiveResultSets;
            Encrypt = encrypt;
            TrustServerCertificate = trustServerCertificate;
            ConnectionTimeout = connectionTimeout;
        }

        public string Server = "tcp:twist.database.windows.net,1433";

        public string InitialCatalog = "TWIST-DB";

        public bool PersistSecurityInfo = false;

        /// <summary>
        /// The database user ID stored in an environment variable or the "secret.json" file.
        /// </summary>
        public string UserId
        {
            get
            {
                return Environment.GetEnvironmentVariable(databaseUserIdKey) ?? config[databaseUserIdKey] ?? "";
            }
        }

        /// <summary>
        /// The database password stored in an environment variable or the "secret.json" file.
        /// </summary>
        public string Password
        {
            get
            {
                return Environment.GetEnvironmentVariable(databasePasswordKey) ?? config[databasePasswordKey] ?? "";
            }
        }

        public bool MultipleActiveResultSets = false;

        public bool Encrypt = true;

        public bool TrustServerCertificate = false;

        public int ConnectionTimeout = 30;

        /// <summary>
        /// The full connection string combining all properties of the Database class
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return $"Server={Server};Initial Catalog={InitialCatalog};Persist Security Info={PersistSecurityInfo};User ID={UserId};Password={Password};MultipleActiveResultSets={MultipleActiveResultSets};Encrypt={Encrypt};TrustServerCertificate={TrustServerCertificate};Connection Timeout={ConnectionTimeout};";
            }
        }

        /// <summary>
        /// Queries the database
        /// </summary>
        /// <typeparam name="T">The type of data record to return from the query.</typeparam>
        /// <param name="query"></param>
        /// <param name="recordConverterMethod">Function that returns a <typeparamref name="T"/> from a <see cref="DataRow"/></param>
        /// <returns>An enumerable of rows in the form of type <typeparamref name="T"/></returns>
        public IEnumerable<T> Query<T>(string sql, Func<DataRow, T> recordConverterMethod)
        {
            DataTable table = new DataTable();
            SqlDataReader reader;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    reader = command.ExecuteReader();
                    table.Load(reader);
                    reader.Close();
                    connection.Close();
                }
            }

            // Convert each row to a record
            List<T> rows = new();
            foreach (DataRow row in table.Rows)
            {
                rows.Add(recordConverterMethod(row));
            }

            return rows;
        }

        /// <summary>
        /// Queries the database
        /// </summary>
        /// <typeparam name="T">The type of data record to return from the query.</typeparam>
        /// <param name="query"></param>
        /// <param name="recordConverterMethod">Function that returns a <typeparamref name="T"/> from a <see cref="DataRow"/></param>
        /// <param name="parameters">Parameters of the SQL statement</param>
        /// <returns>An enumerable of rows in the form of type <typeparamref name="T"/></returns>
        public IEnumerable<T> Query<T>(string sql, Func<DataRow, T> recordConverterMethod, SqlParameter[] parameters)
        {
            DataTable table = new DataTable();
            SqlDataReader reader;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    foreach (SqlParameter parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }

                    reader = command.ExecuteReader();
                    table.Load(reader);
                    reader.Close();
                    connection.Close();
                }
            }

            // Convert each row to a record
            List<T> rows = new();
            foreach (DataRow row in table.Rows)
            {
                rows.Add(recordConverterMethod(row));
            }

            return rows;
        }

        /// <summary>
        /// Executes a non-query onto the database
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns>The number of rows affected by the non-query</returns>
        public int NonQuery(string sql, SqlParameter[] parameters)
        {
            DataTable table = new DataTable();
            SqlDataReader reader;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    foreach (SqlParameter parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }

                    reader = command.ExecuteReader();
                    table.Load(reader);
                    reader.Close();
                    connection.Close();
                }
            }

            return table.GetChanges()?.Rows.Count ?? 0;
        }
    }
}