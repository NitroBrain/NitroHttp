using System.Diagnostics;
using NitroHttp.Core.Helpers;
using NitroHttp.Cli.Views.Interfaces;
using NitroHttp.Core.Models;

namespace NitroHttp.Cli.Views.Components;

/// <summary>
/// Renders HTTP responses and falls back to error output when formatting fails.
/// </summary>
/// <param name="stats">The view used to display request statistics.</param>
/// <param name="errorView">The view used to display errors.</param>
/// <param name="table">The table renderer used for formatted JSON output.</param>
/// <param name="cookiesView">The view used to display cookies.</param>
/// <param name="headersView">The view used to display HTTP headers.</param>
public class ResponseView(
    IResponseStatsView stats,
    IErrorView errorView,
    ITable table,
    ICookies cookiesView,
    IHeaders headersView
    ) : IResponseView
{
    /// <summary>
    /// Displays the response payload and statistics.
    /// </summary>
    /// <param name="url">The request URL or label to show in the output.</param>
    /// <param name="response">The response body.</param>
    /// <param name="status">The HTTP response status code.</param>
    /// <param name="count">The number of returned items.</param>
    /// <param name="size">The response size in bytes.</param>
    /// <param name="headers">The HTTP headers.</param>
    /// <param name="cookies">The HTTP cookies.</param>
    public void Display(string url, string response, int status, int count, long size, IReadOnlyList<HttpHeader> headers, IReadOnlyList<CookieModel> cookies)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            string formattedJson = FormatJson.TryFormatJson(response);

            table.Display(formattedJson, url);

            stats.Display(sw.ElapsedMilliseconds, status, count, size);
            cookiesView.Display(cookies);
            headersView.Display(headers);
        }
        catch (Exception ex)
        {
            errorView.Display(ex.Message);
        }
    }

}
