namespace NitroHttp.Core.Http;

/// <summary>
/// Represents an HTTP client.
/// </summary>
public interface IHttpClient
{
    /// <summary>
    /// Sends an HTTP request.
    /// </summary>
    /// <param name="request">Request message.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP response.</returns>
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
}
