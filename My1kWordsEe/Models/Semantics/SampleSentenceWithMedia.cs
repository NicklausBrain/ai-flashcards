using System.Text.Json.Serialization;

using static My1kWordsEe.Models.Conventions;

namespace My1kWordsEe.Models.Semantics
{
    /// <summary>
    /// A sample sentence illustrating the use of a given Estonian word form.
    /// </summary>
    public readonly struct SampleSentenceWithMedia : ISampleEtSentence
    {
        public required Guid Id { get; init; }

        public required TranslatedString Sentence { get; init; }

        [JsonIgnore]
        public string AudioFileName => $"{Id}.{AudioFormat}";

        [JsonIgnore]
        public Uri AudioUrl => new Uri($"/{AudioContainer}/{AudioFileName}", UriKind.Relative);

        [JsonIgnore]
        public string ImageFileName => $"{Id}.{ImageFormat}";

        [JsonIgnore]
        public Uri ImageUrl => new Uri($"/{ImageContainer}/{ImageFileName}", UriKind.Relative);

        [JsonIgnore]
        public string ImagePromptFileName => $"{Id}.{TextFormat}";

        [JsonIgnore]
        public Uri ImagePromptUrl => new Uri($"/{ImageContainer}/{ImagePromptFileName}", UriKind.Relative);
    }
}