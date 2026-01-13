using System.Text.Json;
using System.Text.RegularExpressions;

namespace NitroHttp.Helpers;

public static partial class JsonSyntaxHighlighter
{
    public static FormattedString Highlight(string json)
    {
        var formatted = new FormattedString();

        try
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);
            json = JsonSerializer.Serialize(jsonElement, new JsonSerializerOptions { WriteIndented = true });
        }
        catch
        {
            formatted.Spans.Add(new Span { Text = json, TextColor = Color.FromArgb("#E8E9ED") });
            return formatted;
        }

        var regex = JsonTokenRegex();

        foreach (Match match in regex.Matches(json))
        {
            var span = new Span { Text = match.Value };

            if (match.Value.StartsWith('"') && match.Value.EndsWith("\":"))
            {
                span.TextColor = Color.FromArgb("#FC4850");
            }
            else if (match.Value.StartsWith('"'))
            {
                span.TextColor = Color.FromArgb("#4ADE80");
            }
            else if (match.Value is "true" or "false")
            {
                span.TextColor = Color.FromArgb("#F59E0B");
            }
            else if (match.Value is "null")
            {
                span.TextColor = Color.FromArgb("#6B6D75");
            }
            else if (double.TryParse(match.Value, out _))
            {
                span.TextColor = Color.FromArgb("#4F7CFF");
            }
            else
            {
                span.TextColor = Color.FromArgb("#E8E9ED");
            }

            formatted.Spans.Add(span);
        }

        return formatted;
    }

    [GeneratedRegex(@"""[^""\\]*(?:\\.[^""\\]*)*""\s*:|""[^""\\]*(?:\\.[^""\\]*)*""|-?\d+\.?\d*|true|false|null|[{}\[\],:\s]+")]
    private static partial Regex JsonTokenRegex();
}