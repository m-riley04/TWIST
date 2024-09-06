using System.Data;
using TWISTServer.Enums;
using TWISTServer.Interfaces;

namespace TWISTServer.DatabaseComponents.Records
{
    public record ScoreRecord(int ScoreId, int TeamId, int SimulationId, int TotalScore, string Breakdown) : IDatabaseRecord<ScoreRecord>
    {
        public static Dictionary<string, SqlDbType> Columns { get; } = new Dictionary<string, SqlDbType>()
        {
            { "score_id", SqlDbType.Int },
            { "team_id", SqlDbType.Int },
            { "simulation_id", SqlDbType.Int },
            { "total_score", SqlDbType.Int },
            { "breakdown", SqlDbType.NVarChar },
        };

        public static ScoreRecord FromRow(DataRow row)
        {
            return new ScoreRecord(
                row.Field<int>("score_id")
                , row.Field<int>("team_id")
                , row.Field<int>("simulation_id")
                , row.Field<int>("total_score")
                , row.Field<string>("breakdown") ?? ""
                );
        }
    }
}
