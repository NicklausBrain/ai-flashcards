using System.Text.Json.Serialization;

using My1kWordsEe.Models.Grammar;

namespace My1kWordsEe.Models.Semantics
{
    /// <summary>
    /// A word in the Estonian language with respective senses and forms.
    /// </summary>
    public readonly struct EtWord
    {
        public required string Value { get; init; }

        public LanguageCode Language => LanguageCode.Et;

        public required WordSense[] Senses { get; init; }

        /// <summary>
        /// Sample pronunciation of the word.
        /// </summary>
        public Uri? AudioUrl { get; init; } // todo: use relative url

        [JsonIgnore]
        public WordSense DefaultSense => Senses[0];
    }
}