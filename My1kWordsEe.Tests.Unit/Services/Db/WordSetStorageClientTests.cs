using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

using CSharpFunctionalExtensions;

using Moq;

using My1kWordsEe.Models;
using My1kWordsEe.Services.Db;

using Xunit;

namespace My1kWordsEe.Tests.Unit.Services.Db
{
    public class WordSetStorageClientTests
    {
        private readonly Mock<AzureStorageClient> _azureStorageClientMock;
        private readonly WordSetStorageClient _wordSetStorageClient;
        private readonly string _userId = "user123";
        private readonly string _wordSetId = "set456";

        public WordSetStorageClientTests()
        {
            // AzureStorageClient has no parameterless constructor, providing nulls as it will be mocked anyway
            _azureStorageClientMock = new Mock<AzureStorageClient>(null!, null!);
            _wordSetStorageClient = new WordSetStorageClient(_azureStorageClientMock.Object);
        }

        [Fact]
        public async Task SaveWordSet_ShouldReturnUri_WhenUploadSucceeds()
        {
            // Arrange
            var wordSet = new WordSet
            {
                Id = _wordSetId,
                UserId = _userId,
                Name = "Test Set",
                Words = new List<string> { "tere", "head aega" }
            };
            var expectedUri = new Uri("https://test.blob.core.windows.net/word-sets/user123/set456.json");
            var blobContainerMock = new Mock<BlobContainerClient>();
            var blobClientMock = new Mock<BlobClient>();

            _azureStorageClientMock.Setup(x => x.GetOrCreateContainer("word-sets"))
                .ReturnsAsync(Result.Success(blobContainerMock.Object));
            blobContainerMock.Setup(x => x.GetBlobClient($"{_userId}/{_wordSetId}.json"))
                .Returns(blobClientMock.Object);
            _azureStorageClientMock.Setup(x => x.UploadJsonAsync(blobClientMock.Object, wordSet))
                .ReturnsAsync(Result.Success(expectedUri));

            // Act
            var result = await _wordSetStorageClient.SaveWordSet(wordSet);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedUri, result.Value);
        }

        [Fact]
        public async Task GetWordSet_ShouldReturnWordSet_WhenBlobExists()
        {
            // Arrange
            var expectedWordSet = new WordSet
            {
                Id = _wordSetId,
                UserId = _userId,
                Name = "Test Set",
                Words = new List<string> { "tere" }
            };
            var blobContainerMock = new Mock<BlobContainerClient>();
            var blobClientMock = new Mock<BlobClient>();

            _azureStorageClientMock.Setup(x => x.GetOrCreateContainer("word-sets"))
                .ReturnsAsync(Result.Success(blobContainerMock.Object));
            blobContainerMock.Setup(x => x.GetBlobClient($"{_userId}/{_wordSetId}.json"))
                .Returns(blobClientMock.Object);
            _azureStorageClientMock.Setup(x => x.DownloadJsonAsync<WordSet>(blobClientMock.Object))
                .ReturnsAsync(Result.Success(Maybe<WordSet>.From(expectedWordSet)));

            // Act
            var result = await _wordSetStorageClient.GetWordSet(_userId, _wordSetId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Value.HasValue);
            Assert.Equal(expectedWordSet.Name, result.Value.Value.Name);
        }

        [Fact]
        public async Task DeleteWordSet_ShouldReturnTrue_WhenDeletionSucceeds()
        {
            // Arrange
            var blobContainerMock = new Mock<BlobContainerClient>();
            var blobClientMock = new Mock<BlobClient>();

            _azureStorageClientMock.Setup(x => x.GetOrCreateContainer("word-sets"))
                .ReturnsAsync(Result.Success(blobContainerMock.Object));
            blobContainerMock.Setup(x => x.GetBlobClient($"{_userId}/{_wordSetId}.json"))
                .Returns(blobClientMock.Object);
            _azureStorageClientMock.Setup(x => x.DeleteIfExistsAsync(blobClientMock.Object))
                .ReturnsAsync(Result.Success(true));

            // Act
            var result = await _wordSetStorageClient.DeleteWordSet(_userId, _wordSetId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
        }

        [Fact]
        public async Task ListWordSets_ShouldReturnWordSets_WhenBlobsExist()
        {
            // Arrange
            var wordSet1 = new WordSet { Id = "1", UserId = _userId, Name = "Set 1", Words = new List<string>(), UpdatedAt = DateTime.UtcNow.AddMinutes(-1) };
            var wordSet2 = new WordSet { Id = "2", UserId = _userId, Name = "Set 2", Words = new List<string>(), UpdatedAt = DateTime.UtcNow };
            
            var blobContainerMock = new Mock<BlobContainerClient>();
            var blobItem1 = BlobsModelFactory.BlobItem("user123/1.json");
            var blobItem2 = BlobsModelFactory.BlobItem("user123/2.json");
            
            var blobItems = new List<BlobItem> { blobItem1, blobItem2 };
            var page = Page<BlobItem>.FromValues(blobItems, null, Mock.Of<Response>());
            var asyncPageable = AsyncPageable<BlobItem>.FromPages(new[] { page });

            _azureStorageClientMock.Setup(x => x.GetOrCreateContainer("word-sets"))
                .ReturnsAsync(Result.Success(blobContainerMock.Object));
            
            blobContainerMock.Setup(x => x.GetBlobsAsync(It.IsAny<BlobTraits>(), It.IsAny<BlobStates>(), $"{_userId}/", default))
                .Returns(asyncPageable);

            var blobClient1Mock = new Mock<BlobClient>();
            var blobClient2Mock = new Mock<BlobClient>();
            
            blobContainerMock.Setup(x => x.GetBlobClient("user123/1.json")).Returns(blobClient1Mock.Object);
            blobContainerMock.Setup(x => x.GetBlobClient("user123/2.json")).Returns(blobClient2Mock.Object);

            _azureStorageClientMock.Setup(x => x.DownloadJsonAsync<WordSet>(blobClient1Mock.Object))
                .ReturnsAsync(Result.Success(Maybe<WordSet>.From(wordSet1)));
            _azureStorageClientMock.Setup(x => x.DownloadJsonAsync<WordSet>(blobClient2Mock.Object))
                .ReturnsAsync(Result.Success(Maybe<WordSet>.From(wordSet2)));

            // Act
            var result = await _wordSetStorageClient.ListWordSets(_userId);

            // Assert
            Assert.True(result.IsSuccess);
            var list = result.Value.ToList();
            Assert.Equal(2, list.Count);
            Assert.Equal("Set 2", list[0].Name); // Ordered by UpdatedAt descending
            Assert.Equal("Set 1", list[1].Name);
        }
    }
}
