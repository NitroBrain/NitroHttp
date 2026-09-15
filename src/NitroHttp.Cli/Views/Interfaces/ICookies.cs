using NitroHttp.Core.Models;

namespace NitroHttp.Cli.Views.Interfaces
{
    /// <summary>
    /// Displays request/response cookies
    /// </summary>
    public interface ICookies
    {
        /// <summary>
        /// Displays formatted cookies.
        /// </summary>
        /// <param name="cookies">The HTTP cookies to display.</param>
        void Display(IReadOnlyList<CookieModel> cookies);
    }
}
