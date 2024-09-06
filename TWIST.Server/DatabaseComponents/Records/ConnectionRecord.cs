using System.Data;
using TWISTServer.Interfaces;

namespace TWISTServer.DatabaseComponents.Records
{
    public record ConnectionRecord(int ConnectionId, int SimulationId, string Participants, string SessionDescription, string IceCandidates) : IDatabaseRecord<ConnectionRecord>
    {
        public static Dictionary<string, SqlDbType> Columns { get; } = new Dictionary<string, SqlDbType>()
        {
            { "connection_id", SqlDbType.Int },
            { "simulation_id", SqlDbType.Int },
            { "participants", SqlDbType.NVarChar },
            { "session_description", SqlDbType.NVarChar },
            { "ice_candidates", SqlDbType.NVarChar }
        };

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
