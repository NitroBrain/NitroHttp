using System.CommandLine;

namespace NitroHttp.Cli.Commands.Interfaces;

public interface ICommand
{
  Command Build();
}
