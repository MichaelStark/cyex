using Cyex.Enums;
using Cyex.Models;

namespace Cyex.Interfaces;

public interface IThirdPartyService
{
    Task<SecurityVulnerabilityResponse> GetSecurityVulnerabilitiesAsync(EcosystemType ecosystem, string packageName);
}