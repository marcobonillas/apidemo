using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using ApiDemo.Contracts;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using NUnit.Framework;

namespace ApiDemoTests
{
    public class UserApiTests
    {
        private static IHost _server;

        [SetUp]
        public void Setup()
        {
            var hostBuilder = TestSelfHostHelper.GetTestSelfWebHost();
            _server = hostBuilder.Start();
        }

        #region Create Tests
        [Test]
        public void CREATEShouldbeAbleToCreateUserWithValidData()
        {
            var result = CommonUtils.CreateRandomUser(_server);
            
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(result.Headers.Location, Is.Not.Null);
        }
        
        [Test]
        public void CREATEShouldFailToCreateWithIncompleteParameters()
        {
            var client = _server.GetTestClient();
            var requestUrl = "/user";
            var newUser = new UserRequest
            {
                FirstName = "The",
                LastName = "Dude",
                PhoneNumber = "555-123-4567"
            };

            var jsonStringUser = JsonConvert.SerializeObject(newUser);

            var data = new StringContent(jsonStringUser, Encoding.UTF8, "application/json");

            var response = client.PostAsync(requestUrl, data);
            var result = response.Result;

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public void CREATEShouldFailToCreateWithMissingFirstName()
        {
            var client = _server.GetTestClient();
            var requestUrl = "/user";
            var newUser = new UserRequest
            {
                LastName = "Dude",
                PhoneNumber = "555-123-marco",
                EmailAddress = "thetest@apidemo.com"
            };

            var jsonStringUser = JsonConvert.SerializeObject(newUser);

            var data = new StringContent(jsonStringUser, Encoding.UTF8, "application/json");

            var response = client.PostAsync(requestUrl, data);
            var result = response.Result;

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public void CREATEShouldFailToCreateWithMissingLastName()
        {
            var client = _server.GetTestClient();
            var requestUrl = "/user";
            var newUser = new UserRequest
            {
                FirstName = "somethingFN",
                PhoneNumber = "555-123-marco",
                EmailAddress = "thetest@apidemo.com"
            };

            var jsonStringUser = JsonConvert.SerializeObject(newUser);

            var data = new StringContent(jsonStringUser, Encoding.UTF8, "application/json");

            var response = client.PostAsync(requestUrl, data);
            var result = response.Result;

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public void CREATEShouldFailToCreateWithInvalidPhone()
        {
            var client = _server.GetTestClient();
            var requestUrl = "/user";
            var newUser = new UserRequest
            {
                FirstName = "The",
                LastName = "Dude",
                PhoneNumber = "555-123-marco",
                EmailAddress = "thetest@apidemo.com"
            };

            var jsonStringUser = JsonConvert.SerializeObject(newUser);

            var data = new StringContent(jsonStringUser, Encoding.UTF8, "application/json");

            var response = client.PostAsync(requestUrl, data);
            var result = response.Result;

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public void CREATEShouldFailToCreateWhenIsDuplicateEmail()
        {
            var client = _server.GetTestClient();
            var requestUrl = "/user";

            var randomString = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 8);
            var emailToSearch = $"found_{randomString}@apidemo.com";

            CommonUtils.CreateRandomUser(_server, emailToSearch);

            var newUser = new UserRequest
            {
                FirstName = "The",
                LastName = "Dude",
                PhoneNumber = "555-123-3333",
                EmailAddress = emailToSearch
            };
            
            var jsonStringUser = JsonConvert.SerializeObject(newUser);

            var data = new StringContent(jsonStringUser, Encoding.UTF8, "application/json");

            var response = client.PostAsync(requestUrl, data);
            var result = response.Result;

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
        }

        #endregion

        #region GET Tests

        [Test]
        public void GETShoulGetUserWhenEmailExists()
        {
            var randomString = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 8);
            var emailToSearch = $"found_{randomString}@apidemo.com";

            CommonUtils.CreateRandomUser(_server,emailToSearch);

            Thread.Sleep(1000);

            var client = _server.GetTestClient();

            var requestUrl = $"/user?emailAddress={emailToSearch}";

            var response = client.GetAsync(requestUrl);
            var result = response.Result;
            var content = result.Content.ReadAsStringAsync();

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public void GETShoulGetUserWhenEmailExistsWithProperName()
        {
            var randomString = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 8);
            var emailToSearch = $"found_{randomString}@apidemo.com";

            var email = $"theDude_{randomString}@apidemo.com";

            var newUser = new UserRequest
            {
                FirstName = $"The_{randomString}",
                LastName = $"Dude_{randomString}",
                MiddleName = $"mn_{randomString}",
                PhoneNumber = $"555-123-4567",
                EmailAddress = email
            };

            CommonUtils.CreateTestUser(_server,newUser);

            Thread.Sleep(1000);

            var client = _server.GetTestClient();
            var userFromApi = CommonUtils.GetUserByEmailAddress(email,client);
            var expectedName = $"{newUser.FirstName} {newUser.MiddleName} {newUser.LastName}";
            Assert.That(userFromApi.Name, Is.EqualTo(expectedName));
        }

        [Test]
        public void GETShoulGetNotFoundWhenEmailDoesNotExist()
        {
            var client = _server.GetTestClient();
            var requestUrl = "/user?emailAddress=testuserFake@apidemo.com";

            var response = client.GetAsync(requestUrl);
            var result = response.Result;
            var content = result.Content.ReadAsStringAsync();

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public void GETShoulGetBadArgumentsWhenNotSearchingByEmail()
        {
            var client = _server.GetTestClient();
            var requestUrl = "/user?firstName=testuserFake@apidemo.com";

            var response = client.GetAsync(requestUrl);
            var result = response.Result;

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        #endregion

        #region PUT

        [Test]
        public void PUTShoulUpdateUserWhenAccountExists()
        {
            var randomString = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 8);
            var emailForUpdate = $"found_{randomString}@apidemo.com";

            CommonUtils.CreateRandomUser(_server, emailForUpdate);

            Thread.Sleep(1000);

            var client = _server.GetTestClient();

            var requestUrl = $"/user/{emailForUpdate}";
            var updateUserRequest = new UserUpdateRequest()
            {
                FirstName = "The",
                LastName = "DudeUpdated",
                MiddleName = "Up_Down",
                PhoneNumber = "555-123-9999",
            };

            var jsonStringUser = JsonConvert.SerializeObject(updateUserRequest);

            var data = new StringContent(jsonStringUser, Encoding.UTF8, "application/json");

            var response = client.PutAsync(requestUrl, data);
            var result = response.Result;

            var userReponse = CommonUtils.GetUserByEmailAddress(emailForUpdate, client);

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
            Assert.That(userReponse.Name.Contains(updateUserRequest.MiddleName));
            Assert.That(userReponse.PhoneNumber, Is.EqualTo(updateUserRequest.PhoneNumber));
        }

        [Test]
        public void PUTShoulNotUpdateUserWhenAccountDoesNotExists()
        {

            var client = _server.GetTestClient();

            var requestUrl = $"/user/fakeEmailHere@apidemo.com";
            var updateUserRequest = new UserUpdateRequest()
            {
                FirstName = "The",
                LastName = "DudeUpdated",
                MiddleName = "Up_Down",
                PhoneNumber = "555-123-9999",
            };

            var jsonStringUser = JsonConvert.SerializeObject(updateUserRequest);

            var data = new StringContent(jsonStringUser, Encoding.UTF8, "application/json");

            var response = client.PutAsync(requestUrl, data);
            var result = response.Result;

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        }
        

        #endregion

    }
}