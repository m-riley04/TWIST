using System.Data.SqlTypes;
using TWISTServer.DatabaseComponents.Records;
using System.Data;
using System.Data.SqlClient;

namespace TWISTServer.DatabaseComponents.DataAccessors
{
    public class SimulationsDataAccessor : DataAccessor<SimulationRecord>
    {
        public override string PrimaryKeyColumn => "simulation_id";
        public override string TableName => "simulations";

        /// <summary>
        /// Get a simulation by its code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public virtual IEnumerable<SimulationRecord> GetByCode(string code)
        {
            string sql = @$"SELECT {GetColumnsAsSql(SimulationRecord.Columns.Keys)} FROM {TableName} WHERE code = @code;";
            return Database.Query<SimulationRecord>(
                sql,
                SimulationRecord.FromRow,
                [
                    new($"@code", SqlDbType.NVarChar) { Value = code },
                ]
            );
        }
    }
}
