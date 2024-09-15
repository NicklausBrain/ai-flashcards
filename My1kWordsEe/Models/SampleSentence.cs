namespace My1kWordsEe.Models
{
    /// <summary>
    /// Sample sentence illustrating the use of a give Estonian word
    /// </summary>
    public record SampleSentence
    {
        /// <summary>
        /// Target Estonian word
        /// </summary>
        public string EeWord { get; init; }

        /// <summary>
        /// Illustrative sentence in Estonian
        /// </summary>
        public string EeSentence { get; init; }

        /// <summary>
        /// Translation of the illustrative sentence in English
        /// </summary>
        public string EnSentence { get; init; }

        /// <summary>
        /// Sentence spoken in Estonian
        /// </summary>
        public Uri EeAudioUrl { get; init; }

        /// <summary>
        /// Image associated with the sentence
        /// </summary>
        public Uri ImageUrl { get; init; }
    }
}
