using System.Text.Json.Serialization;

namespace My1kWordsEe.Models.Grammar
{
    /// <summary>
    /// ISO 639 language codes
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
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