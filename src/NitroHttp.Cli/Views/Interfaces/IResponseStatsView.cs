namespace NitroHttp.Cli.Views.Interfaces;

/// <summary>
/// Displays HTTP response statistics.
/// </summary>
public interface IResponseStatsView
{
    /// <summary>
    /// Displays response statistics.
    /// </summary>
    /// <param name="responseTime">The elapsed request time in milliseconds.</param>
    /// <param name="responseStatus">The HTTP response status code.</param>
    /// <param name="responseCount">The number of returned items.</param>
    /// <param name="responseSize">The response size in bytes.</param>
    void Display(long responseTime, int responseStatus, int responseCount, int responseSize);
}
