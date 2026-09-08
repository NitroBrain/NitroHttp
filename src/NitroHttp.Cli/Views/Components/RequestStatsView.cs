using NitroHttp.Core.Helpers;
using NitroHttp.Cli.Views.Interfaces;
using Spectre.Console;
using SpectreTable = Spectre.Console.Table;

namespace NitroHttp.Cli.Views.Components;

/// <summary>
/// Displays request statistics in a formatted table.
/// </summary>
public class ResponseStatsView : IResponseStatsView
{
    /// <summary>
    /// Displays the request timing and response details.
    /// </summary>
    /// <param name="time">The elapsed request time in milliseconds.</param>
    /// <param name="status">The HTTP response status code.</param>
    /// <param name="count">The number of returned items.</param>
    /// <param name="size">The response size in bytes.</param>
    public void Display(long time, int status, int count, long size)
    {
        SpectreTable statsTable = new()
        {
            Border = TableBorder.Rounded
        };

        statsTable.AddColumn("[green]Status[/]");
        statsTable.AddColumn("[green]Time[/]");
        statsTable.AddColumn("[green]Size[/]");
        statsTable.AddColumn("[green]Items[/]");

        string formattedSize = FormatBytes.Format(size);
        string statusCode = HttpStatusHelper.GetStatusText(status);

        statsTable.AddRow(
            $"[yellow]{statusCode}[/]",
            $"[yellow]{time}ms[/]",
            $"[yellow]{formattedSize}[/]",
            $"[yellow]{count:#,##,##,##0}[/]"
        );

        var table = Align.Right(statsTable);

        AnsiConsole.Write(new Align(new Markup("[bold green]Statistics[/]"), HorizontalAlignment.Right));
        AnsiConsole.Write(table);
    }
}
