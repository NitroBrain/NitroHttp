using System.Net;
using Microsoft.Extensions.DependencyInjection;

namespace NitroHttp.Core.Http;

/// <summary>
/// Registers NitroHttp HTTP services.
/// </summary>
public static class HttpServiceCollectionExtensions
{
    /// <summary>
    /// Registers the NitroHttp HTTP engine.
    /// </summary>
    public static IServiceCollection AddNitroHttp(this IServiceCollection services)
    {
        services.AddSingleton<HttpClient>(provider =>
        {
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                AutomaticDecompression = DecompressionMethods.All,
                UseCookies = false
            };

            return new HttpClient(handler)
            {
                Timeout = Timeout.InfiniteTimeSpan
            };
        });

        services.AddSingleton<IHttpClient, HttpClientAdapter>();

        return services;
    }
}
