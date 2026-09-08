namespace NitroHttp.Cli.Views.Interfaces;

/// <summary>
/// Displays HTTP response statistics.
/// </summary>
public interface IResponseStatsView
{
    /// <summary>
    /// Displays response statistics.
    /// </summary>
    /// <param name="time">The elapsed request time in milliseconds.</param>
    /// <param name="status">The HTTP response status code.</param>
    /// <param name="count">The number of returned items.</param>
    /// <param name="size">The response size in bytes.</param>
    void Display(long time, int status, int count, long size);
}
