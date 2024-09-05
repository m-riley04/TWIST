using System.Data;
using TWISTServer.Enums;

namespace TWISTServer.DatabaseComponents.Records
{
    public record ScoreRecord(int ScoreId, int TeamId, int SimulationId, int TotalScore, string Breakdown)
    {
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
