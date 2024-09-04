using Microsoft.AspNetCore.Mvc;
using TWISTServer.Database.DataAccessors;
using TWISTServer.Database.Records;

namespace TWISTServer.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController
    {
        private readonly UserDataAccessor dataAccessor = new();

        private readonly ILogger<UsersController> _logger;

        public UsersController(ILogger<UsersController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("")]
        public IEnumerable<UserRecord> GetUsers()
        {
            return dataAccessor.GetAllUsers();
        }

        [HttpGet]
        [Route("{id}")]
        public IEnumerable<UserRecord> GetUser([FromRoute] int id)
        {
            return dataAccessor.GetUser(id);
        }

        [HttpPut]
        [Route("")]
        public JsonResult Put([FromBody] UserRecord user)
        {
            dataAccessor.InsertUser(user);
            return new JsonResult($"Successfully added user {user.Username}!");
        }
    }
}
