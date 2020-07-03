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
            var userResponse = new UserResponse();

            var isUserExists = _userRepository.GetUserByEmailAddressAsync(userCreateRequest.EmailAddress).Result;
            if (isUserExists != null)
            {
                return Conflict($"Email: {userCreateRequest.EmailAddress} already in use");
            }

            try
            {
                var userCreated = _userRepository.CreateUser(user).Result;
                userResponse = _mapper.Map<UserResponse>(userCreated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return Problem($"There was a problem processing your request:{ex}");
            }

            return CreatedAtAction(nameof(Get), new { id = user.Id }, userResponse);
        }

        [HttpPut("{emailAddress}")]
        public ActionResult Put(string emailAddress, UserUpdateRequest userUpdateRequest)
        {
            User existingUser = new User();

            existingUser = _userRepository.GetUserByEmailAddressAsync(emailAddress).Result;

            if (existingUser == null)
            {
                return NotFound($"Cannot update [{emailAddress}], does not exist");
            }

            try
            {
                var user = _mapper.Map<User>(userUpdateRequest);
                user.Id = existingUser.Id;
                user.EmailAddress = existingUser.EmailAddress;

                var userUpdated = _userRepository.UpdateUser(emailAddress, user).Result;
                var userResponse = _mapper.Map<UserResponse>(userUpdated);
                _logger.LogInformation($"updated user: {userResponse}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return Problem($"There was a problem processing your request:{ex}");
            }

            return NoContent();
        }
    }
}
