using Newtonsoft.Json;

namespace Ephany.Framework.Sdk.Models
{
    /// <summary>
    /// Represents a classification or group for assets in the Ephany catalog.
    /// Used to organize products into logical collections (e.g., "Refrigerators", "Ovens").
    /// </summary>
    public class Category
    {
        /// <summary>
        /// The unique database identifier for the category.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// The display name of the category.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}