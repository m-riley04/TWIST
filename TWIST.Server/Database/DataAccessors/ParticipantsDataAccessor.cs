using System.Data.SqlClient;
using System.Data;
using TWISTServer.Database.Records;

namespace TWISTServer.Database.DataAccessors
{
    public class ParticipantsDataAccessor : DataAccessor
    {
        public IEnumerable<ParticipantRecord> GetAllParticipants()
        {
            string sql = $"select participant_id, team_id, role, user_id from participants;";
            return Database.Query(
                sql,
                ParticipantRecord.FromRow
            );
        }

        public IEnumerable<ParticipantRecord> GetParticipant(int participantId)
        {
            string sql = @"select 
participant_id, team_id, role, user_id 
from participants WHERE 
participant_id = @participant_id;";
            return Database.Query(
                sql,
                ParticipantRecord.FromRow,
                [
                    new("@participant_id", SqlDbType.Int) { Value = participantId },
                ]
            );
        }

        public IEnumerable<ParticipantRecord> GetParticipantsByTeamId(int teamId)
        {
            string sql = @"select 
participant_id, team_id, role, user_id 
from participants WHERE 
team_id = @team_id;";
            return Database.Query(
                sql,
                ParticipantRecord.FromRow,
                [
                    new("@team_id", SqlDbType.Int) { Value = teamId },
                ]
            );
        }

        public int InsertParticipant(ParticipantRecord participant)
        {
            string sql = @"
INSERT INTO participants 
(team_id, role, user_id) 
VALUES 
(@team_id, @role, @user_id)";

            SqlParameter[] parameters = [
                new("@team_id", SqlDbType.Int) { Value = participant.TeamId },
                new("@role", SqlDbType.Int) { Value = participant.Role },
                new("@user_id", SqlDbType.Int) { Value = participant.UserId },
            ];

            return Database.NonQuery(sql, parameters);
        }
    }
}
