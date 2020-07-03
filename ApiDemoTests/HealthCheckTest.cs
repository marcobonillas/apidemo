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
    public class HealthCheckTest

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


    }
}