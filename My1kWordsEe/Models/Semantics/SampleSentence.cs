
using System.Text.Json.Serialization;

namespace My1kWordsEe.Models.Grammar
{
    /// <summary>
    /// A sample sentence illustrating the use of a given Estonian word form.
    /// </summary>
    public record SampleSentence
    {
        // word?

        public required IDictionary<LanguageCode, string> Sentence { get; init; } = new Dictionary<LanguageCode, string>();
        public Uri? EeAudioUrl { get; init; }
        public Uri? ImageUrl { get; init; }
    }
}