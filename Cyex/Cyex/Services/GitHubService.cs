using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Cyex.Enums;
using Cyex.Interfaces;
using Cyex.Models;

namespace Cyex.Services;

public class GitHubService(HttpClient httpClient, string accessToken) : IThirdPartyService
{
    private const string ApiUrl = "https://api.github.com/graphql";

    public async Task<SecurityVulnerabilityResponse> GetSecurityVulnerabilitiesAsync(
        EcosystemType ecosystem,
        string packageName
    )
    {
        // TODO check for possible injections
        var query = $$"""
                      {
                        "query": "{\n  securityVulnerabilities(ecosystem: {{ecosystem.GetName()}}, first: 100, package: \"{{packageName}}\") {\n    nodes {\n      severity\n      advisory {\n        summary\n      }\n      package {\n        name\n        ecosystem\n      }\n      vulnerableVersionRange\n      firstPatchedVersion {\n        identifier\n      }\n    }\n  }\n}"
                      }
                      """;

        var request = new HttpRequestMessage(HttpMethod.Post, ApiUrl);
        request.Headers.Add("Authorization", $"Bearer {accessToken}");
        request.Headers.UserAgent.Add(new ProductInfoHeaderValue("Cyex", null));
        request.Content = new StringContent(query, Encoding.UTF8, "application/json");

        // TODO limit execution
        var response = await httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Failed to get info from GitHub API. Status code: {response.StatusCode}");
        }

        var responseData = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<SecurityVulnerabilityResponse>(responseData, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        return result ?? throw new InvalidOperationException("Failed to deserialize response.");
    }
}