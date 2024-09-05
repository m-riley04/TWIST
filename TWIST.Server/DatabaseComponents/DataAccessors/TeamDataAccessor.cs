using System.Data.SqlTypes;
using TWISTServer.DatabaseComponents.Records;
using System.Data;
using System.Data.SqlClient;

namespace TWISTServer.DatabaseComponents.DataAccessors
{
    public class TeamDataAccessor : DataAccessor
    {
        public IEnumerable<TeamRecord> GetAllTeams()
        {
            string sql = $"select team_id, simulation_id, country, interest_group, members from teams;";
            return Database.Query(
                sql,
                TeamRecord.FromRow
            );
        }

        public IEnumerable<TeamRecord> GetTeam(int teamId)
        {
            string sql = @"select 
team_id, simulation_id, country, interest_group, members 
from teams WHERE
team_id = @team_id;";
            return Database.Query(
                sql,
                TeamRecord.FromRow,
                [
                    new("@team_id", SqlDbType.Int) { Value = teamId },
                ]
            );
        }

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

        public int InsertTeam(TeamRecord team)
        {
            string sql = @"
INSERT INTO teams 
(simulation_id, country, interest_group, members) 
VALUES 
(@simulation_id, @country, @interest_group, @members)";

            SqlParameter[] parameters = [
                new("@simulation_id", SqlDbType.Int) { Value = team.SimulationId },
                new("@country", SqlDbType.Int) { Value = team.Country },
                new("@interest_group", SqlDbType.Int) { Value = team.InterestGroup },
                new("@members", SqlDbType.NVarChar, -1) { Value = team.Members},
            ];

            return Database.NonQuery(sql, parameters);
        }
    }
}
