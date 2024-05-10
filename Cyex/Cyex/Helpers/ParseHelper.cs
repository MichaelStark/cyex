﻿using System.Text;
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
            throw new ArgumentException("Invalid Base64.");
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
            throw new ArgumentException("Invalid JSON.");
        }
    }
}