using Microsoft.Extensions.DependencyInjection;
using NitroHttp.Cli.Commands.Http;
using NitroHttp.Cli.Commands.Interfaces;
using NitroHttp.Cli.Views;
using NitroHttp.Cli.Views.Interfaces;
using NitroHttp.Core.Services;
using NitroHttp.Core.Services.Interfaces;

namespace NitroHttp.Cli.Composition;

public static class DependencyInjection
{
  public static IServiceProvider Build()
  {
    var services = new ServiceCollection();

    services.AddSingleton<IHttpService, HttpService>();
    services.AddSingleton<ICommand, GetCommand>();

    services.AddSingleton<IResponseView, ResponseView>();

    return services.BuildServiceProvider();
  }
}
