using System.Data;
using System.Text.Json.Serialization;
using TWISTServer.Enums;
using TWISTServer.Interfaces;

namespace TWISTServer.DatabaseComponents.Records
{
    public record DefaultAskRecord(
        [property: JsonPropertyName("default_ask_id")] int DefaultAskId,
        string Description,
        CountryEnum Country,
        [property: JsonPropertyName("creation_date")] DateTime CreationDate,
        [property: JsonPropertyName("modified_date")] DateTime ModifiedDate
    ) : IDatabaseRecord<DefaultAskRecord>
    {
        public static Dictionary<string, SqlDbType> Columns { get; } = new Dictionary<string, SqlDbType>()
        {
            { "default_ask_id", SqlDbType.Int },
            { "description", SqlDbType.NVarChar },
            { "country", SqlDbType.Int },
            { "creation_date", SqlDbType.DateTime },
            { "modified_date", SqlDbType.DateTime },
        };

        public static DefaultAskRecord FromRow(DataRow row)
        {
            return new DefaultAskRecord(
                row.Field<int>("default_ask_id")
                , row.Field<string>("description") ?? ""
                , row.Field<CountryEnum>("country")
                , row.Field<DateTime>("creation_date")
                , row.Field<DateTime>("modified_date")
                );
        }
    }
}
