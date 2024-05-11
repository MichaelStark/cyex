namespace Cyex.Models;

public class ScanResult(List<VulnerablePackage> vulnerablePackages)
{
    public List<VulnerablePackage> VulnerablePackages { get; set; } = vulnerablePackages;
}