using System.Text;
using System.Text.RegularExpressions;
using NitroHttp.Cli.Views.Interfaces;
using NitroHttp.Core.Rendering;
using NitroHttp.Core.Enums;

namespace NitroHttp.Cli.Views.Components;

/// <summary>
/// Renders JSON output as a numbered table.
/// </summary>
public class Table : ITable
{
    private const int ChunkSize = 500;
    private const int LeftColumnWidth = 8;

    private static readonly Regex AnsiRegex = new(@"\x1B\[[0-9;]*m", RegexOptions.Compiled);

    /// <summary>
    /// Displays formatted JSON with the supplied endpoint label.
    /// </summary>
    /// <param name="formattedJson">The JSON payload to render.</param>
    /// <param name="endpoint">The request endpoint label.</param>
    public void Display(string formattedJson, string endpoint)
    {
        using var reader = new StringReader(formattedJson);
        var chunk = new List<string>(ChunkSize);
        var lineNumber = 0;

        TableHeader(Console.WindowWidth, endpoint);
        TableBody(chunk, lineNumber, reader);
        TableBottom();
    }

    /// <summary>
    /// Writes the table header.
    /// </summary>
    /// <param name="windowWidth">The current console width.</param>
    /// <param name="endpoint">The request endpoint label.</param>
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

    /// <summary>
    /// Writes the table bottom border.
    /// </summary>
    public static void TableBottom()
    {
        var rightWidth = Math.Max(10, Console.WindowWidth - (LeftColumnWidth + 5));

        Console.Write('└');
        Console.Write(new string('─', LeftColumnWidth + 2));
        Console.Write('┴');
        Console.Write(new string('─', rightWidth));
        Console.WriteLine('┘');
    }

    /// <summary>
    /// Writes the table body for the provided chunked lines.
    /// </summary>
    /// <param name="chunk">The buffered JSON lines.</param>
    /// <param name="lineNumber">The current line number.</param>
    /// <param name="reader">The text reader for the JSON payload.</param>
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

    /// <summary>
    /// Appends rendered rows to the output buffer.
    /// </summary>
    /// <param name="sb">The output buffer.</param>
    /// <param name="chunk">The buffered JSON lines.</param>
    /// <param name="startLine">The starting line number for the chunk.</param>
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
