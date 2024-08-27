using System.Data;
using TWISTServer.Enums;

namespace TWISTServer.Database.Records
{
    public record AskRecord(int AskId, int TeamId, int SimulationId, string Description, int Points, StatusEnum Status)
    {
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
