using NitroHttp.Core.Models;

namespace NitroHttp.Core.Http;

/// <summary>
/// Creates HTTP response models.
/// </summary>
public interface IHttpResponseFactory
{
    /// <summary>
    /// Creates a response model.
    /// </summary>
    /// <param name="response">HTTP response.</param>
    /// <param name="duration">Request duration.</param>
    Task<HttpResponseResult> CreateAsync(HttpResponseMessage response, TimeSpan duration);
}
