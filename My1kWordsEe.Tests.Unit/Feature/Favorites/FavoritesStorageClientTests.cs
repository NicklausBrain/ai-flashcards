using Azure.Storage.Blobs;

using CSharpFunctionalExtensions;

using Moq;

using FavoriteSet = My1kWordsEe.Feature.Favorites.Favorites;

namespace My1kWordsEe.Tests.Unit.Feature.Favorites
{
    public class FavoritesStorageClientTests
    {
        [Fact]
        public async Task GetFavorites_WhenBlobExists_ReturnsFavorites()
        {
            var azureStorageClientMock = new Mock<AzureStorageClient>(null!, null!);
            var blobContainerMock = new Mock<BlobContainerClient>();
            var blobClientMock = new Mock<BlobClient>();

            var favorites = new FavoriteSet { UserId = "user-1" };

            azureStorageClientMock
                .Setup(x => x.GetOrCreateContainer("favorites"))
                .ReturnsAsync(Result.Success(blobContainerMock.Object));

            blobContainerMock
                .Setup(x => x.GetBlobClient("user-1.json"))
                .Returns(blobClientMock.Object);

            azureStorageClientMock
                .Setup(x => x.DownloadJsonAsync<FavoriteSet>(blobClientMock.Object))
                .ReturnsAsync(Result.Success(Maybe.From(favorites)));

            var client = new FavoritesStorageClient(azureStorageClientMock.Object);

            var result = await client.GetFavorites("user-1");

            Assert.True(result.IsSuccess);
            Assert.True(result.Value.HasValue);
            Assert.Equal("user-1", result.Value.Value.UserId);
        }

        [Fact]
        public async Task SaveFavorites_WhenUploadSucceeds_ReturnsUri()
        {
            var azureStorageClientMock = new Mock<AzureStorageClient>(null!, null!);
            var blobContainerMock = new Mock<BlobContainerClient>();
            var blobClientMock = new Mock<BlobClient>();
            var expectedUri = new Uri("https://example.test/favorites/user-1.json");

            azureStorageClientMock
                .Setup(x => x.GetOrCreateContainer("favorites"))
                .ReturnsAsync(Result.Success(blobContainerMock.Object));

            blobContainerMock
                .Setup(x => x.GetBlobClient("user-1.json"))
                .Returns(blobClientMock.Object);

            azureStorageClientMock
                .Setup(x => x.UploadJsonAsync(blobClientMock.Object, It.IsAny<FavoriteSet>()))
                .ReturnsAsync(Result.Success(expectedUri));

            var client = new FavoritesStorageClient(azureStorageClientMock.Object);
            var favorites = new FavoriteSet { UserId = "user-1" };

            var result = await client.SaveFavorites(favorites);

            Assert.True(result.IsSuccess);
            Assert.Equal(expectedUri, result.Value);
        }
    }
}
