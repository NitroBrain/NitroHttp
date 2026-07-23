using NitroHttp.Core.Models;

namespace NitroHttp.Core.Services.Interfaces;

/// <summary>
/// Provides HTTP request execution.
/// </summary>
public interface IHttpService
{
    /// <summary>
    /// Executes an HTTP request.
    /// </summary>
    /// <param name="request">HTTP request.</param>
    /// <returns>HTTP response.</returns>
    Task<HttpResponseResult> ExecuteAsync(HttpRequestModel request);
}
