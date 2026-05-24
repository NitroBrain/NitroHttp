using System.Diagnostics;
using NitroHttp.Core.Helpers;
using NitroHttp.Cli.Views.Interfaces;

namespace NitroHttp.Cli.Views.Components;

public class ResponseView(
    IResponseStatsView stats,
    IErrorView errorView,
    ITable table
    ) : IResponseView
{

    public void Display(string response, int responseStatus, int responseCount, int responseSize)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            string formattedJson = FormatJson.TryFormatJson(response);

            table.Display(formattedJson, "todos");

            stats.Display(sw.ElapsedMilliseconds, responseStatus, responseCount, responseSize);
        }
        catch (Exception ex)
        {
            errorView.Display(ex.Message);
        }
    }

}
