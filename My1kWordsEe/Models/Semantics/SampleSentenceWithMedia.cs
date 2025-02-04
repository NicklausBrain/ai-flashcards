namespace My1kWordsEe.Models.Semantics
{
    /// <summary>
    /// A sample sentence illustrating the use of a given Estonian word form.
    /// </summary>
    public readonly struct SampleSentenceWithMedia : ISampleEtSentence
    {
        public required TranslatedString Sentence { get; init; }

        // todo: use relative url
        public required Uri AudioUrl { get; init; }
        // todo: use relative url
        public required Uri ImageUrl { get; init; }
    }
}