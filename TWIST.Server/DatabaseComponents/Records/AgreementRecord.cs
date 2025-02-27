using System.Data;
using System.Text.Json.Serialization;
using TWISTServer.Enums;
using TWISTServer.Interfaces;

namespace TWISTServer.DatabaseComponents.Records
{
    public record AgreementRecord(
        [property: JsonPropertyName("agreement_id")] int AgreementId,
        [property: JsonPropertyName("simulation_id")] int SimulationId, 
        AgreementType Type,
        [property: JsonPropertyName("type_id")] int TypeId,
        string Description, 
        int Points, 
        CountryEnum Country
    ) : IDatabaseRecord<AgreementRecord>
    {
        public static Dictionary<string, SqlDbType> Columns { get; } = new Dictionary<string, SqlDbType>()
        {
            { "agreement_id", SqlDbType.Int },
            { "simulation_id", SqlDbType.Int },
            { "type", SqlDbType.Int },
            { "type_id", SqlDbType.Int },
            { "description", SqlDbType.NVarChar },
            { "points", SqlDbType.Int },
            { "country", SqlDbType.Int },
        };

        public static AgreementRecord FromRow(DataRow row)
        {
            return new AgreementRecord(
                row.Field<int>("agreement_id")
                , row.Field<int>("simulation_id")
                , row.Field<AgreementType>("type")
                , row.Field<int>("type_id")
                , row.Field<string>("description") ?? ""
                , row.Field<int>("points")
                , row.Field<CountryEnum>("country")
                );
        }
    }
}
