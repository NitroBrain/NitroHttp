using System.Text.RegularExpressions;

namespace NitroHttp.Core.Rendering;

public static partial class JsonHighlighter
{
  [GeneratedRegex(@"
        (?<key>""(?:[^""\\]|\\.)*"")(?=\s*:)
      | (?<str>""(?:[^""\\]|\\.)*"")
      | :\s*(?<num>-?\d+(?:\.\d+)?(?:[eE][+-]?\d+)?)
      | :\s*(?<bool>true|false)\b
      | :\s*(?<null>null)\b
    ", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled)]
  private static partial Regex TokenRegex();

  public static string Highlight(string json)
  {
    return TokenRegex().Replace(json, static m =>
    {
      return m switch
      {
        _ when m.Groups["key"].Success => Colorize("cyan", m.Groups["key"].Value),
        _ when m.Groups["str"].Success => Colorize("green", m.Groups["str"].Value),
        _ when m.Groups["num"].Success => m.Value[..^m.Groups["num"].Length] + Colorize("orange", m.Groups["num"].Value),
        _ when m.Groups["bool"].Success => m.Value[..^m.Groups["bool"].Length] + Colorize("purple", m.Groups["bool"].Value),
        _ when m.Groups["null"].Success => m.Value[..^m.Groups["null"].Length] + Colorize("gray", m.Groups["null"].Value),
        _ => m.Value
      };
    });
  }

  public static string Colorize(string color, string text)
  {
    return color switch
    {
      "cyan" => Ansi.Cyan + text + Ansi.Reset,
      "green" => Ansi.Green + text + Ansi.Reset,
      "orange" => Ansi.Orange + text + Ansi.Reset,
      "purple" => Ansi.Purple + text + Ansi.Reset,
      "gray" => Ansi.Gray + text + Ansi.Reset,
      _ => text,
    };
  }
}
