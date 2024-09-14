using System.Text.Json.Serialization;

namespace My1kWordsEe.Models
{
    // possible parts_of_speech values : Nimisõna, Omadussõna, Asesõna, Arvsõna, Tegusõna, Määrsõna, Hüüdsõna, Sidesõna, Kaassõna
    // {
    //     "value": <estonian word>,
    //     "part_of_speech": <kõne kõige tõenäolisem osa>,
    //     "parts_of_speech": <võimalusel alternatiivsed kõneosad>,
    //     "en_word": <default english translations>,
    //     "en_words": <alternative english translations>
    // }
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

        // /// <summary>
        // /// Alternatives to PartOfSpeech
        // /// </summary>
        // [JsonPropertyName("parts_of_speech")]
        // public EePartOfSpeech[] PartsOfSpeech
        // {
        //     get => this.partsOfSpeech;
        //     init => this.partsOfSpeech = value ?? this.partsOfSpeech;
        // }

        /// <summary>
        /// Default translation to English
        /// </summary>
        [JsonPropertyName("en_word")]
        public required string EnWord { get; init; }

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