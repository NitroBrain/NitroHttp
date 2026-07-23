using System.Text.Json;
using NitroHttp.Core.Models;

namespace NitroHttp.Core.Helpers;

/// <summary>
/// Parses HTTP headers from text.
/// </summary>
public static class HeaderParser
{
    /// <summary>
    /// Parses headers from either a JSON object or a
    /// semicolon-separated string.
    /// </summary>
    /// <param name="input">
    /// Examples:
    ///
    /// Authorization:Bearer token;Accept:application/json
    ///
    /// or
    ///
    /// {
    ///   "Authorization": "Bearer token",
    ///   "Accept": "application/json"
    /// }
    /// </param>
    public static IReadOnlyList<HttpHeader> Parse(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return [];
        }

        input = input.Trim();

        return input.StartsWith('{') ? ParseJson(input) : ParseText(input);
    }

    private static IReadOnlyList<HttpHeader> ParseJson(string json)
    {
        var headers = new List<HttpHeader>();

        using var document = JsonDocument.Parse(json);

        foreach (var property in document.RootElement.EnumerateObject())
        {
            headers.Add(new HttpHeader
            {
                Name = property.Name,
                Value = property.Value.GetString() ?? string.Empty
            });
        }

        return headers;
    }

    private static IReadOnlyList<HttpHeader> ParseText(string text)
    {
        var headers = new List<HttpHeader>();

        foreach (var item in text.Split(';', StringSplitOptions.RemoveEmptyEntries))
        {
            var pair = item.Split(':', 2);

            if (pair.Length != 2)
            {
                continue;
            }

            headers.Add(new HttpHeader
            {
                Name = pair[0].Trim(),
                Value = pair[1].Trim()
            });
        }

        return headers;
    }
}
