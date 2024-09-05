using Microsoft.AspNetCore.Mvc;
using TWISTServer.DatabaseComponents.DataAccessors;
using TWISTServer.DatabaseComponents.Records;

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
            [FromQuery(Name = "team")] int? teamId, [FromQuery(Name = "simulation")] int? simulationId)
        {
            if (id.HasValue)
            {
                return dataAccessor.GetParticipant(id.Value);
            }

            if (simulationId.HasValue)
            {
                return dataAccessor.GetParticipantsBySimulation(simulationId.Value);
            }

            if (teamId.HasValue)
            {
                return dataAccessor.GetParticipantsByTeam(teamId.Value);
            }

            return dataAccessor.GetAllParticipants();
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
