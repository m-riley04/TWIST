using System.Data;
using TWISTServer.Enums;
using TWISTServer.Interfaces;

namespace TWISTServer.DatabaseComponents.Records
{
    public record DocumentRecord(int DocumentId, int SimulationId, int TeamId, DocumentTypeEnum Type, 
        string Body, string Editors, DateTime CreationDate, DateTime ModifiedDate) : IDatabaseRecord<DocumentRecord>
    {
        public static Dictionary<string, SqlDbType> Columns { get; } = new Dictionary<string, SqlDbType>()
        {
            { "document_id", SqlDbType.Int },
            { "simulation_id", SqlDbType.Int },
            { "team_id", SqlDbType.Int },
            { "type", SqlDbType.Int },
            { "body", SqlDbType.NVarChar },
            { "editors", SqlDbType.NVarChar },
            { "creation_date", SqlDbType.DateTime },
            { "modified_date", SqlDbType.DateTime }
        };

        public static DocumentRecord FromRow(DataRow row)
        {
            return new DocumentRecord(
                row.Field<int>("document_id")
                , row.Field<int>("simulation_id")
                , row.Field<int>("team_id")
                , (DocumentTypeEnum)row.Field<int>("type")
                , row.Field<string>("body") ?? ""
                , row.Field<string>("editors") ?? ""
                , row.Field<DateTime>("creation_date")
                , row.Field<DateTime>("modified_date")
                );
        }
    }
}
