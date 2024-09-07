using System.Text.Json.Serialization;

namespace My1kWordsEe.Models
{
    public class SampleSentence
    {
        [JsonPropertyName("ee_word")]
        public string EeWord { get; set; }

        [JsonPropertyName("ee_sentence")]
        public string EeSentence { get; set; }

        [JsonPropertyName("en_sentence")]
        public string EnSentence { get; set; }

        [JsonPropertyName("en_audio_url")]
        public Uri EeAudioUrl { get; set; }

        [JsonPropertyName("image_url")]
        public Uri ImageUrl { get; set; }
    }
}

/*
 /wwwroot
    /content
        /index.json - tbd
        /samples
            /ma
                /xxxx.json
                /yyyyy.json
            /ta
            /sind
        /images
    /css
    /js
    /lib
    /_framework
    /_content
 */