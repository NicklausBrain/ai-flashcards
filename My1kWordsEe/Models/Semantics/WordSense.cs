using System.Text.Json.Serialization;

using My1kWordsEe.Models.Grammar;

namespace My1kWordsEe.Models.Semantics
{
    /// <summary>
    /// A sense or meaning of a word with respective forms.
    /// </summary>
    public record WordSense
    {
        /// <summary>
        /// The form of a word to which prefixes and suffixes can be added.
        /// Use to locate word forms based on <see cref="PartOfSpeech"/>
        /// </summary>
        public required string BaseForm { get; init; }

        public required IDictionary<LanguageCode, string> Explanation { get; init; } = new Dictionary<LanguageCode, string>();

        public required PartOfSpeech PartOfSpeech { get; init; } = PartOfSpeech.None;

        public SampleSentence[] Samples { get; init; } = Array.Empty<SampleSentence>();

        /// <summary>
        /// Can be used to load respecive forms/conjugations
        /// </summary>
        [JsonIgnore]
        public dynamic? Forms { get; init; }
    }
}