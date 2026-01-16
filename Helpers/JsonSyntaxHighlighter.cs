using System.Text.Json;
using System.Text.RegularExpressions;

namespace NitroHttp.Helpers;

public static partial class JsonSyntaxHighlighter
{
    public static FormattedString Highlight(string json)
    {
        var formatted = new FormattedString();
        var isDark = Application.Current?.RequestedTheme == AppTheme.Dark;

        var keyColor = Color.FromArgb("#FF4757");
        var stringColor = Color.FromArgb("#2ED573");
        var boolColor = Color.FromArgb("#FFA502");
        var nullColor = isDark ? Color.FromArgb("#6B6D75") : Color.FromArgb("#999999");
        var numberColor = Color.FromArgb("#5B8CFF");
        var punctuationColor = isDark ? Color.FromArgb("#CCCCCC") : Color.FromArgb("#333333");

        try
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);
            json = JsonSerializer.Serialize(jsonElement, new JsonSerializerOptions { WriteIndented = true });
        }
        catch
        {
            formatted.Spans.Add(new Span { Text = json, TextColor = punctuationColor });
            return formatted;
        }

        var regex = JsonTokenRegex();

        MatchCollection list = regex.Matches(json);
        for (int i = 0; i < list.Count; i++)
        {
            Match match = list[i];
            var span = new Span { Text = match.Value };

            if (match.Value.StartsWith('"') && match.Value.EndsWith("\":"))
            {
                span.TextColor = keyColor;
            }
            else if (match.Value.StartsWith('"'))
            {
                span.TextColor = stringColor;
            }
            else if (match.Value is "true" or "false")
            {
                span.TextColor = boolColor;
            }
            else if (match.Value is "null")
            {
                span.TextColor = nullColor;
            }
            else if (double.TryParse(match.Value, out _))
            {
                span.TextColor = numberColor;
            }
            else
            {
                span.TextColor = punctuationColor;
            }

            formatted.Spans.Add(span);
        }

        return formatted;
    }

    [GeneratedRegex(@"""[^""\\]*(?:\\.[^""\\]*)*""\s*:|""[^""\\]*(?:\\.[^""\\]*)*""|-?\d+\.?\d*|true|false|null|[{}\[\],:\s]+")]
    private static partial Regex JsonTokenRegex();
}