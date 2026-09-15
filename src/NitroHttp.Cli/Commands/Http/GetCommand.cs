using System.CommandLine;
using NitroHttp.Cli.Commands.Interfaces;
using NitroHttp.Cli.Commands.Options;
using NitroHttp.Cli.Views.Interfaces;
using NitroHttp.Core.Models;
using NitroHttp.Core.Services.Interfaces;
using NitroHttp.Core.Helpers;

namespace NitroHttp.Cli.Commands.Http;

/// <summary>
/// Builds the command used to send HTTP GET requests.
/// </summary>
public sealed class GetCommand(
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
        var command = new Command("get", "Send an HTTP GET request.");
        command.Aliases.Add("g");

        command.Add(HttpOptions.Url);
        command.Add(HttpOptions.Headers);

        command.SetAction(async result =>
        {
            try
            {
                var request = new HttpRequestModel
                {
                    Method = HttpMethod.Get,
                    Url = result.GetValue(HttpOptions.Url)!,
                    Headers = HeaderParser.Parse(result.GetValue(HttpOptions.Headers))
                };

                var response = await httpService.ExecuteAsync(request);

                responseView.Display(
                    $"{request.Method} {request.Url}",
                    response.Content,
                    response.StatusCode,
                    response.Count,
                    response.Size,
                    response.Headers,
                    response.Cookies
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
