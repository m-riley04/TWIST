using TWISTServer.DatabaseComponents;

namespace TWISTServer.Interfaces
{
    /// <summary>
    /// An interface that represents a data accessor.
    /// </summary>
    /// <remarks>For our use case, the tables this will work with are for tables that a) have a primary key, and b) increment that primary key column. </remarks>
    /// <typeparam name="T">The type of record that is used to transfer data.</typeparam>
    public interface IDataAccessor<T>
    {
        /// <summary>
        /// The database object that will be recieving and handling all SQL calls.
        /// </summary>
        public Database Database { get; }
        
        /// <summary>
        /// The primary key column name for the table.
        /// </summary>
        public abstract string PrimaryKeyColumn { get; }

        /// <summary>
        /// The table name that we are accessing data from
        /// </summary>
        public abstract string TableName { get; }

        /// <summary>
        /// A dictionary representing the columns of the table, with the column name as the key and the column type as the value.
        /// </summary>
        /// <remarks>The primary key column <b><i>must</i></b> be the first item in the dictionary, and should match that of the associated record. It is advised to have the items in-order.</remarks>
        public abstract Dictionary<string, Type> Columns { get; }


        /// <summary>
        /// Inserts a single row into the table (<ref cref="TableName"/>) given the 
        /// </summary>
        /// <param name="record">A record holding the data to insert. The primary key value is ignored.</param>
        /// <returns></returns>
        public abstract int Insert(T record);

        /// <summary>
        /// Deletes a single row from a table <see cref="TableName"/> given an ID based on the primary key <see cref="PrimaryKeyColumn"/>, which is a NOT NULL INT
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The number of columns affected by the deletion. -1 if there was an error.</returns>
        public abstract int Delete(int id); 

    }
}
