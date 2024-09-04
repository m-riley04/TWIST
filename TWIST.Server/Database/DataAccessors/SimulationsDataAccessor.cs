using System.Data.SqlTypes;
using TWISTServer.Database.Records;
using System.Data;
using System.Data.SqlClient;

namespace TWISTServer.Database.DataAccessors
{
    public class SimulationsDataAccessor : DataAccessor
    {

        public IEnumerable<SimulationRecord> GetAllSimulations()
        {
            string sql = $"select simulation_id, name, participants, start_date, end_date, active from simulations;";
            return Database.Query(
                sql,
                SimulationRecord.FromRow
            );
        }

        public IEnumerable<SimulationRecord> GetSimulation(int simulationId)
        {
            string sql = @"select 
simulation_id, name, participants, start_date, end_date, active 
from simulations WHERE 
simulation_id = @simulation_id;";
            return Database.Query(
                sql,
                SimulationRecord.FromRow,
                [
                    new("@simulation_id", SqlDbType.Int) { Value = simulationId },
                ]
            );
        }

        public int InsertSimulation(SimulationRecord simulation)
        {
            string sql = @"
INSERT INTO simulations 
(name, participants, start_date, end_date, active) 
VALUES 
(@name, @participants, @start_date, @end_date, @active)";

            SqlParameter[] parameters = [
                new("@name", SqlDbType.NVarChar, -1) { Value = simulation.Name },
                new("@participants", SqlDbType.DateTime) { Value = simulation.Participants },
                new("@start_date", SqlDbType.DateTime, 64) { Value = simulation.StartDate },
                new("@end_date", SqlDbType.DateTime) { Value = simulation.EndDate},
                new("@active", SqlDbType.Bit) { Value = simulation.Active },
            ];

            return Database.NonQuery(sql, parameters);
        }
    }
}
