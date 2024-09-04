using System.Data.SqlTypes;
using TWISTServer.Database.Records;
using System.Data;
using System.Data.SqlClient;

namespace TWISTServer.Database.DataAccessors
{
    public class UserDataAccessor : DataAccessor
    {
        public IEnumerable<UserRecord> GetAllUsers()
        {
            string sql = $"select user_id, email, username, password_hash, avatar_uri, type, creation_date, modification_date, login_date from users;";
            return Database.Query(
                sql,
                UserRecord.FromRow
            );
        }

        public IEnumerable<UserRecord> GetUser(int userId)
        {
            string sql = @"select 
user_id, email, username, password_hash, avatar_uri, type, creation_date, modification_date, login_date 
from 
users
WHERE
user_id = @user_id;";
            return Database.Query(
                sql,
                UserRecord.FromRow,
                [
                    new("@user_id", SqlDbType.Int) { Value = userId },
                ]
            );
        }

        public int InsertUser(UserRecord user)
        {
            string sql = @"
INSERT INTO users 
(email, username, password_hash, avatar_uri, type, creation_date, modification_date, login_date) 
VALUES 
(@email, @username, @password_hash, @avatar_uri, @type, @creation_date, @modification_date, @login_date)";

            SqlParameter[] parameters = [
                new("@email", SqlDbType.NVarChar, -1) { Value = user.Email },
                new("@username", SqlDbType.NVarChar, -1) { Value = user.Username },
                new("@password_hash", SqlDbType.NVarChar, 64) { Value = user.PasswordHash },
                new("@avatar_uri", SqlDbType.NVarChar, -1) { Value = user.AvatarUri},
                new("@type", SqlDbType.Int) { Value = user.Type },
                new("@creation_date", SqlDbType.DateTime) { Value = user.CreationDate },
                new("@modification_date", SqlDbType.DateTime) { Value = user.ModificationDate },
                new("@login_date", SqlDbType.DateTime) { Value = user.LoginDate }
            ];

            return Database.NonQuery(sql, parameters);
        }
    }
}
