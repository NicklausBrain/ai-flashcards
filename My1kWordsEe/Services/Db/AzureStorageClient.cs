using System.Text.Json;

using Azure;
using Azure.Storage.Blobs;

using CSharpFunctionalExtensions;

using My1kWordsEe.Models;

namespace My1kWordsEe.Services.Db
{
    /// <summary>
    /// Facade for Azure blob storage API
    /// </summary>
    public class AzureStorageClient
    {
        public const string ApiSecretKey = "Secrets:AzureBlobConnectionString";

        private readonly IConfiguration config;
        private readonly ILogger logger;

        public AzureStorageClient(
            IConfiguration config,
            ILogger<AzureStorageClient> logger)
        {
            this.config = config;
            this.logger = logger;
        }

        public async Task<Result<Maybe<SampleWord>>> GetWordData(string word)
        {
            var container = await GetWordsContainer();

            if (container.IsFailure)
            {
                return Result.Failure<Maybe<SampleWord>>(container.Error);
            }

            BlobClient blob = container.Value.GetBlobClient(JsonBlobName(word));

            if (!await blob.ExistsAsync())
            {
                return Maybe<SampleWord>.None;
            }

            try
            {
                var response = await blob.DownloadContentAsync();
                if (response != null && response.HasValue)
                {
                    var sampleWord = JsonSerializer.Deserialize<SampleWord>(response.Value.Content);
                    return Maybe<SampleWord>.From(sampleWord);
                }
                else
                {
                    return Maybe<SampleWord>.None;
                }
            }
            catch (RequestFailedException ex)
            {
                this.logger.LogError(ex, "Failure to download data from blob {name}", blob.Name);
                return Result.Failure<Maybe<SampleWord>>("Failure to download data from blob");
            }
            catch (Exception ex) when (ex is JsonException || ex is NotSupportedException)
            {
                this.logger.LogError(ex, "Failure to parse JSON from blob {name}", blob.Name);
                return Result.Failure<Maybe<SampleWord>>("Failure to parse JSON data from blob");
            }
        }

        public Task<Result<Uri>> SaveWordData(SampleWord word) =>
            this.GetWordsContainer().Bind(container =>
            this.UploadStreamAsync(
                container.GetBlobClient(JsonBlobName(word.EeWord)),
                new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(word))));

        public Task<Result<Uri>> SaveAudio(Stream audioStream, string blobName) =>
            this.GetAudioContainer().Bind((container) =>
            this.UploadStreamAsync(
                container.GetBlobClient(blobName),
                audioStream));

        public Task<Result<Uri>> SaveAudio(Stream audioStream) =>
            this.SaveAudio(audioStream, WavBlobName());

        public Task<Result<Uri>> SaveImage(Stream imageStream) =>
            this.GetImageContainer().Bind((container) =>
            this.UploadStreamAsync(
                container.GetBlobClient(JpgBlobName()),
                imageStream));

        private Task<Result<BlobContainerClient>> GetWordsContainer() => this.GetOrCreateContainer("words");

        private Task<Result<BlobContainerClient>> GetAudioContainer() => this.GetOrCreateContainer("audio");

        private Task<Result<BlobContainerClient>> GetImageContainer() => this.GetOrCreateContainer("image");

        private async Task<Result<BlobContainerClient>> GetOrCreateContainer(string containerId)
        {
            try
            {
                var container = new BlobContainerClient(
                    this.config[ApiSecretKey],
                    containerId);
                await container.CreateIfNotExistsAsync();
                return container;
            }
            catch (RequestFailedException exception)
            {
                this.logger.LogError(exception, "Failure to get or create container {ContainerId}", containerId);
                return Result.Failure<BlobContainerClient>("Azure storage access error");
            }
        }

        private async Task<Result<Uri>> UploadStreamAsync(BlobClient blob, Stream stream)
        {
            try
            {
                await blob.UploadAsync(stream, overwrite: true);
                return blob.Uri;
            }
            catch (RequestFailedException exception)
            {
                this.logger.LogError(exception, "Failure to upload data to blob {name}", blob.Name);
                return Result.Failure<Uri>("Azure storage upload error");
            }
        }

        static string JsonBlobName(string word) => word.ToLower() + ".json";

        static string WavBlobName() => Guid.NewGuid() + ".wav";

        static string JpgBlobName() => Guid.NewGuid() + ".jpg";
    }
}
