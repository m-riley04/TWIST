using System.Data;

namespace TWISTServer.Interfaces
{
    public interface IDatabaseRecord<T> where T : IDatabaseRecord<T>
    {
        /// <summary>
        /// A dictionary representing the columns of the table, with the column name as the key and the column type as the value.
        /// </summary>
        /// <remarks>The primary key column <b><i>must</i></b> be the first item in the dictionary, and should match that of the associated record. It is advised to have the items in-order.</remarks>
        public abstract static Dictionary<string, SqlDbType> Columns { get; }

        /// <summary>
        /// Create an instance of the record from a DataRow.
        /// </summary>
        /// <param name="row">The data row to map from.</param>
        /// <returns>An instance of the implementing record.</returns>
        public static abstract T FromRow(DataRow row);
    }
}
