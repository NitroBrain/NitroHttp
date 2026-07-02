using System.Text.Json;

namespace NitroHttp.Core.Helpers;

public static class FormatJson
{
    public static string TryFormatJson(string text)
    {
        try
        {
            using var doc = JsonDocument.Parse(text);
            using var stream = new MemoryStream();
            using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true }))
            {
                doc.RootElement.WriteTo(writer);
            }

            return System.Text.Encoding.UTF8.GetString(stream.ToArray());
        }
        catch
        {
            return text;
        }
    }
}
