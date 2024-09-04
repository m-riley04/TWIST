using System.Data.SqlClient;
using System.Data;
using TWISTServer.Database.Records;

namespace TWISTServer.Database.DataAccessors
{
    public class ScoreDataAccessor : DataAccessor
    {
        public IEnumerable<ScoreRecord> GetAllScores()
        {
            string sql = $"select score_id, team_id, simulation_id, total_score, breakdown from scores;";
            return Database.Query(
                sql,
                ScoreRecord.FromRow
            );
        }

        public IEnumerable<ScoreRecord> GetScore(int scoreId)
        {
            string sql = @"select 
score_id, team_id, simulation_id, total_score, breakdown 
from scores WHERE 
score_id = @score_id;";
            return Database.Query(
                sql,
                ScoreRecord.FromRow,
                [
                    new("@score_id", SqlDbType.Int) { Value = scoreId },
                ]
            );
        }

        public int InsertScore(ScoreRecord score)
        {
            string sql = @"
INSERT INTO scores 
(team_id, simulation_id, total_score, breakdown) 
VALUES 
(@team_id, @simulation_id, @total_score, @breakdown)";

            SqlParameter[] parameters = [
                new("@team_id", SqlDbType.Int) { Value = score.TeamId },
                new("@simulation_id", SqlDbType.Int) { Value = score.SimulationId },
                new("@total_score", SqlDbType.Int) { Value = score.TotalScore },
                new("@breakdown", SqlDbType.NVarChar, -1) { Value = score.Breakdown},
            ];

            return Database.NonQuery(sql, parameters);
        }
    }
}
