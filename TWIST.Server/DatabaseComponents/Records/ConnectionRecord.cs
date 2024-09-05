using System.Data;

namespace TWISTServer.DatabaseComponents.Records
{
    public record ConnectionRecord(int ConnectionId, int SimulationId, string Participants, string SessionDescription, string IceCandidates)
    {
        public static ConnectionRecord FromRow(DataRow row)
        {
            return new ConnectionRecord(
                row.Field<int>("connection_id")
                , row.Field<int>("simulation_id")
                , row.Field<string>("participants") ?? ""
                , row.Field<string>("session_description") ?? ""
                , row.Field<string>("ice_candidates") ?? ""
                );
        }
    }
}
