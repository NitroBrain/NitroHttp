using NitroHttp.Cli.Views.Interfaces;
using NitroHttp.Core.Models;
using Spectre.Console;
using SpectreTable = Spectre.Console.Table;

namespace NitroHttp.Cli.Views.Components
{
    /// <summary>
    /// Displays HTTP headers in a formatted table
    /// </summary>
    public class HeadersView : IHeaders
    {
        /// <summary>
        /// Hello
        /// </summary>
        public void Display(IReadOnlyList<HttpHeader> headers)
        {
            if (headers is null || headers.Count == 0)
            {
                return;
            }

            SpectreTable table = new()
            {
                Border = TableBorder.Rounded,
                ShowRowSeparators = true,
            };

            table.AddColumn("[green]Key[/]");
            table.AddColumn("[green]Value[/]");

            foreach (var header in headers)
            {
                table.AddRow(
                    $"[aqua]{Markup.Escape(header.Name)}[/]",
                    $"[yellow]{Markup.Escape(header.Value)}[/]"
                );
            }

            AnsiConsole.Write(new Align(new Markup("[bold green]HTTP Headers[/]"), HorizontalAlignment.Left));
            AnsiConsole.Write(Align.Left(table));
        }
    }
}
