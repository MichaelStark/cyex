using Cyex.Enums;
using Cyex.Interfaces;
using Cyex.Models;

namespace Cyex.Services;

public class ScanService(
    IServiceProvider serviceProvider,
    IThirdPartyService thirdPartyService) : IScanService
{
    public async Task<ScanResult> Scan(ScanRequest request)
    {
        // TODO add tests
        var dependencyPackages = request
            .Ecosystem
            .GetRequiredService(serviceProvider)
            .GetDependencyPackagesAsync(request.FileContent);

        var vulnerablePackages = new List<VulnerablePackage>();

        await foreach (var (packageName, packageVersion) in dependencyPackages)
        {
            var vulnerableResponse =
                await thirdPartyService.GetSecurityVulnerabilitiesAsync(request.Ecosystem, packageName);

            // TODO add version checking logic
            vulnerablePackages.AddRange(vulnerableResponse.Data.SecurityVulnerabilities.Nodes.Select(
                vulnerabilityNode => new VulnerablePackage
                (
                    packageName,
                    packageVersion,
                    vulnerabilityNode.Advisory.Summary,
                    vulnerabilityNode.Severity,
                    vulnerabilityNode.FirstPatchedVersion?.Identifier
                )
            ));
        }

        return new ScanResult(vulnerablePackages);
    }
}