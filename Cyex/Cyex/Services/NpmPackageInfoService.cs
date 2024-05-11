using Cyex.Enums;
using Cyex.Helpers;
using Cyex.Interfaces;
using Cyex.Models;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Cyex.Services;

public class NpmPackageInfoService(ILogger<NpmPackageInfoService> logger) : IPackageInfoService
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

    public VulnerablePackage? GetVulnerablePackage(SecurityVulnerabilityResponse response, string packageName, string packageVersion, EcosystemType ecosystemType)
    {
        foreach (var vulnerabilityNode in response.Data.SecurityVulnerabilities.Nodes)
        {
            try
            {
                var parts = vulnerabilityNode.VulnerableVersionRange.Split(' ');

                if (parts.Length < 2) continue;

                bool isInRange;
                if (parts.Length == 4)
                {
                    isInRange = ParseHelper.IsVersionInRange(packageVersion, parts[0], parts[1].TrimEnd(','))
                                && ParseHelper.IsVersionInRange(packageVersion, parts[2], parts[3]);
                }
                else
                {
                    isInRange = ParseHelper.IsVersionInRange(packageVersion, parts[0], parts[1]);
                }

                if (!isInRange) continue;

                return new VulnerablePackage
                (
                    packageName,
                    packageVersion,
                    vulnerabilityNode.Advisory.Summary,
                    vulnerabilityNode.Severity,
                    vulnerabilityNode.FirstPatchedVersion?.Identifier
                );
            }
            catch (FormatException ex)
            {
                // skip parse errors
                logger.LogError(
                    "Invalid version format: \"{Range}\"; Message: \"{Message}\"; PackageName: \"{Name}\"; Ecosystem: \"{Ecosystem}\".",
                    vulnerabilityNode.VulnerableVersionRange,
                    ex.Message,
                    packageName,
                    ecosystemType
                );
            }
        }

        return null;
    }
}