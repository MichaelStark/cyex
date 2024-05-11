using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Cyex.Enums;
using Cyex.Interfaces;
using Cyex.Models;

namespace Cyex.Services;

public class GitHubService(string accessToken, HttpClient httpClient, ILogger<GitHubService> logger)
    : IThirdPartyService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private const string ApiUrl = "https://api.github.com/graphql";
    private const int PageSize = 100;

    private int _rateLimitRemaining;
    private DateTime _rateLimitReset;
    private readonly object _rateLimitLock = new();

    public async IAsyncEnumerable<SecurityVulnerabilityResponse> GetSecurityVulnerabilitiesAsync(
        EcosystemType ecosystem,
        string packageName
    )
    {
        var cursor = "";
        var hasNextPage = true;

        while (hasNextPage)
        {
            var response = await Query(ecosystem, packageName, cursor);
            yield return response;

            hasNextPage = response.Data.SecurityVulnerabilities.PageInfo.HasNextPage;
            cursor = response.Data.SecurityVulnerabilities.PageInfo.EndCursor;
        }
    }

    private async Task<SecurityVulnerabilityResponse> Query(
        EcosystemType ecosystem,
        string packageName,
        string cursor
    )
    {
        // TODO add a cache service
        await CheckRateLimit();

        var query = $$"""
                      {
                        "query": "{\n  securityVulnerabilities(ecosystem: {{ecosystem.GetName()}}, first: {{PageSize}}, after: \"{{cursor}}\", package: \"{{packageName}}\") {\n    nodes {\n      severity\n      advisory {\n        summary\n      }\n      package {\n        name\n        ecosystem\n      }\n      vulnerableVersionRange\n      firstPatchedVersion {\n        identifier\n      }\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}"
                      }
                      """;
        var request = new HttpRequestMessage(HttpMethod.Post, ApiUrl);
        request.Headers.Add("Authorization", $"Bearer {accessToken}");
        request.Headers.UserAgent.Add(new ProductInfoHeaderValue("Cyex", null));
        request.Content = new StringContent(query, Encoding.UTF8, "application/json");

        var response = await httpClient.SendAsync(request);
        UpdateRateLimit(response);
        var responseData = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Failed to get info from GitHub API. Status code: \"{response.StatusCode}\"; Response: \"{responseData}\"."
            );
        }

        var result = JsonSerializer.Deserialize<SecurityVulnerabilityResponse>(responseData, JsonOptions);
        if (result?.Data == null)
        {
            throw new InvalidOperationException($"Failed to deserialize response: \"{responseData}\".");
        }

        return result;
    }

    private void UpdateRateLimit(HttpResponseMessage response)
    {
        var headers = response.Headers;
        if (!headers.Contains("x-ratelimit-limit") ||
            !headers.Contains("x-ratelimit-remaining") ||
            !headers.Contains("x-ratelimit-reset")) return;

        lock (_rateLimitLock)
        {
            _rateLimitRemaining = int.Parse(headers.GetValues("x-ratelimit-remaining").First());
            var resetTimeUnix = long.Parse(headers.GetValues("x-ratelimit-reset").First());
            logger.LogInformation(
                "Response ratelimit-remaining: {RaitLimitRemaining}; ratelimit-reset: {ResetTimeUnix}",
                _rateLimitRemaining, resetTimeUnix
            );

            var newResetTime = DateTimeOffset.FromUnixTimeSeconds(resetTimeUnix).UtcDateTime;

            if (_rateLimitReset < newResetTime)
            {
                _rateLimitReset = newResetTime;
            }
        }
    }

    private async Task CheckRateLimit()
    {
        if (DateTime.UtcNow < _rateLimitReset && _rateLimitRemaining == 0)
        {
            var waitTime = _rateLimitReset - DateTime.UtcNow;
            logger.LogInformation("Rate limit wait time: {WaitTime}.", waitTime);
            await Task.Delay(waitTime);
        }
    }
}