using System.ComponentModel;
using System.Text.Json.Serialization;

using My1kWordsEe.Models.Grammar;

namespace My1kWordsEe.Models
{
    [Description("A map of LanguageCode and string in a given language")]
    public class TranslatedString
    {
        [JsonPropertyName(nameof(LanguageCode.Et))]
        [Description("Translation in Estonian (ISO 639 code: et)")]
        public required string Et { get; set; }

        [JsonPropertyName(nameof(LanguageCode.En))]
        [Description("Translation in English (ISO 639 code: en)")]
        public required string En { get; set; }
    }
}