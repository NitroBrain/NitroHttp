namespace NitroHttp.Core.Models;

public sealed class HttpResponseResult
{
    public required int StatusCode { get; init; }
    public required string Content { get; init; }
    public IReadOnlyList<HttpHeader> Headers { get; init; } = [];
    public IReadOnlyList<CookieModel> Cookies { get; init; } = [];
    public long Size { get; init; }
    public int Count { get; init; }
    public TimeSpan Duration { get; init; }
}
