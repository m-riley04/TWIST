using System.Data.SqlClient;
using System.Data;
using TWISTServer.DatabaseComponents.Records;
using System.Reflection.Metadata;
using TWISTServer.Interfaces;

namespace TWISTServer.DatabaseComponents.DataAccessors
{
    public class AsksDataAccessor : DataAccessor<AskRecord>
    {
        public override string PrimaryKeyColumn { get; } = "ask_id";
        public override string TableName { get; } = "asks";

        public IEnumerable<AskRecord> GetAsksBySimulation(int simulationId)
        {
            string sql = @"select 
ask_id, team_id, simulation_id, description, points, status from asks 
WHERE 
simulation_id = @simulation_id;";
            return Database.Query(
                sql,
                AskRecord.FromRow,
                [
                    new("@simulation_id", SqlDbType.Int) { Value = simulationId },
                ]
            );
        }

        public IEnumerable<AskRecord> GetAsksByTeam(int teamId)
        {
            string sql = @"select 
ask_id, team_id, simulation_id, description, points, status from asks 
WHERE 
team_id = @team_id;";
            return Database.Query(
                sql,
                AskRecord.FromRow,
                [
                    new("@team_id", SqlDbType.Int) { Value = teamId },
                ]
            );
        }

        public IEnumerable<AskRecord> GetAsksBySimulationAndTeam(int simulationId, int teamId)
        {
            string sql = @"select 
ask_id, team_id, simulation_id, description, points, status from asks 
WHERE 
simulation_id = @simulation_id AND
team_id = @team_id;";
            return Database.Query(
                sql,
                AskRecord.FromRow,
                [
                    new("@simulation_id", SqlDbType.Int) { Value = simulationId },
                    new("@team_id", SqlDbType.Int) { Value = teamId },
                ]
            );
        }
    }
}
