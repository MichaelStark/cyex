namespace Cyex.Interfaces;

public interface IPackageInfoService
{
    IAsyncEnumerable<(string Name, string Version)> GetDependencyPackagesAsync(string fileContent);
}