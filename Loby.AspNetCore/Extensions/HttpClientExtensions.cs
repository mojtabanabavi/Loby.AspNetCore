using System;
using System.Text;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Loby.AspNetCore.Extensions
{
    /// <summary>
    /// A collection of extension methods for <see cref="HttpClient"/>.
    /// </summary>
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Sends a POST request to the specified url as an asynchronous operation.
        /// </summary>
        /// <param name="httpClient">
        /// An instance of <see cref="HttpClient"/>.
        /// </param>
        /// <param name="requestUrl">
        /// The url the request is sent to.
        /// </param>
        /// <param name="content">
        /// The request content sent to the server.
        /// </param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The requestUrl is null.
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// The request failed due to an underlying issue such as network connectivity, DNS
        /// failure, server certificate validation or timeout.
        /// </exception>
        public static async Task<HttpResponseMessage> PostJsonAsync(this HttpClient httpClient, string requestUrl, object content)
        {
            if (httpClient == null)
            {
                throw new NullReferenceException(nameof(httpClient));
            }

            if (requestUrl == null)
            {
                throw new NullReferenceException(nameof(requestUrl));
            }

            var jsonContent = CreateJsonContent(content);
            var response = await httpClient.PostAsync(requestUrl, jsonContent);

            return response;
        }

        /// <summary>
        /// Sends a PUT request to the specified url as an asynchronous operation.
        /// </summary>
        /// <param name="httpClient">
        /// An instance of <see cref="HttpClient"/>.
        /// </param>
        /// <param name="requestUrl">
        /// The url the request is sent to.
        /// </param>
        /// <param name="content">
        /// The request content sent to the server.
        /// </param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The requestUrl is null.
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// The request failed due to an underlying issue such as network connectivity, DNS
        /// failure, server certificate validation or timeout.
        /// </exception>
        public static async Task<HttpResponseMessage> PutJsonAsync(this HttpClient httpClient, string requestUrl, object content)
        {
            if (httpClient == null)
            {
                throw new NullReferenceException(nameof(httpClient));
            }

            if (requestUrl == null)
            {
                throw new NullReferenceException(nameof(requestUrl));
            }

            var jsonContent = CreateJsonContent(content);
            var response = await httpClient.PutAsync(requestUrl, jsonContent);

            return response;
        }

        /// <summary>
        /// Creates a HTTP content based on a JSON string.
        /// </summary>
        /// <param name="content">
        /// The request content sent to the server.
        /// </param>
        /// <returns>
        /// A new instance of <see cref="StringContent"/> configured for json content.
        /// </returns>
        private static StringContent CreateJsonContent(object content)
        {
            var jsonData = JsonSerializer.Serialize(content);
            var jsonContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            return jsonContent;
        }
    }
}
