using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Ephany.Framework.Sdk.Models
{
    /// <summary>
    /// Represents a product or resource in the Ephany catalog.
    /// Contains specifications, manufacturer details, and dimensional data.
    /// </summary>
    public class Asset
    {
        /// <summary>
        /// The unique database identifier for the asset.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// The alphanumeric identifier or SKU used for external references (e.g., "AVN-1").
        /// </summary>
        [JsonProperty("type_id")]
        public string TypeId { get; set; }

        /// <summary>
        /// The detailed manufacturer information associated with this asset.
        /// </summary>
        [JsonProperty("manufacturer")]
        public Manufacturer Manufacturer { get; set; }

        /// <summary>
        /// The display name of the manufacturer.
        /// Useful for quick display without accessing the nested <see cref="Manufacturer"/> object.
        /// </summary>
        [JsonProperty("manufacturer_name")]
        public string ManufacturerName { get; set; }

        /// <summary>
        /// The categorization details for this asset.
        /// </summary>
        [JsonProperty("category")]
        public Category Category { get; set; }

        /// <summary>
        /// The display name of the category (e.g., "Refrigerators - Closed Door").
        /// </summary>
        [JsonProperty("category_name")]
        public string CategoryName { get; set; }

        /// <summary>
        /// The specific model number or code assigned by the manufacturer.
        /// </summary>
        [JsonProperty("model")]
        public string Model { get; set; }

        /// <summary>
        /// The full display name or title of the asset.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// A detailed textual description of the asset's features and specifications.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// A direct link to the product page on the manufacturer's or distributor's website.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// The fully qualified URL to the catalog image or thumbnail for this asset.
        /// </summary>
        [JsonProperty("catalog_img")]
        public string CatalogImg { get; set; }

        /// <summary>
        /// The overall vertical height of the asset.
        /// <br/>
        /// Check <see cref="DisplayUnits"/> to determine the unit of measurement (usually "mm").
        /// </summary>
        [JsonProperty("overall_height")]
        public decimal? OverallHeight { get; set; }

        /// <summary>
        /// The overall horizontal width of the asset.
        /// <br/>
        /// Check <see cref="DisplayUnits"/> to determine the unit of measurement (usually "mm").
        /// </summary>
        [JsonProperty("overall_width")]
        public decimal? OverallWidth { get; set; }

        /// <summary>
        /// The overall depth (front to back) of the asset.
        /// <br/>
        /// Check <see cref="DisplayUnits"/> to determine the unit of measurement (usually "mm").
        /// </summary>
        [JsonProperty("overall_depth")]
        public decimal? OverallDepth { get; set; }

        /// <summary>
        /// A dictionary of dynamic fields specific to this asset type.
        /// Common keys include "door_type", "door_quantity", and "sku".
        /// </summary>
        [JsonProperty("custom_fields")]
        public Dictionary<string, object> CustomFields { get; set; }

        /// <summary>
        /// A list of associated files for this asset, such as CAD files and Revit families.
        /// </summary>
        [JsonProperty("files")]
        public List<AssetFile> Files { get; set; } = new List<AssetFile>();

        // --- SDK Helper Properties (Not in JSON, but useful for Revit) ---

        /// <summary>
        /// Returns the first file categorized as a Revit Family (RFA), if any.
        /// </summary>
        [JsonIgnore]
        public AssetFile? RevitFamilyFile => Files.FirstOrDefault(f => f.Category == AssetFileCategory.RevitFamily);

        /// <summary>
        /// Quick check to see if this asset can be placed in a Revit project.
        /// </summary>
        [JsonIgnore]
        public bool HasRevitFamily => RevitFamilyFile != null;

        /// <summary>
        /// Returns the first PDF file (Cut Sheet), often used for a "View Specs" button.
        /// </summary>
        [JsonIgnore]
        public AssetFile? CutSheetFile => Files.FirstOrDefault(f => f.Category == AssetFileCategory.CutSheet);

        /// <summary>
        /// Metadata defining the units of measurement for numeric properties.
        /// <br/>
        /// Common keys: "length" (e.g., "mm"), "mass" (e.g., "kg").
        /// </summary>
        [JsonProperty("_display_units")]
        public Dictionary<string, string> DisplayUnits { get; set; }
    }
}