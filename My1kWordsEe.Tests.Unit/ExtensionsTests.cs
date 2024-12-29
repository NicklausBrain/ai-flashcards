using My1kWordsEe.Models;

namespace My1kWordsEe.Tests.Models
{
    public class ExtensionsTests
    {
        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData(" ", false)]
        [InlineData("<!", false)]
        [InlineData("a", true)]
        [InlineData("abc", true)]
        [InlineData("abc123", true)]
        [InlineData("123", true)]
        [InlineData("abc-def", false)]
        [InlineData("äöü", true)]
        public void ValidateWord_ShouldReturnExpectedResult(string word, bool expected)
        {
            // Act
            var result = word.ValidateWord();

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
