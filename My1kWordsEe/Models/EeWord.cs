using System.Text.Json.Serialization;

namespace My1kWordsEe.Models
{
    /// <summary>
    /// currently serves as view model for the 1k words page
    /// todo: must be unified with <see cref="SampleWord"/>
    /// </summary>
    [JsonSourceGenerationOptions(UseStringEnumConverter = true)]
    [JsonSerializable(typeof(EePartOfSpeech))]
    public record EeWord
    {
        private readonly EePartOfSpeech[] partsOfSpeech = [];
        private readonly string[] enWords = [];

        [JsonPropertyName("value")]
        public required string Value { get; init; }

        [JsonPropertyName("part_of_speech")]
        public EePartOfSpeech EePartOfSpeech { get; init; }

        /// <summary>
        /// Default translation to English
        /// </summary>
        public string EnWord => this.EnWords.First();

        /// <summary>
        /// Alternatives to EnWord
        /// </summary>
        [JsonPropertyName("en_words")]
        public string[] EnWords
        {
            get => this.enWords;
            init => this.enWords = value ?? this.enWords;
        }
    }
}