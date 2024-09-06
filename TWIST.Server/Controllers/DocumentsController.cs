using Microsoft.AspNetCore.Mvc;
using TWISTServer.DatabaseComponents.DataAccessors;
using TWISTServer.DatabaseComponents.Records;

namespace TWISTServer.Controllers
{
    [ApiController]
    [Route("api/documents")]
    public class DocumentsController(ILogger<DocumentsController> logger)
    {
        private readonly DocumentsDataAccessor dataAccessor = new();

        private readonly ILogger<DocumentsController> _logger = logger;

        [HttpGet]
        [Route("")]
        public IEnumerable<DocumentRecord> GetDocuments()
        {
            return dataAccessor.GetAll();
        }

        [HttpGet]
        [Route("{id}")]
        public IEnumerable<DocumentRecord> GetDocument([FromRoute] int id)
        {
            return dataAccessor.Get(id);
        }

        [HttpPut]
        [Route("")]
        public JsonResult Put([FromBody] DocumentRecord document)
        {
            dataAccessor.Insert(document);
            return new JsonResult($"Successfully added document {document.Type}!");
        }
    }
}
