using System.Data.SqlClient;
using System.Data;
using TWISTServer.Database.Records;
using System.Reflection.Metadata;

namespace TWISTServer.Database.DataAccessors
{
    public class AsksDataAccessor : DataAccessor
    {
        public IEnumerable<AskRecord> GetAllAsks()
        {
            string sql = $"select ask_id, team_id, simulation_id, description, points, status from asks;";
            return Database.Query(
                sql,
                AskRecord.FromRow
            );
        }

        public IEnumerable<AskRecord> GetAsk(int askId)
        {
            string sql = @"select 
ask_id, team_id, simulation_id, description, points, status from asks 
WHERE 
ask_id = @ask_id;";
            return Database.Query(
                sql,
                AskRecord.FromRow,
                [
                    new("@ask_id", SqlDbType.Int) { Value = askId },
                ]
            );
        }

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

        public int InsertAsk(AskRecord ask)
        {
            string sql = @"
INSERT INTO asks 
(team_id, simulation_id, description, points, status) 
VALUES 
(@team_id, @simulation_id, @description, @points, @status)";

            SqlParameter[] parameters = [
                new("@team_id", SqlDbType.Int) { Value = ask.TeamId },
                new("@simulation_id", SqlDbType.Int) { Value = ask.SimulationId },
                new("@description", SqlDbType.NVarChar, -1) { Value = ask.Description },
                new("@points", SqlDbType.Int) { Value = ask.Points},
                new("@status", SqlDbType.Int) { Value = ask.Status},
            ];

            return Database.NonQuery(sql, parameters);
        }
    }
}
