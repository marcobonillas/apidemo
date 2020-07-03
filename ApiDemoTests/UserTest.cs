using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using ApiDemo;
using ApiDemo.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using NUnit.Framework;

namespace ApiDemoTests
{
    public class UserTest
    {
        private static IHost _server;
        
        [SetUp]
        public void Setup()
        {
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.UseStartup<Startup>();
                });

            _server = hostBuilder.Start();
        }

        [Test]
        public void ShouldbeAbleToCreateUserWithValidData()
        {
            var client = _server.GetTestClient();
            var requestUrl = "/user";
            var newUser = new UserRequest
            {
                FirstName = "The",
                LastName = "Dude",
                PhoneNumber = "555-123-4567",
                EmailAddress = "theDude@apidemo.com"
            };

            var jsonStringUser = JsonConvert.SerializeObject(newUser);

            var data = new StringContent(jsonStringUser, Encoding.UTF8,"application/json");

            var response = client.PostAsync(requestUrl,data);
            var result = response.Result;

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(result.Headers.Location,Is.Not.Null);
        }


        [Test]
        public void ShouldFailToCreateWithIncompleteParameters()
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
        public void ShouldFailToCreateWithMissingFirstName()
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
        public void ShouldFailToCreateWithMissingLastName()
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
        public void ShouldFailToCreateWithInvalidPhone()
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
        public void ShoulGetUserWhenEmailExists()
        {
            var client = _server.GetTestClient();
            var requestUrl = "/user?emailAddress=testuser@apidemo.com";

            var response = client.GetAsync(requestUrl);
            var result = response.Result;
            var content = result.Content.ReadAsStringAsync();

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            
        }

        [Test]
        public void ShoulGetNotFoundWhenEmailDoesNotExist()
        {
            var client = _server.GetTestClient();
            var requestUrl = "/user?emailAddress=testuserFake@apidemo.com";

            var response = client.GetAsync(requestUrl);
            var result = response.Result;
            var content = result.Content.ReadAsStringAsync();

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public void ShoulGetBadArgumentsWhenNotSearchingByEmail()
        {
            var client = _server.GetTestClient();
            var requestUrl = "/user?firstName=testuserFake@apidemo.com";

            var response = client.GetAsync(requestUrl);
            var result = response.Result;
            var content = result.Content.ReadAsStringAsync();

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }
    }
}