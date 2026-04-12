using Azure.Storage.Blobs;

using CSharpFunctionalExtensions;

using Moq;

using My1kWordsEe.Services.Db;

namespace My1kWordsEe.Tests.Unit.Services.Db
{
    public class GameStorageClientTest
    {
        private readonly Mock<AzureStorageClient> _azureStorageClientMock;
        private readonly GameStorageClient _gameStorageClient;

        public GameStorageClientTest()
        {
            _azureStorageClientMock = new Mock<AzureStorageClient>(null!, null!);
            _gameStorageClient = new GameStorageClient(_azureStorageClientMock.Object);
        }

        [Fact]
        public async Task GetGameData_ShouldReturnGameData_WhenBlobExists()
        {
            // Arrange
            var gameId = "test-game";
            var expectedData = new TestGameData { Name = "Test" };
            var blobContainerMock = new Mock<BlobContainerClient>();
            var blobClientMock = new Mock<BlobClient>();

            _azureStorageClientMock.Setup(x => x.GetOrCreateContainer("cached-games"))
                .ReturnsAsync(Result.Success(blobContainerMock.Object));
            blobContainerMock.Setup(x => x.GetBlobClient($"{gameId}.json"))
                .Returns(blobClientMock.Object);
            _azureStorageClientMock.Setup(x => x.DownloadJsonAsync<TestGameData>(blobClientMock.Object))
                .ReturnsAsync(Result.Success(Maybe<TestGameData>.From(expectedData)));

            // Act
            var result = await _gameStorageClient.GetGameData<TestGameData>(gameId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Value.HasValue);
            Assert.Equal(expectedData.Name, result.Value.Value.Name);
        }

        [Fact]
        public async Task SaveGameData_ShouldReturnUri_WhenUploadSucceeds()
        {
            // Arrange
            var gameId = "test-game";
            var gameData = new TestGameData { Name = "Test" };
            var expectedUri = new Uri("https://test.blob.core.windows.net/cached-games/test-game.json");
            var blobContainerMock = new Mock<BlobContainerClient>();
            var blobClientMock = new Mock<BlobClient>();

            _azureStorageClientMock.Setup(x => x.GetOrCreateContainer("cached-games"))
                .ReturnsAsync(Result.Success(blobContainerMock.Object));
            blobContainerMock.Setup(x => x.GetBlobClient($"{gameId}.json"))
                .Returns(blobClientMock.Object);
            _azureStorageClientMock.Setup(x => x.UploadJsonAsync<TestGameData>(blobClientMock.Object, gameData))
                .ReturnsAsync(Result.Success(expectedUri));

            // Act
            var result = await _gameStorageClient.SaveGameData(gameId, gameData);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedUri, result.Value);
        }

        public class TestGameData
        {
            public string Name { get; set; } = string.Empty;
        }
    }
}