namespace My1kWordsEe.Models.Semantics
{
    /// <summary>
    /// A sample sentence illustrating the use of a given Estonian word form.
    /// </summary>
    public struct SampleEtSentence : ISampleEtSentence
    {
        public required TranslatedString Sentence { get; init; }
    }
}