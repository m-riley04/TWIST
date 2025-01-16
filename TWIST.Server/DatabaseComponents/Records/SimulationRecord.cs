using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using TWISTServer.Enums;
using TWISTServer.Interfaces;

namespace TWISTServer.DatabaseComponents.Records
{
    public record SimulationRecord(
        [property: JsonPropertyName("simulation_id")] int SimulationId,
        string Code,
        string Name, 
        [property: JsonPropertyName("start_date")] DateTime StartDate,
        [property: JsonPropertyName("end_date")] DateTime? EndDate,
        [property: JsonPropertyName("modified_date")] DateTime? ModifiedDate,
        RoundEnum Round,
        SimulationStateEnum State,
        bool Active,
        [property: JsonPropertyName("instructor_id")] int InstructorId
    ) : IDatabaseRecord<SimulationRecord>
    {
        public static Dictionary<string, SqlDbType> Columns { get; } = new Dictionary<string, SqlDbType>()
        {
            { "simulation_id", SqlDbType.Int },
            { "code", SqlDbType.NVarChar },
            { "name", SqlDbType.NVarChar },
            { "start_date", SqlDbType.DateTime },
            { "end_date", SqlDbType.DateTime },
            { "modified_date", SqlDbType.DateTime },
            { "round", SqlDbType.Int },
            { "state", SqlDbType.Int },
            { "active", SqlDbType.Bit },
            { "instructor_id", SqlDbType.Int },
        };

        public static SimulationRecord FromRow(DataRow row)
        {
            return new SimulationRecord(
                row.Field<int>("simulation_id")
                , row.Field<string>("code") ?? ""
                , row.Field<string>("name") ?? ""
                , row.Field<DateTime>("start_date")
                , row.Field<DateTime?>("end_date")
                , row.Field<DateTime?>("modified_date")
                , row.Field<RoundEnum>("round")
                , row.Field<SimulationStateEnum>("state")
                , row.Field<bool>("active")
                , row.Field<int>("instructor_id")
                );
        }
    }
}
