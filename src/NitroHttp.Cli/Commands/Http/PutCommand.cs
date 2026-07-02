using NitroHttp.Cli.Commands.Interfaces;
using System.CommandLine;
using NitroHttp.Cli.Views.Interfaces;
using NitroHttp.Core.Services.Interfaces;
using NitroHttp.Cli.Commands.Options;

namespace NitroHttp.Cli.Commands.Http;

public class PutCommand(
    IHttpService httpService,
    IResponseView responseView,
    IErrorView errorView
    ) : ICommand
{
    public Command Build()
    {
        var command = new Command("put", "Send an HTTP PUT request to replace a resource.");
        command.Aliases.Add("pu");

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

                var request = await httpService.PutAsync(url, body);
                responseView.Display($"PUT {url}", request.Content, request.StatusCode, request.Count, request.Size);
            }
            catch (Exception ex)
            {
                errorView.Display(ex.Message);
            }
        });

        return command;
    }
}
