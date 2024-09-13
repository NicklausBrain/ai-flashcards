namespace My1kWordsEe.Models
{
    public record SampleWord
    {

        private readonly string eeWord = "";
        private readonly string[] enWords = [];

        private readonly SampleSentence[] samples = [];


        /// <summary>
        /// Estonian word
        /// </summary>
        public string EeWord
        {
            get => this.eeWord;
            init => this.eeWord = value?.ToLower() ?? "";
        }

        /// <summary>
        /// Default translation to English
        /// </summary>
        public string EnWord { get; init; }

        /// <summary>
        /// Alternatives to EnWord
        /// </summary>
        public string[] EnWords
        {
            get => this.enWords;
            init => this.enWords = value ?? this.enWords;
        }

        public string EnExplanation { get; init; }

        public Uri EeAudioUrl { get; init; }

        public SampleSentence[] Samples
        {
            get => this.samples;
            init => this.samples = value ?? this.samples;
        }
    }
}
