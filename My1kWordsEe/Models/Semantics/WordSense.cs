using System.ComponentModel;

using My1kWordsEe.Models.Grammar;

namespace My1kWordsEe.Models.Semantics
{
    /// <summary>
    /// A sense or meaning of a word with respective forms.
    /// </summary>
    [Description("A sense or meaning of the Estonian word with respective forms.")]
    public record WordSense
    {
        [Description("The given Estonian word")]
        public required string Word { get; init; }

        [Description("Explanation of the word meaning and its given grammar form")]
        public required TranslatedString Explanation { get; init; } = new TranslatedString();

        /// <summary>
        /// The form of a word to which prefixes and suffixes can be added.
        /// Use to locate word forms based on <see cref="PartOfSpeech"/>
        /// </summary>
        [Description("The base grammar form of the given Estonian word to which prefixes and suffixes can be added.")]
        public required string BaseForm { get; init; }

        [Description("Part of speech, None if cannot be determined")]
        public required PartOfSpeech PartOfSpeech { get; init; } = PartOfSpeech.None;
    }
}