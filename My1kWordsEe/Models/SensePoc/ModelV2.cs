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

        public required string BaseForm { get; init; } // Reference to the base form

        public required IDictionary<LanguageCode, string> Explanation { get; init; } = new Dictionary<LanguageCode, string>();

        public required PartOfSpeech PartOfSpeech { get; init; } = PartOfSpeech.None;

        public SampleSentence[] Samples { get; init; } = Array.Empty<SampleSentence>();

        [JsonIgnore]
        public dynamic? Forms { get; init; }
    }

    public class NounForms
    {
        public PartOfSpeech PartOfSpeech => PartOfSpeech.Noun;
        public required string BaseForm { get; init; }
        public NounForm[] List { get; init; } = Array.Empty<NounForm>();
    }

    public record NounForm
    {
        GrammaticalCase Case { get; init; }
        Number Number { get; init; }
        string Value { get; init; }
    }

    public class VerbForms
    {
        public PartOfSpeech PartOfSpeech => PartOfSpeech.Verb;
        public required string BaseForm { get; init; }
        public VerbForm[] List { get; init; } = Array.Empty<VerbForm>();
    }

    public record VerbForm
    {
        Pronoun Pronoun { get; init; }
        Tense Tense { get; init; }
        string Value { get; init; }
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

    public enum PartOfSpeech
    {
        None, // Default value | cannot be determined
        Noun,
        Adjective,
        Pronoun,
        Numeral,
        Verb,
        Adverb,
        Interjection,
        Conjunction,
        Preposition,
        Article,
        Determiner
    }

    /// <summary>
    /// Grammatical cases in Estonian language
    /// </summary>
    public enum GrammaticalCase
    {
        None, // Default value | cannot be determined
        Nominative,
        Genitive,
        Partitive,
        Illative,
        Inessive,
        Elative,
        Allative,
        Adessive,
        Ablative,
        Translative,
        Terminative,
        Essive,
        Abessive,
        Comitative
    }

    public enum Tense { Present, Past, Future }

    public enum Number
    {
        Singular, Plural
    }

    public enum Pronoun
    {
        // Personal Pronouns
        I,          // mina
        YouSingular, // sina
        He,         // tema (ta)
        She,        // tema (ta)
        It,         // see (ta)
        We,         // meie
        YouPlural,  // teie
        They,       // nemad (nad)

        // Possessive Pronouns
        My,         // minu
        YourSingular, // sinu
        His,        // tema (ta)
        Her,        // tema (ta)
        Its,        // selle
        Our,        // meie
        YourPlural, // teie
        Their       // nende
    }
}

