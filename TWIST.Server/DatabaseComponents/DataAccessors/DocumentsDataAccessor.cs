using System.Data.SqlClient;
using System.Data;
using TWISTServer.DatabaseComponents.Records;
using System.Reflection.Metadata;

namespace TWISTServer.DatabaseComponents.DataAccessors
{
    public class DocumentsDataAccessor : DataAccessor<DocumentRecord>
    {
        public override string PrimaryKeyColumn => "document_id";
        public override string TableName => "documents";
        public IEnumerable<DocumentRecord> GetDocumentsByTeam(int teamId)
        {
            string sql = @"select 
document_id, simulation_id, team_id, type, body, editors, creation_date, modified_date from documents 
WHERE 
team_id = @team_id;";
            return Database.Query(
                sql,
                DocumentRecord.FromRow,
                [
                    new("@team_id", SqlDbType.Int) { Value = teamId },
                ]
            );
        }

        public IEnumerable<DocumentRecord> GetDocumentsBySimulation(int simulationId)
        {
            string sql = @"select 
document_id, simulation_id, team_id, type, body, editors, creation_date, modified_date from documents 
WHERE 
simulation_id = @simulation_id;";
            return Database.Query(
                sql,
                DocumentRecord.FromRow,
                [
                    new("@simulation_id", SqlDbType.Int) { Value = simulationId },
                ]
            );
        }
    }
}
