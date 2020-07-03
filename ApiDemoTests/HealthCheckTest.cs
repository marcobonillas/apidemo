using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
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
    public class HealthCheckTest

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


    }
}