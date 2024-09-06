using System.Data.SqlClient;
using System.Data;
using TWISTServer.DatabaseComponents.Records;
using System.Reflection.Metadata;

namespace TWISTServer.DatabaseComponents.DataAccessors
{
    public class ConnectionsDataAccessor : DataAccessor<ConnectionRecord>
    {
        public override string PrimaryKeyColumn => "connection_id";
        public override string TableName => "connections";

        public IEnumerable<ConnectionRecord> GetConnectionsBySimulation(int simulationId)
        {
            string sql = @"select 
connection_id, simulation_id, participants, session_description, ice_candidates from connections 
WHERE 
simulation_id = @simulation_id;";
            return Database.Query(
                sql,
                ConnectionRecord.FromRow,
                [
                    new("@simulation_id", SqlDbType.Int) { Value = simulationId },
                ]
            );
        }
    }
}
