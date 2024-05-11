using Cyex.Enums;
using Cyex.Models;

namespace Cyex.Interfaces;

public interface IPackageInfoService
{
    IAsyncEnumerable<(string Name, string Version)> GetDependencyPackagesAsync(string fileContent);

    VulnerablePackage? GetVulnerablePackage(SecurityVulnerabilityResponse response, string packageName, string packageVersion, EcosystemType ecosystemType);
}