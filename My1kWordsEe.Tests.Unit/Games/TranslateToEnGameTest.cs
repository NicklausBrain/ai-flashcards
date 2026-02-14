using CSharpFunctionalExtensions;

using Moq;

using My1kWordsEe.Models;
using My1kWordsEe.Models.Games;
using My1kWordsEe.Models.Semantics;
using My1kWordsEe.Services.Cqs;
using My1kWordsEe.Services.Scoped;

namespace My1kWordsEe.Tests.Models.Games
{
    public class TranslateToEnGameTest
    {
        private readonly Mock<CheckEnTranslationCommand> _checkEnTranslationCommandMock;
        private readonly Mock<FavoritesStateContainer> _favoritesStateContainer;
        private readonly SampleSentenceWithMedia _sampleSentence;

        public TranslateToEnGameTest()
        {
            _checkEnTranslationCommandMock = new Mock<CheckEnTranslationCommand>(null!, null!);
            _favoritesStateContainer = new Mock<FavoritesStateContainer>(null!, null!, null!, null!, null!, null!);
            _sampleSentence = new SampleSentenceWithMedia
            {
                Id = Guid.NewGuid(),
                Sentence = new TranslatedString
                {
                    Et = "Tere",
                    En = "Hello",
                },
            };
        }

        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            var game = new TranslateToEnGame("Tere", 1, _sampleSentence, _checkEnTranslationCommandMock.Object, _favoritesStateContainer.Object);

            Assert.Equal("Tere", game.EtWord);
            Assert.Equal(1, game.SampleIndex);
            Assert.Equal(_sampleSentence, game.SampleSentence);
            Assert.False(game.IsFinished);
            Assert.Equal(_sampleSentence.Sentence.Et, game.EtSentence);
            Assert.Equal(_sampleSentence.ImageUrl, game.ImageUrl);
            Assert.Equal(_sampleSentence.AudioUrl, game.AudioUrl);
            Assert.Equal(string.Empty, game.UserTranslation);
            Assert.False(game.IsCheckInProgress);
            Assert.False(game.CheckResult.HasValue);
        }

        [Fact]
        public async Task Submit_ShouldReturnSuccess_WhenUserTranslationMatches()
        {
            var game = new TranslateToEnGame("Tere", 1, _sampleSentence, _checkEnTranslationCommandMock.Object, _favoritesStateContainer.Object);
            game.UserTranslation = "Hello";

            await game.Submit();

            Assert.True(game.CheckResult.HasValue);
            Assert.True(game.CheckResult.Value.IsSuccess);
            Assert.Equal("Hello", game.CheckResult.Value.Value.EnExpectedSentence);
        }

        [Fact]
        public async Task Submit_ShouldInvokeCheckEnTranslationCommand_WhenUserTranslationDoesNotMatch()
        {
            var game = new TranslateToEnGame("Tere", 1, _sampleSentence, _checkEnTranslationCommandMock.Object, _favoritesStateContainer.Object);
            game.UserTranslation = "Hi";

            _checkEnTranslationCommandMock.Setup(cmd => cmd.Invoke(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(Result.Success(new EnTranslationCheckResult
                {
                    EeSentence = "Tere",
                    EnExpectedSentence = "Hi",
                    EnUserSentence = "Hi",
                    EnComment = "ok",
                    Match = 5
                }));

            await game.Submit();

            Assert.True(game.CheckResult.HasValue);
            Assert.True(game.CheckResult.Value.IsSuccess);
            Assert.Equal("Hi", game.CheckResult.Value.Value.EnExpectedSentence);
            _checkEnTranslationCommandMock.Verify(cmd => cmd.Invoke("Tere", "Hi"), Times.Once);
        }

        [Fact]
        public async Task Submit_ShouldNotProceed_WhenUserTranslationIsNullOrWhitespace()
        {
            var game = new TranslateToEnGame("Tere", 1, _sampleSentence, _checkEnTranslationCommandMock.Object, _favoritesStateContainer.Object);
            game.UserTranslation = " ";

            await game.Submit();

            Assert.False(game.CheckResult.HasValue);
        }

        [Fact]
        public async Task Submit_ShouldReturnFailure_WhenTranslationCommandFails()
        {
            var game = new TranslateToEnGame("Tere", 1, _sampleSentence, _checkEnTranslationCommandMock.Object, _favoritesStateContainer.Object);
            game.UserTranslation = "Hi";

            _checkEnTranslationCommandMock.Setup(cmd => cmd.Invoke(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(Result.Failure<EnTranslationCheckResult>("Translation failed"));

            await game.Submit();

            Assert.True(game.CheckResult.HasValue);
            Assert.True(game.CheckResult.Value.IsFailure);
            Assert.Equal("Translation failed", game.CheckResult.Value.Error);
        }
    }
}