using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Ephany.Framework.Sdk.Models
{
    public enum AssetFileCategory
    {
        [EnumMember(Value = "PDS")]
        CutSheet,

        [EnumMember(Value = "DWG")]
        CadFile,

        [EnumMember(Value = "RFA")]
        RevitFamily,

        [EnumMember(Value = "ETC")]
        Other
    }

    public class AssetFile
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// The absolute URL to the file hosted on the Ephany media server.
        /// </summary>
        [JsonProperty("file")]
        public string Url { get; set; }

        /// <summary>
        /// The 3-character code from Django (PDS, DWG, RFA, ETC).
        /// </summary>
        [JsonProperty("category")]
        public AssetFileCategory Category { get; set; }

        /// <summary>
        /// The human-readable string (e.g., "Revit Family") from get_category_display().
        /// </summary>
        [JsonProperty("category_display")]
        public string CategoryDisplay { get; set; }

        [JsonProperty("uploaded_at")]
        public DateTime UploadedAt { get; set; }
    }
}