using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Cyex.Enums;
using Cyex.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace Cyex.Tests.IntegrationTests
{
    public class ScanControllerTests : IClassFixture<TestServerFixture>
    {
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        private readonly HttpClient _client;

        public ScanControllerTests(TestServerFixture testServerFixture)
        {
            _client = testServerFixture.Server.CreateClient();
            _client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
        }

        [Fact]
        public async Task Scan_ReturnsBadRequestForInvalidRequest()
        {
            // Arrange
            var request = new ScanRequest(EcosystemType.Npm, null!);

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/Scan", request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData(
            100,
            EcosystemType.Npm,
            "ewogICAgIm5hbWUiOiAiTXkgQXBwbGljYXRpb24iLAogICAgInZlcnNpb24iOiAiMS4wLjAiLAogICAgImRlcGVuZGVuY2llcyI6IHsKICAgICAgICAidW5kZXJzY29yZSI6ICIxLjMuMSIKICAgIH0KfQ==")]
        public async Task Scan_ReturnsOkWithEmptyResult(int numberOfRequests, EcosystemType ecosystem,
            string fileContent)
        {
            // Act
            await DoLoadRequestAsync(numberOfRequests, ecosystem, fileContent, async task =>
            {
                var response = await task;
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ScanResult>(content, JsonOptions);
                Assert.NotNull(result);
                Assert.Empty(result.VulnerablePackages);
            });
        }

        [Theory]
        [InlineData(
            100,
            EcosystemType.Npm,
            "ewogICJuYW1lIjogIk15IEFwcGxpY2F0aW9uIiwKICAidmVyc2lvbiI6ICIxLjAuMCIsCiAgImRlcGVuZGVuY2llcyI6IHsKICAgICJkZWVwLW92ZXJyaWRlIjogIjEuMC4xIiwKICAgICJleHByZXNzIjogIjQuMTcuMSIKICB9Cn0=")]
        public async Task Scan_ReturnsOkWithResult(int numberOfRequests, EcosystemType ecosystem, string fileContent)
        {
            // Act
            await DoLoadRequestAsync(numberOfRequests, ecosystem, fileContent, async task =>
            {
                var response = await task;
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ScanResult>(content, JsonOptions);
                Assert.NotNull(result);
                Assert.NotEmpty(result.VulnerablePackages);
            });
        }

        private async Task DoLoadRequestAsync(
            int numberOfRequests,
            EcosystemType ecosystem,
            string fileContent,
            Func<Task<HttpResponseMessage>, Task> checkResponseFunc
        )
        {
            // Arrange
            var requestTasks = new List<Task<HttpResponseMessage>>();
            for (var i = 0; i < numberOfRequests; i++)
            {
                var request = new ScanRequest(ecosystem, fileContent);
                var task = _client.PostAsJsonAsync("/api/v1/Scan", request);
                requestTasks.Add(task);
            }

            // Act
            await Task.WhenAll(requestTasks);

            // Assert
            foreach (var task in requestTasks)
            {
                await checkResponseFunc(task);
            }
        }
    }

    public class TestServerFixture : IDisposable
    {
        public TestServer Server { get; }

        public TestServerFixture()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();

            var hostBuilder = new WebHostBuilder()
                .UseConfiguration(configuration)
                .UseEnvironment("Development")
                .UseStartup(typeof(Startup));

            Server = new TestServer(hostBuilder);
        }

        public void Dispose()
        {
            Server.Dispose();
        }
    }
}