using Microsoft.AspNetCore.Mvc;
using TWISTServer.Database.DataAccessors;
using TWISTServer.Database.Records;
using TWISTServer.Enums;

namespace TWISTServer.Controllers
{
    [ApiController]
    [Route("api/responses")]
    public class ResponsesController(ILogger<ResponsesController> logger)
    {
        private readonly ResponsesDataAccessor dataAccessor = new();

        private readonly ILogger<ResponsesController> _logger = logger;

        [HttpGet]
        [Route("")]
        public IEnumerable<ResponseRecord> GetResponses([FromQuery] int? id)
        {
            if (id.HasValue)
            {
                return dataAccessor.GetResponse(id.Value);
            }

            return dataAccessor.GetAllResponses();
        }

        [HttpGet]
        [Route("pre")]
        public IEnumerable<ResponseRecord> GetPreResponses()
        {
            return dataAccessor.GetResponsesByType(SurveyTypeEnum.Pre);
        }

        [HttpGet]
        [Route("post")]
        public IEnumerable<ResponseRecord> GetPostResponses()
        {
            return dataAccessor.GetResponsesByType(SurveyTypeEnum.Post);
        }

        [HttpPut]
        [Route("")]
        public JsonResult AddResponse([FromBody] ResponseRecord response)
        {
            dataAccessor.InsertResponse(response);
            return new JsonResult($"Successfully added response (Type = {response.SurveyType}).");
        }
    }
}
