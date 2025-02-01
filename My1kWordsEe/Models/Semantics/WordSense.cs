using System.ComponentModel;

using My1kWordsEe.Models.Grammar;

namespace My1kWordsEe.Models.Semantics
{
    /// <summary>
    /// A sense or meaning of a word with respective forms.
    /// </summary>
    [Description("Eesti sõna tähendus või tähendus vastavate vormidega.")]
    public record WordSense
    {
        [Description("Sõna tähenduse ja antud grammatikavormi selgitus")]
        public required TranslatedString Explanation { get; init; }

        /// <summary>
        /// The form of a word to which prefixes and suffixes can be added.
        /// Use to locate word forms based on <see cref="PartOfSpeech"/>
        /// </summary>
        [Description("BaseForm on sõna grammatika põhivorm (nt nimisõna ainsuse nimetav või tegusõna ma-da-ta-tegevusnimi)")]
        public required string BaseForm { get; init; }

        [Description("Kõneosa, puudub, kui ei saa määrata")]
        public required EtPartOfSpeech PartOfSpeech { get; init; }
    }
}