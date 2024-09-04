using Microsoft.AspNetCore.Mvc;
using TWISTServer.Database.DataAccessors;
using TWISTServer.Database.Records;

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
            return dataAccessor.GetAllDocuments();
        }

        [HttpGet]
        [Route("{id}")]
        public IEnumerable<DocumentRecord> GetDocument([FromRoute] int id)
        {
            return dataAccessor.GetDocument(id);
        }

        [HttpPut]
        [Route("")]
        public JsonResult Put([FromBody] DocumentRecord document)
        {
            dataAccessor.InsertDocument(document);
            return new JsonResult($"Successfully added document {document.Type}!");
        }
    }
}
