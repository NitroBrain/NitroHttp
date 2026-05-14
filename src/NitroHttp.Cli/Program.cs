using System.CommandLine;
using NitroHttp.Cli.Commands;
using NitroHttp.Cli.Services;

namespace NitroHttp.Cli
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            RootCommand rootCommand = new("NitroHttp CLI");
            var httpService = new HttpService();

            rootCommand.Add(GetCommand.Create(httpService));

            return await rootCommand.Parse(args).InvokeAsync();
        }
    }
}
