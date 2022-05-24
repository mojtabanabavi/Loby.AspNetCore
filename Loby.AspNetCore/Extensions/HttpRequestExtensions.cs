using System;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Loby.AspNetCore.Extensions
{
    /// <summary>
    /// A collection of extension methods for <see cref="HttpRequest"/>.
    /// </summary>
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// Determines whether the specified HTTP request is an AJAX request.
        /// </summary>
        /// <param name="httpRequest">
        /// An instance of <see cref="HttpRequest"/>.
        /// </param>
        /// <returns>
        /// Returns true if the specified HTTP request is an AJAX request; 
        /// otherwise, false.
        /// </returns>
        public static bool IsAjaxRequest(this HttpRequest httpRequest)
        {
            if (httpRequest == null)
            {
                throw new ArgumentNullException(nameof(httpRequest));
            }

            if (httpRequest.Headers != null)
            {
                return httpRequest.Headers["X-Requested-With"] == "XMLHttpRequest";
            }

            return false;
        }

        /// <summary>
        /// Returns the base url of application.
        /// </summary>
        /// <param name="httpRequest">
        /// An instance of <see cref="HttpRequest"/>.
        /// </param>
        /// <returns>
        /// Returns An string representing root url of application.
        /// </returns>
        public static string GetBaseUrl(this HttpRequest httpRequest)
        {
            if (httpRequest == null)
            {
                throw new ArgumentNullException(nameof(httpRequest));
            }

            return string.Format("{0}:://{1}", httpRequest.Scheme, httpRequest.Host);
        }
    }
}
