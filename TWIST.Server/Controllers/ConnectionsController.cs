using Microsoft.AspNetCore.Mvc;
using TWISTServer.DatabaseComponents.DataAccessors;
using TWISTServer.DatabaseComponents.Records;

namespace TWISTServer.Controllers
{
    [ApiController]
    [Route("api/connections")]
    public class ConnectionsController(ILogger<ConnectionsController> logger)
    {
        private readonly ConnectionsDataAccessor dataAccessor = new();

        private readonly ILogger<ConnectionsController> _logger = logger;

        [HttpGet]
        [Route("")]
        public IEnumerable<ConnectionRecord> GetMessages([FromQuery] int? id)
        {
            if (id.HasValue)
            {
                return dataAccessor.Get(id.Value);
            }

            return dataAccessor.GetAll();
        }

        [HttpPut]
        [Route("")]
        public JsonResult AddMessage([FromBody] ConnectionRecord connection)
        {
            dataAccessor.Insert(connection);
            return new JsonResult($"Successfully added connection (for simulation #{connection.SimulationId}).");
        }
    }
}
