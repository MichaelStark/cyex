using System.Text;
using Cyex.Interfaces;

namespace Cyex.Services;

public class GitHubService(HttpClient httpClient, string accessToken) : IThirdPartyService
{
    private const string ApiUrl = "https://api.github.com/graphql";

    public async Task<string> RenameMe()
    {
        var query = "";
        var request = new HttpRequestMessage(HttpMethod.Post, ApiUrl);
        request.Headers.Add("Authorization", "Bearer " + accessToken);
        request.Content = new StringContent(query, Encoding.UTF8, "application/json");

        var response = await httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Failed to get info from GitHub API. Status code: " + response.StatusCode);
        }

        return await response.Content.ReadAsStringAsync();
    }
}