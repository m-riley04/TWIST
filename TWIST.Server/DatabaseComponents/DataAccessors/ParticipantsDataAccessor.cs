using System.Data.SqlClient;
using System.Data;
using TWISTServer.DatabaseComponents.Records;
using TWISTServer.Enums;

namespace TWISTServer.DatabaseComponents.DataAccessors
{
    public class ParticipantsDataAccessor : DataAccessor<ParticipantRecord>
    {
        public override string PrimaryKeyColumn => "participant_id";
        public override string TableName => "participants";

        public IEnumerable<ParticipantRecord> GetParticipantsByCountry(CountryEnum country)
        {
            string sql = @$"select 
{GetColumnsAsSql(ParticipantRecord.Columns.Keys)} from {TableName}
WHERE 
country = @country;";
            return Database.Query(
                sql,
                ParticipantRecord.FromRow,
                [
                    new("@country", SqlDbType.Int) { Value = country },
                ]
            );
        }

        public IEnumerable<ParticipantRecord> GetParticipantsByConnectionId(string connectionId)
        {
            string sql = @$"select 
{GetColumnsAsSql(ParticipantRecord.Columns.Keys)} from {TableName}
WHERE 
connection_id = @connection_id;";
            return Database.Query(
                sql,
                ParticipantRecord.FromRow,
                [
                    new("@connection_id", SqlDbType.Int) { Value = connectionId },
                ]
            );
        }

        public IEnumerable<ParticipantRecord> GetParticipantsBySimulation(int simulationId)
        {
            string sql = @$"select 
{GetColumnsAsSql(ParticipantRecord.Columns.Keys)} from {TableName}
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

        public IEnumerable<ParticipantRecord> GetParticipantFromSimulationAndEmail(int simulationId, string email)
        {
            string sql = @$"select
{GetColumnsAsSql(ParticipantRecord.Columns.Keys)} from {TableName}
WHERE
simulation_id = @simulation_id
AND email = @email;";
            return Database.Query(
                sql,
                ParticipantRecord.FromRow,
                [
                    new("@simulation_id", SqlDbType.Int) { Value = simulationId },
                    new("@email", SqlDbType.NVarChar) { Value = email },
                ]
            );
        }

        public void UpdateParticipantConnectionId(int participantId, string? connectionId)
        {
            string sql = @$"UPDATE {TableName}
SET connection_id = @connection_id
WHERE participant_id = @participant_id;";
            Database.NonQuery(
                sql,
                [
                    new("@connection_id", SqlDbType.NVarChar) { Value = connectionId },
                    new("@participant_id", SqlDbType.Int) { Value = participantId },
                ]
            );
        }
    }
}
