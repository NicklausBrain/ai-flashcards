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
            string.Equals(this.PartOfSpeech.Et, "nimisõna", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(this.PartOfSpeech.En, "noun", StringComparison.OrdinalIgnoreCase);
    }
}