using System.Data;
using TWISTServer.Enums;

namespace TWISTServer.DatabaseComponents.Records
{
    public record ConcessionRecord(int ConcessionId, int TeamId, int SimulationId, string Description, int Points, StatusEnum Status)
    {
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
