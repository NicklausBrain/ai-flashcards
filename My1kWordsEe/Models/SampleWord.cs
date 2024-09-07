using System.Text.Json.Serialization;

namespace My1kWordsEe.Models
{
    public record SampleWord
    {
        /// <summary>
        /// Estonian word
        /// </summary>
        public string EeWord { get; init; }

        /// <summary>
        /// Default translation to English
        /// </summary>
        public string EnWord { get; init; }

        /// <summary>
        /// Alternatives to EnWord
        /// </summary>
        public string EnWords { get; init; }

        public string EnExplanation { get; init; }

        public Uri EeAudioUrl { get; init; }
    }
}
