namespace Cyex.Models;

public class VulnerablePackage(
    string name,
    string version,
    string summary,
    string severity,
    string? firstPatchedVersion)
{
    public string Name { get; set; } = name;
    public string Version { get; set; } = version;
    public string Summary { get; set; } = summary;
    public string Severity { get; set; } = severity;
    public string? FirstPatchedVersion { get; set; } = firstPatchedVersion;
}