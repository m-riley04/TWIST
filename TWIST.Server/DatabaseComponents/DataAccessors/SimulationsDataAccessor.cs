using System.Data.SqlTypes;
using TWISTServer.DatabaseComponents.Records;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using TWISTServer.Enums;

namespace TWISTServer.DatabaseComponents.DataAccessors
{
    public class SimulationsDataAccessor : DataAccessor<SimulationRecord>
    {
        public override string PrimaryKeyColumn => "simulation_id";
        public override string TableName => "simulations";

        /// <summary>
        /// Get a simulation by its code.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public virtual IEnumerable<SimulationRecord> GetByCode(string code)
        {
            string sql = @$"SELECT {GetColumnsAsSql(SimulationRecord.Columns.Keys)} FROM {TableName} WHERE code = @code;";
            return Database.Query<SimulationRecord>(
                sql,
                SimulationRecord.FromRow,
                [
                    new($"@code", SqlDbType.NVarChar) { Value = code },
                ]
            );
        }

        /// <summary>
        /// Close a simulation (non-deleting)
        /// </summary>
        /// <param name="code"></param>
        /// <param name="date"></param>
        public virtual void CloseSimulation(string code, DateTime date)
        {
            string sql = @$"UPDATE {TableName} SET active = 0, end_date = @date WHERE code = @code;";
            Database.NonQuery(
                sql,
                [
                    new($"@code", SqlDbType.NVarChar) { Value = code },
                    new($"@date", SqlDbType.DateTime) { Value = date },
                ]
            );
        }

        /// <summary>
        /// Delete a simulation from the database.
        /// </summary>
        /// <param name="code"></param>
        public virtual void Delete(string code)
        {
            string sql = @$"DELETE FROM {TableName} WHERE code = @code;";
            Database.NonQuery(
                sql,
                [
                    new($"@code", SqlDbType.NVarChar) { Value = code },
                ]
            );
        }

        public void UpdateRound(int simulationId, RoundEnum round)
        {
            string sql = @$"UPDATE {TableName} SET round = @round WHERE {PrimaryKeyColumn} = @simulationId;";
            Database.NonQuery(
                sql,
                [
                    new($"@round", SqlDbType.Int) { Value = (int)round },
                    new($"@simulationId", SqlDbType.Int) { Value = simulationId },
                ]
            );
        }

        public void UpdateState(int simulationId, SimulationStateEnum state)
        {
            string sql = @$"UPDATE {TableName} SET state = @state WHERE {PrimaryKeyColumn} = @simulationId;";
            Database.NonQuery(
                sql,
                [
                    new($"@state", SqlDbType.Int) { Value = (int)state },
                    new($"@simulationId", SqlDbType.Int) { Value = simulationId },
                ]
            );
        }
    }
}
