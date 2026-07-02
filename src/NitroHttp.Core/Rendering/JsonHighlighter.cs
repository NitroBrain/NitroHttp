using System.Text.RegularExpressions;
using NitroHttp.Core.Enums;

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
        _ when m.Groups["key"].Success => Colorize(Colors.Cyan, m.Groups["key"].Value),
        _ when m.Groups["str"].Success => Colorize(Colors.Green, m.Groups["str"].Value),
        _ when m.Groups["num"].Success => m.Value[..^m.Groups["num"].Length] + Colorize(Colors.Orange, m.Groups["num"].Value),
        _ when m.Groups["bool"].Success => m.Value[..^m.Groups["bool"].Length] + Colorize(Colors.Purple, m.Groups["bool"].Value),
        _ when m.Groups["null"].Success => m.Value[..^m.Groups["null"].Length] + Colorize(Colors.Gray, m.Groups["null"].Value),
        _ => m.Value
      };
    });
  }

  public static string Colorize(Colors color, string text)
  {
    return color switch
    {
      Colors.Cyan => Ansi.Cyan + text + Ansi.Reset,
      Colors.Green => Ansi.Green + text + Ansi.Reset,
      Colors.Orange => Ansi.Orange + text + Ansi.Reset,
      Colors.Purple => Ansi.Purple + text + Ansi.Reset,
      Colors.Gray => Ansi.Gray + text + Ansi.Reset,
      Colors.Yellow => Ansi.Yellow + text + Ansi.Reset,
      Colors.Red => Ansi.Red + text + Ansi.Reset,
      _ => text,
    };
  }
}
