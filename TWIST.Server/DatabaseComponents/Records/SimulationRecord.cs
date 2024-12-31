using System.Data;
using TWISTServer.Enums;
using TWISTServer.Interfaces;

namespace TWISTServer.DatabaseComponents.Records
{
    public record SimulationRecord(int SimulationId, string Name, string Participants, 
        DateTime StartDate, DateTime EndDate, bool Active, string Responses, string Asks, 
        string Concessions, int Round, string Code) : IDatabaseRecord<SimulationRecord>
    {
        public static Dictionary<string, SqlDbType> Columns { get; } = new Dictionary<string, SqlDbType>()
        {
            { "simulation_id", SqlDbType.Int },
            { "name", SqlDbType.NVarChar },
            { "participants", SqlDbType.NVarChar },
            { "start_date", SqlDbType.DateTime },
            { "end_date", SqlDbType.DateTime },
            { "active", SqlDbType.Bit },
            { "responses", SqlDbType.NVarChar },
            { "asks", SqlDbType.NVarChar },
            { "concessions", SqlDbType.NVarChar },
            { "round", SqlDbType.Int },
            { "code", SqlDbType.NVarChar },
        };

        public static SimulationRecord FromRow(DataRow row)
        {
            return new SimulationRecord(
                row.Field<int>("simulation_id")
                , row.Field<string>("name") ?? ""
                , row.Field<string>("participants") ?? ""
                , row.Field<DateTime>("start_date")
                , row.Field<DateTime>("end_date")
                , row.Field<bool>("active")
                , row.Field<string>("responses") ?? ""
                , row.Field<string>("asks") ?? ""
                , row.Field<string>("concessions") ?? ""
                , row.Field<int>("round")
                , row.Field<string>("code") ?? ""
                );
        }
    }
}
