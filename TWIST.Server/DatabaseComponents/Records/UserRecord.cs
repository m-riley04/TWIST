using System.Data;
using TWISTServer.Enums;

namespace TWISTServer.DatabaseComponents.Records
{
    public record UserRecord(int UserId, string Email, string Username, string PasswordHash, string? AvatarUri, 
        UserType Type, DateTime CreationDate, DateTime ModificationDate, DateTime LoginDate)
    {
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
