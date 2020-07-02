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
            var response = client.GetAsync("/user/info");
            var result = response.Result;
            
            Assert.That(result.StatusCode,Is.EqualTo(HttpStatusCode.OK));

            var content = result.Content.ReadAsStringAsync();
            Assert.That(content.Result, Is.EqualTo("alive"));
        }

    }
}