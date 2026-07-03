using NitroHttp.Core.Services.Interfaces;
using NitroHttp.Cli.Commands.Interfaces;
using System.CommandLine;
using NitroHttp.Cli.Views.Interfaces;
using NitroHttp.Cli.Commands.Options;

namespace NitroHttp.Cli.Commands.Http;

/// <summary>
/// Builds the command used to send HTTP POST requests.
/// </summary>
/// <param name="httpService">The HTTP service used to execute the request.</param>
/// <param name="responseView">The view used to render successful responses.</param>
/// <param name="errorView">The view used to render errors.</param>
public class PostCommand(
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
        var command = new Command("post", "Send an HTTP POST request to create a resource.");
        command.Aliases.Add("p");

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

                var request = await httpService.PostAsync(url, body);
                responseView.Display(
                  $"POST {url}",
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
