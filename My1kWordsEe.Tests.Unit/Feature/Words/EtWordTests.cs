namespace My1kWordsEe.Tests.Unit.Feature.Words
{
    public class EtWordTests
    {
        [Fact]
        public void AudioUrl_ReturnsExpectedRelativePath()
        {
            var word = CreateWord("tere", "hello");

            var audioUrl = word.AudioUrl;

            Assert.Equal("/audio/tere.wav", audioUrl.OriginalString);
        }

        [Fact]
        public void DefaultSense_ReturnsFirstSense()
        {
            var word = new EtWord
            {
                Value = "maa",
                Senses =
                [
                    new WordSense
                    {
                        BaseForm = "maa",
                        Word = new TranslatedString { Et = "maa", En = "land" },
                        Definition = new TranslatedString { Et = "maapind", En = "ground" },
                        PartOfSpeech = new TranslatedString { Et = "nimisõna", En = "noun" }
                    },
                    new WordSense
                    {
                        BaseForm = "maa",
                        Word = new TranslatedString { Et = "maa", En = "country" },
                        Definition = new TranslatedString { Et = "riik", En = "state" },
                        PartOfSpeech = new TranslatedString { Et = "nimisõna", En = "noun" }
                    }
                ]
            };

            var defaultSense = word.DefaultSense;

            Assert.Equal("land", defaultSense.Word.En);
        }

        private static EtWord CreateWord(string et, string en) =>
            new()
            {
                Value = et,
                Senses =
                [
                    new WordSense
                    {
                        BaseForm = et,
                        Word = new TranslatedString { Et = et, En = en },
                        Definition = new TranslatedString { Et = "def", En = "def" },
                        PartOfSpeech = new TranslatedString { Et = "nimisõna", En = "noun" }
                    }
                ]
            };
    }
}
