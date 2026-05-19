using NitroHttp.Core.Models;

namespace NitroHttp.Core.Services.Interfaces;

public interface IHttpService
{
    Task<HttpResponseResult> GetAsync(string url);
    Task<HttpResponseResult> PostAsync(string url, string content);
    Task<HttpResponseResult> PutAsync(string url, string content);
    Task<HttpResponseResult> PatchAsync(string url, string content);
    Task<HttpResponseResult> DeleteAsync(string url);
}
