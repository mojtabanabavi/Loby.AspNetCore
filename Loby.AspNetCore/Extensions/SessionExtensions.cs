using System;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Loby.AspNetCore.Extensions
{
    /// <summary>
    /// A collection of extension methods for <see cref="ISession"/>.
    /// </summary>
    public static class SessionExtensions
    {
        /// <summary>
        /// Set the given key and value in the current session. This will 
        /// throw if the session was not established prior to sending the response.
        /// </summary>
        /// <param name="session">
        /// An implementation of <see cref="ISession"/>.
        /// </param>
        /// <param name="key">
        /// A unique key that provide access to current <paramref name="value"/>.
        /// </param>
        /// <param name="value">
        /// The data that is going to store in session.
        /// </param>
        /// <exception cref="ArgumentException">
        /// The key is null or empty or white space.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The session or value is null.
        /// </exception>
        public static void SetObject(this ISession session, string key, object value)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException($"{nameof(key)} is null or empty or white space.");
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var jsonData = JsonSerializer.Serialize(value);

            session.SetString(key, jsonData);
        }

        /// <summary>
        /// Retrieve the value of the given <paramref name="key"/>, if present.
        /// </summary>
        /// <typeparam name="T">
        /// The target type of the value.
        /// </typeparam>
        /// <param name="session">
        /// An implementation of <see cref="ISession"/>.
        /// </param>
        /// <param name="key">
        /// The data key that you looking for.
        /// </param>
        /// <returns>
        /// A <typeparamref name="T"/> representation of the data associated 
        /// to the <paramref name="key"/>, if present; otherwise, the default 
        /// value for <typeparamref name="T"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The key is null or empty or white space.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The session is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The value could not be converted to <typeparamref name="T"/>.
        /// </exception>
        public static T GetObject<T>(this ISession session, string key)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException($"{nameof(key)} is null or empty or white space.");
            }

            var jsonData = session.GetString(key);

            if (jsonData != null)
            {
                try
                {
                    return JsonSerializer.Deserialize<T>(jsonData);
                }
                catch (JsonException)
                {
                    throw new InvalidOperationException($"The value could not be converted to ${typeof(T)}");
                }
            }

            return default(T);
        }
    }
}
