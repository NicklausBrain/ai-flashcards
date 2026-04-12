using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using My1kWordsEe.Models.Games;
using My1kWordsEe.Models.Games.Generation;
using My1kWordsEe.Services;
using My1kWordsEe.Services.Db;
using CSharpFunctionalExtensions;

namespace My1kWordsEe.Tests.Unit.Models.Games.Generation
{
    public class GrindWordsGameFactoryTest
    {
        private readonly Mock<OpenAiClient> _openAiClientMock;
        private readonly Mock<GameStorageClient> _gameStorageClientMock;
        private readonly GrindWordsGameFactory _factory;

        public GrindWordsGameFactoryTest()
        {
            _openAiClientMock = new Mock<OpenAiClient>(null!, null!);
            _gameStorageClientMock = new Mock<GameStorageClient>(null!);
            _factory = new GrindWordsGameFactory(_openAiClientMock.Object, _gameStorageClientMock.Object);
        }

        [Fact]
        public async Task Generate_ReturnsFailure_WhenNoWordsProvided()
        {
            var result = await _factory.Generate(new List<string>());
            Assert.True(result.IsFailure);
            Assert.Equal("No words provided", result.Error);
        }

        [Fact]
        public async Task Generate_ReturnsCachedGame_WhenCacheHit()
        {
            var words = new List<string> { "kass" };
            var gameId = "GrindWordsGame-kass";
            var cachedGame = new GrindWordsGame { Items = new List<GrindWordItem> { new GrindWordItem { OriginalWord = "kass" } } };
            _gameStorageClientMock.Setup(x => x.GetGameData<GrindWordsGame>(gameId))
                .ReturnsAsync(Result.Success(Maybe<GrindWordsGame>.From(cachedGame)));

            var result = await _factory.Generate(words);
            Assert.True(result.IsSuccess);
            Assert.Equal("kass", result.Value.Items[0].OriginalWord);
        }

        [Fact]
        public async Task Generate_CallsOpenAiAndSavesGame_WhenCacheMiss()
        {
            var words = new List<string> { "kass" };
            var gameId = "GrindWordsGame-kass";
            _gameStorageClientMock.Setup(x => x.GetGameData<GrindWordsGame>(gameId))
                .ReturnsAsync(Result.Success(Maybe<GrindWordsGame>.None));
            _openAiClientMock.Setup(x => x.CompleteAsync(It.IsAny<string>(), ""))
                .ReturnsAsync(Result.Success("{\"sentence\":\"Kass istub toolil.\",\"translation\":\"The cat sits on the chair.\"}"));
            _gameStorageClientMock.Setup(x => x.SaveGameData(gameId, It.IsAny<GrindWordsGame>()))
                .ReturnsAsync(Result.Success(new Uri("https://dummy")));

            var result = await _factory.Generate(words);
            Assert.True(result.IsSuccess);
            Assert.Single(result.Value.Items);
            Assert.Equal("kass", result.Value.Items[0].OriginalWord);
            Assert.Contains("____", result.Value.Items[0].SentenceWithBlank);
        }

        [Fact]
        public async Task Generate_ReturnsFailure_WhenAiFails()
        {
            var words = new List<string> { "kass" };
            var gameId = "GrindWordsGame-kass";
            _gameStorageClientMock.Setup(x => x.GetGameData<GrindWordsGame>(gameId))
                .ReturnsAsync(Result.Success(Maybe<GrindWordsGame>.None));
            _openAiClientMock.Setup(x => x.CompleteAsync(It.IsAny<string>(), ""))
                .ReturnsAsync(Result.Failure<string>("AI error"));

            var result = await _factory.Generate(words);
            Assert.True(result.IsFailure);
            Assert.Contains("AI failed", result.Error);
        }

        [Fact]
        public async Task Generate_ReturnsFailure_WhenJsonParseFails()
        {
            var words = new List<string> { "kass" };
            var gameId = "GrindWordsGame-kass";
            _gameStorageClientMock.Setup(x => x.GetGameData<GrindWordsGame>(gameId))
                .ReturnsAsync(Result.Success(Maybe<GrindWordsGame>.None));
            _openAiClientMock.Setup(x => x.CompleteAsync(It.IsAny<string>(), ""))
                .ReturnsAsync(Result.Success("not a json"));

            var result = await _factory.Generate(words);
            Assert.True(result.IsFailure);
            Assert.Contains("Failed to parse AI response", result.Error);
        }
    }
}
