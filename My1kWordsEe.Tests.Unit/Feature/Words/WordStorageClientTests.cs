using Azure.Storage.Blobs;

using CSharpFunctionalExtensions;

using Moq;

namespace My1kWordsEe.Tests.Unit.Feature.Words
{
    public class WordStorageClientTests
    {
        [Fact]
        public async Task GetEtWordData_WhenBlobExists_ReturnsWord()
        {
            var azureStorageClientMock = new Mock<AzureStorageClient>(null!, null!);
            var blobContainerMock = new Mock<BlobContainerClient>();
            var blobClientMock = new Mock<BlobClient>();

            var word = new EtWord
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

            azureStorageClientMock
                .Setup(x => x.GetOrCreateContainer("et-word"))
                .ReturnsAsync(Result.Success(blobContainerMock.Object));

            blobContainerMock
                .Setup(x => x.GetBlobClient("tere.json"))
                .Returns(blobClientMock.Object);

            azureStorageClientMock
                .Setup(x => x.DownloadJsonAsync<EtWord>(blobClientMock.Object))
                .ReturnsAsync(Result.Success(Maybe.From(word)));

            var client = new WordStorageClient(azureStorageClientMock.Object);

            var result = await client.GetEtWordData("tere");

            Assert.True(result.IsSuccess);
            Assert.True(result.Value.HasValue);
            Assert.Equal("tere", result.Value.Value.Value);
        }

        [Fact]
        public async Task SaveEtWordData_WhenUploadSucceeds_ReturnsUri()
        {
            var azureStorageClientMock = new Mock<AzureStorageClient>(null!, null!);
            var blobContainerMock = new Mock<BlobContainerClient>();
            var blobClientMock = new Mock<BlobClient>();
            var expectedUri = new Uri("https://example.test/et-word/tere.json");

            azureStorageClientMock
                .Setup(x => x.GetOrCreateContainer("et-word"))
                .ReturnsAsync(Result.Success(blobContainerMock.Object));

            blobContainerMock
                .Setup(x => x.GetBlobClient("tere.json"))
                .Returns(blobClientMock.Object);

            azureStorageClientMock
                .Setup(x => x.UploadJsonAsync(blobClientMock.Object, It.IsAny<EtWord>()))
                .ReturnsAsync(Result.Success(expectedUri));

            var client = new WordStorageClient(azureStorageClientMock.Object);
            var word = new EtWord
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

            var result = await client.SaveEtWordData(word);

            Assert.True(result.IsSuccess);
            Assert.Equal(expectedUri, result.Value);
        }
    }
}
