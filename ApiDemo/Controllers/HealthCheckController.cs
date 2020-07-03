using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ApiDemo.Domain;
using ApiDemo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiDemo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public HealthCheckController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public string Get()
        {
            var healthStatus = "down";
            var testAccount = "testaccount@apidemo.com";
            var user = _userRepository.GetUserByEmailAddressAsync(testAccount).Result;
            if (user != null)
            {
                healthStatus = "alive";
            }
            else
            {
                var userCreate = new ApiDemo.Domain.User
                {
                    EmailAddress = testAccount,
                    FirstName = "test",
                    LastName = "test",
                    PhoneNumber = "555-555-5555",
                    Id = Guid.NewGuid().ToString()
                };
                var added = _userRepository.CreateUser(userCreate).Result;
                healthStatus = "alive";
            }

            return healthStatus;
        }

    }
}
