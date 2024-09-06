using System.Data;
using TWISTServer.Enums;
using TWISTServer.Interfaces;

namespace TWISTServer.DatabaseComponents.Records
{
    public record TeamRecord(int TeamId, int SimulationId, CountryEnum Country, InterestGroupEnum InterestGroup, string Members) : IDatabaseRecord<TeamRecord>
    {

        public static Dictionary<string, SqlDbType> Columns { get; } = new Dictionary<string, SqlDbType>()
        {
            { "team_id", SqlDbType.Int },
            { "simulation_id", SqlDbType.Int },
            { "country", SqlDbType.Int },
            { "interest_group", SqlDbType.Int },
            { "members", SqlDbType.NVarChar },
        };

        public static TeamRecord FromRow(DataRow row)
        {
            return new TeamRecord(
                row.Field<int>("team_id")
                , row.Field<int>("simulation_id")
                , (CountryEnum)row.Field<int>("country")
                , (InterestGroupEnum)row.Field<int>("interest_group")
                , row.Field<string>("members") ?? ""
                );
        }
    }
}
