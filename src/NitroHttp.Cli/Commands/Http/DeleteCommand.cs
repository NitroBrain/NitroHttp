using System.CommandLine;
using NitroHttp.Cli.Commands.Interfaces;
using NitroHttp.Cli.Commands.Options;
using NitroHttp.Cli.Views.Interfaces;
using NitroHttp.Core.Models;
using NitroHttp.Core.Services.Interfaces;
using NitroHttp.Core.Helpers;

namespace NitroHttp.Cli.Commands.Http;

/// <summary>
/// Builds the command used to send HTTP DELETE requests.
/// </summary>
/// <param name="httpService">The HTTP service used to execute the request.</param>
/// <param name="responseView">The view used to render successful responses.</param>
/// <param name="errorView">The view used to render errors.</param>
public sealed class DeleteCommand(IHttpService httpService, IResponseView responseView, IErrorView errorView) : ICommand
{
    /// <summary>
    /// Creates the configured command.
    /// </summary>
    /// <returns>The configured command instance.</returns>
    public Command Build()
    {
        var command = new Command("delete", "Send an HTTP DELETE request to remove a resource.");

        command.Aliases.Add("del");

        command.Add(HttpOptions.Url);
        command.Add(HttpOptions.Headers);

        command.SetAction(async result =>
        {
            try
            {
                var url = result.GetValue(HttpOptions.Url)!;
                var headers = result.GetValue(HttpOptions.Headers);

                if (!string.IsNullOrWhiteSpace(headers) && File.Exists(headers))
                {
                    headers = await File.ReadAllTextAsync(headers);
                }

                var request = new HttpRequestModel
                {
                    Method = HttpMethod.Delete,
                    Url = url,
                    Headers = HeaderParser.Parse(headers)
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
