using Cyex.Interfaces;
using Cyex.Models;

namespace Cyex.Services;

public class ScanService(IThirdPartyService thirdPartyService, ILogger<ScanService> logger) : IScanService
{
    public async Task<ScanResult> Scan(ScanRequest request)
    {
        var response = await thirdPartyService.RenameMe();
        var vulnerablePackages = new List<VulnerablePackage>
        {
            new()
            {
                Name = "deep-override",
                Version = "1.0.1",
                Summary = "Prototype Pollution in deep-override",
                Severity = "CRITICAL",
                FirstPatchedVersion = "1.0.2"
            },
            new()
            {
                Name = "express",
                Version = "4.17.1",
                Summary = "qs vulnerable to Prototype Pollution",
                Severity = "HIGH",
                FirstPatchedVersion = "4.17.3"
            }
        };

        return new ScanResult { VulnerablePackages = vulnerablePackages };
    }
}