using System.Data.SqlClient;
using System.Data;
using TWISTServer.DatabaseComponents.Records;
using System.Reflection.Metadata;

namespace TWISTServer.DatabaseComponents.DataAccessors
{
    public class DocumentsDataAccessor : DataAccessor
    {
        public IEnumerable<DocumentRecord> GetAllDocuments()
        {
            string sql = $"select document_id, simulation_id, team_id, type, body, editors, creation_date, modified_date from documents;";
            return Database.Query(
                sql,
                DocumentRecord.FromRow
            );
        }

        public IEnumerable<DocumentRecord> GetDocument(int documentId)
        {
            string sql = @"select 
document_id, simulation_id, team_id, type, body, editors, creation_date, modified_date from documents 
WHERE 
document_id = @document_id;";
            return Database.Query(
                sql,
                DocumentRecord.FromRow,
                [
                    new("@document_id", SqlDbType.Int) { Value = documentId },
                ]
            );
        }

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

        public int InsertDocument(DocumentRecord document)
        {
            string sql = @"
INSERT INTO documents 
(simulation_id, team_id, type, body, editors, creation_date, modified_date) 
VALUES 
(@simulation_id, @team_id, @type, @body, @editors, @creation_date, @modified_date)";

            SqlParameter[] parameters = [
                new("@simulation_id", SqlDbType.Int) { Value = document.SimulationId },
                new("@team_id", SqlDbType.Int) { Value = document.TeamId },
                new("@type", SqlDbType.Int) { Value = document.Type },
                new("@body", SqlDbType.NVarChar, -1) { Value = document.Body},
                new("@editors", SqlDbType.NVarChar, -1) { Value = document.Editors},
                new("@creation_date", SqlDbType.DateTime) { Value = document.CreationDate},
                new("@modified_date", SqlDbType.DateTime) { Value = document.ModifiedDate},
            ];

            return Database.NonQuery(sql, parameters);
        }
    }
}
