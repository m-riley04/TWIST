using System.Data;
using TWISTServer.DatabaseComponents.Records;
using System.Text;
using Microsoft.Data.SqlClient;
using TWISTServer.Enums;

namespace TWISTServer.DatabaseComponents.DataAccessors
{
    public class AgreementsDataAccessor : DataAccessor<AgreementRecord>
    {
        public override string PrimaryKeyColumn { get; } = "agreement_id";
        public override string TableName { get; } = "agreements";

        public IEnumerable<AgreementRecord> GetBySimulation(int simulationId)
        {
            string sql = $"SELECT {GetColumnsAsSql(AgreementRecord.Columns.Keys)} FROM {TableName} WHERE simulation_id = @simulation_id";
            return Database.Query(
                sql,
                AgreementRecord.FromRow,
                [
                    new("@simulation_id", SqlDbType.Int) { Value = simulationId },
                ]
            );
        }

        public void InsertBatchAgreements(IEnumerable<AgreementRecord> agreements)
        {
            var agreementsList = agreements.ToList();
            if (!agreementsList.Any())
                return;

            // Build the INSERT statement dynamically.
            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append($"INSERT INTO {TableName} ({GetColumnsAsSql(AgreementRecord.Columns.Keys)}) VALUES ");

            var parameters = new List<SqlParameter>();
            for (int i = 0; i < agreementsList.Count; i++)
            {
                if (i > 0)
                    sqlBuilder.Append(", ");

                // Create unique parameter names per row.
                sqlBuilder.Append($"(@simulation_id{i}, @description{i}, @points{i}, @status{i}, @creation_date{i}, @modified_date{i}, @country{i}, @last_editor{i})");

                parameters.Add(new SqlParameter($"@simulation_id{i}", SqlDbType.Int) { Value = agreementsList[i].SimulationId });
                parameters.Add(new SqlParameter($"@description{i}", SqlDbType.NVarChar) { Value = agreementsList[i].Description });
                parameters.Add(new SqlParameter($"@points{i}", SqlDbType.Int) { Value = agreementsList[i].Points });
                parameters.Add(new SqlParameter($"@status{i}", SqlDbType.Int) { Value = agreementsList[i].Status });
                parameters.Add(new SqlParameter($"@creation_date{i}", SqlDbType.DateTime) { Value = agreementsList[i].CreationDate });
                parameters.Add(new SqlParameter($"@modified_date{i}", SqlDbType.DateTime) { Value = agreementsList[i].ModifiedDate });
                parameters.Add(new SqlParameter($"@country{i}", SqlDbType.Int) { Value = agreementsList[i].Country });

                // If LastEditor is null, use DBNull.Value.
                parameters.Add(new SqlParameter($"@last_editor{i}", SqlDbType.Int)
                {
                    Value = agreementsList[i].LastEditor == null ? DBNull.Value : agreementsList[i].LastEditor
                });
            }

            string sql = sqlBuilder.ToString();
            Database.NonQuery(sql, parameters.ToArray());
        }

        public IEnumerable<AgreementRecord> GetBySimulationAndCountry(int simulationId, CountryEnum country)
        {
            string sql = @$"SELECT {GetColumnsAsSql(AgreementRecord.Columns.Keys)} FROM {TableName} WHERE simulation_id = @simulation_id AND country = @country";
            return Database.Query(
                sql,
                AgreementRecord.FromRow,
                [
                    new("@simulation_id", SqlDbType.Int) { Value = simulationId },
                    new("@country", SqlDbType.Int) { Value = country },
                ]
            );
        }

        public void UpdateFromSimAndCountry(int simulationId, CountryEnum country, AgreementRecord newAgreement)
        {
            string sql = @$"UPDATE {TableName} SET 
points = @points,
status = @status,
creation_date = @creation_date,
modified_date = @modified_date,
last_editor = @last_editor
WHERE
simulation_id = @simulation_id AND country = @country AND description = @description";
            Database.NonQuery(
                sql,
                [
                    new("@simulation_id", SqlDbType.Int) { Value = simulationId },
                    new("@country", SqlDbType.Int) { Value = country },
                    new("@description", SqlDbType.NVarChar) { Value = newAgreement.Description },
                    new("@points", SqlDbType.Int) { Value = newAgreement.Points },
                    new("@status", SqlDbType.Int) { Value = newAgreement.Status },
                    new("@creation_date", SqlDbType.DateTime) { Value = newAgreement.CreationDate },
                    new("@modified_date", SqlDbType.DateTime) { Value = newAgreement.ModifiedDate },
                    new("@last_editor", SqlDbType.Int) { Value = newAgreement.LastEditor == null ? DBNull.Value : newAgreement.LastEditor } // If LastEditor is null, use DBNull.Value.
                ]
            );
        }
    }
}
