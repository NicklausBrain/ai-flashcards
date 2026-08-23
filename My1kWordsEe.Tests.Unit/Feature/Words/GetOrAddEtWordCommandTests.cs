using Azure.Storage.Blobs;

using CSharpFunctionalExtensions;

using Moq;

namespace My1kWordsEe.Tests.Unit.Feature.Words
{
    public class GetOrAddEtWordCommandTests
    {
        [Fact]
        public async Task Invoke_WhenWordIsInvalid_ReturnsFailure()
        {
            var command = new GetOrAddEtWordCommand(wordStorageClient: null!, addEtWordCommand: null!);

            var result = await command.Invoke("@@@");

            Assert.True(result.IsFailure);
            Assert.Equal("Not an Estonian word", result.Error);
        }

        [Fact]
        public async Task Invoke_WhenStorageFails_ReturnsFailure()
        {
            var storageClient = BuildStorageClient(
                setupDownload: (azure, blob) => azure
                    .Setup(x => x.DownloadJsonAsync<EtWord>(blob))
                    .ReturnsAsync(Result.Failure<Maybe<EtWord>>("storage error")));

            var command = new GetOrAddEtWordCommand(storageClient, addEtWordCommand: null!);

            var result = await command.Invoke("tere");

            Assert.True(result.IsFailure);
            Assert.Equal("storage error", result.Error);
        }

        [Fact]
        public async Task Invoke_WhenWordExistsInStorage_ReturnsStoredWord()
        {
            var storedWord = new EtWord
            {
                Value = "tere",
                Senses =
                [
                    new WordSense
                    {
                        BaseForm = "tere",
                        Word = new TranslatedString { Et = "tere", En = "hello" },
                        Definition = new TranslatedString { Et = "tervitus", En = "greeting" },
                        PartOfSpeech = new TranslatedString { Et = "hüüdsõna", En = "interjection" }
                    }
                ]
            };

            var storageClient = BuildStorageClient(
                setupDownload: (azure, blob) => azure
                    .Setup(x => x.DownloadJsonAsync<EtWord>(blob))
                    .ReturnsAsync(Result.Success(Maybe.From(storedWord))));

            var command = new GetOrAddEtWordCommand(storageClient, addEtWordCommand: null!);

            var result = await command.Invoke("tere");

            Assert.True(result.IsSuccess);
            Assert.Equal("tere", result.Value.Value);
        }

        private static WordStorageClient BuildStorageClient(
            Action<Mock<AzureStorageClient>, BlobClient> setupDownload)
        {
            var azureStorageClientMock = new Mock<AzureStorageClient>(null!, null!);
            var blobContainerMock = new Mock<BlobContainerClient>();
            var blobClientMock = new Mock<BlobClient>();

            azureStorageClientMock
                .Setup(x => x.GetOrCreateContainer("et-word"))
                .ReturnsAsync(Result.Success(blobContainerMock.Object));

            blobContainerMock
                .Setup(x => x.GetBlobClient(It.IsAny<string>()))
                .Returns(blobClientMock.Object);

            setupDownload(azureStorageClientMock, blobClientMock.Object);

            return new WordStorageClient(azureStorageClientMock.Object);
        }
    }
}
