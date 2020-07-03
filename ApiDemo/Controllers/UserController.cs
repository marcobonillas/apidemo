using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
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
        public ActionResult<User> Get(string emailAddress)
        {
            if (emailAddress == null)
            {
                return BadRequest();
            }

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = "Test",
                LastName = "User",
                PhoneNumber = "555-555-5656",
                EmailAddress = "testuser@apidemo.com"
            };

            var user2 = new User
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = "Test",
                LastName = "User",
                PhoneNumber = "555-555-5656",
                EmailAddress = "testuser2@apidemo.com"
            };

            var users = new List<User>();
            users.Add(user);
            users.Add(user2);

            var foundUser = users.FirstOrDefault(u => u.EmailAddress.ToLower() == emailAddress.ToLower());

            if (foundUser != null)
            {
                return foundUser;
            }
            
            return NotFound();
        }

    }
}
