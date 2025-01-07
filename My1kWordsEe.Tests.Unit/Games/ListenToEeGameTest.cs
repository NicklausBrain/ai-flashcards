using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Moq;
using My1kWordsEe.Models;
using My1kWordsEe.Models.Games;
using My1kWordsEe.Services.Cqs;
using Xunit;

namespace My1kWordsEe.Tests.Models.Games
{
    public class ListenToEeGameTest
    {
        private readonly Mock<CheckEeListeningCommand> _checkEeListeningCommandMock;
        private readonly SampleSentence _sampleSentence;

        public ListenToEeGameTest()
        {
            _checkEeListeningCommandMock = new Mock<CheckEeListeningCommand>(null);
            _sampleSentence = new SampleSentence
            {
                EeWord = "Tere",
                EeSentence = "Tere tulemast",
                EnSentence = "Welcome",
                EeAudioUrl = new Uri("http://example.com/audio"),
                ImageUrl = new Uri("http://example.com/image")
            };
        }

        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            var game = new ListenToEeGame("Tere", 1, _sampleSentence, _checkEeListeningCommandMock.Object);

            Assert.Equal("Tere", game.EeWord);
            Assert.Equal(1, game.SampleIndex);
            Assert.Equal(_sampleSentence, game.SampleSentence);
            Assert.False(game.IsFinished);
            Assert.Equal(_sampleSentence.EeSentence, game.EeSentence);
            Assert.Equal(_sampleSentence.ImageUrl, game.ImageUrl);
            Assert.Equal(_sampleSentence.EeAudioUrl, game.AudioUrl);
            Assert.Equal(string.Empty, game.UserInput);
            Assert.False(game.IsCheckInProgress);
            Assert.False(game.CheckResult.HasValue);
        }

        [Fact]
        public async Task Submit_ShouldReturnSuccess_WhenUserInputMatches()
        {
            var game = new ListenToEeGame("Tere", 1, _sampleSentence, _checkEeListeningCommandMock.Object);
            game.UserInput = "Tere tulemast";

            await game.Submit();

            Assert.True(game.CheckResult.HasValue);
            Assert.True(game.CheckResult.Value.IsSuccess);
            Assert.Equal("Tere tulemast", game.CheckResult.Value.Value.EeSentence);
        }

        [Fact]
        public async Task Submit_ShouldInvokeCheckEeListeningCommand_WhenUserInputDoesNotMatch()
        {
            var game = new ListenToEeGame("Tere", 1, _sampleSentence, _checkEeListeningCommandMock.Object);
            game.UserInput = "Tere";

            _checkEeListeningCommandMock.Setup(cmd => cmd.Invoke(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(Result.Success(new EeListeningCheckResult
                {
                    EeSentence = "Tere tulemast",
                    EnSentence = "Welcome",
                    EeUserSentence = "Tere",
                    EnComment = "ok",
                    Match = 5
                }));

            await game.Submit();

            Assert.True(game.CheckResult.HasValue);
            Assert.True(game.CheckResult.Value.IsSuccess);
            Assert.Equal("Tere tulemast", game.CheckResult.Value.Value.EeSentence);
            _checkEeListeningCommandMock.Verify(cmd => cmd.Invoke("Tere tulemast", "Tere"), Times.Once);
        }

        [Fact]
        public async Task Submit_ShouldNotProceed_WhenUserInputIsNullOrWhitespace()
        {
            var game = new ListenToEeGame("Tere", 1, _sampleSentence, _checkEeListeningCommandMock.Object);
            game.UserInput = " ";

            await game.Submit();

            Assert.True(game.CheckResult.HasValue);
            Assert.True(game.CheckResult.Value.IsFailure);
            Assert.Equal("Bad input", game.CheckResult.Value.Error);
        }

        [Fact]
        public async Task Submit_ShouldReturnFailure_WhenListeningCommandFails()
        {
            var game = new ListenToEeGame("Tere", 1, _sampleSentence, _checkEeListeningCommandMock.Object);
            game.UserInput = "Pere";

            _checkEeListeningCommandMock.Setup(cmd => cmd.Invoke(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(Result.Failure<EeListeningCheckResult>("Listening failed"));

            await game.Submit();

            Assert.True(game.CheckResult.HasValue);
            Assert.True(game.CheckResult.Value.IsFailure);
            Assert.Equal("Listening failed", game.CheckResult.Value.Error);
        }

        [Fact]
        public void RandomizedWords_ShouldRandomizeWords()
        {
            var sampleSentence = new SampleSentence
            {
                EeWord = "pere",
                EeSentence = "See on minu pere.",
                EnSentence = "This is my family.",
                EeAudioUrl = new Uri("http://example.com/audio"),
                ImageUrl = new Uri("http://example.com/image")
            };
            var game = new ListenToEeGame("pere", 1, sampleSentence, _checkEeListeningCommandMock.Object);
            Assert.Equivalent(new[] { "minu", "pere", "See", "on" }, game.RandomizedWords);
            Assert.Equal(4, game.RandomizedWords.Length);
        }
    }
}