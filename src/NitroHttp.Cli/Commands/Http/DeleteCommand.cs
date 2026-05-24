using System.CommandLine;
using NitroHttp.Cli.Commands.Interfaces;
using NitroHttp.Cli.Commands.Options;
using NitroHttp.Cli.Views.Interfaces;
using NitroHttp.Core.Services.Interfaces;

namespace NitroHttp.Cli.Commands.Http;

public class DeleteCommand(
    IHttpService httpService,
    IResponseView responseView,
    IErrorView errorView
    ) : ICommand
{
    public Command Build()
    {
        var command = new Command("delete", "Send an HTTP DELETE request to remove a resource.");
        command.Aliases.Add("del");

        command.Add(HttpOptions.Url);

        command.SetAction(async result =>
        {
            var url = result.GetValue(HttpOptions.Url)!;
            try
            {
                var request = await httpService.DeleteAsync(url);
                responseView.Display(
                    "No Content",
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
