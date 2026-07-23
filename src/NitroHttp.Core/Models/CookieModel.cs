namespace NitroHttp.Core.Models
{
    public class CookieModel
    {
        public required string Name { get; init; }
        public required string Value { get; init; }
        public string? Domain { get; init; }
        public string Path { get; init; } = "/";
    }
}
