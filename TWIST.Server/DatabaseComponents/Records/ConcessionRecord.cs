using System.Data;
using TWISTServer.Enums;
using TWISTServer.Interfaces;

namespace TWISTServer.DatabaseComponents.Records
{
    public record ConcessionRecord(int ConcessionId, int TeamId, int SimulationId, string Description, int Points, StatusEnum Status) : IDatabaseRecord<ConcessionRecord>
    {
        public static Dictionary<string, SqlDbType> Columns { get; } = new Dictionary<string, SqlDbType>()
        {
            { "concession_id", SqlDbType.Int },
            { "team_id", SqlDbType.Int },
            { "simulation_id", SqlDbType.Int },
            { "description", SqlDbType.NVarChar },
            { "points", SqlDbType.Int },
            { "status", SqlDbType.Int }
        };

        public static ConcessionRecord FromRow(DataRow row)
        {
            return new ConcessionRecord(
                row.Field<int>("concession_id")
                , row.Field<int>("team_id")
                , row.Field<int>("simulation_id")
                , row.Field<string>("description") ?? ""
                , row.Field<int>("points")
                , (StatusEnum)row.Field<int>("status")
                );
        }
    }
}
