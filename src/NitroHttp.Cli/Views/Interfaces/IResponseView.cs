namespace NitroHttp.Cli.Views.Interfaces;

/// <summary>
/// Displays HTTP responses.
/// </summary>
public interface IResponseView
{
    /// <summary>
    /// Displays the response payload and metadata.
    /// </summary>
    /// <param name="requestUrl">The request URL or label.</param>
    /// <param name="response">The response body.</param>
    /// <param name="responseStatus">The HTTP response status code.</param>
    /// <param name="responseCount">The number of returned items.</param>
    /// <param name="responseSize">The response size in bytes.</param>
    void Display(string requestUrl, string response, int responseStatus, int responseCount, int responseSize);
}
