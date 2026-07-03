using NitroHttp.Cli.Commands.Interfaces;
using System.CommandLine;
using NitroHttp.Cli.Views.Interfaces;
using NitroHttp.Core.Services.Interfaces;
using NitroHttp.Cli.Commands.Options;

namespace NitroHttp.Cli.Commands.Http;

/// <summary>
/// Builds the command used to send HTTP PATCH requests.
/// </summary>
/// <param name="httpService">The HTTP service used to execute the request.</param>
/// <param name="responseView">The view used to render successful responses.</param>
/// <param name="errorView">The view used to render errors.</param>
public class PatchCommand(
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
        var command = new Command("patch", "Send an HTTP PATCH request to update a resource.");
        command.Aliases.Add("pa");

        command.Add(HttpOptions.Url);
        command.Add(HttpOptions.Body);

        command.SetAction(async result =>
        {
            var url = result.GetValue(HttpOptions.Url)!;
            var body = result.GetValue(HttpOptions.Body)!;

            try
            {
                if (File.Exists(body))
                {
                    body = await File.ReadAllTextAsync(body);
                }

                var request = await httpService.PatchAsync(url, body);
                responseView.Display(
                    $"PATCH {url}",
                    request.Content,
                    request.StatusCode,
                    request.Count,
                    request.Size
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
