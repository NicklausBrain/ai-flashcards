using Azure.Storage.Blobs;

using CSharpFunctionalExtensions;

using Moq;

using FavoriteSet = My1kWordsEe.Feature.Favorites.Favorites;

namespace My1kWordsEe.Tests.Unit.Feature.Favorites
{
    public class GetFavoritesQueryTests
    {
        [Fact]
        public async Task Invoke_WithEmptyUserId_ReturnsFailure()
        {
            var query = new GetFavoritesQuery(new FavoritesStorageClient(new Mock<AzureStorageClient>(null!, null!).Object));

            var result = await query.Invoke(" ");

            Assert.True(result.IsFailure);
        }

        [Fact]
        public async Task Invoke_WhenStorageFails_ReturnsFailure()
        {
            var azureStorageClientMock = new Mock<AzureStorageClient>(null!, null!);
            var blobContainerMock = new Mock<BlobContainerClient>();
            var blobClientMock = new Mock<BlobClient>();

            azureStorageClientMock
                .Setup(x => x.GetOrCreateContainer("favorites"))
                .ReturnsAsync(Result.Success(blobContainerMock.Object));
            blobContainerMock
                .Setup(x => x.GetBlobClient(It.IsAny<string>()))
                .Returns(blobClientMock.Object);
            azureStorageClientMock
                .Setup(x => x.DownloadJsonAsync<FavoriteSet>(blobClientMock.Object))
                .ReturnsAsync(Result.Failure<Maybe<FavoriteSet>>("download failed"));

            var query = new GetFavoritesQuery(new FavoritesStorageClient(azureStorageClientMock.Object));

            var result = await query.Invoke("user-1");

            Assert.True(result.IsFailure);
            Assert.Equal("download failed", result.Error);
        }

        [Fact]
        public async Task Invoke_WhenFavoritesExist_ReturnsStoredFavorites()
        {
            var favorites = new FavoriteSet { UserId = "user-1" };

            var azureStorageClientMock = new Mock<AzureStorageClient>(null!, null!);
            var blobContainerMock = new Mock<BlobContainerClient>();
            var blobClientMock = new Mock<BlobClient>();

            azureStorageClientMock
                .Setup(x => x.GetOrCreateContainer("favorites"))
                .ReturnsAsync(Result.Success(blobContainerMock.Object));
            blobContainerMock
                .Setup(x => x.GetBlobClient(It.IsAny<string>()))
                .Returns(blobClientMock.Object);
            azureStorageClientMock
                .Setup(x => x.DownloadJsonAsync<FavoriteSet>(blobClientMock.Object))
                .ReturnsAsync(Result.Success(Maybe.From(favorites)));

            var query = new GetFavoritesQuery(new FavoritesStorageClient(azureStorageClientMock.Object));

            var result = await query.Invoke("user-1");

            Assert.True(result.IsSuccess);
            Assert.Equal("user-1", result.Value.UserId);
        }

        [Fact]
        public async Task Invoke_WhenFavoritesDoNotExist_ReturnsEmptyFavoritesForUser()
        {
            var azureStorageClientMock = new Mock<AzureStorageClient>(null!, null!);
            var blobContainerMock = new Mock<BlobContainerClient>();
            var blobClientMock = new Mock<BlobClient>();

            azureStorageClientMock
                .Setup(x => x.GetOrCreateContainer("favorites"))
                .ReturnsAsync(Result.Success(blobContainerMock.Object));
            blobContainerMock
                .Setup(x => x.GetBlobClient(It.IsAny<string>()))
                .Returns(blobClientMock.Object);
            azureStorageClientMock
                .Setup(x => x.DownloadJsonAsync<FavoriteSet>(blobClientMock.Object))
                .ReturnsAsync(Result.Success(Maybe<FavoriteSet>.None));

            var query = new GetFavoritesQuery(new FavoritesStorageClient(azureStorageClientMock.Object));

            var result = await query.Invoke("user-2");

            Assert.True(result.IsSuccess);
            Assert.Equal("user-2", result.Value.UserId);
        }
    }
}
