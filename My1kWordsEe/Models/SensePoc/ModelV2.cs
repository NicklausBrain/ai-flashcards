using System.Text.Json.Serialization;

/*
 * Discussion:
 * https://copilot.microsoft.com/chats/HRqmx1oACcDCK2gGgQ5he
 */
namespace My1kWordsEe.Models.SensePoc
{
    /// <summary>
    /// ISO 639 language codes
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum LanguageCode
    {
        /// <summary>
        /// Estonian language
        /// </summary>
        Et,
        /// <summary>
        /// English language
        /// </summary>
        En
    }

    /// <summary>
    /// A word in the Estonian language with respective senses and forms.
    /// </summary>
    public record Word
    {
        public required string Value { get; init; }

        public required LanguageCode Language { get; init; }

        public Sense[] Senses { get; init; } = Array.Empty<Sense>();

        [JsonIgnore]
        public Sense DefaultSense => Senses[0];
    }

    /// <summary>
    /// A sense or meaning of a word with respective forms.
    /// </summary>
    public record Sense
    {
        // does it actually make sense id? so that we can navigate to form in this sense?
        // public required string SenseId { get; init; } // Unique identifier for the sense

        public required IDictionary<LanguageCode, string> Explanation { get; init; } = new Dictionary<LanguageCode, string>();

        // imho too much nesting for now
        // public Form[] Forms { get; init; } = Array.Empty<Form>();

        public SampleSentence[] Samples { get; init; } = Array.Empty<SampleSentence>();
    }

    /// <summary>
    /// A form of a sense of a word in the Estonian language.
    /// </summary>
    public record Form
    {
        public required string EeForm { get; init; }
        public required string EeGrammarCase { get; init; }  // E.g., nominative, genitive, etc.
        public Uri? EeAudioUrl { get; init; }
    }

    /// <summary>
    /// A sample sentence illustrating the use of a given Estonian word form.
    /// </summary>
    public record SampleSentence
    {
        public required string EeWord { get; init; }
        public required string EeSentence { get; init; }
        public required string EnSentence { get; init; }
        public Uri? EeAudioUrl { get; init; }
        public Uri? ImageUrl { get; init; }
    }
}
