using System.Data;
using TWISTServer.Enums;
using TWISTServer.Interfaces;

namespace TWISTServer.DatabaseComponents.Records
{
    public record ParticipantRecord(int ParticipantId, CountryEnum? Country, 
        ParticipantRoleEnum? Role, int SimulationId, 
        string Username, string Email) : IDatabaseRecord<ParticipantRecord>
    {
        public static Dictionary<string, SqlDbType> Columns { get; } = new Dictionary<string, SqlDbType>()
        {
            { "participant_id", SqlDbType.Int },
            { "country", SqlDbType.Int },
            { "role", SqlDbType.Int },
            { "simulation_id", SqlDbType.Int },
            { "username", SqlDbType.NVarChar },
            { "email", SqlDbType.NVarChar },
        };
        public static ParticipantRecord FromRow(DataRow row)
        {
            return new ParticipantRecord(
                row.Field<int>("participant_id")
                , Convert.ToBoolean(row.Field<int?>("country")) ? (CountryEnum)row.Field<int?>("role") : CountryEnum.None
                , Convert.ToBoolean(row.Field<int?>("role")) ? (ParticipantRoleEnum)row.Field<int?>("role") : ParticipantRoleEnum.None
                , row.Field<int>("simulation_id")
                , row.Field<string>("username") ?? ""
                , row.Field<string>("email") ?? ""
                );
        }
    }
}
