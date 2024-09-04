using Microsoft.AspNetCore.Mvc;
using TWISTServer.Database.DataAccessors;
using TWISTServer.Database.Records;

namespace TWISTServer.Controllers
{
    [ApiController]
    [Route("api/scores")]
    public class ScoresController(ILogger<ScoresController> logger)
    {
        private readonly ScoreDataAccessor dataAccessor = new();

        private readonly ILogger<ScoresController> _logger = logger;

        [HttpGet]
        [Route("")]
        public IEnumerable<ScoreRecord> GetScores([FromQuery] int? id)
        {
            if (id.HasValue)
            {
                return dataAccessor.GetScore(id.Value);
            }

            return dataAccessor.GetAllScores();
        }

        [HttpPut]
        [Route("")]
        public JsonResult AddScore([FromBody] ScoreRecord score)
        {
            dataAccessor.InsertScore(score);
            return new JsonResult($"Successfully added score of value {score.TotalScore}).");
        }
    }
}
