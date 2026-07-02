namespace NitroHttp.Cli.Views.Interfaces;

public interface IResponseView
{
    void Display(string requestUrl, string response, int responseStatus, int responseCount, int responseSize);
}
