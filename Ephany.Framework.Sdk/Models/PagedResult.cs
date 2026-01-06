using Newtonsoft.Json;
using System.Collections.Generic;

namespace Ephany.Framework.Sdk.Models
{
    /// <summary>
    /// A generic container for paginated API responses.
    /// Wraps the list of items with metadata about the total count and navigation links.
    /// </summary>
    /// <typeparam name="T">The type of model contained in the list (e.g., <see cref="Asset"/>).</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// The total number of items available across all pages.
        /// Use this to calculate the total number of pages.
        /// </summary>
        [JsonProperty("count")]
        public long TotalCount { get; set; }

        /// <summary>
        /// The list of items returned for the current page.
        /// </summary>
        [JsonProperty("results")]
        public List<T> Items { get; set; } = new List<T>();

        /// <summary>
        /// The full URL to retrieve the next page of results, or null if this is the last page.
        /// </summary>
        [JsonProperty("next")]
        public string? NextPageUrl { get; set; }

        /// <summary>
        /// The full URL to retrieve the previous page of results, or null if this is the first page.
        /// </summary>
        [JsonProperty("previous")]
        public string? PreviousPageUrl { get; set; }

        /// <summary>
        /// The current page number (1-based index).
        /// <br/>
        /// Note: This is populated manually by the client and is not returned directly by the API.
        /// </summary>
        [JsonIgnore]
        public int PageNumber { get; set; }

        /// <summary>
        /// The number of items requested per page.
        /// <br/>
        /// Note: This is populated manually by the client and is not returned directly by the API.
        /// </summary>
        [JsonIgnore]
        public int PageSize { get; set; }
    }
}