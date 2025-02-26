using System.Data;
using TWISTServer.DatabaseComponents.Records;
using System.Reflection.Metadata;
using TWISTServer.Interfaces;
using System.Text;
using Microsoft.Data.SqlClient;
using TWISTServer.Enums;
using System.Security.Cryptography;

namespace TWISTServer.DatabaseComponents.DataAccessors
{
    public class DefaultAsksDataAccessor : DataAccessor<DefaultAskRecord>
    {
        public override string PrimaryKeyColumn { get; } = "default_ask_id";
        public override string TableName { get; } = "default_asks";

        public IEnumerable<DefaultAskRecord> GetByCountry(CountryEnum country)
        {
            string sql = @$"SELECT {GetColumnsAsSql(DefaultAskRecord.Columns.Keys)} FROM {TableName} WHERE country = @country;";
            return Database.Query(
                sql,
                DefaultAskRecord.FromRow,
                [
                    new("@country", SqlDbType.Int) { Value = country },
                ]
            );
        }

        public void UpdateByCountryAndDescription(CountryEnum country, string description, DefaultAskRecord newDefaultAsk)
        {
            string sql = @$"UPDATE {TableName} SET
creation_date = @creation_date,
modified_date = @modified_date,
WHERE
country = @country AND description = @description";
            Database.NonQuery(
                sql,
                [
                    new("@country", SqlDbType.Int) { Value = country },
                    new("@description", SqlDbType.NVarChar) { Value = newDefaultAsk.Description },
                    new("@creation_date", SqlDbType.DateTime) { Value = newDefaultAsk.CreationDate },
                    new("@modified_date", SqlDbType.DateTime) { Value = newDefaultAsk.ModifiedDate },
                ]
            );
        }
    }
}
