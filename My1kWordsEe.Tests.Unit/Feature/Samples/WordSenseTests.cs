namespace My1kWordsEe.Tests.Unit.Feature.Samples
{
    public class WordSenseTests
    {
        [Theory]
        [InlineData("nimisõna", "noun")]
        [InlineData("Nimisõna", "Noun")]                          // case-insensitive
        [InlineData("substantiiv", "noun")]
        [InlineData("nimisõna", "subst.")]
        [InlineData("tegusõna (ma-tegevusnimi)", "verb (ma-infinitive/gerund)")] // sõitmine-style
        [InlineData("tegevusnimi", "verbal noun")]
        [InlineData("nimisõna", "verbal noun")]
        public void IsNoun_ReturnsTrue_WhenPartOfSpeechIsNoun(string et, string en)
        {
            var sense = CreateSense(et, en);

            Assert.True(sense.IsNoun);
        }

        [Theory]
        [InlineData("tegusõna", "verb")]
        [InlineData("omadussõna", "adjective")]
        [InlineData("määrsõna", "adverb")]
        [InlineData("sidesõna", "conjunction")]
        public void IsNoun_ReturnsFalse_WhenPartOfSpeechIsNotNoun(string et, string en)
        {
            var sense = CreateSense(et, en);

            Assert.False(sense.IsNoun);
        }

        private static WordSense CreateSense(string posEt, string posEn) =>
            new()
            {
                Word = new TranslatedString { Et = "sõna", En = "word" },
                Definition = new TranslatedString { Et = "definitsioon", En = "definition" },
                BaseForm = "sõna",
                PartOfSpeech = new TranslatedString { Et = posEt, En = posEn }
            };
    }
}
