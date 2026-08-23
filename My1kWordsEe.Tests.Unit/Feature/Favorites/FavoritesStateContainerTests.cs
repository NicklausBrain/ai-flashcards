using System.Security.Claims;

using Azure.Storage.Blobs;

using CSharpFunctionalExtensions;

using Microsoft.AspNetCore.Components.Authorization;

using Moq;

using My1kWordsEe.Tests.Unit.Common;

using FavoriteSet = My1kWordsEe.Feature.Favorites.Favorites;

namespace My1kWordsEe.Tests.Unit.Feature.Favorites
{
    public class FavoritesStateContainerTests
    {
        [Fact]
        public async Task GetAsync_WhenUserIsNotAuthenticated_ReturnsAuthRequired()
        {
            var context = CreateContext(TestAuthenticationStateProvider.CreateUnauthenticated(), new FavoriteSet { UserId = "ignored" });

            var result = await context.StateContainer.GetAsync();

            Assert.True(result.IsFailure);
            Assert.Equal(Errors.AuthRequired, result.Error);
        }

        [Fact]
        public async Task GetAsync_WhenNameIdentifierClaimMissing_ReturnsAuthRequired()
        {
            var identity = new ClaimsIdentity([new Claim(ClaimTypes.Email, "user@example.com")], authenticationType: "TestAuth");
            var provider = new TestAuthenticationStateProvider(new ClaimsPrincipal(identity));
            var context = CreateContext(provider, new FavoriteSet { UserId = "ignored" });

            var result = await context.StateContainer.GetAsync();

            Assert.True(result.IsFailure);
            Assert.Equal(Errors.AuthRequired, result.Error);
        }

        [Fact]
        public async Task GetAsync_WhenClaimHasInvalidGuidFormat_ReturnsFailure()
        {
            var provider = TestAuthenticationStateProvider.CreateAuthenticated("ApplicationUser|not-a-guid");
            var context = CreateContext(provider, new FavoriteSet { UserId = "ignored" });

            var result = await context.StateContainer.GetAsync();

            Assert.True(result.IsFailure);
            Assert.Equal("Invalid user ID format", result.Error);
        }

        [Fact]
        public async Task GetAsync_WhenAuthenticated_LoadsAndCachesFavorites()
        {
            var userId = Guid.NewGuid().ToString();
            var provider = TestAuthenticationStateProvider.CreateAuthenticated($"ApplicationUser|{userId}");
            var context = CreateContext(provider, new FavoriteSet { UserId = userId });

            var first = await context.StateContainer.GetAsync();
            var second = await context.StateContainer.GetAsync();

            Assert.True(first.IsSuccess);
            Assert.True(second.IsSuccess);
            Assert.Equal(userId, first.Value.UserId);
            context.AzureStorageClientMock.Verify(x => x.DownloadJsonAsync<FavoriteSet>(It.IsAny<BlobClient>()), Times.Once);
        }

        [Fact]
        public async Task AddAsync_WhenAuthenticated_UpdatesCachedFavorites()
        {
            var userId = Guid.NewGuid().ToString();
            var provider = TestAuthenticationStateProvider.CreateAuthenticated($"ApplicationUser|{userId}");
            var initialFavorites = new FavoriteSet { UserId = userId };
            var context = CreateContext(provider, initialFavorites);
            var word = CreateWord("Tere", "hello");

            var result = await context.StateContainer.AddAsync(word);

            Assert.True(result.IsSuccess);
            Assert.True(result.Value.Words.ContainsKey("tere"));

            var cached = await context.StateContainer.GetAsync();
            Assert.True(cached.Value.Words.ContainsKey("tere"));
        }

        [Fact]
        public async Task RemoveAsync_WhenWordExists_RemovesFromFavorites()
        {
            var userId = Guid.NewGuid().ToString();
            var provider = TestAuthenticationStateProvider.CreateAuthenticated($"ApplicationUser|{userId}");
            var word = CreateWord("Tere", "hello");
            var initialFavorites = new FavoriteSet
            {
                UserId = userId,
                Words = new Dictionary<string, EtWord>
                {
                    ["tere"] = word
                }
            };
            var context = CreateContext(provider, initialFavorites);

            var result = await context.StateContainer.RemoveAsync(word);

            Assert.True(result.IsSuccess);
            Assert.False(result.Value.Words.ContainsKey("tere"));
        }

        private static EtWord CreateWord(string et, string en) =>
            new()
            {
                Value = et,
                Senses =
                [
                    new WordSense
                    {
                        BaseForm = et,
                        Word = new TranslatedString { Et = et, En = en },
                        Definition = new TranslatedString { Et = "def", En = "def" },
                        PartOfSpeech = new TranslatedString { Et = "nimisõna", En = "noun" }
                    }
                ]
            };

        private static FavoritesStateContainerTestContext CreateContext(
            AuthenticationStateProvider authProvider,
            FavoriteSet storedFavorites)
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
                .ReturnsAsync(Result.Success(Maybe.From(storedFavorites)));

            azureStorageClientMock
                .Setup(x => x.UploadJsonAsync(blobClientMock.Object, It.IsAny<FavoriteSet>()))
                .ReturnsAsync(new Uri("https://example.test/favorites/user.json"));

            var storageClient = new FavoritesStorageClient(azureStorageClientMock.Object);
            var query = new GetFavoritesQuery(storageClient);
            var add = new AddToFavoritesCommand(query, storageClient);
            var remove = new RemoveFromFavoritesCommand(query, storageClient);
            var reorder = new ReorderFavoritesCommand(query, storageClient);
            var update = new UpdateScoreCommand(query, storageClient);

            var stateContainer = new FavoritesStateContainer(authProvider, query, add, remove, reorder, update);
            return new FavoritesStateContainerTestContext(stateContainer, azureStorageClientMock);
        }

        private sealed record FavoritesStateContainerTestContext(
            FavoritesStateContainer StateContainer,
            Mock<AzureStorageClient> AzureStorageClientMock);
    }
}
