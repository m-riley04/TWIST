using Microsoft.AspNetCore.Mvc;
using TWISTServer.DatabaseComponents.DataAccessors;
using TWISTServer.DatabaseComponents.Records;
using TWISTServer.Enums;

namespace TWISTServer.Controllers
{
    [ApiController]
    [Route("api/participants")]
    public class ParticipantsController(ILogger<ParticipantsController> logger)
    {
        private readonly ParticipantsDataAccessor dataAccessor = new();

        private readonly ILogger<ParticipantsController> _logger = logger;

        [HttpGet]
        [Route("")]
        public IEnumerable<ParticipantRecord> GetParticipants([FromQuery(Name = "id")] int? id, 
            [FromQuery(Name = "country")] int? country, [FromQuery(Name = "simulation")] int? simulationId)
        {
            if (id.HasValue)
            {
                return dataAccessor.Get(id.Value);
            }

            if (simulationId.HasValue)
            {
                return dataAccessor.GetParticipantsBySimulation(simulationId.Value);
            }

            if (country.HasValue)
            {
                return dataAccessor.GetParticipantsByCountry((CountryEnum)country.Value);
            }

            return dataAccessor.GetAll();
        }

        [HttpGet]
        [Route("simulation/{id}")]
        public IEnumerable<ParticipantRecord> GetParticipantsFromSimulation([FromRoute] int simulationId)
        {
            return dataAccessor.GetParticipantsBySimulation(simulationId);
        }

        [HttpPut]
        [Route("")]
        public JsonResult AddParticipant([FromBody] ParticipantRecord participant)
        {
            dataAccessor.Insert(participant);
            return new JsonResult($"Successfully added participant '{participant.Username}'.");
        }
    }
}
