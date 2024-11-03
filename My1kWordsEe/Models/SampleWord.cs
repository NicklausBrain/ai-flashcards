using System.Text.Json.Serialization;

namespace My1kWordsEe.Models
{
    /// <summary>
    /// A word of the Estonian language with the respective translations and usage examples
    /// </summary>
    [JsonSourceGenerationOptions(UseStringEnumConverter = true)]
    [JsonSerializable(typeof(EePartOfSpeech))]
    public record SampleWord
    {
        private readonly string eeWord = "";
        private readonly string[] enWords = [];
        private readonly SampleSentence[] samples = [];

        /// <summary>
        /// Estonian word
        /// </summary>
        public string EeWord
        {
            get => this.eeWord;
            init => this.eeWord = value?.ToLower() ?? "";
        }

        public EePartOfSpeech? EePartOfSpeech { get; init; }

        /// <summary>
        /// Default translation to English
        /// </summary>
        public required string EnWord { get; init; }

        /// <summary>
        /// Alternative translations to English
        /// </summary>
        public string[] EnWords
        {
            get => this.enWords;
            init => this.enWords = value ?? this.enWords;
        }

        /// <summary>
        /// Explaining the word in English
        /// </summary>
        public string? EnExplanation { get; init; }

        /// <summary>
        /// Sample pronunciation of the word
        /// </summary>
        public Uri? EeAudioUrl { get; init; }

        /// <summary>
        /// Word usage examples
        /// </summary>
        public SampleSentence[] Samples
        {
            get => this.samples;
            init => this.samples = value ?? this.samples;
        }
    }
}
