using System.Text;
using Spectre.Console;
using NitroHttp.Cli.Views.Interfaces;

namespace NitroHttp.Cli.Views.Components;

public class Table : ITable
{
    private const int ChunkSize = 500;
    private const int LeftColumnWidth = 8;

    public void Display(string formattedJson, string endpoint = "JSON")
    {
        using var reader = new StringReader(formattedJson);
        var chunk = new List<string>(ChunkSize);
        const int lineNumber = 0;
        var windowWidth = Console.WindowWidth;

        TableHeader(windowWidth, endpoint);
        TableBody(chunk, lineNumber, reader);
        TableBottom();
    }

    public static void TableHeader(int windowWidth, string endpoint)
    {
        var headerSb = new StringBuilder();
        var leftHeader = "#".PadRight(LeftColumnWidth);
        var rightHeader = endpoint.ToUpper().PadRight(windowWidth - 14);

        Console.ForegroundColor = ConsoleColor.Red;
        headerSb.Append('┌')
          .Append('─', LeftColumnWidth + 2)
          .Append('┬')
          .Append('─', Math.Max(10, windowWidth - (LeftColumnWidth + 5)))
          .Append('┐')
          .AppendLine();

        headerSb.Append('│')
          .Append(' ')
          .Append(leftHeader)
          .Append(' ')
          .Append('│')
          .Append(' ')
          .Append(rightHeader)
          .Append('│')
          .AppendLine();

        headerSb.Append('├')
          .Append('─', LeftColumnWidth + 2)
          .Append('┼')
          .Append('─', Math.Max(10, windowWidth - (LeftColumnWidth + 5)))
          .Append('┤')
          .AppendLine();

        Console.Out.Write(headerSb.ToString());
        Console.ResetColor();
    }

    public static void TableBottom()
    {
        Console.Out.Write('└');
        Console.Out.Write(new string('─', LeftColumnWidth + 2));
        Console.Out.Write('┴');
        Console.Out.Write(new string('─', Math.Max(10, Console.WindowWidth - (LeftColumnWidth + 5))));
        Console.Out.WriteLine('┘');
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
                    Console.Out.Write(outSb.ToString());
                }
                break;
            }

            chunk.Add(line);
            lineNumber++;

            if (chunk.Count < ChunkSize)
                continue;

            AppendRows(outSb, chunk, lineNumber - ChunkSize + 1);
            Console.Out.Write(outSb.ToString());
            outSb.Clear();
            chunk.Clear();
        }
    }


    public static void AppendRows(StringBuilder sb, List<string> chunk, int startLine)
    {
        for (var i = 0; i < chunk.Count; i++)
        {
            var num = (startLine + i).ToString().PadLeft(LeftColumnWidth);
            var escaped = Markup.Escape(chunk[i]);
            sb.Append('│')
              .Append(' ')
              .Append(num)
              .Append(' ')
              .Append('│')
              .Append(' ')
              .Append(escaped)
              .AppendLine();
        }
    }
}
