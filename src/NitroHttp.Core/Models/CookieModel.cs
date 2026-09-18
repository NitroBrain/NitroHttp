namespace NitroHttp.Core.Models
{
    public class CookieModel
    {
        public required string Name { get; init; }
        public required string Value { get; init; }
        public string? Domain { get; init; }
        public string Path { get; init; } = "/";
        public string? Age { get; init; }
        public int Size { get; init; }
        public bool HttpOnly { get; init; }
        public string? SameSite { get; init; }
    }
}
