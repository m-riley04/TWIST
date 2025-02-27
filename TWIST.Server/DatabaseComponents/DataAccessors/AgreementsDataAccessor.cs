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
                sqlBuilder.Append($"(@simulation_id{i}, @type{i}, @type_id{i}, @description{i}, @points{i}, @country{i}");

                parameters.Add(new SqlParameter($"@simulation_id{i}", SqlDbType.Int) { Value = agreementsList[i].SimulationId });
                parameters.Add(new SqlParameter($"@type{i}", SqlDbType.Int) { Value = agreementsList[i].Type });
                parameters.Add(new SqlParameter($"@type_id{i}", SqlDbType.Int) { Value = agreementsList[i].TypeId });
                parameters.Add(new SqlParameter($"@description{i}", SqlDbType.NVarChar) { Value = agreementsList[i].Description });
                parameters.Add(new SqlParameter($"@points{i}", SqlDbType.Int) { Value = agreementsList[i].Points });
                parameters.Add(new SqlParameter($"@country{i}", SqlDbType.Int) { Value = agreementsList[i].Country });
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

        public void UpdateFromSimAndDescription(int simulationId, AgreementRecord newAgreement)
        {
            string sql = @$"UPDATE {TableName} SET 
type = @type,
type_id = @type_id,
points = @points 
WHERE
simulation_id = @simulation_id AND description = @description";
            Database.NonQuery(
                sql,
                [
                    new("@simulation_id", SqlDbType.Int) { Value = simulationId },
                    new("@country", SqlDbType.Int) { Value = newAgreement.Country },
                    new("@description", SqlDbType.NVarChar) { Value = newAgreement.Description },
                    new("@type", SqlDbType.Int) { Value = newAgreement.Type },
                    new("@type_id", SqlDbType.Int) { Value = newAgreement.TypeId },
                    new("@points", SqlDbType.Int) { Value = newAgreement.Points },
                ]
            );
        }

        public void DeleteBySimulation(int simulationId)
        {
            string sql = @$"DELETE FROM {TableName} WHERE simulation_id = @simulation_id";
            Database.NonQuery(
                sql,
                [
                    new("@simulation_id", SqlDbType.Int) { Value = simulationId },
                ]
            );
        }
    }
}
