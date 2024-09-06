using System.Data;
using TWISTServer.Enums;
using TWISTServer.Interfaces;

namespace TWISTServer.DatabaseComponents.Records
{
    public record UserRecord(int UserId, string Email, string Username, string PasswordHash, string? AvatarUri, 
        UserType Type, DateTime CreationDate, DateTime ModificationDate, DateTime LoginDate) : IDatabaseRecord<UserRecord>
    {

        public static Dictionary<string, SqlDbType> Columns { get; } = new Dictionary<string, SqlDbType>()
        {
            { "user_id", SqlDbType.Int },
            { "email", SqlDbType.NVarChar },
            { "username", SqlDbType.NVarChar },
            { "password_hash", SqlDbType.DateTime },
            { "avatar_uri", SqlDbType.DateTime },
            { "type", SqlDbType.Bit },
            { "creation_date", SqlDbType.DateTime },
            { "modification_date", SqlDbType.DateTime },
            { "login_date", SqlDbType.DateTime },
        };

        public static UserRecord FromRow(DataRow row)
        {
            return new UserRecord(
                row.Field<int>("user_id")
                , row.Field<string>("email") ?? ""
                , row.Field<string>("username") ?? ""
                , row.Field<string>("password_hash") ?? ""
                , row.Field<string?>("avatar_uri") ?? ""
                , (UserType)row.Field<int>("type")
                , row.Field<DateTime>("creation_date")
                , row.Field<DateTime>("modification_date")
                , row.Field<DateTime>("login_date")
                );
        }
    }
}
