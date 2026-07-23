namespace NitroHttp.Core.Http;

/// <summary>
/// Adapter around <see cref="HttpClient"/>.
/// </summary>
public sealed class HttpClientAdapter(HttpClient httpClient) : IHttpClient
{
    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        return await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
    }
}
