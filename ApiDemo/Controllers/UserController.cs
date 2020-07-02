using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiDemo.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ApiDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {


        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<User> Get()
        {
            var users = new List<User>();
            users.Add(new User { Id = Guid.NewGuid().ToString() });
            users.Add(new User { Id = Guid.NewGuid().ToString() });
            return users;
        }

        [HttpGet("/user/info")]
        public string Info()
        {
            return "alive";
        }
    }
}
