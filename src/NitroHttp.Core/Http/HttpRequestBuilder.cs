using System.Text;
using NitroHttp.Core.Models;

namespace NitroHttp.Core.Http;

/// <summary>
/// Builds <see cref="HttpRequestMessage"/> instances.
/// </summary>
public static class HttpRequestBuilder
{
    /// <summary>
    /// Builds an HTTP request message.
    /// </summary>
    /// <param name="request">Request model.</param>
    /// <returns>Configured HTTP request.</returns>
    public static HttpRequestMessage Build(HttpRequestModel request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var message = new HttpRequestMessage
        {
            Method = request.Method,
            RequestUri = BuildUri(request)
        };

        AddContent(message, request);
        AddHeaders(message, request);
        AddCookies(message, request);

        return message;
    }

    /// <summary>
    /// Builds the request URI.
    /// </summary>
    private static Uri BuildUri(HttpRequestModel request)
    {
        var builder = new UriBuilder(request.Url);

        if (request.QueryParameters.Count == 0)
        {
            return builder.Uri;
        }

        builder.Query = string.Join("&",
            request.QueryParameters.Select(parameter =>
              $"{Uri.EscapeDataString(parameter.Key)}={Uri.EscapeDataString(parameter.Value)}"));

        return builder.Uri;
    }

    /// <summary>
    /// Adds request content.
    /// </summary>
    private static void AddContent(HttpRequestMessage message, HttpRequestModel request)
    {
        if (string.IsNullOrWhiteSpace(request.Body))
        {
            return;
        }

        message.Content = new StringContent(request.Body, Encoding.UTF8, request.ContentType);
    }

    /// <summary>
    /// Adds request headers.
    /// </summary>
    private static void AddHeaders(HttpRequestMessage message, HttpRequestModel request)
    {
        foreach (var header in request.Headers)
        {
            if (!message.Headers.TryAddWithoutValidation(header.Name, header.Value))
            {
                message.Content?.Headers.TryAddWithoutValidation(header.Name, header.Value);
            }
        }
    }

    /// <summary>
    /// Adds cookies.
    /// </summary>
    private static void AddCookies(HttpRequestMessage message, HttpRequestModel request)
    {
        if (request.Cookies.Count == 0)
        {
            return;
        }

        var cookieHeader = string.Join("; ", request.Cookies.Select(cookie => $"{cookie.Name}={cookie.Value}"));

        message.Headers.TryAddWithoutValidation("Cookie", cookieHeader);
    }
}
