using System.Text;
using System.Text.Json;
using NitroHttp.Core.Models;

namespace NitroHttp.Core.Http;

/// <summary>
/// Default HTTP response factory.
/// </summary>
public sealed class HttpResponseFactory : IHttpResponseFactory
{
    /// <inheritdoc/>
    public async Task<HttpResponseResult> CreateAsync(HttpResponseMessage response, TimeSpan duration)
    {
        ArgumentNullException.ThrowIfNull(response);

        string content = await response.Content.ReadAsStringAsync();

        return new HttpResponseResult
        {
            StatusCode = (int)response.StatusCode,
            Content = content,
            Duration = duration,
            Size = Encoding.UTF8.GetByteCount(content),
            Count = CountJsonItems(content),
            Headers = ReadHeaders(response),
            Cookies = ReadCookies(response)
        };
    }

    private static IReadOnlyList<HttpHeader> ReadHeaders(HttpResponseMessage response)
    {
        var headers = new List<HttpHeader>();

        foreach (var header in response.Headers)
        {
            headers.Add(new HttpHeader
            {
                Name = header.Key,
                Value = string.Join(", ", header.Value)
            });
        }

        foreach (var header in response.Content.Headers)
        {
            headers.Add(new HttpHeader
            {
                Name = header.Key,
                Value = string.Join(", ", header.Value)
            });
        }

        return headers;
    }

    private static IReadOnlyList<CookieModel> ReadCookies(HttpResponseMessage response)
    {
        var cookies = new List<CookieModel>();

        if (!response.Headers.TryGetValues(
                "Set-Cookie",
                out var values))
        {
            return cookies;
        }

        foreach (var value in values)
        {
            var parts = value.Split(';', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
            {
                continue;
            }

            var pair = parts[0].Split('=', 2);

            if (pair.Length != 2)
            {
                continue;
            }

            string name = pair[0].Trim();
            string cookieValue = pair[1].Trim();
            string? domain = null;
            string path = "/";
            string? age = null;
            bool httpOnly = false;
            string? sameSite = null;

            for (int i = 1; i < parts.Length; i++)
            {
                var attr = parts[i].Trim();
                if (attr.Equals("HttpOnly", StringComparison.OrdinalIgnoreCase))
                {
                    httpOnly = true;
                }
                else
                {
                    var attrPair = attr.Split('=', 2);
                    var attrName = attrPair[0].Trim();
                    var attrVal = attrPair.Length > 1 ? attrPair[1].Trim() : string.Empty;

                    if (attrName.Equals("Domain", StringComparison.OrdinalIgnoreCase))
                    {
                        domain = attrVal;
                    }
                    else if (attrName.Equals("Path", StringComparison.OrdinalIgnoreCase))
                    {
                        path = attrVal;
                    }
                    else if (attrName.Equals("Max-Age", StringComparison.OrdinalIgnoreCase) || attrName.Equals("Expires", StringComparison.OrdinalIgnoreCase))
                    {
                        age = attrVal;
                    }
                    else if (attrName.Equals("SameSite", StringComparison.OrdinalIgnoreCase))
                    {
                        sameSite = attrVal;
                    }
                }
            }

            int size = (name?.Length ?? 0) + (cookieValue?.Length ?? 0);

            cookies.Add(new CookieModel
            {
                Name = name!,
                Value = cookieValue!,
                Domain = domain,
                Path = path,
                Age = age,
                Size = size,
                HttpOnly = httpOnly,
                SameSite = sameSite
            });
        }

        return cookies;
    }

    private static int CountJsonItems(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return 0;
        }

        try
        {
            using JsonDocument document = JsonDocument.Parse(content);

            return document.RootElement.ValueKind switch
            {
                JsonValueKind.Array => document.RootElement.GetArrayLength(),
                JsonValueKind.Object => 1,
                _ => 0
            };
        }
        catch
        {
            return 0;
        }
    }
}
