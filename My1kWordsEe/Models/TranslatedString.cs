using System.ComponentModel;
using System.Text.Json.Serialization;

using My1kWordsEe.Models.Grammar;

namespace My1kWordsEe.Models
{
    public class TranslatedString
    {
        [JsonPropertyName(nameof(LanguageCode.Et))]
        [Description("Tõlge eesti keelde (ISO 639 code: et)")]
        public required string Et { get; set; }

        [JsonPropertyName(nameof(LanguageCode.En))]
        [Description("Translation in English (ISO 639 code: en)")]
        public required string En { get; set; }
    }
}