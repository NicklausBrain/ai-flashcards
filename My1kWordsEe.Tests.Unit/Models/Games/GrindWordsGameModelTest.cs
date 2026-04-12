using System.Collections.Generic;
using Xunit;
using My1kWordsEe.Models.Games;

namespace My1kWordsEe.Tests.Unit.Models.Games
{
    public class GrindWordsGameModelTest
    {
        [Fact]
        public void GrindWordsGame_InitialState_IsCorrect()
        {
            var game = new GrindWordsGame();
            Assert.NotNull(game.Items);
            Assert.Equal(0, game.CurrentIndex);
            Assert.Equal(0, game.Score);
            Assert.True(game.Items.Count == 0);
            Assert.True(game.IsCompleted);
        }

        [Fact]
        public void GrindWordsGame_IsCompleted_True_WhenIndexAtCount()
        {
            var game = new GrindWordsGame
            {
                Items = new List<GrindWordItem> { new GrindWordItem(), new GrindWordItem() },
                CurrentIndex = 2
            };
            Assert.True(game.IsCompleted);
        }

        [Fact]
        public void GrindWordItem_Defaults_AreCorrect()
        {
            var item = new GrindWordItem();
            Assert.Equal(string.Empty, item.OriginalWord);
            Assert.Equal(string.Empty, item.SentenceWithBlank);
            Assert.Equal(string.Empty, item.CorrectAnswer);
            Assert.Null(item.UserInput);
            Assert.Null(item.IsCorrect);
        }
    }
}
