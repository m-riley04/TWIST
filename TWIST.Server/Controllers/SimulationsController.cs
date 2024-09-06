using Microsoft.AspNetCore.Mvc;
using TWISTServer.DatabaseComponents.DataAccessors;
using TWISTServer.DatabaseComponents.Records;

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
                return dataAccessor.Get(id.Value);
            }

            return dataAccessor.GetAll();
        }

        [HttpPut]
        [Route("")]
        public JsonResult AddSimulation([FromBody] SimulationRecord simulation)
        {
            dataAccessor.Insert(simulation);
            return new JsonResult($"Successfully added simulation {simulation.Name}.");
        }
    }
}
