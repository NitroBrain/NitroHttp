using System.Text;
using System.Text.Json;
using NitroHttp.Core.Helpers;
using NitroHttp.Core.Models;
using NitroHttp.Core.Services.Interfaces;

namespace NitroHttp.Core.Services
{
    public class HttpService : IHttpService
    {
        private static readonly HttpClient http = new() { Timeout = TimeSpan.FromSeconds(10) };

        public async Task<HttpResponseResult> GetAsync(string url)
        {
            var response = await http.GetAsync(BuildUri.Build(url));
            var content = await response.Content.ReadAsStringAsync();

            using JsonDocument doc = JsonDocument.Parse(content);

            int count = doc.RootElement.ValueKind == JsonValueKind.Array
                ? doc.RootElement.GetArrayLength()
                : 1;

            return new HttpResponseResult
            {
                Content = content,
                StatusCode = (int)response.StatusCode,
                Count = count,
                Size = Encoding.UTF8.GetByteCount(content)
            };
        }

        public async Task<HttpResponseResult> PostAsync(string url, string content)
        {
            var request = await http.PostAsync(BuildUri.Build(url), new StringContent(content, Encoding.UTF8, "application/json"));

            return new HttpResponseResult
            {
                Content = content,
                StatusCode = (int)request.StatusCode,
                Count = 0,
                Size = Encoding.UTF8.GetByteCount(content)
            };
        }

        public async Task<HttpResponseResult> PutAsync(string url, string content)
        {

            var request = await http.PutAsync(BuildUri.Build(url), new StringContent(content, Encoding.UTF8, "application/json"));

            return new HttpResponseResult
            {
                Content = content,
                StatusCode = (int)request.StatusCode,
                Count = 0,
                Size = Encoding.UTF8.GetByteCount(content)
            };
        }

        public async Task<HttpResponseResult> PatchAsync(string url, string content)
        {
            var request = await http.PatchAsync(BuildUri.Build(url), new StringContent(content, Encoding.UTF8, "application/json"));

            return new HttpResponseResult
            {
                Content = content,
                StatusCode = (int)request.StatusCode,
                Count = 0,
                Size = Encoding.UTF8.GetByteCount(content)
            };
        }

        public async Task<HttpResponseResult> DeleteAsync(string url)
        {
            var request = await http.DeleteAsync(BuildUri.Build(url));

            return new HttpResponseResult
            {
                StatusCode = (int)request.StatusCode,
                Count = 0,
            };
        }
    }
}
