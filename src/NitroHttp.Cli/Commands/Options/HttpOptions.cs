using System.CommandLine;

namespace NitroHttp.Cli.Commands.Options
{
    public static class HttpOptions
    {
        public static Option<string> Url { get; } = new("--url")
        {
            Required = true,
            Description = "The API endpoint URL to send the HTTP request to."
        };

        public static Option<string> Body { get; } = new("--body")
        {
            Required = true,
            Description = "Request body content to include in the HTTP request."
        };
    }
}
