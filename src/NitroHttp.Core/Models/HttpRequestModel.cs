namespace NitroHttp.Core.Models;

/// <summary>
/// Represents an HTTP request.
/// </summary>
public sealed class HttpRequestModel
{
    public required HttpMethod Method { get; init; }
    public required string Url { get; init; }
    public string? Body { get; init; }
    public string ContentType { get; init; } = "application/json";
    public IReadOnlyList<HttpHeader> Headers { get; init; } = [];
    public IReadOnlyList<QueryParameter> QueryParameters { get; init; } = [];
    public IReadOnlyList<CookieModel> Cookies { get; init; } = [];
    public RequestOptions Options { get; init; } = new();
}
