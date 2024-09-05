using System.Data.SqlClient;
using System.Data;
using TWISTServer.Interfaces;
using TWISTServer.DatabaseComponents.Records;

namespace TWISTServer.DatabaseComponents.DataAccessors
{
    public abstract class DataAccessor<T> : IDataAccessor<T>
    {
        public Database Database { get; } = new();

        public virtual string PrimaryKeyColumn { get; } = "";
        public virtual string TableName { get; } = "";

        public virtual Dictionary<string, Type> Columns { get; } = new();

        public DataAccessor() { }

        ///<inheritdoc/>///
        public virtual int Insert(T record)
        {

        }

        ///<inheritdoc/>///
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
    }
}
