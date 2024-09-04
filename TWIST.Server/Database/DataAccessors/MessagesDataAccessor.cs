using System.Data.SqlClient;
using System.Data;
using TWISTServer.Database.Records;

namespace TWISTServer.Database.DataAccessors
{
    public class MessagesDataAccessor : DataAccessor
    {
        public IEnumerable<MessageRecord> GetAllMessages()
        {
            string sql = $"select message_id, simulation_id, participant_id, team_id, body, timestamp, attachments, reactions from messages;";
            return Database.Query(
                sql,
                MessageRecord.FromRow
            );
        }

        public IEnumerable<MessageRecord> GetMessage(int messageId)
        {
            string sql = @"select 
message_id, simulation_id, participant_id, team_id, body, timestamp, attachments, reactions from messages 
WHERE 
message_id = @message_id;";
            return Database.Query(
                sql,
                MessageRecord.FromRow,
                [
                    new("@message_id", SqlDbType.Int) { Value = messageId },
                ]
            );
        }

        public int InsertMessage(MessageRecord message)
        {
            string sql = @"
INSERT INTO messages 
(simulation_id, participant_id, team_id, body, timestamp, attachments, reactions) 
VALUES 
(@simulation_id, @participant_id, @team_id, @body, @timestamp, @attachments, @reactions)";

            SqlParameter[] parameters = [
                new("@simulation_id", SqlDbType.Int) { Value = message.SimulationId },
                new("@participant_id", SqlDbType.Int) { Value = message.ParticipantId },
                new("@team_id", SqlDbType.Int) { Value = message.TeamId },
                new("@body", SqlDbType.NVarChar, -1) { Value = message.Body},
                new("@timestamp", SqlDbType.DateTime) { Value = message.Timestamp},
                new("@attachments", SqlDbType.NVarChar, -1) { Value = message.Attachments},
                new("@reactions", SqlDbType.NVarChar, -1) { Value = message.Reactions},
            ];

            return Database.NonQuery(sql, parameters);
        }
    }
}
