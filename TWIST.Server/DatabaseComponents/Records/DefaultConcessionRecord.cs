using System.Data;
using System.Text.Json.Serialization;
using TWISTServer.Enums;
using TWISTServer.Interfaces;

namespace TWISTServer.DatabaseComponents.Records
{
    public record DefaultConcessionRecord(
        [property: JsonPropertyName("default_concession_id")] int DefaultConcessionId,
        string Description,
        CountryEnum Country,
        [property: JsonPropertyName("creation_date")] DateTime CreationDate,
        [property: JsonPropertyName("modified_date")] DateTime ModifiedDate
    ) : IDatabaseRecord<DefaultConcessionRecord>
    {
        public static Dictionary<string, SqlDbType> Columns { get; } = new Dictionary<string, SqlDbType>()
        {
            { "default_concession_id", SqlDbType.Int },
            { "description", SqlDbType.NVarChar },
            { "country", SqlDbType.Int },
            { "creation_date", SqlDbType.DateTime },
            { "modified_date", SqlDbType.DateTime },
        };

        public static DefaultConcessionRecord FromRow(DataRow row)
        {
            return new DefaultConcessionRecord(
                row.Field<int>("default_concession_id")
                , row.Field<string>("description") ?? ""
                , row.Field<CountryEnum>("country")
                , row.Field<DateTime>("creation_date")
                , row.Field<DateTime>("modified_date")
                );
        }
    }
}
