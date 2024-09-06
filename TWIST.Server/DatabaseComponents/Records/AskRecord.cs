using System.Data;
using TWISTServer.Enums;
using TWISTServer.Interfaces;

namespace TWISTServer.DatabaseComponents.Records
{
    public record AskRecord(int AskId, int TeamId, int SimulationId, string Description, int Points, StatusEnum Status) : IDatabaseRecord<AskRecord>
    {
        public static Dictionary<string, SqlDbType> Columns { get; } = new Dictionary<string, SqlDbType>()
        {
            { "ask_id", SqlDbType.Int },
            { "team_id", SqlDbType.Int },
            { "simulation_id", SqlDbType.Int },
            { "description", SqlDbType.NVarChar },
            { "points", SqlDbType.Int },
            { "status", SqlDbType.Int }
        };

        public static AskRecord FromRow(DataRow row)
        {
            return new AskRecord(
                row.Field<int>("ask_id")
                , row.Field<int>("team_id")
                , row.Field<int>("simulation_id")
                , row.Field<string>("description") ?? ""
                , row.Field<int>("points")
                , (StatusEnum)row.Field<int>("status")
                );
        }
    }
}
