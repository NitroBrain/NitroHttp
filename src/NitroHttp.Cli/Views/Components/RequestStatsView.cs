using NitroHttp.Core.Helpers;
using NitroHttp.Cli.Views.Interfaces;
using Spectre.Console;
using SpectreTable = Spectre.Console.Table;

namespace NitroHttp.Cli.Views.Components;

public class ResponseStatsView : IResponseStatsView
{
    public void Display(long responseTime, int responseStatus, int responseCount, int responseSize)
    {
        SpectreTable statsTable = new();

        statsTable.AddColumn("[green]Status[/]");
        statsTable.AddColumn("[green]Time[/]");
        statsTable.AddColumn("[green]Size[/]");
        statsTable.AddColumn("[green]Items[/]");

        string formattedSize = FormatBytes.Format(responseSize);
        string status = HttpStatusHelper.GetStatusText(responseStatus);

        statsTable.AddRow(
            $"[yellow]{status}[/]",
            $"[yellow]{responseTime}ms[/]",
            $"[yellow]{formattedSize}[/]",
            $"[yellow]{responseCount:#,##,##,##0}[/]"
        );

        var table = Align.Right(statsTable);

        AnsiConsole.Write(table);
    }
}
