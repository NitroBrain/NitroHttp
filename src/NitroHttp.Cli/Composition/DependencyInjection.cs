using Microsoft.Extensions.DependencyInjection;
using NitroHttp.Cli.Commands.Http;
using NitroHttp.Cli.Commands.Interfaces;
using NitroHttp.Cli.Views.Components;
using NitroHttp.Cli.Views.Interfaces;
using NitroHttp.Core.Services;
using NitroHttp.Core.Services.Interfaces;

namespace NitroHttp.Cli.Composition;

/// <summary>
/// Registers the CLI services used by the application.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Builds the application service provider.
    /// </summary>
    /// <returns>The configured service provider.</returns>
    public static IServiceProvider Build()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IHttpService, HttpService>();
        services.AddSingleton<ICommand, GetCommand>();
        services.AddSingleton<ICommand, PostCommand>();
        services.AddSingleton<ICommand, PutCommand>();
        services.AddSingleton<ICommand, PatchCommand>();
        services.AddSingleton<ICommand, DeleteCommand>();

        services.AddSingleton<IResponseView, ResponseView>();
        services.AddSingleton<IResponseStatsView, ResponseStatsView>();
        services.AddSingleton<IErrorView, ErrorView>();
        services.AddSingleton<ITable, Table>();

        return services.BuildServiceProvider();
    }
}
