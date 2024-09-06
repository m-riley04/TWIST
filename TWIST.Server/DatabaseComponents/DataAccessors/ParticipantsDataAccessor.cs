using System.Data.SqlClient;
using System.Data;
using TWISTServer.DatabaseComponents.Records;

namespace TWISTServer.DatabaseComponents.DataAccessors
{
    public class ParticipantsDataAccessor : DataAccessor<ParticipantRecord>
    {
        public override string PrimaryKeyColumn => "participant_id";
        public override string TableName => "participants";

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
    }
}
