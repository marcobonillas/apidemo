using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using ApiDemo;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace ApiDemoTests
{
    public class ApiDemoUsersTests 
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
        public void ShouldCallUserHealthCheckEndPoint()
        {
            var client = _server.GetTestClient();
            var response = client.GetAsync("/healthcheck");
            var result = response.Result;
            var content = result.Content.ReadAsStringAsync();

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content.Result, Is.EqualTo("alive"));
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