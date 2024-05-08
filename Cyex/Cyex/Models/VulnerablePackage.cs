namespace Cyex.Models;

public class VulnerablePackage
{
    public string Name { get; set; }

    public string Version { get; set; }

    public string Summary { get; set; }

    public string Severity { get; set; }

    public string? FirstPatchedVersion { get; set; }
}