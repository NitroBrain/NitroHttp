namespace NitroHttp.Cli.Views.Interfaces;

/// <summary>
/// Displays error messages.
/// </summary>
public interface IErrorView
{
    /// <summary>
    /// Displays an error message.
    /// </summary>
    /// <param name="message">The error message to display.</param>
    void Display(string message);
}
