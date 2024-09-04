using Microsoft.AspNetCore.Mvc;
using TWISTServer.Database.DataAccessors;
using TWISTServer.Database.Records;

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
                return dataAccessor.GetConnection(id.Value);
            }

            return dataAccessor.GetAllConnections();
        }

        [HttpPut]
        [Route("")]
        public JsonResult AddMessage([FromBody] ConnectionRecord connection)
        {
            dataAccessor.InsertConnection(connection);
            return new JsonResult($"Successfully added connection (for simulation #{connection.SimulationId}).");
        }
    }
}
