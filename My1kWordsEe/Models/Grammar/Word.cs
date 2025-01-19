using System.Text.Json.Serialization;

using My1kWordsEe.Models.Grammar;

namespace My1kWordsEe.Models.Semantics
{
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
}