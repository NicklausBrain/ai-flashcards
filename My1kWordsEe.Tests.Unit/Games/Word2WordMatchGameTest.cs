using My1kWordsEe.Models.Games;

namespace My1kWordsEe.Tests.Models.Games
{
    public class Word2WordMatchGameTest
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            var pairs = new List<Word2WordMatchGame.Pair>
            {
                new Word2WordMatchGame.Pair { EeWord = "Tere", EnWord = "Hello" },
                new Word2WordMatchGame.Pair { EeWord = "Kass", EnWord = "Cat" }
            };
            var eeWords = pairs.ToDictionary(p => p.EeWord);
            var enWords = pairs.ToDictionary(p => p.EnWord);

            var game = new Word2WordMatchGame(pairs, eeWords, enWords);

            Assert.Equal(eeWords, game.EeWords);
            Assert.Equal(enWords, game.EnWords);
            Assert.Equivalent(eeWords.Keys, game.EeWords2Match);
            Assert.Equivalent(enWords.Keys, game.EnWords2Match);
            Assert.True(game.IsReady);
            Assert.False(game.IsFinished);
        }

        [Fact]
        public void TryMatch_ShouldReturnTrue_WhenWordsMatch()
        {
            var pair = new Word2WordMatchGame.Pair { EeWord = "Tere", EnWord = "Hello" };
            var pairs = new List<Word2WordMatchGame.Pair> { pair };
            var eeWords = pairs.ToDictionary(p => p.EeWord);
            var enWords = pairs.ToDictionary(p => p.EnWord);

            var game = new Word2WordMatchGame(pairs, eeWords, enWords);

            var result = game.TryMatch("Tere", "Hello");

            Assert.True(result);
            Assert.Contains(pair, game.Matches);
            Assert.DoesNotContain("Tere", game.EeWords2Match);
            Assert.DoesNotContain("Hello", game.EnWords2Match);
            Assert.True(pair.IsMatched);
        }

        [Fact]
        public void TryMatch_ShouldReturnFalse_WhenWordsDoNotMatch()
        {
            var pair = new Word2WordMatchGame.Pair { EeWord = "Tere", EnWord = "Hello" };
            var pairs = new List<Word2WordMatchGame.Pair> { pair };
            var eeWords = pairs.ToDictionary(p => p.EeWord);
            var enWords = pairs.ToDictionary(p => p.EnWord);

            var game = new Word2WordMatchGame(pairs, eeWords, enWords);

            var result = game.TryMatch("Tere", "Cat");

            Assert.False(result);
            Assert.DoesNotContain(pair, game.Matches);
            Assert.Contains("Tere", game.EeWords2Match);
            Assert.DoesNotContain("Cat", game.EnWords2Match);
            Assert.False(pair.IsMatched);
        }

        [Fact]
        public void IsReady_ShouldReturnTrue_WhenPairsExist()
        {
            var pairs = new List<Word2WordMatchGame.Pair>
            {
                new Word2WordMatchGame.Pair { EeWord = "Tere", EnWord = "Hello" }
            };
            var eeWords = pairs.ToDictionary(p => p.EeWord);
            var enWords = pairs.ToDictionary(p => p.EnWord);

            var game = new Word2WordMatchGame(pairs, eeWords, enWords);

            Assert.True(game.IsReady);
        }

        [Fact]
        public void IsFinished_ShouldReturnTrue_WhenAllPairsAreMatched()
        {
            var pairs = new List<Word2WordMatchGame.Pair>
            {
                new Word2WordMatchGame.Pair { EeWord = "Tere", EnWord = "Hello", IsMatched = true }
            };
            var eeWords = pairs.ToDictionary(p => p.EeWord);
            var enWords = pairs.ToDictionary(p => p.EnWord);

            var game = new Word2WordMatchGame(pairs, eeWords, enWords);

            Assert.True(game.IsFinished);
        }

    }
}