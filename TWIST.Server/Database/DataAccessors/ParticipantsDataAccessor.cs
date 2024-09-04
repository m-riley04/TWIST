using System.Data.SqlClient;
using System.Data;
using TWISTServer.Database.Records;

namespace TWISTServer.Database.DataAccessors
{
    public class ParticipantsDataAccessor : DataAccessor
    {
        public IEnumerable<ParticipantRecord> GetAllParticipants()
        {
            string sql = $"select participant_id, team_id, role, user_id, simulation_id, username from participants;";
            return Database.Query(
                sql,
                ParticipantRecord.FromRow
            );
        }

        public IEnumerable<ParticipantRecord> GetParticipant(int participantId)
        {
            string sql = @"select 
participant_id, team_id, role, user_id, simulation_id, username 
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

        public IEnumerable<ParticipantRecord> GetParticipantsByTeam(int teamId)
        {
            string sql = @"select 
participant_id, team_id, role, user_id, simulation_id, username from participants
WHERE 
team_id = @team_id;";
            return Database.Query(
                sql,
                ParticipantRecord.FromRow,
                [
                    new("@team_id", SqlDbType.Int) { Value = teamId },
                ]
            );
        }

        public IEnumerable<ParticipantRecord> GetParticipantsBySimulation(int simulationId)
        {
            string sql = @"select 
participant_id, team_id, role, user_id, simulation_id, username from participants
WHERE 
simulation_id = @simulation_id;";
            return Database.Query(
                sql,
                ParticipantRecord.FromRow,
                [
                    new("@simulation_id", SqlDbType.Int) { Value = simulationId },
                ]
            );
        }

        public int Insert(ParticipantRecord participant)
        {
            string sql = @"
INSERT INTO participants 
(team_id, role, user_id, simulation_id, username) 
VALUES 
(@team_id, @role, @user_id, @simulation_id, @username)";

            SqlParameter[] parameters = [
                new("@team_id", SqlDbType.Int) { Value = participant.TeamId },
                new("@role", SqlDbType.Int) { Value = participant.Role },
                new("@user_id", SqlDbType.Int) { Value = participant.UserId },
                new("@simulation_id", SqlDbType.Int) { Value = participant.SimulationId },
                new("@username", SqlDbType.NVarChar, -1) { Value = participant.Username },
            ];

            return Database.NonQuery(sql, parameters);
        }

        public int Delete(int participantId)
        {
            string sql = @"
DELETE FROM participants 
WHERE participant_id = @participant_id;";

            SqlParameter[] parameters = [
                new("@participant_id", SqlDbType.Int) { Value = participantId},
            ];

            return Database.NonQuery(sql, parameters);
        }
    }
}
