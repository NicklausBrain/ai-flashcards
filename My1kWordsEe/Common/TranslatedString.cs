using System.ComponentModel;
using System.Text.Json.Serialization;

namespace My1kWordsEe.Common
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
