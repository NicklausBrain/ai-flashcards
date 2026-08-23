using Azure.Storage.Blobs;

using CSharpFunctionalExtensions;

using Moq;

using FavoriteSet = My1kWordsEe.Feature.Favorites.Favorites;

namespace My1kWordsEe.Tests.Unit.Feature.Favorites
{
    public class FavoritesCommandsTests
    {
        [Fact]
        public async Task AddToFavoritesCommand_WithWord_AddsLowercaseKeyAndSaves()
        {
            var context = CreateContext(new FavoriteSet { UserId = "user-1" });
            var command = new AddToFavoritesCommand(context.Query, context.StorageClient);
            var word = CreateWord("Tere", "hello");

            var result = await command.Invoke("user-1", word);

            Assert.True(result.IsSuccess);
            Assert.NotNull(context.UploadedFavorites);
            Assert.True(context.UploadedFavorites!.Words.ContainsKey("tere"));
        }

        [Fact]
        public async Task RemoveFromFavoritesCommand_WithWord_RemovesEntryAndSaves()
        {
            var existingWord = CreateWord("Tere", "hello");
            var context = CreateContext(new FavoriteSet
            {
                UserId = "user-1",
                Words = new Dictionary<string, EtWord>
                {
                    ["tere"] = existingWord
                }
            });
            var command = new RemoveFromFavoritesCommand(context.Query, context.StorageClient);

            var result = await command.Invoke("user-1", existingWord);

            Assert.True(result.IsSuccess);
            Assert.NotNull(context.UploadedFavorites);
            Assert.False(context.UploadedFavorites!.Words.ContainsKey("tere"));
        }

        [Fact]
        public async Task ReorderFavoritesCommand_WithWords_RewritesWordDictionary()
        {
            var word1 = CreateWord("tere", "hello");
            var word2 = CreateWord("aitäh", "thanks");
            var context = CreateContext(new FavoriteSet
            {
                UserId = "user-1",
                Words = new Dictionary<string, EtWord>
                {
                    ["old"] = word1
                }
            });
            var command = new ReorderFavoritesCommand(context.Query, context.StorageClient);

            var result = await command.Invoke("user-1", new[] { word1, word2 });

            Assert.True(result.IsSuccess);
            Assert.NotNull(context.UploadedFavorites);
            Assert.Equal(2, context.UploadedFavorites!.Words.Count);
            Assert.True(context.UploadedFavorites.Words.ContainsKey("tere"));
            Assert.True(context.UploadedFavorites.Words.ContainsKey("aitäh"));
        }

        [Theory]
        [InlineData(UpdateScoreCommand.ScoreUpdate.Up, 5, 6)]
        [InlineData(UpdateScoreCommand.ScoreUpdate.Down, 5, 4)]
        [InlineData(UpdateScoreCommand.ScoreUpdate.Max, 2, FavoriteSet.MaxWordScore)]
        [InlineData(UpdateScoreCommand.ScoreUpdate.Up, FavoriteSet.MaxWordScore, FavoriteSet.MaxWordScore)]
        [InlineData(UpdateScoreCommand.ScoreUpdate.Down, FavoriteSet.MinWordScore, FavoriteSet.MinWordScore)]
        public async Task UpdateScoreCommand_UpdatesScoreWithinExpectedBounds(
            UpdateScoreCommand.ScoreUpdate update,
            int initialScore,
            int expectedScore)
        {
            var context = CreateContext(new FavoriteSet
            {
                UserId = "user-1",
                Stats = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
                {
                    ["tere"] = initialScore
                }
            });
            var command = new UpdateScoreCommand(context.Query, context.StorageClient);

            var result = await command.Invoke("user-1", "Tere", update);

            Assert.True(result.IsSuccess);
            Assert.NotNull(context.UploadedFavorites);
            Assert.Equal(expectedScore, context.UploadedFavorites!.Stats["tere"]);
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

        private static FavoritesCommandTestContext CreateContext(FavoriteSet existingFavorites)
        {
            var azureStorageClientMock = new Mock<AzureStorageClient>(null!, null!);
            var blobContainerMock = new Mock<BlobContainerClient>();
            var blobClientMock = new Mock<BlobClient>();

            FavoriteSet? uploadedFavorites = null;

            azureStorageClientMock
                .Setup(x => x.GetOrCreateContainer("favorites"))
                .ReturnsAsync(Result.Success(blobContainerMock.Object));

            blobContainerMock
                .Setup(x => x.GetBlobClient(It.IsAny<string>()))
                .Returns(blobClientMock.Object);

            azureStorageClientMock
                .Setup(x => x.DownloadJsonAsync<FavoriteSet>(blobClientMock.Object))
                .ReturnsAsync(Result.Success(Maybe.From(existingFavorites)));

            azureStorageClientMock
                .Setup(x => x.UploadJsonAsync(blobClientMock.Object, It.IsAny<FavoriteSet>()))
                .Callback<BlobClient, FavoriteSet>((_, value) => uploadedFavorites = value)
                .ReturnsAsync(new Uri("https://example.test/favorites/user-1.json"));

            var storageClient = new FavoritesStorageClient(azureStorageClientMock.Object);
            var query = new GetFavoritesQuery(storageClient);

            return new FavoritesCommandTestContext(storageClient, query, uploadedFavoritesAccessor: () => uploadedFavorites);
        }

        private sealed class FavoritesCommandTestContext
        {
            private readonly Func<FavoriteSet?> uploadedFavoritesAccessor;

            public FavoritesCommandTestContext(
                FavoritesStorageClient storageClient,
                GetFavoritesQuery query,
                Func<FavoriteSet?> uploadedFavoritesAccessor)
            {
                StorageClient = storageClient;
                Query = query;
                this.uploadedFavoritesAccessor = uploadedFavoritesAccessor;
            }

            public FavoritesStorageClient StorageClient { get; }

            public GetFavoritesQuery Query { get; }

            public FavoriteSet? UploadedFavorites => uploadedFavoritesAccessor();
        }
    }
}
