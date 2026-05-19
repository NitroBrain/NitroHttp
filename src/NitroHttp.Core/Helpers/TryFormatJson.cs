using System.Text.Json;

namespace NitroHttp.Core.Helpers;

public class FormatJson
{
    public static string TryFormatJson(string text)
    {
        try
        {
            using var doc = JsonDocument.Parse(text);
            return JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
        }
        catch
        {
            return text;
        }
    }
}
