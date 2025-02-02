using System.ComponentModel;
using System.Text.Json.Serialization;

using My1kWordsEe.Models.Grammar;

namespace My1kWordsEe.Models
{
    public struct TranslatedString
    {
        [JsonPropertyName(nameof(LanguageCode.Et))]
        [Description("Tõlge eesti keelde")]
        public required string Et { get; set; }

        [JsonPropertyName(nameof(LanguageCode.En))]
        [Description("Translation in English")]
        public required string En { get; set; }
    }
}