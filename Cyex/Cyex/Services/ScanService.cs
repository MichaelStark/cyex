using Cyex.Enums;
using Cyex.Interfaces;
using Cyex.Models;

namespace Cyex.Services;

public class ScanService(
    IServiceProvider serviceProvider,
    IThirdPartyService thirdPartyService
) : IScanService
{
    public async Task<ScanResult> Scan(ScanRequest request)
    {
        var packageInfoService = request.Ecosystem.GetRequiredService(serviceProvider);
        var dependencyPackages = packageInfoService.GetDependencyPackagesAsync(request.FileContent);
        var vulnerablePackages = new List<VulnerablePackage>();

        await foreach (var (packageName, packageVersion) in dependencyPackages)
        {
            var vulnerabilityEnumerable = thirdPartyService.GetSecurityVulnerabilitiesAsync(
                request.Ecosystem,
                packageName
            );
            await foreach (var vulnerability in vulnerabilityEnumerable)
            {
                var package = packageInfoService.GetVulnerablePackage(
                    vulnerability,
                    packageName,
                    packageVersion,
                    request.Ecosystem
                );

                if (package == null) continue;
                vulnerablePackages.Add(package);
                break;
            }
        }

        return new ScanResult(vulnerablePackages);
    }
}