using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

using CSharpFunctionalExtensions;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;

using Moq;

using My1kWordsEe.Tests.Unit.Common;

namespace My1kWordsEe.Tests.Unit.Feature.Words
{
    public class WordSetsStateContainerTests
    {
        [Fact]
        public async Task GetUserIdAsync_WhenUnauthenticated_ReturnsFailure()
        {
            var context = CreateContext(TestAuthenticationStateProvider.CreateUnauthenticated());

            var result = await context.StateContainer.GetUserIdAsync();

            Assert.True(result.IsFailure);
            Assert.Equal(Errors.AuthRequired, result.Error);
        }

        [Fact]
        public async Task GetUserIdAsync_WhenAuthenticated_ReturnsIdFromNameIdentifier()
        {
            var context = CreateContext(TestAuthenticationStateProvider.CreateAuthenticated("ApplicationUser|user-42"));

            var result = await context.StateContainer.GetUserIdAsync();

            Assert.True(result.IsSuccess);
            Assert.Equal("user-42", result.Value);
        }

        [Fact]
        public async Task SaveWordSetAsync_WhenWordsMissing_ReturnsFailure()
        {
            var context = CreateContext(TestAuthenticationStateProvider.CreateAuthenticated("ApplicationUser|user-42"));

            var result = await context.StateContainer.SaveWordSetAsync("My Set", "... !!!");

            Assert.True(result.IsFailure);
            Assert.Equal("No words provided", result.Error);
        }

        [Fact]
        public async Task SaveWordSetAsync_WhenValidInput_ParsesWordsAndPersists()
        {
            var context = CreateContext(TestAuthenticationStateProvider.CreateAuthenticated("ApplicationUser|user-42"));

            var result = await context.StateContainer.SaveWordSetAsync("Travel", "tere, aitäh! head-aega");

            Assert.True(result.IsSuccess);
            Assert.NotNull(context.UploadedWordSet);
            Assert.Equal("Travel", context.UploadedWordSet!.Name);
            Assert.Equal("user-42", context.UploadedWordSet.UserId);
            Assert.Equal(["tere", "aitäh", "head-aega"], context.UploadedWordSet.Words);
        }

        [Fact]
        public async Task DeleteWordSetAsync_WhenStorageDeleteSucceeds_ReturnsTrue()
        {
            var context = CreateContext(TestAuthenticationStateProvider.CreateAuthenticated("ApplicationUser|user-42"));

            var result = await context.StateContainer.DeleteWordSetAsync("set-1");

            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
        }

        private static WordSetsStateContainerTestContext CreateContext(AuthenticationStateProvider authenticationStateProvider)
        {
            var azureStorageClientMock = new Mock<AzureStorageClient>(null!, null!);
            var blobContainerMock = new Mock<BlobContainerClient>();

            WordSet? uploadedWordSet = null;

            azureStorageClientMock
                .Setup(x => x.GetOrCreateContainer("word-sets"))
                .ReturnsAsync(Result.Success(blobContainerMock.Object));

            blobContainerMock
                .Setup(x => x.GetBlobClient(It.IsAny<string>()))
                .Returns((string path) =>
                {
                    var blobClientMock = new Mock<BlobClient>();
                    blobClientMock.SetupGet(x => x.Name).Returns(path);
                    return blobClientMock.Object;
                });

            azureStorageClientMock
                .Setup(x => x.UploadJsonAsync(It.IsAny<BlobClient>(), It.IsAny<WordSet>()))
                .Callback<BlobClient, WordSet>((_, wordSet) => uploadedWordSet = wordSet)
                .ReturnsAsync(Result.Success(new Uri("https://example.test/word-sets/user-42/set-1.json")));

            azureStorageClientMock
                .Setup(x => x.DeleteIfExistsAsync(It.IsAny<BlobClient>()))
                .ReturnsAsync(Result.Success(true));

            var emptyPage = Page<BlobItem>.FromValues([], continuationToken: null, Mock.Of<Response>());
            var emptyPageable = AsyncPageable<BlobItem>.FromPages([emptyPage]);

            blobContainerMock
                .Setup(x => x.GetBlobsAsync(BlobTraits.None, BlobStates.None, It.IsAny<string>(), default))
                .Returns(emptyPageable);

            azureStorageClientMock
                .Setup(x => x.DownloadJsonAsync<WordSet>(It.IsAny<BlobClient>()))
                .ReturnsAsync(Result.Success(Maybe<WordSet>.None));

            var logger = new Mock<ILogger<WordSetStorageClient>>();
            var wordSetStorageClient = new WordSetStorageClient(azureStorageClientMock.Object, logger.Object);
            var stateContainer = new WordSetsStateContainer(authenticationStateProvider, wordSetStorageClient);

            return new WordSetsStateContainerTestContext(stateContainer, uploadedWordSetAccessor: () => uploadedWordSet);
        }

        private sealed class WordSetsStateContainerTestContext
        {
            private readonly Func<WordSet?> uploadedWordSetAccessor;

            public WordSetsStateContainerTestContext(
                WordSetsStateContainer stateContainer,
                Func<WordSet?> uploadedWordSetAccessor)
            {
                StateContainer = stateContainer;
                this.uploadedWordSetAccessor = uploadedWordSetAccessor;
            }

            public WordSetsStateContainer StateContainer { get; }

            public WordSet? UploadedWordSet => uploadedWordSetAccessor();
        }
    }
}
