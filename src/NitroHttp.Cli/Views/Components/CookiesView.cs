using NitroHttp.Cli.Views.Interfaces;
using NitroHttp.Core.Models;
using Spectre.Console;
using SpectreTable = Spectre.Console.Table;

namespace NitroHttp.Cli.Views.Components
{
    /// <summary>
    /// Displays HTTP cookies in a formatted table.
    /// </summary>
    public class CookiesView : ICookies
    {
        /// <summary>
        /// Displays formatted cookies.
        /// </summary>
        /// <param name="cookies">The HTTP cookies.</param>
        public void Display(IReadOnlyList<CookieModel> cookies)
        {
            if (cookies is null || cookies.Count == 0)
            {
                return;
            }

            SpectreTable table = new()
            {
                Border = TableBorder.Rounded,
                ShowRowSeparators = true
            };

            table.AddColumn("[green]Name[/]");
            table.AddColumn("[green]Value[/]");
            table.AddColumn("[green]Domain[/]");
            table.AddColumn("[green]Path[/]");
            table.AddColumn("[green]Age[/]");
            table.AddColumn("[green]Size[/]");
            table.AddColumn("[green]HttpOnly[/]");
            table.AddColumn("[green]SameSite[/]");

            foreach (var cookie in cookies)
            {
                table.AddRow(
                    $"[aqua]{Markup.Escape(cookie.Name ?? string.Empty)}[/]",
                    $"[yellow]{Markup.Escape(cookie.Value ?? string.Empty)}[/]",
                    $"[aqua]{Markup.Escape(cookie.Domain ?? string.Empty)}[/]",
                    $"[yellow]{Markup.Escape(cookie.Path ?? string.Empty)}[/]",
                    $"[white]{Markup.Escape(cookie.Age ?? string.Empty)}[/]",
                    $"[yellow]{cookie.Size}[/]",
                    cookie.HttpOnly ? "[green]true[/]" : "[grey]false[/]",
                    $"[aqua]{Markup.Escape(cookie.SameSite ?? string.Empty)}[/]"
                );
            }

            AnsiConsole.Write(new Align(new Markup("[bold green]Cookies[/]"), HorizontalAlignment.Left));
            AnsiConsole.Write(Align.Left(table));
        }
    }
}

