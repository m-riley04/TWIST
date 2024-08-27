using System.Data;
using TWISTServer.Enums;

namespace TWISTServer.Database.Records
{
    public record ParticipantRecord(int ParticipantId, int TeamId, ParticipantRoleEnum Role, int? UserId)
    {
        public static ParticipantRecord FromRow(DataRow row)
        {
            return new ParticipantRecord(
                row.Field<int>("participant_id")
                , row.Field<int>("team_id")
                , (ParticipantRoleEnum)row.Field<int>("role")
                , row.Field<int?>("user_id")
                );
        }
    }
}
