using Spectre.Console;
using NitroHttp.Cli.Views.Interfaces;

namespace NitroHttp.Cli.Views.Components
{
    public class ErrorView : IErrorView
    {
        public void Display(string message)
        {
            var panel = new Panel($"[red]{message}[/]")
                .Header("Error").Expand().BorderColor(Color.Red);

            AnsiConsole.Write(panel);
        }
    }
}
