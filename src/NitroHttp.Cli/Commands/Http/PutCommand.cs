using System.CommandLine;
using NitroHttp.Cli.Commands.Interfaces;
using NitroHttp.Cli.Commands.Options;
using NitroHttp.Cli.Views.Interfaces;
using NitroHttp.Core.Helpers;
using NitroHttp.Core.Models;
using NitroHttp.Core.Services.Interfaces;

namespace NitroHttp.Cli.Commands.Http;

/// <summary>
/// Builds the command used to send HTTP PUT requests.
/// </summary>
/// <param name="httpService">The HTTP service used to execute the request.</param>
/// <param name="responseView">The view used to render successful responses.</param>
/// <param name="errorView">The view used to render errors.</param>
public sealed class PutCommand(
    IHttpService httpService,
    IResponseView responseView,
    IErrorView errorView
) : ICommand
{
    /// <summary>
    /// Creates the configured command.
    /// </summary>
    /// <returns>The configured command instance.</returns>
    public Command Build()
    {
        var command = new Command("put", "Send an HTTP PUT request to replace a resource.");

        command.Aliases.Add("pu");

        command.Add(HttpOptions.Url);
        command.Add(HttpOptions.Body);
        command.Add(HttpOptions.Headers);

        command.SetAction(async result =>
        {
            try
            {
                var url = result.GetValue(HttpOptions.Url)!;

                var body = await InputReader.ReadAsync(result.GetValue(HttpOptions.Body));
                var headers = await InputReader.ReadAsync(result.GetValue(HttpOptions.Headers));

                var request = new HttpRequestModel
                {
                    Method = HttpMethod.Put,
                    Url = url,
                    Body = body,
                    Headers = HeaderParser.Parse(headers)
                };

                var response = await httpService.ExecuteAsync(request);

                responseView.Display(
                    $"{request.Method} {request.Url}",
                    response.Content,
                    response.StatusCode,
                    response.Count,
                    response.Size,
                    response.Headers
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
