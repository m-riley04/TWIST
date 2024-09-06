using System.Data.SqlClient;
using System.Data;
using TWISTServer.DatabaseComponents.Records;
using System.Reflection.Metadata;
using TWISTServer.Interfaces;

namespace TWISTServer.DatabaseComponents.DataAccessors
{
    public class ConcessionsDataAccessor : DataAccessor<ConcessionRecord>, IDataAccessor<ConcessionRecord>
    {
        public override string PrimaryKeyColumn => "concession_id";

        public override string TableName => "concessions";

        public IEnumerable<ConcessionRecord> GetConcessionsBySimulation(int simulationId)
        {
            string sql = @"select 
concession_id, team_id, simulation_id, description, points, status from concessions 
WHERE 
simulation_id = @simulation_id;";
            return Database.Query(
                sql,
                ConcessionRecord.FromRow,
                [
                    new("@simulation_id", SqlDbType.Int) { Value = simulationId },
                ]
            );
        }

        public IEnumerable<ConcessionRecord> GetConcessionsByTeam(int teamId)
        {
            string sql = @"select 
concession_id, team_id, simulation_id, description, points, status from concessions 
WHERE 
team_id = @team_id;";
            return Database.Query(
                sql,
                ConcessionRecord.FromRow,
                [
                    new("@team_id", SqlDbType.Int) { Value = teamId },
                ]
            );
        }

        public IEnumerable<ConcessionRecord> GetConcessionsBySimulationAndTeam(int simulationId, int teamId)
        {
            string sql = @"select 
concession_id, team_id, simulation_id, description, points, status from concessions 
WHERE 
simulation_id = @simulation_id AND
team_id = @team_id;";
            return Database.Query(
                sql,
                ConcessionRecord.FromRow,
                [
                    new("@simulation_id", SqlDbType.Int) { Value = simulationId },
                    new("@team_id", SqlDbType.Int) { Value = teamId },
                ]
            );
        }
    }
}
