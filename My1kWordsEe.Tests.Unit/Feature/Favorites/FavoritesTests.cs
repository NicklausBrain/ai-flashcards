using FavoriteSet = My1kWordsEe.Feature.Favorites.Favorites;

namespace My1kWordsEe.Tests.Unit.Feature.Favorites
{
    public class FavoritesTests
    {
        [Fact]
        public void IsKnown_WithEtWordAtMaxScore_ReturnsTrue()
        {
            var word = new EtWord
            {
                Value = "Tere",
                Senses =
                [
                    new WordSense
                    {
                        BaseForm = "tere",
                        Word = new TranslatedString { Et = "tere", En = "hello" },
                        Definition = new TranslatedString { Et = "tervitus", En = "greeting" },
                        PartOfSpeech = new TranslatedString { Et = "hüüdsõna", En = "interjection" },
                    }
                ]
            };

            var favorites = new FavoriteSet
            {
                Stats = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
                {
                    ["tere"] = FavoriteSet.MaxWordScore
                }
            };

            var result = favorites.IsKnown(word);

            Assert.True(result);
        }

        [Fact]
        public void IsFavorite_WithStoredWordAndSampleSentence_ReturnsTrue()
        {
            var word = new EtWord
            {
                Value = "Tere",
                Senses =
                [
                    new WordSense
                    {
                        BaseForm = "tere",
                        Word = new TranslatedString { Et = "tere", En = "hello" },
                        Definition = new TranslatedString { Et = "tervitus", En = "greeting" },
                        PartOfSpeech = new TranslatedString { Et = "hüüdsõna", En = "interjection" },
                    }
                ]
            };

            var sample = new SampleSentenceWithMedia
            {
                Id = Guid.NewGuid(),
                Sentence = new TranslatedString { Et = "Tere maailm", En = "Hello world" }
            };

            var favorites = new FavoriteSet
            {
                Words = new Dictionary<string, EtWord>
                {
                    ["tere"] = word
                },
                Sentences = new Dictionary<string, SampleSentenceWithMedia>
                {
                    ["tere maailm"] = sample
                }
            };

            var isFavoriteWord = favorites.IsFavorite(word);
            var isFavoriteSentence = favorites.IsFavorite(sample);

            Assert.True(isFavoriteWord);
            Assert.True(isFavoriteSentence);
        }
    }
}
