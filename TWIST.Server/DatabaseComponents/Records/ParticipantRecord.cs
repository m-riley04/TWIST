using System.Data;
using TWISTServer.Enums;
using TWISTServer.Interfaces;

namespace TWISTServer.DatabaseComponents.Records
{
    public record ParticipantRecord(int ParticipantId, int TeamId, ParticipantRoleEnum Role, int? UserId, int SimulationId, string Username) : IDatabaseRecord<ParticipantRecord>
    {

        public static Dictionary<string, SqlDbType> Columns { get; } = new Dictionary<string, SqlDbType>()
        {
            { "participant_id", SqlDbType.Int },
            { "team_id", SqlDbType.Int },
            { "role", SqlDbType.Int },
            { "user_id", SqlDbType.Int },
            { "simulation_id", SqlDbType.Int },
            { "username", SqlDbType.NVarChar },
        };
        public static ParticipantRecord FromRow(DataRow row)
        {
            return new ParticipantRecord(
                row.Field<int>("participant_id")
                , row.Field<int>("team_id")
                , (ParticipantRoleEnum)row.Field<int>("role")
                , row.Field<int?>("user_id")
                , row.Field<int>("simulation_id")
                , row.Field<string>("username") ?? ""
                );
        }
    }
}
