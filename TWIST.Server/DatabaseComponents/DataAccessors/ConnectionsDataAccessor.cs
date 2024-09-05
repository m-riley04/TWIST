using System.Data.SqlClient;
using System.Data;
using TWISTServer.DatabaseComponents.Records;
using System.Reflection.Metadata;

namespace TWISTServer.DatabaseComponents.DataAccessors
{
    public class ConnectionsDataAccessor : DataAccessor
    {
        public IEnumerable<ConnectionRecord> GetAllConnections()
        {
            string sql = $"select connection_id, simulation_id, participants, session_description, ice_candidates from connections;";
            return Database.Query(
                sql,
                ConnectionRecord.FromRow
            );
        }

        public IEnumerable<ConnectionRecord> GetConnection(int connectionId)
        {
            string sql = @"select 
connection_id, simulation_id, participants, session_description, ice_candidates from connections 
WHERE 
connection_id = @connection_id;";
            return Database.Query(
                sql,
                ConnectionRecord.FromRow,
                [
                    new("@connection_id", SqlDbType.Int) { Value = connectionId },
                ]
            );
        }

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

        public int InsertConnection(ConnectionRecord connection)
        {
            string sql = @"
INSERT INTO connections 
(simulation_id, participants, session_description, ice_candidates) 
VALUES 
(@simulation_id, @participants, @session_description, @ice_candidates)";

            SqlParameter[] parameters = [
                new("@simulation_id", SqlDbType.Int) { Value = connection.SimulationId },
                new("@participants", SqlDbType.NVarChar, -1) { Value = connection.Participants },
                new("@session_description", SqlDbType.NVarChar, -1) { Value = connection.SessionDescription },
                new("@ice_candidates", SqlDbType.NVarChar, -1) { Value = connection.IceCandidates},
            ];

            return Database.NonQuery(sql, parameters);
        }
    }
}
