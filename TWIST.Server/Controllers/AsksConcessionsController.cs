using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Eventing.Reader;
using TWISTServer.DatabaseComponents.DataAccessors;
using TWISTServer.DatabaseComponents.Records;

namespace TWISTServer.Controllers
{
    [ApiController]
    [Route("api/asks-concessions")]
    public class AsksConcessionsController(ILogger<AsksConcessionsController> logger)
    {
        private readonly AsksDataAccessor asksAccessor = new();
        private readonly ConcessionsDataAccessor concessionsAccessor = new();

        private readonly ILogger<AsksConcessionsController> _logger = logger;

        [HttpGet]
        [Route("asks")]
        public IEnumerable<AskRecord> GetAsks([FromQuery(Name = "id")] int? id)
        {
            // Check for ID
            if (id.HasValue)
            {
                return asksAccessor.Get(id.Value);
            }

            return asksAccessor.GetAll();
        }

        [HttpGet]
        [Route("concessions")]
        public IEnumerable<ConcessionRecord> GetConcessions([FromQuery(Name = "id")] int? id)
        {
            // Check for ID
            if (id.HasValue)
            {
                return concessionsAccessor.Get(id.Value);
            }

            return concessionsAccessor.GetAll();
        }

        [HttpPut]
        [Route("asks")]
        public JsonResult AddAsk([FromBody] AskRecord ask)
        {
            asksAccessor.Insert(ask);
            return new JsonResult($"Successfully added ask {ask.AskId}");
        }

        [HttpPut]
        [Route("concessions")]
        public JsonResult AddConcession([FromBody] ConcessionRecord concession)
        {
            concessionsAccessor.Insert(concession);
            return new JsonResult($"Successfully added concession {concession.ConcessionId}");
        }
    }
}
