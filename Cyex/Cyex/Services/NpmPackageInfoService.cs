using Cyex.Helpers;
using Cyex.Interfaces;
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Cyex.Services;

public class NpmPackageInfoService : IPackageInfoService
{
    public async IAsyncEnumerable<(string Name, string Version)> GetDependencyPackagesAsync(string fileContent)
    {
        var packageJson = ParseHelper.DecodeBase64(fileContent);
        var root = ParseHelper.ParseJson(packageJson).RootElement;

        if (!root.TryGetProperty("dependencies", out var dependenciesElement))
        {
            yield break;
        }

        foreach (var property in dependenciesElement.EnumerateObject())
        {
            var name = property.Name;
            var version = property.Value.GetString();
            if (version == null)
            {
                throw new ArgumentException($"Version is missing for dependency: {name}.");
            }

            yield return (name, version);
        }
    }
}