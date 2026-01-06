using Ephany.Framework.Sdk.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers; // Required for Authorization header
using System.Threading;
using System.Threading.Tasks;

namespace Ephany.Framework.Sdk
{
    /// <summary>
    /// The primary client for interacting with the Ephany Framework API.
    /// Supports both User Authentication (Token) and Machine Authentication (API Key).
    /// </summary>
    public class EphanyClient : IDisposable
    {
        private readonly HttpClient _http;
        private readonly EphanyClientOptions _opts;

        /// <summary>
        /// Initializes a new instance of the <see cref="EphanyClient"/> class.
        /// </summary>
        /// <param name="opts">Configuration options including BaseUrl and the Auth Credential (Key or Token).</param>
        public EphanyClient(EphanyClientOptions opts)
        {
            _opts = opts ?? throw new ArgumentNullException(nameof(opts));

            if (string.IsNullOrWhiteSpace(opts.ApiKey))
            {
                throw new ArgumentException("Authentication credential (ApiKey or Token) is required.", nameof(opts.ApiKey));
            }

            var baseUriStr = opts.BaseUrl.EndsWith("/") ? opts.BaseUrl : opts.BaseUrl + "/";

            _http = new HttpClient
            {
                BaseAddress = new Uri(baseUriStr), // e.g., "http://localhost:8000/api/"
                Timeout = TimeSpan.FromSeconds(opts.TimeoutSeconds)
            };

            // ========================================================================
            // EXPLICIT AUTHENTICATION LOGIC
            // ========================================================================
            // We switch based on the Scheme provided in options, ensuring 100% accuracy.

            switch (opts.Scheme)
            {
                case EphanyClientOptions.AuthScheme.ApiKey:
                    // Machine-to-Machine Auth
                    // Header: "X-Api-Key: <key>"
                    _http.DefaultRequestHeaders.Add("X-Api-Key", opts.ApiKey);
                    break;

                case EphanyClientOptions.AuthScheme.UserToken:
                    // User-to-Server Auth
                    // Header: "Authorization: Token <value>"
                    _http.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Token", opts.ApiKey);
                    break;

                default:
                    // Fallback or throw if you add more schemes later
                    _http.DefaultRequestHeaders.Add("X-Api-Key", opts.ApiKey);
                    break;
            }

            _http.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<PagedResult<Asset>> GetAssetPageAsync(int page = 1, int pageSize = 20, CancellationToken ct = default)
        {
            if (page < 1) throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater than 0");
            if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize), "PageSize must be greater than 0");

            var requestUrl = $"assets/?page={page}&pageSize={pageSize}";

            try
            {
                var response = await _http.GetAsync(requestUrl, ct);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                    response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    // Updated error message to reflect hybrid auth
                    throw new UnauthorizedAccessException("Authentication failed. Your API Key or User Token is invalid/expired.");
                }

                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<PagedResult<Asset>>(json);

                if (result == null)
                    return new PagedResult<Asset> { Items = new List<Asset>(), PageNumber = page, PageSize = pageSize };

                result.Items ??= new List<Asset>();
                result.PageNumber = page;
                result.PageSize = pageSize;

                return result;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Network error connecting to Ephany API: {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                throw new Exception("Failed to parse the response from the server.", ex);
            }
        }

        public async Task<List<Asset>> GetAllAssetsAsync(CancellationToken ct = default)
        {
            var allAssets = new List<Asset>();
            int currentPage = 1;
            bool hasNextPage = true;

            while (hasNextPage)
            {
                var result = await GetAssetPageAsync(currentPage, pageSize: 50, ct);

                if (result.Items != null && result.Items.Count > 0)
                {
                    allAssets.AddRange(result.Items);
                }

                hasNextPage = !string.IsNullOrEmpty(result.NextPageUrl);
                currentPage++;
            }

            return allAssets;
        }

        public async Task<List<Asset>> GetRevitAssetsAsync(CancellationToken ct = default)
        {
            var allAssets = await GetAllAssetsAsync(ct);
            return allAssets.Where(a => a.HasRevitFamily).ToList();
        }

        public async Task<PagedResult<Asset>> SearchAssetsAsync(string keyword, int page = 1, int pageSize = 20, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return await GetAssetPageAsync(page, pageSize, ct);

            var encodedKeyword = Uri.EscapeDataString(keyword);
            var requestUrl = $"assets/?search={encodedKeyword}&page={page}&pageSize={pageSize}";

            try
            {
                var response = await _http.GetAsync(requestUrl, ct);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    throw new UnauthorizedAccessException("Authentication credentials rejected.");

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<PagedResult<Asset>>(json);

                if (result != null)
                {
                    result.PageNumber = page;
                    result.PageSize = pageSize;
                }

                return result ?? new PagedResult<Asset>();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error searching Ephany assets: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Downloads a specific file from the Ephany media server to the local file system.
        /// Handles multi-targeting for .NET Framework 4.8 and .NET 8.0+.
        /// </summary>
        public async Task DownloadFileAsync(AssetFile file, string destinationPath, CancellationToken ct = default)
        {
            if (file == null || string.IsNullOrEmpty(file.Url))
                throw new ArgumentException("Invalid file or URL provided.");

            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, file.Url))
                {
                    using (var response = await _http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct))
                    {
                        response.EnsureSuccessStatusCode();

                        using (var responseStream = await response.Content.ReadAsStreamAsync())
                        {
                            using (var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                            {
                                /* DEV NOTE: Handle CopyToAsync signature differences between .NET versions */
#if NET48
                                // .NET Framework 4.8 requires bufferSize as the 2nd argument
                                await responseStream.CopyToAsync(fileStream, 8192, ct);
#else
                                // .NET 8.0+ supports CancellationToken as the 2nd argument
                                await responseStream.CopyToAsync(fileStream, ct);
#endif
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                if (File.Exists(destinationPath)) File.Delete(destinationPath);
                throw;
            }
        }

        public void Dispose()
        {
            _http?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}