using System.Data.SqlTypes;
using TWISTServer.DatabaseComponents.Records;
using System.Data;
using System.Data.SqlClient;

namespace TWISTServer.DatabaseComponents.DataAccessors
{
    public class SimulationsDataAccessor : DataAccessor<SimulationRecord>
    {
        public override string PrimaryKeyColumn => "simulation_id";
        public override string TableName => "simulations";
    }
}
