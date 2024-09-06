using System.Data.SqlClient;
using System.Data;
using TWISTServer.DatabaseComponents.Records;

namespace TWISTServer.DatabaseComponents.DataAccessors
{
    public class ScoreDataAccessor : DataAccessor<ScoreRecord>
    {
        public override string PrimaryKeyColumn => "score_id";
        public override string TableName => "scores";
    }
}
