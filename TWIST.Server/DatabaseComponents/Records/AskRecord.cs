using System.Data;
using TWISTServer.Enums;
using TWISTServer.Interfaces;

namespace TWISTServer.DatabaseComponents.Records
{
    public record AskRecord(
        int AskId, 
        int SimulationId, 
        string Description, 
        int Points, 
        StatusEnum Status,
        DateTime CreationDate,
        DateTime ModifiedDate,
        CountryEnum Country,
        int? LastEditor
    ) : IDatabaseRecord<AskRecord>
    {
        public static Dictionary<string, SqlDbType> Columns { get; } = new Dictionary<string, SqlDbType>()
        {
            { "ask_id", SqlDbType.Int },
            { "simulation_id", SqlDbType.Int },
            { "description", SqlDbType.NVarChar },
            { "points", SqlDbType.Int },
            { "status", SqlDbType.Int },
            { "creation_date", SqlDbType.DateTime },
            { "modified_date", SqlDbType.DateTime },
            { "country", SqlDbType.Int },
            { "last_editor", SqlDbType.Int },
        };

        public static AskRecord FromRow(DataRow row)
        {
            return new AskRecord(
                row.Field<int>("ask_id")
                , row.Field<int>("simulation_id")
                , row.Field<string>("description") ?? ""
                , row.Field<int>("points")
                , row.Field<StatusEnum>("status")
                , row.Field<DateTime>("creation_date")
                , row.Field<DateTime>("modified_date")
                , row.Field<CountryEnum>("country")
                , row.Field<int>("last_editor")
                );
        }
    }
}
