namespace NitroHttp.Core.Helpers;

public static class InputReader
{
    public static async Task<string?> ReadAsync(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }

        return File.Exists(input) ? await File.ReadAllTextAsync(input) : input;
    }
}
