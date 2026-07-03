using Spectre.Console;
using NitroHttp.Cli.Views.Interfaces;

namespace NitroHttp.Cli.Views.Components
{
    /// <summary>
    /// Displays formatted error messages in the terminal.
    /// </summary>
    public class ErrorView : IErrorView
    {
        /// <summary>
        /// Displays an error message.
        /// </summary>
        /// <param name="message">The error message to display.</param>
        public void Display(string message)
        {
            var panel = new Panel($"[red]{message}[/]")
                .Header("Error").Expand().BorderColor(Color.Red);

            AnsiConsole.Write(panel);
        }
    }
}
