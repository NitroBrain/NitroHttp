
using NitroHttp.Core.Models;

namespace NitroHttp.Cli.Views.Interfaces
{
    /// <summary>
    /// Displays request/response headers
    /// </summary>
    public interface IHeaders
    {
        /// <summary>
        /// Displays formatted HTTP headers.
        /// </summary>
        /// <param name="headers">The HTTP headers to display.</param>
        void Display(IReadOnlyList<HttpHeader> headers);
    }
}
