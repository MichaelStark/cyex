using Cyex.Models;

namespace Cyex.Interfaces;

public interface IScanService
{
    Task<ScanResult> Scan(ScanRequest request);
}