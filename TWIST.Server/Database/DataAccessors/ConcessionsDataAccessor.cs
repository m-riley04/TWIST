using System.Data.SqlClient;
using System.Data;
using TWISTServer.Database.Records;
using System.Reflection.Metadata;

namespace TWISTServer.Database.DataAccessors
{
    public class ConcessionsDataAccessor : DataAccessor
    {
        public IEnumerable<ConcessionRecord> GetAllConcessions()
        {
            string sql = $"select concession_id, team_id, simulation_id, description, points, status from concessions;";
            return Database.Query(
                sql,
                ConcessionRecord.FromRow
            );
        }

        public IEnumerable<ConcessionRecord> GetConcession(int concessionId)
        {
            string sql = @"select 
concession_id, team_id, simulation_id, description, points, status from concessions 
WHERE 
concession_id = @concession_id;";
            return Database.Query(
                sql,
                ConcessionRecord.FromRow,
                [
                    new("@concession_id", SqlDbType.Int) { Value = concessionId },
                ]
            );
        }

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

        public int InsertConcession(ConcessionRecord concession)
        {
            string sql = @"
INSERT INTO concessions 
(team_id, simulation_id, description, points, status) 
VALUES 
(@team_id, @simulation_id, @description, @points, @status)";

            SqlParameter[] parameters = [
                new("@team_id", SqlDbType.Int) { Value = concession.TeamId },
                new("@simulation_id", SqlDbType.Int) { Value = concession.SimulationId },
                new("@description", SqlDbType.NVarChar, -1) { Value = concession.Description },
                new("@points", SqlDbType.Int) { Value = concession.Points},
                new("@status", SqlDbType.Int) { Value = concession.Status},
            ];

            return Database.NonQuery(sql, parameters);
        }
    }
}
