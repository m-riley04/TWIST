using System.Data;
using TWISTServer.DatabaseComponents;

namespace TWISTServer.Interfaces
{
    /// <summary>
    /// An interface that represents a data accessor.
    /// </summary>
    /// <remarks>For our use case, the tables this will work with are for tables that a) have a primary key, and b) increment that primary key column. </remarks>
    /// <typeparam name="T">The type of record that is used to transfer data.</typeparam>
    public interface IDataAccessor<T> where T : IDatabaseRecord<T>
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
        /// Gets a single row from the table (<see cref="TableName"/>) using the table's primary key (<see cref="PrimaryKeyColumn"/>) as an enumerable of records, where the record is of type <typeparamref name="T"/>
        /// </summary>
        /// <param name="id">The primary key value of the record</param>
        /// <returns></returns>
        public abstract IEnumerable<T> Get(int id);

        /// <summary>
        /// Gets all the rows of a table (<see cref="TableName"/>) as an enumerable of records, where the record is of type <typeparamref name="T"/>
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<T> GetAll();

        /// <summary>
        /// Inserts a single row into the table (<ref cref="TableName"/>) given the 
        /// </summary>
        /// <param name="record">A record holding the data to insert. The primary key value is ignored.</param>
        /// <returns>The number of columns affected by the insertion. -1 if there was an error.</returns>
        public abstract int Insert(T record);

        /// <summary>
        /// Deletes a single row from a table <see cref="TableName"/> given an ID based on the primary key <see cref="PrimaryKeyColumn"/>, which is a NOT NULL INT
        /// </summary>
        /// <param name="id">The primary key value of the record</param>
        /// <returns>The number of columns affected by the deletion. -1 if there was an error.</returns>
        public abstract int Delete(int id); 

    }
}
