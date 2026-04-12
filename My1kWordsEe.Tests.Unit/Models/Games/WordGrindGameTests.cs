using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using Moq;

using My1kWordsEe.Models;
using My1kWordsEe.Models.Games;
using My1kWordsEe.Models.Games.Generation;
using My1kWordsEe.Services;
using My1kWordsEe.Services.Db;

using Xunit;

namespace My1kWordsEe.Tests.Unit.Models.Games
{
    public class WordGrindGameTests
    {
        [Fact]
        public void WordGrindGame_Submit_ShouldCalculateCorrectScore()
        {
            // Arrange
            var data = new WordGrindGameData
            {
                WordSetId = "set1",
                Items = new List<WordGrindItemData>
                {
                    new WordGrindItemData { Word = "tere", Sentence = new TranslatedString { Et = "Tere hommikust", En = "Good morning" } },
                    new WordGrindItemData { Word = "auto", Sentence = new TranslatedString { Et = "See on punane auto", En = "This is a red car" } }
                }
            };
            var game = new WordGrindGame(data);

            // Act
            game.Items[0].UserAnswer = "tere";
            game.Items[1].UserAnswer = "ratas"; // Incorrect
            game.Submit();

            // Assert
            Assert.True(game.IsFinished);
            Assert.Equal(1, game.Score);
            Assert.True(game.Items[0].IsCorrect);
            Assert.False(game.Items[1].IsCorrect);
        }

        [Fact]
        public void WordGrindGame_GiveUp_ShouldFinishGame()
        {
            // Arrange
            var data = new WordGrindGameData
            {
                WordSetId = "set1",
                Items = new List<WordGrindItemData> { new WordGrindItemData { Word = "tere", Sentence = new TranslatedString { Et = "Tere", En = "Hello" } } }
            };
            var game = new WordGrindGame(data);

            // Act
            game.GiveUp();

            // Assert
            Assert.True(game.IsFinished);
        }

        [Fact]
        public async Task WordGrindGameFactory_Generate_ShouldReturnCachedGame_WhenExists()
        {
            // Arrange
            var openAiMock = new Mock<OpenAiClient>(null!, null!);
            var gameStorageMock = new Mock<GameStorageClient>(null!);
            var factory = new WordGrindGameFactory(openAiMock.Object, gameStorageMock.Object);
            var wordSet = new WordSet { Id = "set1", UserId = "u1", Name = "N", Words = new List<string> { "w" } };
            var cachedData = new WordGrindGameData { WordSetId = "set1", Items = new List<WordGrindItemData>() };

            gameStorageMock.Setup(x => x.GetGameData<WordGrindGameData>($"WordGrindGame-set1"))
                .ReturnsAsync(Result.Success(Maybe<WordGrindGameData>.From(cachedData)));

            // Act
            var result = await factory.Generate(wordSet);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("set1", result.Value.WordSetId);
            openAiMock.Verify(x => x.CompleteJsonSchemaAsync<WordGrindGameData>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<JsonSchemaRecord>(), It.IsAny<float?>()), Times.Never);
        }

        [Fact]
        public async Task WordGrindGameFactory_Generate_ShouldCallAI_WhenNoCache()
        {
            // Arrange
            var openAiMock = new Mock<OpenAiClient>(null!, null!);
            var gameStorageMock = new Mock<GameStorageClient>(null!);
            var factory = new WordGrindGameFactory(openAiMock.Object, gameStorageMock.Object);
            var wordSet = new WordSet { Id = "set1", UserId = "u1", Name = "N", Words = new List<string> { "tere" } };
            var aiData = new WordGrindGameData { WordSetId = "will-be-overridden", Items = new List<WordGrindItemData> { new WordGrindItemData { Word = "tere", Sentence = new TranslatedString { Et = "Tere", En = "Hello" } } } };

            gameStorageMock.Setup(x => x.GetGameData<WordGrindGameData>($"WordGrindGame-set1"))
                .ReturnsAsync(Result.Success(Maybe<WordGrindGameData>.None));

            openAiMock.Setup(x => x.CompleteJsonSchemaAsync<WordGrindGameData>(It.IsAny<string>(), "tere", It.IsAny<JsonSchemaRecord>(), It.IsAny<float?>()))
                .ReturnsAsync(Result.Success(aiData));

            // Act
            var result = await factory.Generate(wordSet);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("set1", result.Value.WordSetId);
            gameStorageMock.Verify(x => x.SaveGameData($"WordGrindGame-set1", It.Is<WordGrindGameData>(d => d.WordSetId == "set1")), Times.Once);
        }
    }
}
