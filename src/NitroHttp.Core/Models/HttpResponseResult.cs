namespace NitroHttp.Core.Models;

public class HttpResponseResult
{
    public string? Content { get; set; } = "";
    public int StatusCode { get; set; }
    public int Count { get; set; }
    public int? Size { get; set; }
}
