using Newtonsoft.Json;

namespace Ephany.Framework.Sdk.Models
{
    /// <summary>
    /// Represents the company or entity responsible for producing an asset.
    /// </summary>
    public class Manufacturer
    {
        /// <summary>
        /// The unique database identifier for the manufacturer.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// The common display name of the manufacturer (e.g., "Avantco").
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The official website URL for the manufacturer.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// A direct URL to the manufacturer's logo image (usually a PNG or JPG).
        /// </summary>
        [JsonProperty("logo")]
        public string LogoUrl { get; set; }
    }
}