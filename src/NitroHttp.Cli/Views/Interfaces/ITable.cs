
namespace NitroHttp.Cli.Views.Interfaces;

/// <summary>
/// Displays formatted JSON in a table layout.
/// </summary>
public interface ITable
{
    /// <summary>
    /// Displays formatted JSON.
    /// </summary>
    /// <param name="formattedJson">The JSON payload to render.</param>
    /// <param name="endpoint">The request endpoint label.</param>
    void Display(string formattedJson, string endpoint);
}
