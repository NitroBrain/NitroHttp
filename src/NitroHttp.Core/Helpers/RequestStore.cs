namespace NitroHttp.Core.Helpers;

public sealed class RequestEntry
{
    public string CollectionName { get; set; } = string.Empty;
    public string Method { get; set; } = "GET";
    public string Url { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

    public string DisplayName => $"{Method} {Url}";
    public string TimestampText => Timestamp.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
}

public sealed class RequestStore
{
    public List<RequestEntry> History { get; set; } = [];
    public List<RequestEntry> Collections { get; set; } = [];
}
