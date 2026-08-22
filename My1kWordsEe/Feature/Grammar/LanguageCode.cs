using System.ComponentModel;
using System.Text.Json.Serialization;

namespace My1kWordsEe.Feature.Grammar
{
    /// <summary>
    /// ISO 639 language codes
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Description("ISO 639 language codes")]
    public enum LanguageCode
    {
        /// <summary>
        /// Estonian language
        /// </summary>
        Et,
        /// <summary>
        /// English language
        /// </summary>
        En
    }
}