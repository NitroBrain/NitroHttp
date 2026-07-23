using System.Diagnostics;
using NitroHttp.Core.Http;
using NitroHttp.Core.Models;
using NitroHttp.Core.Services.Interfaces;

namespace NitroHttp.Core.Services;

/// <summary>
/// Default HTTP service.
/// </summary>
public sealed class HttpService(IHttpClient httpClient, IHttpResponseFactory responseFactory) : IHttpService
{
    /// <inheritdoc/>
    public async Task<HttpResponseResult> ExecuteAsync(
        HttpRequestModel request)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var message = HttpRequestBuilder.Build(request);

        var stopwatch = Stopwatch.StartNew();

        using var response = await httpClient.SendAsync(message, request.Options.CancellationToken);

        stopwatch.Stop();

        return await responseFactory.CreateAsync(response, stopwatch.Elapsed);
    }
}
