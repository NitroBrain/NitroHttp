namespace NitroHttp.Cli.Services.interfaces
{
    public interface IHttpService
    {
        Task GetAsync(string url);
    };
}
