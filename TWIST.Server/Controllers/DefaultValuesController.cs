using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Eventing.Reader;
using TWISTServer.DatabaseComponents.DataAccessors;
using TWISTServer.DatabaseComponents.Records;
using TWISTServer.Enums;

namespace TWISTServer.Controllers
{
    [ApiController]
    [Route("api/defaults")]
    public class DefaultValuesController(ILogger<DefaultValuesController> logger)
    {
        private readonly DefaultAsksDataAccessor asksAccessor = new();
        private readonly DefaultConcessionsDataAccessor concessionsAccessor = new();

        private readonly ILogger<DefaultValuesController> _logger = logger;

        [HttpGet]
        [Route("asks/{country}")]
        public IEnumerable<DefaultAskRecord> GetDefaultAsksByCountry([FromRoute] CountryEnum country)
        {
            return asksAccessor.GetByCountry(country);
        }

        [HttpGet]
        [Route("concessions/{country}")]
        public IEnumerable<DefaultConcessionRecord> GetDefaultConcessionsByCountry([FromRoute] CountryEnum country)
        {
            return concessionsAccessor.GetByCountry(country);
        }

        [HttpPut]
        [Route("asks")]
        public JsonResult AddDefaultAsk([FromBody] DefaultAskRecord ask)
        {
            asksAccessor.Insert(ask);
            return new JsonResult($"Successfully added default ask {ask.DefaultAskId}");
        }

        [HttpPut]
        [Route("concessions")]
        public JsonResult AddDefaultConcession([FromBody] DefaultConcessionRecord concession)
        {
            concessionsAccessor.Insert(concession);
            return new JsonResult($"Successfully added default concession {concession.DefaultConcessionId}");
        }
    }
}
