using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using ApiDemo;
using ApiDemo.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
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
            var projectDirectory = GetDirectoryForProject("", typeof(Startup).GetTypeInfo().Assembly);
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer()
                        .UseEnvironment("Development")
                        .UseContentRoot(projectDirectory)
                        .UseConfiguration(new ConfigurationBuilder()
                            .SetBasePath(projectDirectory)
                            .AddJsonFile("appsettings.json")
                            .Build());

                    webHost.UseStartup<Startup>();
                });

            _server = hostBuilder.Start();
        }


        private static string GetDirectoryForProject(string projectRelativePath, Assembly startupAssembly)
        {

            var projectName = startupAssembly.GetName().Name;


            var applicationBasePath = System.AppContext.BaseDirectory;

            var directoryInfo = new DirectoryInfo(applicationBasePath);
            do
            {
                directoryInfo = directoryInfo.Parent;

                var projectDirectoryInfo = new DirectoryInfo(Path.Combine(directoryInfo.FullName, projectRelativePath));
                if (projectDirectoryInfo.Exists)
                {
                    var projectFileInfo = new FileInfo(Path.Combine(projectDirectoryInfo.FullName, projectName, $"{projectName}.csproj"));
                    if (projectFileInfo.Exists)
                    {
                        return Path.Combine(projectDirectoryInfo.FullName, projectName);
                    }
                }
            }
            while (directoryInfo.Parent != null);

            throw new Exception($"Project root could not be located using the application root {applicationBasePath}.");
        }

        [Test]
        public void ShouldbeAbleToCreateUserWithValidData()
        {
            var result = CreateRandomUser();

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(result.Headers.Location, Is.Not.Null);
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
            var randomString = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 8);
            var emailToSearch = $"found_{randomString}@apidemo.com";

            CreateRandomUser(emailToSearch);

            Thread.Sleep(3000);

            var client = _server.GetTestClient();
            var requestUrl = $"/user?emailAddress={emailToSearch}";

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

        private HttpResponseMessage CreateRandomUser(string email=null)
        {
            var client = _server.GetTestClient();
            var requestUrl = "/user";

            var randomString = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 8);

            email ??= $"theDude_{randomString}@apidemo.com";

            var newUser = new UserRequest
            {
                FirstName = $"The_{randomString}",
                LastName = $"Dude_{randomString}",
                MiddleName = $"mn_{randomString}",
                PhoneNumber = $"555-123-4567",
                EmailAddress = email
            };

            var jsonStringUser = JsonConvert.SerializeObject(newUser);

            Console.WriteLine(jsonStringUser);

            var data = new StringContent(jsonStringUser, Encoding.UTF8, "application/json");

            var response = client.PostAsync(requestUrl, data);
            var result = response.Result;
            return result;
        }
    }
}