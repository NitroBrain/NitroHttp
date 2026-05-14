using NitroHttp.Core.Helpers;
using Spectre.Console;

namespace NitroHttp.Cli.View
{
    public class RequestStatsView
    {
        public void Display(long responseTime, int responseStatus, int responseCount, int responseSize)
        {
            Table statsTable = new();

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
                $"[yellow]{responseCount}x[/]"
            );

            var table = Align.Right(statsTable);

            AnsiConsole.Write(table);
        }
    }
}
