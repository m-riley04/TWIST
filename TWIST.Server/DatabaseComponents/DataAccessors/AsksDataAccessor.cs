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
    public class AsksDataAccessor : DataAccessor<AskRecord>
    {
        public override string PrimaryKeyColumn { get; } = "ask_id";
        public override string TableName { get; } = "asks";

        public IEnumerable<AskRecord> GetAsksBySimulation(int simulationId)
        {
            string sql = @"select
ask_id, simulation_id, description, points, status, creation_date, modified_date, country, last_editor from asks
WHERE
simulation_id = @simulation_id;";
            return Database.Query(
                sql,
                AskRecord.FromRow,
                [
                    new("@simulation_id", SqlDbType.Int) { Value = simulationId },
                ]
            );
        }

        public void InsertAsks(IEnumerable<AskRecord> asks)
        {
            var askList = asks.ToList();
            if (!askList.Any())
                return;

            // Build the INSERT statement dynamically.
            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append("INSERT INTO asks (simulation_id, description, points, status, creation_date, modified_date, country, last_editor) VALUES ");

            var parameters = new List<SqlParameter>();
            for (int i = 0; i < askList.Count; i++)
            {
                if (i > 0)
                    sqlBuilder.Append(", ");

                // Create unique parameter names per row.
                sqlBuilder.Append($"(@simulation_id{i}, @description{i}, @points{i}, @status{i}, @creation_date{i}, @modified_date{i}, @country{i}, @last_editor{i})");

                parameters.Add(new SqlParameter($"@simulation_id{i}", SqlDbType.Int) { Value = askList[i].SimulationId });
                parameters.Add(new SqlParameter($"@description{i}", SqlDbType.NVarChar) { Value = askList[i].Description });
                parameters.Add(new SqlParameter($"@points{i}", SqlDbType.Int) { Value = askList[i].Points });
                parameters.Add(new SqlParameter($"@status{i}", SqlDbType.Int) { Value = askList[i].Status });
                parameters.Add(new SqlParameter($"@creation_date{i}", SqlDbType.DateTime) { Value = askList[i].CreationDate });
                parameters.Add(new SqlParameter($"@modified_date{i}", SqlDbType.DateTime) { Value = askList[i].ModifiedDate });
                parameters.Add(new SqlParameter($"@country{i}", SqlDbType.Int) { Value = askList[i].Country });

                // If LastEditor is null, use DBNull.Value.
                parameters.Add(new SqlParameter($"@last_editor{i}", SqlDbType.Int)
                {
                    Value = askList[i].LastEditor == null ? DBNull.Value : askList[i].LastEditor
                });
            }

            string sql = sqlBuilder.ToString();
            Database.NonQuery(sql, parameters.ToArray());
        }

        public IEnumerable<AskRecord> GetAsksBySimulationAndCountry(int simulationId, CountryEnum country)
        {
            string sql = @"select
ask_id, simulation_id, description, points, status, creation_date, modified_date, country, last_editor from asks
WHERE
simulation_id = @simulation_id AND country = @country;";
            return Database.Query(
                sql,
                AskRecord.FromRow,
                [
                    new("@simulation_id", SqlDbType.Int) { Value = simulationId },
                    new("@country", SqlDbType.Int) { Value = country },
                ]
            );
        }

        public void UpdateAskFromSimAndCountry(int simulationId, CountryEnum country, AskRecord newAsk)
        {
            string sql = @"UPDATE asks
SET
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
                    new("@description", SqlDbType.NVarChar) { Value = newAsk.Description },
                    new("@points", SqlDbType.Int) { Value = newAsk.Points },
                    new("@status", SqlDbType.Int) { Value = newAsk.Status },
                    new("@creation_date", SqlDbType.DateTime) { Value = newAsk.CreationDate },
                    new("@modified_date", SqlDbType.DateTime) { Value = newAsk.ModifiedDate },
                    new("@last_editor", SqlDbType.Int) { Value = newAsk.LastEditor == null ? DBNull.Value : newAsk.LastEditor } // If LastEditor is null, use DBNull.Value.
                ]
            );
        }
    }
}
