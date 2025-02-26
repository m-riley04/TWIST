using System.Data;
using TWISTServer.DatabaseComponents.Records;
using System.Reflection.Metadata;
using TWISTServer.Interfaces;
using System.Text;
using Microsoft.Data.SqlClient;
using TWISTServer.Enums;

namespace TWISTServer.DatabaseComponents.DataAccessors
{
    public class ConcessionsDataAccessor : DataAccessor<ConcessionRecord>, IDataAccessor<ConcessionRecord>
    {
        public override string PrimaryKeyColumn => "concession_id";

        public override string TableName => "concessions";

        public void InsertConcessions(IEnumerable<ConcessionRecord> asks)
        {
            var concessionList = asks.ToList();
            if (!concessionList.Any())
                return;

            // Build the INSERT statement dynamically.
            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append($"INSERT INTO {TableName} (simulation_id, description, points, status, creation_date, modified_date, country, last_editor) VALUES ");

            var parameters = new List<SqlParameter>();
            for (int i = 0; i < concessionList.Count; i++)
            {
                if (i > 0)
                    sqlBuilder.Append(", ");

                // Create unique parameter names per row.
                sqlBuilder.Append($"(@simulation_id{i}, @description{i}, @points{i}, @status{i}, @creation_date{i}, @modified_date{i}, @country{i}, @last_editor{i})");

                parameters.Add(new SqlParameter($"@simulation_id{i}", SqlDbType.Int) { Value = concessionList[i].SimulationId });
                parameters.Add(new SqlParameter($"@description{i}", SqlDbType.NVarChar) { Value = concessionList[i].Description });
                parameters.Add(new SqlParameter($"@points{i}", SqlDbType.Int) { Value = concessionList[i].Points });
                parameters.Add(new SqlParameter($"@status{i}", SqlDbType.Int) { Value = concessionList[i].Status });
                parameters.Add(new SqlParameter($"@creation_date{i}", SqlDbType.DateTime) { Value = concessionList[i].CreationDate });
                parameters.Add(new SqlParameter($"@modified_date{i}", SqlDbType.DateTime) { Value = concessionList[i].ModifiedDate });
                parameters.Add(new SqlParameter($"@country{i}", SqlDbType.Int) { Value = concessionList[i].Country });

                // If LastEditor is null, use DBNull.Value.
                parameters.Add(new SqlParameter($"@last_editor{i}", SqlDbType.Int)
                {
                    Value = concessionList[i].LastEditor == null ? DBNull.Value : concessionList[i].LastEditor
                });
            }

            string sql = sqlBuilder.ToString();
            Database.NonQuery(sql, parameters.ToArray());
        }

        public IEnumerable<ConcessionRecord> GetConcessionsBySimulation(int simulationId)
        {
            string sql = @$"SELECT {GetColumnsAsSql(ConcessionRecord.Columns.Keys)} FROM {TableName} WHERE simulation_id = @simulation_id";
            return Database.Query<ConcessionRecord>(
                sql,
                ConcessionRecord.FromRow,
                new SqlParameter[]
                {
                    new("@simulation_id", SqlDbType.Int) { Value = simulationId }
                }
            );
        }

        public IEnumerable<ConcessionRecord> GetConcessionsBySimulationAndCountry(int simulationId, CountryEnum country)
        {
            string sql = @$"SELECT {GetColumnsAsSql(ConcessionRecord.Columns.Keys)} FROM {TableName} WHERE simulation_id = @simulation_id AND country = @country;";
            return Database.Query<ConcessionRecord>(
                sql,
                ConcessionRecord.FromRow,
                new SqlParameter[]
                {
                    new("@simulation_id", SqlDbType.Int) { Value = simulationId },
                    new("@country", SqlDbType.Int) { Value = country }
                }
            );
        }

        public void UpdateConcessionFromSimAndCountry(int simulationId, CountryEnum countryEnum, ConcessionRecord concession)
        {
            string sql = @$"UPDATE {TableName} SET
points = @points,
status = @status,
modified_date = @modified_date,
last_editor = @last_editor
WHERE simulation_id = @simulation_id AND country = @country AND description = @description;";
            Database.NonQuery(
                sql,
                new SqlParameter[]
                {
                    new("@description", SqlDbType.NVarChar) { Value = concession.Description },
                    new("@points", SqlDbType.Int) { Value = concession.Points },
                    new("@status", SqlDbType.Int) { Value = concession.Status },
                    new("@modified_date", SqlDbType.DateTime) { Value = concession.ModifiedDate },
                    new("@simulation_id", SqlDbType.Int) { Value = simulationId },
                    new("@country", SqlDbType.Int) { Value = countryEnum },
                    new ("@last_editor", SqlDbType.Int) { Value = concession.LastEditor == null ? DBNull.Value : concession.LastEditor } // If LastEditor is null, use DBNull.Value.
                }
            );
        }

        public void DeleteConcessionsFromSimulation(int simulationId)
        {
            string sql = @$"DELETE FROM {TableName} WHERE simulation_id = @simulation_id;";
            Database.NonQuery(
                sql,
                [
                    new("@simulation_id", SqlDbType.Int) { Value = simulationId },
                ]
            );
        }

        public void DeleteConcessionsFromSimulationAndCountry(int simulationId, CountryEnum country)
        {
            string sql = @$"DELETE FROM {TableName} WHERE simulation_id = @simulation_id AND country = @country;";
            Database.NonQuery(
                sql,
                [
                    new("@simulation_id", SqlDbType.Int) { Value = simulationId },
                    new("@country", SqlDbType.Int) { Value = country },
                ]
            );
        }
    }
}
