using System.ComponentModel;

namespace My1kWordsEe.Models.Semantics
{
    /// <summary>
    /// A sense or meaning of a word with respective forms.
    /// </summary>
    [Description("Eesti sõna tähendus.")]
    public record WordSense
    {
        [Description("Sama antud sõna ja selle otsetõlge")]
        public required TranslatedString Word { get; init; }

        [Description("Antud sõna tähenduse ja grammatilise vormi selgitus")]
        public required TranslatedString Definition { get; init; }

        /// <summary>
        /// The form of a word to which prefixes and suffixes can be added.
        /// Use to locate word forms based on <see cref="PartOfSpeech"/>
        /// </summary>
        [Description("Sõna grammatika põhivorm (nt nimisõna ainsuse nimetav või tegusõna ma-da-ta-tegevusnimi)")]
        public required string BaseForm { get; init; }

        [Description("Kõneosa")]
        public required TranslatedString PartOfSpeech { get; init; }
    }
}