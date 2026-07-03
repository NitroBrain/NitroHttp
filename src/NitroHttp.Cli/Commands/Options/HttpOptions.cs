using System.CommandLine;

namespace NitroHttp.Cli.Commands.Options;

/// <summary>
/// Provides shared HTTP command-line options.
/// </summary>
public static class HttpOptions
{
    /// <summary>
    /// Gets the target request URL option.
    /// </summary>
    public static Option<string> Url { get; } = new("--url")
    {
        Required = true,
        Description = "The API endpoint URL to send the HTTP request to."
    };

    /// <summary>
    /// Gets the request body option.
    /// </summary>
    public static Option<string> Body { get; } = new("--body")
    {
        Required = true,
        Description = "Request body content to include in the HTTP request."
    };
}
