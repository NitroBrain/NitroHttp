using System.CommandLine;

namespace NitroHttp.Cli.Commands.Interfaces;

/// <summary>
/// Represents a command that can build a System.CommandLine command.
/// </summary>
public interface ICommand
{
    /// <summary>
    /// Builds the command instance.
    /// </summary>
    /// <returns>The configured command.</returns>
    Command Build();
}
