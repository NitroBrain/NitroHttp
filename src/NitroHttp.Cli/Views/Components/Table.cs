using System.Text;
using System.Text.RegularExpressions;
using NitroHttp.Cli.Views.Interfaces;
using NitroHttp.Core.Rendering;
using NitroHttp.Core.Enums;

namespace NitroHttp.Cli.Views.Components;

public class Table : ITable
{
    private const int ChunkSize = 500;
    private const int LeftColumnWidth = 8;

    private static readonly Regex AnsiRegex = new(@"\x1B\[[0-9;]*m", RegexOptions.Compiled);

    public void Display(string formattedJson, string endpoint = "JSON")
    {
        using var reader = new StringReader(formattedJson);
        var chunk = new List<string>(ChunkSize);
        var lineNumber = 0;

        TableHeader(Console.WindowWidth, endpoint);
        TableBody(chunk, lineNumber, reader);
        TableBottom();
    }

    public static void TableHeader(int windowWidth, string endpoint)
    {
        var rightWidth = Math.Max(10, windowWidth - (LeftColumnWidth + 5));
        var headerSb = new StringBuilder();

        headerSb.Append('┌')
            .Append('─', LeftColumnWidth + 2)
            .Append('┬')
            .Append('─', rightWidth)
            .Append('┐')
            .AppendLine();

        var parts = endpoint.Split(' ', 2);
        var method = parts[0];
        var route = parts.Length > 1 ? parts[1] : string.Empty;

        var coloredMethod = method switch
        {
            "GET" => JsonHighlighter.Colorize(Colors.Cyan, method),
            "POST" => JsonHighlighter.Colorize(Colors.Green, method),
            "PUT" => JsonHighlighter.Colorize(Colors.Yellow, method),
            "DELETE" => JsonHighlighter.Colorize(Colors.Red, method),
            _ => method
        };

        var output = $"{coloredMethod} - {route}";

        headerSb.Append('│')
            .Append(' ')
            .Append("#".PadRight(LeftColumnWidth))
            .Append(' ')
            .Append('│')
            .Append(' ')
            .Append(PadRightAnsi(output, rightWidth - 1))
            .Append('│')
            .AppendLine();

        headerSb.Append('├')
            .Append('─', LeftColumnWidth + 2)
            .Append('┼')
            .Append('─', rightWidth)
            .Append('┤')
            .AppendLine();

        Console.Write(headerSb.ToString());
    }

    public static void TableBottom()
    {
        var rightWidth = Math.Max(10, Console.WindowWidth - (LeftColumnWidth + 5));

        Console.Write('└');
        Console.Write(new string('─', LeftColumnWidth + 2));
        Console.Write('┴');
        Console.Write(new string('─', rightWidth));
        Console.WriteLine('┘');
    }

    public static void TableBody(List<string> chunk, int lineNumber, StringReader reader)
    {
        var outSb = new StringBuilder(ChunkSize * 80);

        while (true)
        {
            var line = reader.ReadLine();

            if (line is null)
            {
                if (chunk.Count > 0)
                {
                    AppendRows(outSb, chunk, lineNumber - chunk.Count + 1);
                    Console.Write(outSb.ToString());
                }

                break;
            }

            chunk.Add(line);
            lineNumber++;

            if (chunk.Count < ChunkSize)
            {
                continue;
            }

            AppendRows(outSb, chunk, lineNumber - ChunkSize + 1);

            Console.Write(outSb.ToString());

            outSb.Clear();
            chunk.Clear();
        }
    }

    public static void AppendRows(StringBuilder sb, List<string> chunk, int startLine)
    {
        var rightWidth = Console.WindowWidth - LeftColumnWidth - 7;

        for (var i = 0; i < chunk.Count; i++)
        {
            var num = (startLine + i).ToString().PadLeft(LeftColumnWidth);
            var highlighted = JsonHighlighter.Highlight(chunk[i]);
            var lineNumber = JsonHighlighter.Colorize(Colors.Orange, num);

            sb.Append('│')
              .Append(' ')
              .Append(lineNumber)
              .Append(' ')
              .Append('│')
              .Append(' ')
              .Append(PadRightAnsi(highlighted, rightWidth))
              .Append(' ')
              .Append('│')
              .AppendLine();
        }
    }

    private static int VisibleLength(string text)
    {
        return AnsiRegex.Replace(text, "").Length;
    }

    private static string PadRightAnsi(string text, int width)
    {
        var visible = VisibleLength(text);

        if (visible >= width)
        {
            return text;
        }

        return text + new string(' ', width - visible);
    }
}
