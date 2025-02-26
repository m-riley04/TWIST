using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Eventing.Reader;
using TWISTServer.DatabaseComponents.DataAccessors;
using TWISTServer.DatabaseComponents.Records;
using TWISTServer.Enums;

namespace TWISTServer.Controllers
{
    [ApiController]
    [Route("api/agreements")]
    public class AgreementsController(ILogger<AgreementsController> logger)
    {
        private readonly AgreementsDataAccessor accessor = new();

        private readonly ILogger<AgreementsController> _logger = logger;

        [HttpGet]
        [Route("{simulationId}")]
        public IEnumerable<AgreementRecord> GetAgreements([FromRoute] int simulationId)
        {
            return accessor.GetBySimulation(simulationId);
        }

        [HttpGet]
        [Route("{simulationId}-{country}")]
        public IEnumerable<AgreementRecord> GetAgreements([FromRoute] int simulationId, [FromRoute] CountryEnum country)
        {
            return accessor.GetBySimulationAndCountry(simulationId, country);
        }

        [HttpPut]
        [Route("")]
        public JsonResult AddAgreement([FromBody] AgreementRecord agreement)
        {
            accessor.Insert(agreement);
            return new JsonResult($"Successfully added agreement {agreement.AgreementId}");
        }

        [HttpPut]
        [Route("batch")]
        public JsonResult AddAgreements([FromBody] AgreementRecord[] agreements)
        {
            accessor.InsertBatchAgreements(agreements);
            return new JsonResult($"Successfully added {agreements.Length} agreements.");
        }
    }
}
