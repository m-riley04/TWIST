using System.Data;
using System.Text.Json.Serialization;
using TWISTServer.Enums;
using TWISTServer.Interfaces;

namespace TWISTServer.DatabaseComponents.Records
{
    public record AgreementRecord(
        [property: JsonPropertyName("agreement_id")] int AgreementId,
        [property: JsonPropertyName("simulation_id")] int SimulationId, 
        string Description, 
        int Points, 
        StatusEnum Status,
        [property: JsonPropertyName("creation_date")] DateTime CreationDate,
        [property: JsonPropertyName("modified_date")] DateTime ModifiedDate,
        CountryEnum Country,
        [property: JsonPropertyName("last_editor")] int? LastEditor
    ) : IDatabaseRecord<AgreementRecord>
    {
        public static Dictionary<string, SqlDbType> Columns { get; } = new Dictionary<string, SqlDbType>()
        {
            { "agreement_id", SqlDbType.Int },
            { "simulation_id", SqlDbType.Int },
            { "description", SqlDbType.NVarChar },
            { "points", SqlDbType.Int },
            { "status", SqlDbType.Int },
            { "creation_date", SqlDbType.DateTime },
            { "modified_date", SqlDbType.DateTime },
            { "country", SqlDbType.Int },
            { "last_editor", SqlDbType.Int },
        };

        public static AgreementRecord FromRow(DataRow row)
        {
            return new AgreementRecord(
                row.Field<int>("agreement_id")
                , row.Field<int>("simulation_id")
                , row.Field<string>("description") ?? ""
                , row.Field<int>("points")
                , row.Field<StatusEnum>("status")
                , row.Field<DateTime>("creation_date")
                , row.Field<DateTime>("modified_date")
                , row.Field<CountryEnum>("country")
                , row.Field<int?>("last_editor")
                );
        }
    }
}
