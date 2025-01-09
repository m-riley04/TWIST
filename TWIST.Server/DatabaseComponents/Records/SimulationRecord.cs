using System.Data;
using System.Text.Json;
using TWISTServer.Enums;
using TWISTServer.Interfaces;

namespace TWISTServer.DatabaseComponents.Records
{
    public record SimulationRecord(int SimulationId, string Name, IEnumerable<int> Participants, 
        DateTime StartDate, DateTime? EndDate, bool Active, string? Responses, string? Asks, 
        string? Concessions, int Round, string Code) : IDatabaseRecord<SimulationRecord>
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
            // Deserialize participants list
            var participantsJson = row.Field<string>("participants") ?? "[]";
            var participants = JsonSerializer.Deserialize<IEnumerable<int>>(participantsJson) ?? [];

            return new SimulationRecord(
                row.Field<int>("simulation_id")
                , row.Field<string>("name") ?? ""
                , participants
                , row.Field<DateTime>("start_date")
                , row.Field<DateTime?>("end_date")
                , row.Field<bool>("active")
                , row.Field<string?>("responses")
                , row.Field<string?>("asks")
                , row.Field<string?>("concessions")
                , row.Field<int>("round")
                , row.Field<string>("code") ?? ""
                );
        }
    }
}
