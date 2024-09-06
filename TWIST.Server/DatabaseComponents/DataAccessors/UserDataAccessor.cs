using System.Data.SqlTypes;
using TWISTServer.DatabaseComponents.Records;
using System.Data;
using System.Data.SqlClient;

namespace TWISTServer.DatabaseComponents.DataAccessors
{
    public class UserDataAccessor : DataAccessor<UserRecord>
    {
        public override string PrimaryKeyColumn => "user_id";
        public override string TableName => "users";
    }
}
