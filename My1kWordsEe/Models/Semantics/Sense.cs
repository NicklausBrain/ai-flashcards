using System.Text.Json.Serialization;

namespace My1kWordsEe.Models.Grammar
{
    /// <summary>
    /// A sense or meaning of a word with respective forms.
    /// </summary>
    public record Sense
    {
        // does it actually make sense id? so that we can navigate to form in this sense?
        // public required string SenseId { get; init; } // Unique identifier for the sense

        public required string BaseForm { get; init; } // Reference to the base form

        public required IDictionary<LanguageCode, string> Explanation { get; init; } = new Dictionary<LanguageCode, string>();

        public required PartOfSpeech PartOfSpeech { get; init; } = PartOfSpeech.None;

        public SampleSentence[] Samples { get; init; } = Array.Empty<SampleSentence>();

        [JsonIgnore]
        public dynamic? Forms { get; init; }
    }
}