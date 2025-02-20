using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Eventing.Reader;
using TWISTServer.DatabaseComponents.DataAccessors;
using TWISTServer.DatabaseComponents.Records;
using TWISTServer.Enums;

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
        public IEnumerable<AskRecord> GetAsks([FromQuery(Name = "id")] int? id, [FromQuery(Name ="simulation_id")] int? simulationId, [FromQuery(Name = "country")] CountryEnum? country)
        {
            // Check for ID
            if (id.HasValue)
            {
                return asksAccessor.Get(id.Value);
            }

            if (simulationId.HasValue && country.HasValue)
            {
                return asksAccessor.GetAsksBySimulationAndCountry(simulationId.Value, country.Value);
            }

            if (simulationId.HasValue)
            {
                return asksAccessor.GetAsksBySimulation(simulationId.Value);
            }

            return asksAccessor.GetAll();
        }

        [HttpGet]
        [Route("asks/{simulationId}-{country}")]
        public IEnumerable<AskRecord> GetAsks([FromRoute] int simulationId, [FromRoute] CountryEnum country)
        {
            return asksAccessor.GetAsksBySimulationAndCountry(simulationId, country);
        }

        [HttpGet]
        [Route("concessions")]
        public IEnumerable<ConcessionRecord> GetConcessions([FromQuery(Name = "id")] int? id, [FromQuery(Name = "simulation_id")] int? simulationId, [FromQuery(Name = "simulation_id")] CountryEnum? country)
        {
            // Check for ID
            if (id.HasValue)
            {
                return concessionsAccessor.Get(id.Value);
            }

            if (simulationId.HasValue && country.HasValue)
            {
                return concessionsAccessor.GetConcessionsBySimulationAndCountry(simulationId.Value, country.Value);
            }

            if (simulationId.HasValue)
            {
                return concessionsAccessor.GetConcessionsBySimulation(simulationId.Value);
            }

            return concessionsAccessor.GetAll();
        }

        [HttpGet]
        [Route("concessions/{simulationId}-{country}")]
        public IEnumerable<ConcessionRecord> GetConcessions([FromRoute] int simulationId, [FromRoute] CountryEnum country)
        {
            return concessionsAccessor.GetConcessionsBySimulationAndCountry(simulationId, country);
        }

        [HttpPut]
        [Route("asks")]
        public JsonResult AddAsk([FromBody] AskRecord ask)
        {
            asksAccessor.Insert(ask);
            return new JsonResult($"Successfully added ask {ask.AskId}");
        }

        [HttpPut]
        [Route("asks/batch")]
        public JsonResult AddAsks([FromBody] AskRecord[] asks)
        {
            asksAccessor.InsertAsks(asks);
            return new JsonResult($"Successfully added asks.");
        }

        [HttpPut]
        [Route("concessions/batch")]
        public JsonResult AddConcessions([FromBody] ConcessionRecord[] concessions)
        {
            concessionsAccessor.InsertConcessions(concessions);
            return new JsonResult($"Successfully added asks.");
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
