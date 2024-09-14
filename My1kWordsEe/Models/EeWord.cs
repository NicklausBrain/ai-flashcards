using System.Text.Json.Serialization;

namespace My1kWordsEe.Models
{
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