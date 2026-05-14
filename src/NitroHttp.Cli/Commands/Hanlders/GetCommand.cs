using System.CommandLine;
using NitroHttp.Cli.Services;

namespace NitroHttp.Cli.Commands
{
    public static class GetCommand
    {
        public static Command Create(HttpService http)
        {
            var command = new Command("get", "Send HTTP GET request");

            var urlOption = new Option<string>("--url")
            {
                Required = true,
                Description = "Request URL"
            };

            command.Add(urlOption);

            command.SetAction(async result =>
            {
                var url = result.GetValue(urlOption)!;
                await http.GetAsync(url);
            });

            return command;
        }
    }
}
