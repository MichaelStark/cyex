using System.Text;
using System.Text.Json;

namespace Cyex.Helpers;

public static class ParseHelper
{
    public static string DecodeBase64(string base64Content)
    {
        try
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(base64Content));
        }
        catch (FormatException)
        {
            throw new ArgumentException($"Invalid Base64: \"{base64Content}\".");
        }
    }

    public static JsonDocument ParseJson(string json)
    {
        try
        {
            return JsonDocument.Parse(json);
        }
        catch (JsonException)
        {
            throw new ArgumentException($"Invalid JSON: \"{json}\".");
        }
    }

    public static bool IsVersionInRange(string leftV, string comparisonOperator, string rightV)
    {
        var leftVersion = new Version(leftV);
        var rightVersion = new Version(rightV);

        return comparisonOperator switch
        {
            "=" => leftVersion == rightVersion,
            "<=" => leftVersion <= rightVersion,
            "<" => leftVersion < rightVersion,
            ">=" => leftVersion >= rightVersion,
            ">" => leftVersion > rightVersion,
            _ => false
        };
    }
}