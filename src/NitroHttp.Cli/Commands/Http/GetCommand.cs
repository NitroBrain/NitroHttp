using System.CommandLine;
using NitroHttp.Cli.Commands.Interfaces;
using NitroHttp.Core.Services.Interfaces;
using NitroHttp.Cli.Views.Interfaces;
using NitroHttp.Cli.Commands.Options;

namespace NitroHttp.Cli.Commands.Http;

/// <summary>
/// Builds the command used to send HTTP GET requests.
/// </summary>
/// <param name="httpService">The HTTP service used to execute the request.</param>
/// <param name="responseView">The view used to render successful responses.</param>
/// <param name="errorView">The view used to render errors.</param>
public class GetCommand(
    IHttpService httpService,
    IResponseView responseView,
    IErrorView errorView) : ICommand
{
    /// <summary>
    /// Creates the configured command.
    /// </summary>
    /// <returns>The configured command instance.</returns>
    public Command Build()
    {
        var command = new Command("get", "Send an HTTP GET request to retrieve data.");
        command.Aliases.Add("g");

        command.Add(HttpOptions.Url);

        command.SetAction(async result =>
        {
            var url = result.GetValue(HttpOptions.Url)!;
            try
            {
                var response = await httpService.GetAsync(url);

                responseView.Display(
                    $"GET {url}",
                    response.Content,
                    response.StatusCode,
                    response.Count,
                    response.Size
                );
            }
            catch (Exception ex)
            {
                errorView.Display(ex.Message);
            }
        });

        return command;
    }
}
