using System.Data.SqlTypes;
using TWISTServer.DatabaseComponents.Records;
using System.Data;
using System.Data.SqlClient;

namespace TWISTServer.DatabaseComponents.DataAccessors
{
    public class TeamDataAccessor : DataAccessor<TeamRecord>
    {
        public override string PrimaryKeyColumn => "team_id";
        public override string TableName => "teams";

        public IEnumerable<TeamRecord> GetTeamBySimulation(int simulationId)
        {
            string sql = @"select 
team_id, simulation_id, country, interest_group, members 
from teams WHERE
simulation_id = @simulation_id;";
            return Database.Query(
                sql,
                TeamRecord.FromRow,
                [
                    new("@simulation_id", SqlDbType.Int) { Value = simulationId },
                ]
            );
        }

        public IEnumerable<TeamRecord> GetTeamByCountry(int country)
        {
            string sql = @"select 
team_id, simulation_id, country, interest_group, members 
from teams WHERE
country = @country;";
            return Database.Query(
                sql,
                TeamRecord.FromRow,
                [
                    new("@country", SqlDbType.Int) { Value = country },
                ]
            );
        }

        public IEnumerable<TeamRecord> GetTeamByCountryAndSimulationId(int country, int simulationId)
        {
            string sql = @"select 
team_id, simulation_id, country, interest_group, members 
from teams WHERE
country = @country AND
simulation_id = @simulation_id;";
            return Database.Query(
                sql,
                TeamRecord.FromRow,
                [
                    new("@country", SqlDbType.Int) { Value = country },
                    new("@simulation_id", SqlDbType.Int) { Value = simulationId },
                ]
            );
        }
    }
}
