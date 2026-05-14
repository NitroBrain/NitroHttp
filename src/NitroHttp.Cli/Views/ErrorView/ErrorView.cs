using Spectre.Console;

namespace NitroHttp.Cli.View
{
    public class ErrorView
    {
        public void Display(string message)
        {
            var panel = new Panel($"[red]{message}[/]")
                .Header("Error").Expand().BorderColor(Color.Red);

            AnsiConsole.Write(panel);
        }
    }
}
