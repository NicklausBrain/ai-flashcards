using System.ComponentModel;
using System.Text.Json.Serialization;

namespace My1kWordsEe.Models.Semantics
{
    /// <summary>
    /// A sense or meaning of a word with respective forms.
    /// </summary>
    [Description("Eesti sõna tähendus.")]
    public readonly struct WordSense
    {
        [Description("Sama antud sõna ja selle otsetõlge")]
        public required TranslatedString Word { get; init; }

        [Description("Antud sõna tähenduse ja grammatilise vormi selgitus")]
        public required TranslatedString Definition { get; init; }

        [Description("Sõna grammatika põhivorm (nt nimisõna ainsuse nimetav või tegusõna ma-da-ta-tegevusnimi)")]
        public required string BaseForm { get; init; }

        [Description("Kõneosa")]
        public required TranslatedString PartOfSpeech { get; init; }

        [JsonIgnore]
        public bool IsNoun =>
            this.PartOfSpeech.Et.Contains("nimisõna", StringComparison.OrdinalIgnoreCase) ||
            this.PartOfSpeech.Et.Contains("substantiiv", StringComparison.OrdinalIgnoreCase) ||
            this.PartOfSpeech.En.Contains("noun", StringComparison.OrdinalIgnoreCase) ||
            this.PartOfSpeech.En.Contains("subst.", StringComparison.OrdinalIgnoreCase);

        [JsonIgnore]
        public bool IsAdjective =>
            this.PartOfSpeech.Et.Contains("omadussõna", StringComparison.OrdinalIgnoreCase) ||
            this.PartOfSpeech.Et.Contains("adjektiiv", StringComparison.OrdinalIgnoreCase) ||
            this.PartOfSpeech.Et.Contains("omaduss.", StringComparison.OrdinalIgnoreCase) ||
            this.PartOfSpeech.En.Contains("adjective", StringComparison.OrdinalIgnoreCase) ||
            this.PartOfSpeech.En.Contains("adj.", StringComparison.OrdinalIgnoreCase);
    }
}
