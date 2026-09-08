
using NitroHttp.Core.Models;

namespace NitroHttp.Cli.Views.Interfaces
{
    /// <summary>
    /// Displays request/response headers
    /// </summary>
    public interface IHeaders
    {
        /// <summary>
        /// Displays formmated cookies
        /// </summary>
        void Display(IReadOnlyList<HttpHeader> headers);
    }
}
