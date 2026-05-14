using System.Diagnostics;
using NitroHttp.Core.Helpers;
using NitroHttp.Cli.View;
using Spectre.Console;
using Views.Interfaces;

namespace NitroHttp.Cli.Views
{
    public class ResponseView : IResponseView
    {
        public void Display(string response, int responseStatus, int responseCount, int responseSize)
        {
            var stats = new RequestStatsView();
            var errorView = new ErrorView();
            var sw = Stopwatch.StartNew();

            try
            {
                string formattedJson = FormatJson.TryFormatJson(response);
                var lines = formattedJson.Split('\n');

                var table = new Table();

                table.Expand();
                table.AddColumn(new TableColumn("[yellow]#[/]"));
                table.AddColumn(new TableColumn("[green]JSON[/]") { NoWrap = false });

                for (int i = 0; i < lines.Length; i++)
                {
                    table.AddRow($"[yellow]{i + 1}[/]", Markup.Escape(lines[i]));
                }

                AnsiConsole.Write(table);

                stats.Display(sw.ElapsedMilliseconds, responseStatus, responseCount, responseSize);
            }
            catch (Exception ex)
            {
                errorView.Display(ex.Message);
            }
        }
    }
}
