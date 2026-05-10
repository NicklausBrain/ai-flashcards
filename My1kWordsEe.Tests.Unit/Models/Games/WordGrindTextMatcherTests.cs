using My1kWordsEe.Models.Games;

namespace My1kWordsEe.Tests.Unit.Models.Games
{
    public class WordGrindTextMatcherTests
    {
        [Theory]
        [InlineData("See kulu on suur.", "kulu", true)]
        [InlineData("Ma hakkan raha kulutama.", "kulu", false)]
        [InlineData("Ma hakkan raha kulutama.", "kulutama", true)]
        [InlineData("Ma ei taha palju raha kulutada.", "kulutama", false)]
        [InlineData("Tere hommikust!", "tere", true)]
        [InlineData("TERE hommikust!", "tere", true)]
        public void ContainsExactWord_ShouldMatchOnlyWholeWords(string sentence, string word, bool expected)
        {
            Assert.Equal(expected, WordGrindTextMatcher.ContainsExactWord(sentence, word));
        }

        [Theory]
        [InlineData("See kulu on suur.", "kulu", true)]
        [InlineData("Kulu on suur.", "kulu", true)]
        [InlineData("Mis on kulu?", "kulu", true)]
        [InlineData("(kulu)", "kulu", true)]
        [InlineData("\"kulu\"", "kulu", true)]
        public void ContainsExactWord_ShouldMatchAdjacentToPunctuation(string sentence, string word, bool expected)
        {
            Assert.Equal(expected, WordGrindTextMatcher.ContainsExactWord(sentence, word));
        }

        [Theory]
        [InlineData("kuluma on verb.", "kulu", false)]
        [InlineData("See on kulude arvestus.", "kulu", false)]
        [InlineData("Ülekulutamine on halb.", "kulu", false)]
        public void ContainsExactWord_ShouldNotMatchInsideLargerWords(string sentence, string word, bool expected)
        {
            Assert.Equal(expected, WordGrindTextMatcher.ContainsExactWord(sentence, word));
        }

        [Fact]
        public void ReplaceExactWord_ShouldReplaceSameFormOnly()
        {
            var result = WordGrindTextMatcher.ReplaceExactWord("See kulu on suur.", "kulu", "____");
            Assert.Equal("See ____ on suur.", result);
        }

        [Fact]
        public void ReplaceExactWord_ShouldNotReplaceSubstring()
        {
            var result = WordGrindTextMatcher.ReplaceExactWord("Ma hakkan raha kulutama.", "kulu", "____");
            Assert.Equal("Ma hakkan raha kulutama.", result);
        }

        [Fact]
        public void ReplaceExactWord_ShouldWorkWithPunctuation()
        {
            var result = WordGrindTextMatcher.ReplaceExactWord("Mis on kulu?", "kulu", "____");
            Assert.Equal("Mis on ____?", result);
        }

        [Fact]
        public void ReplaceExactWord_ShouldBeCaseInsensitive()
        {
            var result = WordGrindTextMatcher.ReplaceExactWord("Kulu on suur.", "kulu", "____");
            Assert.Equal("____ on suur.", result);
        }

        [Theory]
        [InlineData("", "kulu", false)]
        [InlineData("See kulu.", "", false)]
        [InlineData(null, "kulu", false)]
        [InlineData("See kulu.", null, false)]
        public void ContainsExactWord_ShouldHandleNullAndEmpty(string? sentence, string? word, bool expected)
        {
            Assert.Equal(expected, WordGrindTextMatcher.ContainsExactWord(sentence!, word!));
        }

        [Theory]
        [InlineData("", "kulu", "____", "")]
        [InlineData("See kulu.", "", "____", "See kulu.")]
        public void ReplaceExactWord_ShouldHandleEmptyInputs(string sentence, string word, string replacement, string expected)
        {
            Assert.Equal(expected, WordGrindTextMatcher.ReplaceExactWord(sentence, word, replacement));
        }
    }
}
