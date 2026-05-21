using NitroHttp.Cli.Commands.Interfaces;
using System.CommandLine;
using NitroHttp.Cli.Views.Interfaces;
using NitroHttp.Core.Services.Interfaces;

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

        var urlOption = new Option<string>("--url")
        {
            Required = true,
            Description = "PUT Request"
        };

        var bodyOption = new Option<string>("--body")
        {
            Required = true,
            Description = "PUT Request"
        };

        command.Add(urlOption);
        command.Add(bodyOption);

        command.SetAction(async result =>
        {
            var url = result.GetValue(urlOption)!;
            var body = result.GetValue(bodyOption)!;
            try
            {
                var request = await httpService.PutAsync(url, body);
                responseView.Display(request.Content, request.StatusCode, request.Count, request.Size);
            }
            catch (Exception ex)
            {
                errorView.Display(ex.Message);
            }
        });

        return command;
    }
}
