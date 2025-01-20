using My1kWordsEe.Models.Grammar;

namespace My1kWordsEe.Models.Semantics
{
    /// <summary>
    /// A sample sentence illustrating the use of a given Estonian word form.
    /// </summary>
    public record SampleSentence
    {
        public required IDictionary<LanguageCode, string> Sentence { get; init; } = new Dictionary<LanguageCode, string>();
        // todo: use relative url
        public Uri? EeAudioUrl { get; init; }
        // todo: use relative url
        public Uri? ImageUrl { get; init; }
    }
}