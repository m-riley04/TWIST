using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Eventing.Reader;
using TWISTServer.Database.DataAccessors;
using TWISTServer.Database.Records;

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
                return asksAccessor.GetAsk(id.Value);
            }

            // Check for team ID
            if (teamId.HasValue)
            {
                return asksAccessor.GetAsksByTeam(teamId.Value);
            }

            return asksAccessor.GetAllAsks();
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
                return concessionsAccessor.GetConcession(id.Value);
            }

            // Check for team ID
            if (teamId.HasValue)
            {
                return concessionsAccessor.GetConcessionsByTeam(teamId.Value);
            }

            return concessionsAccessor.GetAllConcessions();
        }

        [HttpPut]
        [Route("asks")]
        public JsonResult AddAsk([FromBody] AskRecord ask)
        {
            asksAccessor.InsertAsk(ask);
            return new JsonResult($"Successfully added ask (TeamId = {ask.TeamId})");
        }

        [HttpPut]
        [Route("concessions")]
        public JsonResult AddConcession([FromBody] ConcessionRecord concession)
        {
            concessionsAccessor.InsertConcession(concession);
            return new JsonResult($"Successfully added ask (TeamId = {concession.TeamId})");
        }
    }
}
