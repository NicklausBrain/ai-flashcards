using System.Text.Json.Serialization;

using static My1kWordsEe.Models.Conventions;

namespace My1kWordsEe.Models.Semantics
{
    /// <summary>
    /// A sample sentence illustrating the use of a given Estonian word form.
    /// </summary>
    public readonly struct SampleSentenceWithMedia : ISampleEtSentence
    {
        [JsonIgnore]
        public Uri BlobEndpoint { get; init; }

        public required Guid Id { get; init; }

        public required TranslatedString Sentence { get; init; }

        [JsonIgnore]
        public Uri AudioUrl => new Uri(BlobEndpoint, $"/{AudioContainer}/{Id}.{AudioFormat}");

        [JsonIgnore]
        public Uri ImageUrl => new Uri(BlobEndpoint, $"/{ImageContainer}/{Id}.{ImageFormat}");

        [JsonIgnore]
        public Uri ImagePromptUrl => new Uri(BlobEndpoint, $"/{ImageContainer}/{Id}.{TextFormat}");
    }
}