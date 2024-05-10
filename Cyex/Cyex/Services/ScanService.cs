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
            var response = await thirdPartyService.GetSecurityVulnerabilitiesAsync(request.Ecosystem, packageName);
            var package = packageInfoService.GetVulnerablePackage(
                response,
                packageName,
                packageVersion,
                request.Ecosystem
            );
            if (package != null) vulnerablePackages.Add(package);
        }

        return new ScanResult(vulnerablePackages);
    }
}