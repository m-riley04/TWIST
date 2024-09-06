using System.Data.SqlClient;
using System.Data;
using TWISTServer.DatabaseComponents.Records;

namespace TWISTServer.DatabaseComponents.DataAccessors
{
    public class MessagesDataAccessor : DataAccessor<MessageRecord>
    {
        public override string PrimaryKeyColumn => "message_id";
        public override string TableName => "messages";
    }
}
