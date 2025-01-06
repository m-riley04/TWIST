using Microsoft.AspNetCore.Mvc;
using TWISTServer.DatabaseComponents.DataAccessors;
using TWISTServer.DatabaseComponents.Records;
using TWISTServer.Models;

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

        [HttpGet]
        [Route("{code}")]
        public IEnumerable<SimulationRecord> GetSimulationByCode([FromRoute] string code)
        {
            return dataAccessor.GetByCode(code);
        }

        [HttpPut]
        [Route("")]
        public JsonResult AddSimulation([FromBody] SimulationRecord simulation)
        {
            dataAccessor.Insert(simulation);
            return new JsonResult($"Successfully added simulation {simulation.Code}.");
        }

        [HttpPost]
        [Route("{code}/close")]
        public JsonResult CloseSimulation([FromRoute(Name = "code")] string code, [FromBody] CloseSimulationRequest request)
        {
            dataAccessor.CloseSimulation(code, request.Date);
            return new JsonResult($"Successfully closed simulation {code}.");
        }

        [HttpDelete]
        [Route("{code}")]
        public JsonResult DeleteSimulation([FromRoute(Name = "code")] string code)
        {
            dataAccessor.Delete(code);
            return new JsonResult($"Successfully deleted simulation {code}.");
        }
    }
}
