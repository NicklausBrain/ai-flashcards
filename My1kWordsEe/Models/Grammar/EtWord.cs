using System.Text.Json.Serialization;

using My1kWordsEe.Models.Grammar;

using static My1kWordsEe.Models.Conventions;

namespace My1kWordsEe.Models.Semantics
{
    /// <summary>
    /// A word in the Estonian language with respective senses and forms.
    /// </summary>
    public readonly struct EtWord
    {
        [JsonIgnore]
        public Uri BlobEndpoint { get; init; }

        public required string Value { get; init; }

        public LanguageCode Language => LanguageCode.Et;

        public required WordSense[] Senses { get; init; }

        /// <summary>
        /// Sample pronunciation of the word.
        /// </summary>
        [JsonIgnore]
        public Uri AudioUrl => new Uri(BlobEndpoint, $"/{AudioContainer}/{Value}.{AudioFormat}");

        [JsonIgnore]
        public WordSense DefaultSense => Senses[0];
    }
}