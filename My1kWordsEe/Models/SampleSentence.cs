using System.Text.Json.Serialization;

namespace My1kWordsEe.Models
{
    public class SampleSentence
    {
        public string EeWord { get; set; }

        public string EeSentence { get; set; }

        public string EnSentence { get; set; }

        public Uri EeAudioUrl { get; set; }

        public Uri ImageUrl { get; set; }
    }
}
