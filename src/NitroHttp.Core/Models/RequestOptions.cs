namespace NitroHttp.Core.Models
{
    public class RequestOptions
    {
        public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(30);
        public bool FollowRedirects { get; init; } = true;
        public CancellationToken CancellationToken { get; init; }
    }
}
