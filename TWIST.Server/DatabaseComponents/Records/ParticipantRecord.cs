using System.Data;
using System.Text.Json.Serialization;
using TWISTServer.Enums;
using TWISTServer.Interfaces;

namespace TWISTServer.DatabaseComponents.Records
{
    public record ParticipantRecord(
        [property: JsonPropertyName("participant_id")] int ParticipantId, 
        CountryEnum? Country, 
        ParticipantRoleEnum? Role,
        [property: JsonPropertyName("simulation_id")] int SimulationId, 
        string Username, 
        string Email,
        [property: JsonPropertyName("connection_id")] string? ConnectionId
    ) : IDatabaseRecord<ParticipantRecord>
    {
        public static Dictionary<string, SqlDbType> Columns { get; } = new Dictionary<string, SqlDbType>()
        {
            { "participant_id", SqlDbType.Int },
            { "country", SqlDbType.Int },
            { "role", SqlDbType.Int },
            { "simulation_id", SqlDbType.Int },
            { "username", SqlDbType.NVarChar },
            { "email", SqlDbType.NVarChar },
            { "connection_id", SqlDbType.NVarChar },
        };
        public static ParticipantRecord FromRow(DataRow row)
        {
            return new ParticipantRecord(
                row.Field<int>("participant_id")
                , Convert.ToBoolean(row.Field<int?>("country")) ? (CountryEnum)row.Field<int?>("role") : CountryEnum.NONE
                , Convert.ToBoolean(row.Field<int?>("role")) ? (ParticipantRoleEnum)row.Field<int?>("role") : ParticipantRoleEnum.NONE
                , row.Field<int>("simulation_id")
                , row.Field<string>("username") ?? ""
                , row.Field<string>("email") ?? ""
                , row.Field<string?>("connection_id") ?? ""
                );
        }
    }
}
