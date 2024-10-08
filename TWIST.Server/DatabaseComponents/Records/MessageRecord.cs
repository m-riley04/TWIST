﻿using System.Data;
using TWISTServer.Enums;
using TWISTServer.Interfaces;

namespace TWISTServer.DatabaseComponents.Records
{
    public record MessageRecord(int MessageId, int SimulationId, int ParticipantId, int TeamId, string Body, 
        DateTime Timestamp, string Attachments, string Reactions) : IDatabaseRecord<MessageRecord>
    {
        public static Dictionary<string, SqlDbType> Columns { get; } = new Dictionary<string, SqlDbType>()
        {
            { "message_id", SqlDbType.Int },
            { "simulation_id", SqlDbType.Int },
            { "participant_id", SqlDbType.Int },
            { "team_id", SqlDbType.Int },
            { "body", SqlDbType.NVarChar },
            { "timestamp", SqlDbType.DateTime },
            { "attachments", SqlDbType.NVarChar },
            { "reactions", SqlDbType.NVarChar }
        };

        public static MessageRecord FromRow(DataRow row)
        {
            return new MessageRecord(
                row.Field<int>("message_id")
                , row.Field<int>("simulation_id")
                , row.Field<int>("participant_id")
                , row.Field<int>("team_id")
                , row.Field<string>("body") ?? ""
                , row.Field<DateTime>("timestamp")
                , row.Field<string>("attachments") ?? ""
                , row.Field<string>("reactions") ?? ""
                );
        }
    }
}
