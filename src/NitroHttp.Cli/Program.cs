using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using NitroHttp.Cli.Commands.Interfaces;
using NitroHttp.Cli.Composition;

namespace NitroHttp.Cli
{
    static class Program
    {
        static async Task<int> Main(string[] args)
        {
            RootCommand rootCommand = new("NitroHttp CLI");

            var provider = DependencyInjection.Build();

            foreach (var cmd in provider.GetServices<ICommand>())
            {
                rootCommand.Add(cmd.Build());
            }

            return await rootCommand.Parse(args).InvokeAsync();
        }
    }
}
