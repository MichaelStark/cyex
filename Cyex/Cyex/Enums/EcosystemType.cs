using Cyex.Interfaces;
using Cyex.Services;

namespace Cyex.Enums;

public enum EcosystemType
{
    Npm
}

public static class EcosystemTypeExtensions
{
    private class EcosystemInfo(string name, Func<IServiceProvider, IPackageInfoService> requiredService)
    {
        public string Name { get; } = name;
        public Func<IServiceProvider, IPackageInfoService> RequiredService { get; } = requiredService;
    }

    private static readonly Dictionary<EcosystemType, EcosystemInfo> EcosystemMap =
        new()
        {
            {
                EcosystemType.Npm,
                new EcosystemInfo
                (
                    "NPM",
                    serviceProvider => serviceProvider.GetRequiredService<NpmPackageInfoService>()
                )
            }
        };

    public static string GetName(this EcosystemType ecosystem)
        => EcosystemMap[ecosystem].Name;

    public static IPackageInfoService GetRequiredService(
        this EcosystemType ecosystem,
        IServiceProvider serviceProvider
    )
        => EcosystemMap[ecosystem].RequiredService(serviceProvider);
}