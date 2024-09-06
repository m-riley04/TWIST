using Microsoft.AspNetCore.Mvc;
using TWISTServer.DatabaseComponents.DataAccessors;
using TWISTServer.DatabaseComponents.Records;
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
                return dataAccessor.Get(id.Value);
            }

            return dataAccessor.GetAll();
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
            dataAccessor.Insert(response);
            return new JsonResult($"Successfully added response (Type = {response.SurveyType}).");
        }
    }
}
