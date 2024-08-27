using System.Data;
using TWISTServer.Enums;

namespace TWISTServer.Database.Records
{
    public record SimulationRecord(int SimulationId, string Name, string Participants, DateTime StartDate, DateTime EndDate, bool Active)
    {
        public static SimulationRecord FromRow(DataRow row)
        {
            return new SimulationRecord(
                row.Field<int>("simulation_id")
                , row.Field<string>("name") ?? ""
                , row.Field<string>("participants") ?? ""
                , row.Field<DateTime>("start_date")
                , row.Field<DateTime>("end_date")
                , row.Field<bool>("active")
                );
        }
    }
}
