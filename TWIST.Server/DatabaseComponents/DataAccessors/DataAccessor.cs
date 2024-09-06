using System.Data.SqlClient;
using System.Data;
using TWISTServer.Interfaces;
using TWISTServer.DatabaseComponents.Records;
using System.Reflection;
using TWISTServer.Extensions;
using System.Text;

namespace TWISTServer.DatabaseComponents.DataAccessors
{
    /// <summary>
    /// An extra layer of abstraction between the database (through raw SQL) and the higher-level code.
    /// </summary>
    /// <typeparam name="T">The database record type that the data accessor will be responsible for retrieving</typeparam>
    public abstract class DataAccessor<T> : IDataAccessor<T> where T : IDatabaseRecord<T>
    {
        public DataAccessor() { }

        public Database Database { get; } = new();

        public abstract string PrimaryKeyColumn { get; }
        public abstract string TableName { get; }

        //<inheritdoc/>
        public virtual IEnumerable<T> Get(int id)
        {
            string sql = @$"SELECT {GetColumnsAsSql(T.Columns.Keys)} FROM {TableName} WHERE {PrimaryKeyColumn} = @{PrimaryKeyColumn};";
            return Database.Query<T>(
                sql,
                T.FromRow,
                [
                    new($"@{PrimaryKeyColumn}", SqlDbType.Int) { Value = id },
                ]
            );
        }

        //<inheritdoc/>
        public virtual IEnumerable<T> GetAll()
        {
            string sql = @$"SELECT {GetColumnsAsSql(T.Columns.Keys)} FROM {TableName};";
            return Database.Query<T>(
                sql,
                T.FromRow
            );
        }

        //<inheritdoc/>
        public virtual int Insert(T record)
        {
            PropertyInfo[] recordProperties = record.GetType().GetProperties();
            Dictionary<string, object?> columnNameValueDict = new();

            // Go through and get the values of each column for the record
            for (int i = 0; i < recordProperties.Length; i++)
            {
                var pName = recordProperties[i].Name.ToSnakeCase();
                var pValue = recordProperties[i].GetValue(record);
                Console.WriteLine($"{pName} = {pValue}");

                columnNameValueDict.Add(pName, pValue);
            }

            // Create the SQL text input
            string sql = $@"
INSERT INTO {TableName} 
({GetColumnsAsSql(T.Columns.Keys.Skip(1))}) 
VALUES ({GetColumnsAsSql(T.Columns.Keys.Skip(1), "@")});";

            // Construct the list of parameters
            List<SqlParameter> parameters = new();
            foreach (string columnName in T.Columns.Keys)
            {
                parameters.Add(new($"@{columnName}", T.Columns[columnName]) { Value = columnNameValueDict[columnName] });
            }

            // Call to the database
            return Database.NonQuery(sql, parameters.ToArray());
        }

        //<inheritdoc/>
        public virtual int Delete(int id)
        {
            // Check if the primary key column is empty
            if (PrimaryKeyColumn == "")
            {
                return -1;
            }

            // Check if the database name is empty
            if (TableName == "")
            {
                return -1;
            }

            string sql = $@"DELETE FROM {TableName} WHERE {PrimaryKeyColumn} = @{PrimaryKeyColumn};";

            SqlParameter[] parameters = [
                new($"@{PrimaryKeyColumn}", SqlDbType.Int) { Value = id },
            ];

            return Database.NonQuery(sql, parameters);
        }

        /// <summary>
        /// Transforms database columns into a single, comma-seperated string
        /// </summary>
        /// <param name="columns">An array of the column names</param>
        /// <param name="prefix">A substring that will go before each column name in the string.</param>
        /// <returns></returns>
        private string GetColumnsAsSql(IEnumerable<string> columns, string prefix="")
        {
            return String.Join(",", columns.Select(s => prefix + s));
        }
    }
}
