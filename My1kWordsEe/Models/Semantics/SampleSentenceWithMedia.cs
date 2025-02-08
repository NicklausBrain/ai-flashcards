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
        public Uri AudioUrl => new Uri($"/{AudioContainer}/{Id}.{AudioFormat}", UriKind.Relative);

        [JsonIgnore]
        public Uri ImageUrl => new Uri($"/{ImageContainer}/{Id}.{ImageFormat}", UriKind.Relative);

        [JsonIgnore]
        public Uri ImagePromptUrl => new Uri($"/{ImageContainer}/{Id}.{TextFormat}", UriKind.Relative);
    }
}