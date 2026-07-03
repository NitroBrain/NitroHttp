using System.Diagnostics;
using NitroHttp.Core.Helpers;
using NitroHttp.Cli.Views.Interfaces;

namespace NitroHttp.Cli.Views.Components;

/// <summary>
/// Renders HTTP responses and falls back to error output when formatting fails.
/// </summary>
/// <param name="stats">The view used to display request statistics.</param>
/// <param name="errorView">The view used to display errors.</param>
/// <param name="table">The table renderer used for formatted JSON output.</param>
public class ResponseView(
    IResponseStatsView stats,
    IErrorView errorView,
    ITable table
    ) : IResponseView
{
    /// <summary>
    /// Displays the response payload and statistics.
    /// </summary>
    /// <param name="requestUrl">The request URL or label to show in the output.</param>
    /// <param name="response">The response body.</param>
    /// <param name="responseStatus">The HTTP response status code.</param>
    /// <param name="responseCount">The number of returned items.</param>
    /// <param name="responseSize">The response size in bytes.</param>
    public void Display(string requestUrl, string response, int responseStatus, int responseCount, int responseSize)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            string formattedJson = FormatJson.TryFormatJson(response);

            table.Display(formattedJson, requestUrl);

            stats.Display(sw.ElapsedMilliseconds, responseStatus, responseCount, responseSize);
        }
        catch (Exception ex)
        {
            errorView.Display(ex.Message);
        }
    }

}
