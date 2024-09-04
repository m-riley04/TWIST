using System.Data.SqlTypes;
using TWISTServer.Database.Records;
using System.Data;
using System.Data.SqlClient;

namespace TWISTServer.Database.DataAccessors
{
    public class UserDataAccessor : DataAccessor
    {
        public UserDataAccessor() { }

        public IEnumerable<UserRecord> GetAllUsers()
        {
            string sql = $"select user_id, email, username, password_hash, avatar_uri, type, creation_date, modification_date, login_date from users;";
            return Database.Query(
                sql,
                UserRecord.FromRow
            );
        }

        public int InsertUser(UserRecord user)
        {
            string sql = $"insert into users (email, username, password_hash, avatar_uri, type, creation_date, modification_date, login_date) " +
                $"values (@email, @username, @password_hash, @avatar_uri, @type, @creation_date, @modification_date, @login_date)";

            return Database.NonQuery(
                sql,
                new SqlParameter[]
                {
                    new SqlParameter("@email", SqlDbType.NVarChar, 100) { Value = user.Email },
                    new SqlParameter("@username", SqlDbType.NVarChar, 50) { Value = user.Username },
                    new SqlParameter("@password_hash", SqlDbType.NVarChar, 64) { Value = user.PasswordHash },
                    new SqlParameter("@avatar_uri", SqlDbType.NVarChar, 200) { Value = user.AvatarUri},
                    new SqlParameter("@type", SqlDbType.Int) { Value = user.Type },
                    new SqlParameter("@creation_date", SqlDbType.DateTime) { Value = user.CreationDate },
                    new SqlParameter("@modification_date", SqlDbType.DateTime) { Value = user.ModificationDate },
                    new SqlParameter("@login_date", SqlDbType.DateTime) { Value = user.LoginDate }
                }
            );
        }
    }
}
