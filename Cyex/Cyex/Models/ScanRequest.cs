using System.Text.Json.Serialization;
using Cyex.Enums;

namespace Cyex.Models;

public class ScanRequest(EcosystemType ecosystem, string fileContent)
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EcosystemType Ecosystem { get; } = ecosystem;

    public string FileContent { get; } = fileContent;
}