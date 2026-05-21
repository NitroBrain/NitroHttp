using NitroHttp.Core.Services.Interfaces;
using NitroHttp.Cli.Commands.Interfaces;
using System.CommandLine;
using NitroHttp.Cli.Views.Interfaces;

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

        var urlOption = new Option<string>("--url")
        {
            Required = true,
            Description = "Request URL"
        };

        var bodyOption = new Option<string>("--body")
        {
            Required = true,
            Description = "Json Body"
        };

        command.Add(urlOption);
        command.Add(bodyOption);

        command.SetAction(async result =>
        {
            var url = result.GetValue(urlOption)!;
            var body = result.GetValue(bodyOption)!;
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
