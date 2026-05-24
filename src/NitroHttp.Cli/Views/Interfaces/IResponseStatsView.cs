namespace NitroHttp.Cli.Views.Interfaces;

public interface IResponseStatsView
{
    void Display(long responseTime, int responseStatus, int responseCount, int responseSize);
}
