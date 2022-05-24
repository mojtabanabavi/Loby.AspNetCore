using System;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Loby.AspNetCore.Extensions
{
    /// <summary>
    /// A collection of extension methods for <see cref="HttpResponse"/>.
    /// </summary>
    public static class HttpResponseExtensions
    {
        /// <summary>
        /// Get the cookie value associated with the specified key.
        /// </summary>
        /// <param name="httpResponse">
        /// An instance of <see cref="HttpResponse"/>.
        /// </param>
        /// <param name="key">
        /// The key of the value to get.
        /// </param>
        /// <returns>
        /// Returns the element with the specified key, or null if 
        /// the key is not present.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// httpResponse is null.
        /// </exception>
        public static string GetCookieValue(this HttpResponse httpResponse, string key)
        {
            if (httpResponse == null)
            {
                throw new ArgumentNullException(nameof(httpResponse));
            }

            foreach (var headers in httpResponse.Headers.Values)
            {
                foreach (var header in headers)
                {
                    if (header.StartsWith($"{key}="))
                    {
                        int equalsSignIndex = header.IndexOf('=');
                        int semicolonSignIndex = header.IndexOf(';');

                        var cookieValue = header.Substring(equalsSignIndex + 1, semicolonSignIndex - equalsSignIndex - 1);

                        return cookieValue;
                    }
                }
            }

            return null;
        }
    }
}
