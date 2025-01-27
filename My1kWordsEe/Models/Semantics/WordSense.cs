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
        /// <summary>
        /// The form of a word to which prefixes and suffixes can be added.
        /// Use to locate word forms based on <see cref="PartOfSpeech"/>
        /// </summary>
        [Description("The base form of Estonian word to which prefixes and suffixes can be added.")]
        public required string BaseForm { get; init; }

        [Description("Explanation of the word")]
        public required TranslatedString Explanation { get; init; } = new TranslatedString();

        [Description("Part of speech, None if cannot be determined")]
        public required PartOfSpeech PartOfSpeech { get; init; } = PartOfSpeech.None;

        // [Description("Samples, keep empty by default")]
        // public SampleSentence[] Samples { get; init; } = Array.Empty<SampleSentence>();

        /// <summary>
        /// Can be used to load respecive forms/conjugations
        /// </summary>
        // [JsonIgnore]
        // public dynamic? Forms { get; init; }
    }
}