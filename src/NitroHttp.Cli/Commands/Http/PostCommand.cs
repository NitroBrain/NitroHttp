using NitroHttp.Core.Services.Interfaces;
using NitroHttp.Cli.Commands.Interfaces;
using System.CommandLine;
using NitroHttp.Cli.Views.Interfaces;
using NitroHttp.Cli.Commands.Options;

namespace NitroHttp.Cli.Commands.Http;

public class PostCommand(
    IHttpService httpService,
    IResponseView responseView,
    IErrorView errorView
  ) : ICommand
{
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
                var request = await httpService.PostAsync(url, body);
                responseView.Display(
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
