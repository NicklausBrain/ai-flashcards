namespace My1kWordsEe.Models
{
    /// <summary>
    /// Sample sentence illustrating the use of a give Estonian word
    /// </summary>
    [Obsolete]
    public record SampleSentence
    {
        public static readonly SampleSentence Empty = new SampleSentence
        {
            EeWord = string.Empty,
            EeSentence = string.Empty,
            EnSentence = string.Empty,
            EeAudioUrl = new Uri("about:blank"),
            ImageUrl = new Uri("about:blank")
        };

        /// <summary>
        /// Target Estonian word
        /// </summary>
        public required string EeWord { get; init; }

        /// <summary>
        /// Illustrative sentence in Estonian
        /// </summary>
        public required string EeSentence { get; init; }

        /// <summary>
        /// Translation of the illustrative sentence in English
        /// </summary>
        public required string EnSentence { get; init; }

        /// <summary>
        /// Sentence spoken in Estonian
        /// </summary>
        public required Uri EeAudioUrl { get; init; }

        /// <summary>
        /// Image associated with the sentence
        /// </summary>
        public required Uri ImageUrl { get; init; }
    }
}