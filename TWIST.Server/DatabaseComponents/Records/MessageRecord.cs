using System.Data;
using TWISTServer.Enums;

namespace TWISTServer.DatabaseComponents.Records
{
    public record MessageRecord(int MessageId, int SimulationId, int ParticipantId, int TeamId, string Body, DateTime Timestamp, string Attachments, string Reactions)
    {
        public static MessageRecord FromRow(DataRow row)
        {
            return new MessageRecord(
                row.Field<int>("message_id")
                , row.Field<int>("simulation_id")
                , row.Field<int>("participant_id")
                , row.Field<int>("team_id")
                , row.Field<string>("body") ?? ""
                , row.Field<DateTime>("timestamp")
                , row.Field<string>("attachments") ?? ""
                , row.Field<string>("reactions") ?? ""
                );
        }
    }
}
