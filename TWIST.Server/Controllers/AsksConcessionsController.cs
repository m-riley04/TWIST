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
        public IEnumerable<AskRecord> GetAsks([FromQuery(Name = "id")] int? id, [FromQuery(Name = "team")] int? teamId)
        {
            // Check for both
            if (id.HasValue && teamId.HasValue)
            {
                asksAccessor.GetAsksBySimulationAndTeam(id.Value, teamId.Value);
            }
            
            // Check for ID
            if (id.HasValue)
            {
                return asksAccessor.Get(id.Value);
            }

            // Check for team ID
            if (teamId.HasValue)
            {
                return asksAccessor.GetAsksByTeam(teamId.Value);
            }

            return asksAccessor.GetAll();
        }

        [HttpGet]
        [Route("concessions")]
        public IEnumerable<ConcessionRecord> GetConcessions([FromQuery(Name = "id")] int? id, [FromQuery(Name = "team")] int? teamId)
        {
            // Check for both
            if (id.HasValue && teamId.HasValue)
            {
                concessionsAccessor.GetConcessionsBySimulationAndTeam(id.Value, teamId.Value);
            }

            // Check for ID
            if (id.HasValue)
            {
                return concessionsAccessor.Get(id.Value);
            }

            // Check for team ID
            if (teamId.HasValue)
            {
                return concessionsAccessor.GetConcessionsByTeam(teamId.Value);
            }

            return concessionsAccessor.GetAll();
        }

        [HttpPut]
        [Route("asks")]
        public JsonResult AddAsk([FromBody] AskRecord ask)
        {
            asksAccessor.Insert(ask);
            return new JsonResult($"Successfully added ask (TeamId = {ask.TeamId})");
        }

        [HttpPut]
        [Route("concessions")]
        public JsonResult AddConcession([FromBody] ConcessionRecord concession)
        {
            concessionsAccessor.Insert(concession);
            return new JsonResult($"Successfully added ask (TeamId = {concession.TeamId})");
        }
    }
}
