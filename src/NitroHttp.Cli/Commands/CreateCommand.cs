using System.CommandLine;

namespace NitroHttp.Cli.Commands
{
  public class CreateCommand
  {
    public Command Create(
        string name,
        string description,
        Func<string, string?, Task> handler,
        bool hasBody = false
    )
    {
      Command command = new(name, description);

      Option<string> urlOption = new("--url")
      {
        Description = "API URL",
        Required = true
      };

      command.Options.Add(urlOption);

      Option<string>? bodyOption;

      if (hasBody)
      {
        bodyOption = new Option<string>("--body")
        {
          Description = "Request body"
        };
        command.Options.Add(bodyOption);

        command.SetAction(async result =>
        {
          var url = result.GetValue(urlOption)!;
          var body = result.GetValue(bodyOption)!;

          await handler(url, body);
        });
      }
      else
      {
        command.SetAction(async result =>
        {
          var url = result.GetValue(urlOption)!;
          await handler(url, null);
        });
      }

      return command;
    }
  }
}
