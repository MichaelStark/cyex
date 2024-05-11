using Cyex.Enums;
using Cyex.Models;

namespace Cyex.Interfaces;

public interface IThirdPartyService
{
    IAsyncEnumerable<SecurityVulnerabilityResponse> GetSecurityVulnerabilitiesAsync(EcosystemType ecosystem, string packageName);
}