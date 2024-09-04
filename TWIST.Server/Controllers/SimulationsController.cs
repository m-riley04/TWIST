using Microsoft.AspNetCore.Mvc;
using TWISTServer.Database.DataAccessors;
using TWISTServer.Database.Records;

namespace TWISTServer.Controllers
{
    [ApiController]
    [Route("api/simulations")]
    public class SimulationsController(ILogger<SimulationsController> logger)
    {
        private readonly SimulationsDataAccessor dataAccessor = new();

        private readonly ILogger<SimulationsController> _logger = logger;

        [HttpGet]
        [Route("")]
        public IEnumerable<SimulationRecord> GetSimulations([FromQuery] int? id)
        {
            if (id.HasValue)
            {
                return dataAccessor.GetSimulation(id.Value);
            }

            return dataAccessor.GetAllSimulations();
        }

        [HttpPut]
        [Route("")]
        public JsonResult AddSimulation([FromBody] SimulationRecord simulation)
        {
            dataAccessor.InsertSimulation(simulation);
            return new JsonResult($"Successfully added simulation {simulation.Name}.");
        }
    }
}
