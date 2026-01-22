using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

using Azure;
using Azure.Storage.Blobs;

using CSharpFunctionalExtensions;

namespace My1kWordsEe.Services.Db
{
    /// <summary>
    /// Facade for Azure blob storage API
    /// </summary>
    public partial class AzureStorageClient
    {
        public const string ApiSecretKey = "Secrets:AzureBlobConnectionString";

        private readonly IConfiguration config;
        private readonly ILogger logger;
        private readonly BlobServiceClient BlobServiceClient;

        public AzureStorageClient(
            IConfiguration config,
            ILogger<AzureStorageClient> logger)
        {
            this.config = config;
            this.logger = logger;
            this.BlobServiceClient = new BlobServiceClient(this.config[ApiSecretKey]);
        }

        public Uri AzureBlobEndpoint => this.BlobServiceClient.Uri;

        public async Task<Result<BlobContainerClient>> GetOrCreateContainer(string containerId)
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

        public async Task<Result<Uri>> UploadStreamAsync(BlobClient blob, Stream stream)
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

        public Task<Result<Uri>> UploadJsonAsync<T>(BlobClient blob, T record) =>
            this.UploadStreamAsync(
                blob,
                new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(record, options: new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                    WriteIndented = false
                })));

        public async Task<Result<Maybe<T>>> DownloadJsonAsync<T>(BlobClient blob)
        {
            if (!await blob.ExistsAsync())
            {
                return Maybe<T>.None;
            }

            try
            {
                var response = await blob.DownloadContentAsync();
                if (response != null && response.HasValue)
                {
                    var record = JsonSerializer.Deserialize<T>(response.Value.Content);
                    return Maybe<T>.From(record);
                }
                else
                {
                    return Maybe<T>.None;
                }
            }
            catch (RequestFailedException ex)
            {
                this.logger.LogError(ex, "Failure to download data from blob {name}", blob.Name);
                return Result.Failure<Maybe<T>>("Failure to download data from blob");
            }
            catch (Exception ex) when (ex is JsonException || ex is NotSupportedException)
            {
                this.logger.LogError(ex, "Failure to parse JSON from blob {name}", blob.Name);
                return Result.Failure<Maybe<T>>("Failure to parse JSON data from blob");
            }
        }

        public async Task<Result<bool>> DeleteIfExistsAsync(BlobClient blob)
        {
            try
            {
                var response = await blob.DeleteIfExistsAsync();
                return response.Value;
            }
            catch (RequestFailedException exception)
            {
                this.logger.LogError(exception, "Failure to delete blob {name}", blob.Name);
                return Result.Failure<bool>("Failure to delete blob {name}");
            }
        }
    }
}