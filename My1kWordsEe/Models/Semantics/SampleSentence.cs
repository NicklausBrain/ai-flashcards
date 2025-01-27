using My1kWordsEe.Models.Grammar;

namespace My1kWordsEe.Models.Semantics
{
    /// <summary>
    /// A sample sentence illustrating the use of a given Estonian word form.
    /// </summary>
    public record SampleSentence
    {
        public required TranslatedString Sentence { get; init; } = new TranslatedString();
        // todo: use relative url
        public Uri? AudioUrl { get; init; }
        // todo: use relative url
        public Uri? ImageUrl { get; init; }
    }
}