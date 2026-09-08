using NitroHttp.Core.Models;

namespace NitroHttp.Cli.Views.Interfaces;

/// <summary>
/// Displays HTTP responses.
/// </summary>
public interface IResponseView
{
    /// <summary>
    /// Displays the response payload and metadata.
    /// </summary>
    /// <param name="url">The request URL or label.</param>
    /// <param name="response">The response body.</param>
    /// <param name="status">The HTTP response status code.</param>
    /// <param name="count">The number of returned items.</param>
    /// <param name="size">The response size in bytes.</param>
    /// <param name="headers">The HTTP headers.</param>
    void Display(string url, string response, int status, int count, long size, IReadOnlyList<HttpHeader> headers);
}
