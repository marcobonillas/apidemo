using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ApiDemo.Contracts;
using ApiDemo.Domain;
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

        [HttpGet(Name = "Get")]
        public ActionResult<UserResponse> Get(string emailAddress)
        {
            if (emailAddress == null)
            {
                return BadRequest();
            }

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = "Test",
                MiddleName = "Middle",
                LastName = "LastName1",
                PhoneNumber = "555-555-5656",
                EmailAddress = "testuser@apidemo.com"
            };

            var user2 = new User
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = "Test",
                MiddleName = "mister",
                LastName = "LastName2",
                PhoneNumber = "555-555-5656",
                EmailAddress = "testuser2@apidemo.com"
            };

            var users = new List<User>();
            users.Add(user);
            users.Add(user2);

            var foundUserFromDb = users.FirstOrDefault(u => u.EmailAddress.ToLower() == emailAddress.ToLower());

            if (foundUserFromDb != null)
            {
                var foundUser = new UserResponse
                {
                    Id = foundUserFromDb.Id,
                    Name = $"{foundUserFromDb.FirstName} {foundUserFromDb.MiddleName} {foundUserFromDb.LastName}",
                    PhoneNumber = "555-555-5656",
                    EmailAddress = "testuser2@apidemo.com"
                };
                return foundUser;
            }

            return NotFound();
        }

        [HttpPost]
        public ActionResult Post(UserRequest userCreateRequest)
        {
            return Created("Get",userCreateRequest.EmailAddress);
        }
    }
}
