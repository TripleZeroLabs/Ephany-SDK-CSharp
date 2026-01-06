using System;

namespace Ephany.Framework.Sdk
{
    /// <summary>
    /// Configuration options for initializing the <see cref="EphanyClient"/>.
    /// </summary>
    public class EphanyClientOptions
    {
        /// <summary>
        /// The base URL of the Ephany API (e.g., "https://api.ephany.io").
        /// Trailing slashes are automatically removed during initialization.
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// The API Key used for authentication. 
        /// This is sent in the "X-Api-Key" header (or "Authorization" header depending on client config).
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Flags whether the authentication is an API key or authentication token
        /// </summary>
        public AuthScheme Scheme { get; }

        /// <summary>
        /// The request timeout in seconds. Defaults to 100 seconds.
        /// </summary>
        public int TimeoutSeconds { get; set; } = 100;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="apiKey"></param>
        /// <param name="scheme"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public EphanyClientOptions(string baseUrl, string apiKey, AuthScheme scheme = AuthScheme.ApiKey)
        {
            BaseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
            ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            Scheme = scheme;
        }

        /// <summary>
        /// Differentiates an API key from a user token
        /// </summary>
        public enum AuthScheme
        {
            ApiKey,     // Machine-to-Machine (X-Api-Key)
            UserToken   // User-to-Machine (Authorization: Token ...)
        }
    }
}