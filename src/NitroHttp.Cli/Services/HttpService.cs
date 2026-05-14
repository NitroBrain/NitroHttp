using System.Text;
using System.Text.Json;
using NitroHttp.Cli.Services.interfaces;
using NitroHttp.Cli.View;
using NitroHttp.Cli.Views;
using Views;

namespace NitroHttp.Cli.Services
{
  public class HttpService : IHttpService
  {
    private static readonly HttpClient http = new() { Timeout = TimeSpan.FromSeconds(10) };

    public async Task GetAsync(string url)
    {
      var responseTable = new ResponseView();
      var errorView = new ErrorView();

      try
      {
        var response = await http.GetAsync(BuildUri(url));
        var content = await response.Content.ReadAsStringAsync();

        using JsonDocument doc = JsonDocument.Parse(content);

        int responseCount = doc.RootElement.ValueKind == JsonValueKind.Array
                            ? doc.RootElement.GetArrayLength()
                            : 0;
        int responseSize = Encoding.UTF8.GetByteCount(content);
        int responseStatus = (int)response.StatusCode;

        responseTable.Display(content, responseStatus, responseCount, responseSize);
      }
      catch (Exception)
      {
        errorView.Display("only json can be rendered");
      }
    }

    private static Uri BuildUri(string url)
    {
      return url.StartsWith("http") ? new Uri(url) : new Uri($"https://{url}");
    }
  }
}
