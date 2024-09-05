using System.Data;
using TWISTServer.Enums;

namespace TWISTServer.DatabaseComponents.Records
{
    public record TeamRecord(int TeamId, int SimulationId, CountryEnum Country, InterestGroupEnum InterestGroup, string Members)
    {
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
