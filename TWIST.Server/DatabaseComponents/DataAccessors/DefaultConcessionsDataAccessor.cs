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
    public class DefaultConcessionsDataAccessor : DataAccessor<DefaultConcessionRecord>
    {
        public override string PrimaryKeyColumn { get; } = "default_concession_id";
        public override string TableName { get; } = "default-concessions";

        public IEnumerable<DefaultConcessionRecord> GetByCountry(CountryEnum country)
        {
            string sql = @$"SELECT {GetColumnsAsSql(DefaultConcessionRecord.Columns.Keys)} FROM {TableName} WHERE country = @country;";
            return Database.Query(
                sql,
                DefaultConcessionRecord.FromRow,
                [
                    new("@country", SqlDbType.Int) { Value = country },
                ]
            );
        }

        public void UpdateByCountryAndDescription(CountryEnum country, string description, DefaultConcessionRecord newDefaultConcession)
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
                    new("@description", SqlDbType.NVarChar) { Value = newDefaultConcession.Description },
                    new("@creation_date", SqlDbType.DateTime) { Value = newDefaultConcession.CreationDate },
                    new("@modified_date", SqlDbType.DateTime) { Value = newDefaultConcession.ModifiedDate },
                ]
            );
        }
    }
}
