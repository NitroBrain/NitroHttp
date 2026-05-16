using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using NitroHttp.Cli.Commands.Interfaces;
using NitroHttp.Cli.Composition;

namespace NitroHttp.Cli
{
  class Program
  {
    static async Task<int> Main(string[] args)
    {
      var provider = DependencyInjection.Build();

      RootCommand rootCommand = new("NitroHttp CLI");
      var commands = provider.GetServices<ICommand>();

      foreach (var cmd in commands)
      {
        rootCommand.Add(cmd.Build());
      }

      return await rootCommand.Parse(args).InvokeAsync();
    }
  }
}
