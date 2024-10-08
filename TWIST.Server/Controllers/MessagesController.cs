﻿using Microsoft.AspNetCore.Mvc;
using TWISTServer.DatabaseComponents.DataAccessors;
using TWISTServer.DatabaseComponents.Records;

namespace TWISTServer.Controllers
{
    [ApiController]
    [Route("api/messages")]
    public class MessagesController(ILogger<MessagesController> logger)
    {
        private readonly MessagesDataAccessor dataAccessor = new();

        private readonly ILogger<MessagesController> _logger = logger;

        [HttpGet]
        [Route("")]
        public IEnumerable<MessageRecord> GetMessages([FromQuery] int? id)
        {
            if (id.HasValue)
            {
                return dataAccessor.Get(id.Value);
            }

            return dataAccessor.GetAll();
        }

        [HttpPut]
        [Route("")]
        public JsonResult AddMessage([FromBody] MessageRecord message)
        {
            dataAccessor.Insert(message);
            return new JsonResult($"Successfully added message (from participant #{message.ParticipantId}).");
        }
    }
}
