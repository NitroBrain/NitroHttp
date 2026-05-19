namespace NitroHttp.Core.Helpers;

public static class BuildUri
{
    public static Uri Build(string url)
    {
        return url.StartsWith("http") ? new Uri(url) : new Uri($"https://{url}");
    }
}
