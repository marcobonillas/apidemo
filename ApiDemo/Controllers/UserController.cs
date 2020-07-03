using System;
using ApiDemo.Contracts;
using ApiDemo.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using User = ApiDemo.Domain.User;
using UserResponse = ApiDemo.Contracts.UserResponse;

namespace ApiDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UserController> _logger;

        public UserController( IUserRepository userRepository,
                               IMapper mapper,
                               ILogger<UserController> logger)

        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }


        [HttpGet(Name = "Get")]
        public ActionResult<UserResponse> Get(string emailAddress)
        {
            if (emailAddress == null)
            {
                var message = string.Format("Email is required");
                return BadRequest(message);
            }

            var foundUserFromDb = _userRepository.GetUserByEmailAddressAsync(emailAddress).Result;

            if (foundUserFromDb != null)
            {
                var foundUser = new UserResponse
                {
                    Id = foundUserFromDb.Id,
                    Name = $"{foundUserFromDb.FirstName} {foundUserFromDb.MiddleName} {foundUserFromDb.LastName}",
                    PhoneNumber = foundUserFromDb.PhoneNumber,
                    EmailAddress = foundUserFromDb.EmailAddress
                };
                return foundUser;
            }

            var notFoundMessage = $"User with email: {emailAddress} not found";
            return NotFound(notFoundMessage);
        }

        [HttpPost]
        public ActionResult<UserResponse> Post(UserRequest userCreateRequest)
        {
            User user = new User();
            user = _mapper.Map<User>(userCreateRequest);
            user.Id = Guid.NewGuid().ToString();
            var userResponse = _mapper.Map<UserResponse>(user);

            try
            {
                var task = _userRepository.CreateUser(user);
                Console.WriteLine(task.Status);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

            return CreatedAtAction(nameof(Get), new { id = user.Id }, userResponse);
        }
    }
}
